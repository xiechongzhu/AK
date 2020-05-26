%����ת����ȷ��
clear all;
close all;
clc;

%��ȡ����
load('data.mat','out_B','out_lambda','out_h',...
    'out_Ve','out_Vu','out_Vn',...
    'out_Vx','out_Vy','out_Vz',...
    'out_x','out_y','out_z');
param(4 ) = -93.3417;% ��ʼ��λ��
param(1:3) = [41.2805,100.3045,980.2]; %�����λ�ã�γ��������
Vf=zeros(length(out_B),3);
XYZ = zeros(length(out_B),3);
XYZ_yc = zeros(length(out_B),3);
XYZt_yc = zeros(length(out_B),3);
%��ʼ��
[ R0,R0_f, C_e2f, C_fe2, we_f,xyz_e0] = calc_const_launch(param(1:3),param(4) );%���㳣��
for i = 1: length(out_B)%���η�������
    %�״����
    [XYZ(i,1),XYZ(i,2),range,t_range,XYZ(i,3)] = calc_target_ld(...
        [out_x(i),out_y(i),out_z(i),out_Vx(i),out_Vy(i),out_Vz(i),param(2)],...
        R0, R0_f,C_e2f, C_fe2, we_f,xyz_e0,out_h(end));
    
    %ң�����
    [XYZ_yc(i,:),Vf(i,:),XYZt_yc(i,1),XYZt_yc(i,2),range,t_range,XYZt_yc(i,3)] = calc_target_yc(...
        [out_B(i),out_lambda(i),out_h(i),out_Ve(i),out_Vn(i),out_Vu(i)],...
        R0, R0_f, xyz_e0, C_e2f, C_fe2, we_f,out_h(end));
end

%��ͼ
figure()
subplot(121)
plot(XYZ(1:end-2,1)-XYZt_yc(1:end-2,1));grid on;hold on;
% plot(XYZt_yc(1:end-2,1),'r');
title('Ԥʾλ��x���㷨��ֵ');
subplot(122)
plot(XYZ(1:end-2,3)-XYZt_yc(1:end-2,3));grid on;hold on;
% plot(XYZt_yc(1:end-2,3),'r');
title('Ԥʾλ��z���㷨��ֵ')
fprintf('1��ʵ�����x���״�Ԥʾ���x�����Ϊ%0.3f m\n', ...
    out_x(end)-XYZ(end-2,1));
fprintf('2��ʵ�����z���״�Ԥʾ���z�����Ϊ%0.3f m\n', ...
    out_z(end)-XYZ(end-2,3));
fprintf('3��ʵ�����x��ң��Ԥʾ���x�����Ϊ%0.3f m\n', ...
    out_x(end)-XYZt_yc(end-2,1));
fprintf('4��ʵ�����z��ң��Ԥʾ���z�����Ϊ%0.3f m\n', ...
    out_z(end)-XYZt_yc(end-2,3));


