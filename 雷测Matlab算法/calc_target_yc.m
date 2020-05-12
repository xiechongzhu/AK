function  [XYZ,Vf,t_x,t_y,range,t_range,t_z,flighttime] = calc_target_yc(nav_now,...
    R0, R0_f, xyz_e0, C_e2f, C_fe2, we_f, h_end)
%   遥测预示落点计算
% 输入
%   nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
%       6 天向速度(m/s)
%   R0_f
%   R0
%   xyz_e0
%   C_e2f，转换矩阵
%   C_fe2
%   we_f
%   h_end, 落点附近海拔

% 输出
%   XYZ，当前发射系x，y，z
%   Vf，发射系速度x，y，z
%   range,射程
%   t_range,射程，单位m
%   t_x，发射系x
%   t_y，发射系y
%   t_z，发射系z


flighttime = -1;

% 规整输入
B        = nav_now(1)*(pi/180);
lambda   = nav_now(2)*(pi/180);
h        = nav_now(3);
Ve_mix   = nav_now(4);
Vn_mix   = nav_now(5);
Vu_mix   = nav_now(6);

% 参数
we = 7.292115e-5;
miu = 3.986004418e14; 
ee = 0.0818191908425;
rc = 6378140; 

Rn = rc/sqrt(1-ee^2*(sin(B))^2); 
x_e = (Rn+h)*cos(B)*cos(lambda);
y_e = (Rn+h)*cos(B)*sin(lambda);
z_e = ((1-ee^2)*Rn+h)*sin(B);
xyz_e = [ x_e; y_e; z_e ];

%当前发射系坐标
xyz_fp = C_e2f*(xyz_e-xyz_e0);
x = xyz_fp(1);
y = xyz_fp(2);
z = xyz_fp(3);
XYZ = [x,y,z];

% 计算射程
r_f    = xyz_fp + R0_f; 
r_k      = norm(r_f);

R_ave=r_k-h+h_end;
if dot(r_f,R0_f)/(r_k*R0) -1>0
    BetaR = 0;
else
    BetaR = acos( dot(r_f,R0_f)/(r_k*R0) ); 
end
range = R0*BetaR;                        %当前射程


x2y2z2 = C_fe2*r_f;
if x2y2z2(2) > 0 
    dlambda = atan( x2y2z2(3)/x2y2z2(2) );
elseif ((x2y2z2(2) < 0) && (x2y2z2(3) >= 0))
    dlambda = pi + atan( x2y2z2(3)/x2y2z2(2) );
elseif ((x2y2z2(2) < 0) && (x2y2z2(3) < 0))
    dlambda = atan( x2y2z2(3)/x2y2z2(2))-pi;
else
    dlambda = 0;
end

C_e2nue = [ cos(B)   -sin(B)*cos(dlambda)  -sin(B)*sin(dlambda)
    sin(B)   cos(B)*cos(dlambda)   cos(B)*sin(dlambda)
    0      -sin(dlambda)            cos(dlambda)       ];
C_fnue = C_e2nue*C_fe2; 
Vf = C_fnue'*[Vn_mix;Vu_mix;Ve_mix];%计算发射系下的速度分量
V_plus = C_fnue*cross(we_f,r_f);

V_a=[ Vn_mix,Vu_mix,Ve_mix ]'+V_plus;
Vn_a=V_a(1);
Vu_a=V_a(2);
Ve_a=V_a(3);

V= norm(V_a);
if(V == 0)
    Dtheta_ka = pi/2;
else
    Dtheta_ka = asin(Vu_a/V);
end


if(abs(Vn_a) <1e-3)
    Vn_a=1e-3;
end
if(Vn_a>0)
    A_ka= atan(Ve_a/Vn_a);
else
    if(Ve_a>=0)
        A_ka= atan(Ve_a/Vn_a)+pi;
    else
        A_ka= atan(Ve_a/Vn_a)-pi;
    end
end

if(r_k<R_ave)
    t_range = 0;
    t_x = 0;
    t_y = 0;
    t_z = 0;
else
    energy_k = V^2*r_k/miu;
    ebian = sqrt(1+energy_k*(energy_k-2)*(cos(Dtheta_ka))^2); 
    if(ebian>=1)
        t_range = 0;
        t_x = 0;
        t_y = 0;
        t_z = 0;
    elseif((r_k*energy_k*(cos(Dtheta_ka))^2)/(1+ebian)>=R_ave)
        t_range = 0;
        t_x = 0;
        t_y = 0;
        t_z = 0;
    else
        A_equation = 2*R_ave*(1+(tan(Dtheta_ka))^2) - energy_k*( R_ave + r_k);
        B_equation = 2*energy_k*R_ave*tan(Dtheta_ka);
        C_equation = energy_k*(R_ave-r_k);
        D_equation = B_equation+sqrt(B_equation.^2-4*A_equation.*C_equation);
        
        % 中间变量
        A_T = (r_k^3/(miu*(2-energy_k)^3))^(1/2);
        B_T = acos((R_ave*(2-energy_k)-r_k)/(ebian*r_k));
        C_T = acos((1-energy_k)/ebian);

        beta_ca = 2*asin(D_equation/sqrt(4*A_equation^2+D_equation^2));   
        
        if Dtheta_ka >= 0%计算剩余时间
            flighttime = A_T.*(B_T+C_T+ebian.*(sin(B_T)+sin(C_T)));
        else
            flighttime = A_T.*(B_T-C_T+ebian.*(sin(B_T)-sin(C_T)));
        end
        B_C = asin(cos(beta_ca).*sin(B)+sin(beta_ca).*cos(B).*cos(A_ka));  
        
        delta_lambda_A = asin(sin(beta_ca).*sin(A_ka)./cos(B_C));
        
        aa=acos(sin(B)*sin(B_C)+cos(B)*cos(B_C)*cos(delta_lambda_A));
        if abs(aa-beta_ca)>=0.001 
        delta_lambda_A=pi-delta_lambda_A;
        end       
        
        lambda_CA = delta_lambda_A + lambda; 
        lambda_C = lambda_CA - we.*flighttime; 
        
        B_c_xz = B_C;
        lambda_c_xz = lambda_C;

        Rn = rc/sqrt(1-ee^2*(sin(B_c_xz))^2);     
        x_e = (Rn+h_end)*cos(B_c_xz)*cos(lambda_c_xz);
        y_e = (Rn+h_end)*cos(B_c_xz)*sin(lambda_c_xz);
        z_e = ((1-ee^2)*Rn+h_end)*sin(B_c_xz);
        xyz_e = [ x_e; y_e; z_e ];
        
        %落点发射系坐标
        xyz_f = C_e2f*(xyz_e-xyz_e0);
        t_x = xyz_f(1);
        t_y = xyz_f(2);
        t_z = xyz_f(3);
        
        % 计算射程
        r_f    = [ t_x t_y t_z ]' + R0_f;        
        r      = norm(r_f);                     
        if dot(r_f,R0_f)/(r*R0) -1>0
            BetaR = 0;
        else
            BetaR = acos( dot(r_f,R0_f)/(r*R0) );
        end
        t_range = R0*BetaR;                      % 射程，以发射点地心距近似计算
        
    end
end
if(imag(t_z)~=0 ||imag(t_range)~=0)
    fprintf('运算出现虚数\n');
end
end



