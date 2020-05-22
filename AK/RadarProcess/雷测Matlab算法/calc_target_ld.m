function  [t_x,t_y,range,t_range,t_z, flighttime] = calc_target_ld(lambda0, nav_now, R0, R0_f, C_e2f, C_fe2, we_f,xyz_e0,h_end)

%   雷达数据预示落点计算
% 输入
%   nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
%   R0_f,
%   R0,
%   xyz_e0
%   C_e2f,
%   C_fe2
%   we_f
%   h_end, 落点附近海拔

% 输出
%   t_x，发射系x
%   t_y，发射系y
%   t_z，发射系z
%   range,当前射程
%   t_range,射程，单位m
%   t_z,发射系下z，单位m

% 规整输入
x    = nav_now(1);
y    = nav_now(2);
z    = nav_now(3);
Vx   = nav_now(4);
Vy   = nav_now(5);
Vz   = nav_now(6);


% 常量
we = 7.292115e-5;
miu = 3.986004418e14; 
ee = 0.0818191908425;              
rc = 6378140;                      
ae     = 6378140;         
be     = 6356755;     

% 计算当前射程
xyz_fp =[x;y;z];
r_f    = xyz_fp + R0_f;    
r_k      = norm(r_f);  

Phi    = asin(dot(r_f,we_f)/(r_k*we));                  
B = atan((ae/be)^2*tan(Phi));                          

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
lambda=lambda0+dlambda;
R = ae*be/sqrt(ae^2*(sin(Phi))^2+be^2*(cos(Phi))^2);         
h  = r_k - R;                                                    
C_e2nue = [ cos(B)   -sin(B)*cos(dlambda)  -sin(B)*sin(dlambda)
    sin(B)   cos(B)*cos(dlambda)   cos(B)*sin(dlambda)
    0      -sin(dlambda)            cos(dlambda)       ];
C_fnue = C_e2nue*C_fe2;       
V_nue=C_fnue*[Vx;Vy;Vz];
V_plus = C_fnue*cross(we_f,r_f);
V_a=V_nue+V_plus;
Vn_a=V_a(1);
Vu_a=V_a(2);
Ve_a=V_a(3);

R_ave=r_k-h+h_end;

if dot(r_f,R0_f)/(r_k*R0) -1>0
    BetaR = 0;
else
    BetaR = acos( dot(r_f,R0_f)/(r_k*R0) );    
end
range = R0*BetaR;                        % 当前射程

V= norm(V_a);

if(abs(Vn_a) <1e-3)
    Vn_a=1e-3;
end

if(V == 0)
    Dtheta_ka = pi/2;
else
    Dtheta_ka = asin(Vu_a/V);
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
        
        A_T = (r_k^3/(miu*(2-energy_k)^3))^(1/2);
        B_T = acos((R_ave*(2-energy_k)-r_k)/(ebian*r_k));
        C_T = acos((1-energy_k)/ebian);

        beta_ca = 2*asin(D_equation/sqrt(4*A_equation^2+D_equation^2));   
        
        if Dtheta_ka >= 0% 剩余时间
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
        t_range = R0*BetaR;                      % 射程
        
    end
end
if(imag(t_z)~=0 ||imag(t_range)~=0)
    fprintf('运算出现虚数\n');
end
end

