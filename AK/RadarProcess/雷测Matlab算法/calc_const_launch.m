function [ R0,R0_f, C_e2f, C_fe2, we_f,xyz_e0] = calc_const_launch( geo0,A0 )
%���㷢��㳣��
% ����
%   geo0,�����γ����
%   A0������㷽λ��
% ���
%   R0,������ľ࣬��λm
%   R0_f,�м���
%   xyz_e0,�����ĵ���ϵ����
%   C_e2f,�м�ת������
%   C_fe2,�м�ת������
%   we_f

A0  = A0*pi/180;
B0 = geo0(1)*(pi/180);
lambda0 = geo0(2)*(pi/180);
h0 = geo0(3);

% ����
ae     = 6378140; 
be     = 6356755;  
we     = 7.292115e-5;  
rc = 6378140;   
ee = 0.0818191908425;   

% �����
Phi0   = atan((be/ae)^2*tan(B0));     
miu0   = B0 - Phi0;                    
R0     = h0 + ae*be/sqrt(ae^2*(sin(Phi0))^2 + be^2*(cos(Phi0))^2);      
R0_f   = R0*[-sin(miu0)*cos(A0),cos(miu0),sin(miu0)*sin(A0)]';  

Rn0 = rc/sqrt(1-ee^2*(sin(B0))^2);    
x_e0 = (Rn0+h0)*cos(B0)*cos(lambda0);
y_e0 = (Rn0+h0)*cos(B0)*sin(lambda0);
z_e0 = ((1-ee^2)*Rn0+h0)*sin(B0);
xyz_e0 = [ x_e0; y_e0; z_e0 ]; 

% ת������    
C11 = -sin(A0)*sin(lambda0)-cos(A0)*sin(B0)*cos(lambda0);
C12 = sin(A0)*cos(lambda0)-cos(A0)*sin(B0)*sin(lambda0);
C13 = cos(A0)*cos(B0);
C21 = cos(B0)*cos(lambda0);
C22 = cos(B0)*sin(lambda0);
C23 = sin(B0);
C31 = -cos(A0)*sin(lambda0)+sin(A0)*sin(B0)*cos(lambda0);
C32 = cos(A0)*cos(lambda0)+sin(A0)*sin(B0)*sin(lambda0);
C33 = -sin(A0)*cos(B0);
C_e2f = [ C11,C12,C13; C21,C22,C23; C31,C32,C33 ];   

C_fe2 = [ cos(B0)*cos(A0)  sin(B0)  -cos(B0)*sin(A0)
         -sin(B0)*cos(A0)  cos(B0)   sin(B0)*sin(A0)
           sin(A0)           0           cos(A0)     ];         
we_f   = we*[cos(B0)*cos(A0)  sin(B0)  -cos(B0)*sin(A0)]';    
end