%坐标转换正确性
clear all;
close all;
clc;

%读取数据
load('data.mat','out_B','out_lambda','out_h',...
    'out_Ve','out_Vu','out_Vn',...
    'out_Vx','out_Vy','out_Vz',...
    'out_x','out_y','out_z');
param(4 ) = -93.3417;% 初始方位角
param(1:3) = [41.2805,100.3045,980.2]; %发射点位置，纬、经、高
Vf=zeros(length(out_B),3);
XYZ = zeros(length(out_B),3);
XYZ_yc = zeros(length(out_B),3);
XYZt_yc = zeros(length(out_B),3);
%初始化
[ R0,R0_f, C_e2f, C_fe2, we_f,xyz_e0] = calc_const_launch(param(1:3),param(4) );%计算常数
for i = 1: length(out_B)%单次飞行试验
    %雷达计算
    [XYZ(i,1),XYZ(i,2),range,t_range,XYZ(i,3)] = calc_target_ld(...
        [out_x(i),out_y(i),out_z(i),out_Vx(i),out_Vy(i),out_Vz(i),param(2)],...
        R0, R0_f,C_e2f, C_fe2, we_f,xyz_e0,out_h(end));
    
    %遥测计算
    [XYZ_yc(i,:),Vf(i,:),XYZt_yc(i,1),XYZt_yc(i,2),range,t_range,XYZt_yc(i,3)] = calc_target_yc(...
        [out_B(i),out_lambda(i),out_h(i),out_Ve(i),out_Vn(i),out_Vu(i)],...
        R0, R0_f, xyz_e0, C_e2f, C_fe2, we_f,out_h(end));
end

%绘图
figure()
subplot(121)
plot(XYZ(1:end-2,1)-XYZt_yc(1:end-2,1));grid on;hold on;
% plot(XYZt_yc(1:end-2,1),'r');
title('预示位置x两算法差值');
subplot(122)
plot(XYZ(1:end-2,3)-XYZt_yc(1:end-2,3));grid on;hold on;
% plot(XYZt_yc(1:end-2,3),'r');
title('预示位置z两算法差值')
fprintf('1、实际落点x与雷达预示落点x间误差为%0.3f m\n', ...
    out_x(end)-XYZ(end-2,1));
fprintf('2、实际落点z与雷达预示落点z间误差为%0.3f m\n', ...
    out_z(end)-XYZ(end-2,3));
fprintf('3、实际落点x与遥测预示落点x间误差为%0.3f m\n', ...
    out_x(end)-XYZt_yc(end-2,1));
fprintf('4、实际落点z与遥测预示落点z间误差为%0.3f m\n', ...
    out_z(end)-XYZt_yc(end-2,3));


