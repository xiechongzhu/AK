using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
    class Constant
    {
        static public readonly double PI = 3.14159265358979323846264;
        static public readonly double EE = 0.0818191908425;
        static public readonly double WE = 7.292115E-5;
        static public readonly double RC = 6378140.0;
        static public readonly double AE = 6378140.0;
        static public readonly double BE = 6356755.0;
        static public readonly double miu = 3.986004418e14;
        static public readonly double we = 7.292115e-5;
    }

    public class DHINPUT
    {
        public double[] dxyz_f = new double[3];//位置
        public double[] dV_f = new double[3];//速度
    }

    public class ConstantCalculateInput
    {
        public double dLambd0_c;   //经度
        public double dB0;         //纬度
        public double dA0;         //发射方位角
        public double dh0;
    }

    public class ConstantCalculateOutput
    {
        public double dR0;
        public double[,] dGe = new double[3, 3];
        public double[,] dGfe = new double[3,3];
        public double[] dRf0 = new double[3];
        public double[] dWef = new double[3];
        public double[] xyz_e0 = new double[3];
    }

    public class CalculateInput
    {
        public double dh_end;//装载的参数
        public double dlambda_0;//装载的参数
        public DHINPUT dhIn = new DHINPUT();
        public ConstantCalculateOutput constantCalculateOutput = new ConstantCalculateOutput();//算法1输出结果
    }

    public class CalculateOutput
    {
        public double[] dxyz = new double[3];
        public double dT;//剩余飞行时间
        public double dt_range; //预示落点射程RC
        public double dt_z;//预示落点侧偏
    }

    public class Algorithm
    {
        static void MatrixMul(double[,] a_matrix, int r1, int c1, double[,] b_matrix, int c2, ref double[,] d_rmatrix)
        {
            for (int i = 0; i < r1; i++)
            {
                for (int k = 0; k < c2; k++)
                {
                    d_rmatrix[i,k] = 0;
                    for (int l = 0; l < c1; l++)
                    {
                        d_rmatrix[i, k] += (a_matrix[i,l] * b_matrix[l,k]);
                    }
                }
            }
        }

        public static ConstantCalculateOutput ConstantCalculate(ConstantCalculateInput input)
        {
            ConstantCalculateOutput output = new ConstantCalculateOutput();
            double[] dtmparray = new double[5];
            double[] dsindata = new double[5];
            double[] dcosdata = new double[5];
            //	角度转换成弧度
            dtmparray[0] = Constant.PI * input.dLambd0_c / 180.0;
            dtmparray[1] = Constant.PI * input.dA0 / 180.0;
            dtmparray[2] = Constant.PI * input.dB0 / 180.0;
            //	计算3个参数的正弦函数结果sin(λ),sin(A0),sin(B0)
            dsindata[0] = Math.Sin(dtmparray[0]);
            dsindata[1] = Math.Sin(dtmparray[1]);
            dsindata[2] = Math.Sin(dtmparray[2]);
            //	计算3个参数的余弦函数结果cos(λ),cos(A0),cos(B0)
            dcosdata[0] = Math.Cos(dtmparray[0]);
            dcosdata[1] = Math.Cos(dtmparray[1]);
            dcosdata[2] = Math.Cos(dtmparray[2]);

            //求Rn0
            double dpowee = Constant.EE * dsindata[2];
            double du0 = Math.Pow(dpowee, 2);
            double dRn0 = Constant.RC / Math.Sqrt(1.0f - du0);

            //求Xeo，Ye0，Ze0
            //	计算x_e0 = (dRn0 + input.dh0) * cos(B0) * cos(lambda0)
            output.xyz_e0[0] = (dRn0 + input.dh0) * dcosdata[2] * dcosdata[0];
            //计算y_e0 = (dRn0 + input.dh0) * cos(B0) * sin(lambda0)
            output.xyz_e0[1] = (dRn0 + input.dh0) * dcosdata[2] * dsindata[0];
            //计算z_e0
            output.xyz_e0[2] = ((1 - Math.Pow(Constant.EE, 2)) * dRn0 + input.dh0) * dsindata[2];
            //arctan[(AE/BE)的平方*tanBO]
            double dmf0 = Math.Atan(Math.Pow(Constant.BE / Constant.AE, 2) * Math.Tan(dtmparray[2]));
            dtmparray[3] = dmf0;
            dtmparray[4] = dtmparray[2] - dmf0;
            //	计算2个正弦函数结果sin(φ0),sin(μ0)
            dsindata[3] = Math.Sin(dtmparray[3]);
            dsindata[4] = Math.Sin(dtmparray[4]);
            //	计算2个余弦函数结果cos(φ0),cos(μ0)
            dcosdata[3] = Math.Cos(dtmparray[3]);
            dcosdata[4] = Math.Cos(dtmparray[4]);
            double dtmpcal = Math.Pow(Constant.AE * dsindata[3], 2) + Math.Pow(Constant.BE * dcosdata[3], 2);
            //	计算R0
            output.dR0 = input.dh0 + (Constant.AE * Constant.BE) / Math.Sqrt(dtmpcal);
            //	计算向量Rf0
            output.dRf0[0] = -output.dR0 * dsindata[4] * dcosdata[1];
            output.dRf0[1] = output.dR0 * dcosdata[4];
            output.dRf0[2] = output.dR0 * dsindata[4] * dsindata[1];
            //	计算矩阵Ge
            output.dGe[0, 0] = -dsindata[1] * dsindata[0] - dcosdata[1] * dsindata[2] * dcosdata[0];
            output.dGe[0, 1] = dsindata[1] * dcosdata[0] - dcosdata[1] * dsindata[2] * dsindata[0];
            output.dGe[0, 2] = dcosdata[1] * dcosdata[2];// dcosdata[1] * dcosdata[0];	2019/12/23
            output.dGe[1, 0] = dcosdata[2] * dcosdata[0];
            output.dGe[1, 1] = dcosdata[2] * dsindata[0];
            output.dGe[1, 2] = dsindata[2];
            output.dGe[2, 0] = -dcosdata[1] * dsindata[0] + dsindata[1] * dsindata[2] * dcosdata[0];
            output.dGe[2, 1] = dcosdata[1] * dcosdata[0] + dsindata[1] * dsindata[2] * dsindata[0];
            output.dGe[2, 2] = -dsindata[1] * dcosdata[2];
            //	计算矩阵Gfe
            output.dGfe[0, 0] = dcosdata[1] * dcosdata[2];
            output.dGfe[0, 1] = dsindata[2];
            output.dGfe[0, 2] = -dcosdata[2] * dsindata[1];
            output.dGfe[1, 0] = -dsindata[2] * dcosdata[1];
            output.dGfe[1, 1] = dcosdata[2];
            output.dGfe[1, 2] = dsindata[2] * dsindata[1];
            output.dGfe[2, 0] = dsindata[1];
            output.dGfe[2, 1] = 0;
            output.dGfe[2, 2] = dcosdata[1];
            //	计算向量We_f
            output.dWef[0] = Constant.WE * dcosdata[2] * dcosdata[1];
            output.dWef[1] = Constant.WE * dsindata[2];
            output.dWef[2] = -Constant.WE * dcosdata[2] * dsindata[1];

            return output;
        }

        static public bool CalculateFlyParams(CalculateInput input, ref CalculateOutput output)
        {
            double[,] dTemp = new double[,] { { 0.0 }, { 0.0 }, {0.0 } };
            double dTemp1 = 0.0;
             double dTemp2 = 0.0;
            double dTemp3 = 0.0;
             double dTemp4 = 0.0;
            /******************************************公式1*****************************************/
            //	计算向量rk
            double[,] drk_v = new double[,] { { 0.0 }, { 0.0 }, { 0.0 } };
            drk_v[0,0] = input.dhIn.dxyz_f[0] + input.constantCalculateOutput.dRf0[0];
            drk_v[1,0] = input.dhIn.dxyz_f[1] + input.constantCalculateOutput.dRf0[1];
            drk_v[2,0] = input.dhIn.dxyz_f[2] + input.constantCalculateOutput.dRf0[2];
            /******************************************公式2*****************************************/
            double drk_0 = Math.Sqrt(Math.Pow(drk_v[0,0], 2) + Math.Pow(drk_v[1,0], 2) + Math.Pow(drk_v[2,0], 2));//得到RK
            /******************************************公式3*****************************************/
            //计算点积和
            dTemp1 = drk_v[0,0] * input.constantCalculateOutput.dRf0[0] + drk_v[1,0] * input.constantCalculateOutput.dRf0[1]
                    + drk_v[2,0] * input.constantCalculateOutput.dRf0[2];
            dTemp2 = drk_0 * input.constantCalculateOutput.dR0;
            dTemp3 = dTemp1 / dTemp2;
            double dTheta_k = Math.Asin(dTemp3);
            /******************************************公式4*****************************************/
            double dBk = Math.Atan(Math.Pow(Constant.BE / Constant.AE, 2) * Math.Tan(dTheta_k));
            /******************************************公式5*****************************************/
            double[,] d_xyz = new double[,]{ { 0.0 }, { 0.0 }, { 0.0 } };
            MatrixMul(input.constantCalculateOutput.dGfe, 3, 3, drk_v, 1, ref d_xyz);
            /******************************************公式6*****************************************/
            //计算β
            double dlambda = 0.0;
            if (d_xyz[1,0] > 0.0)//y>0
            {
                dlambda = Math.Atan(d_xyz[2,0] / d_xyz[1,0]);
            }
            else if (d_xyz[1,0] < 0.0)//y<0
            {
                if (d_xyz[2,0] < 0.0)
                {
                    dlambda = Math.Atan(d_xyz[2,0] / d_xyz[1,0]) - Constant.PI;
                }
                else
                {
                    dlambda = Math.Atan(d_xyz[2,0] / d_xyz[1,0]) + Constant.PI;
                }
            }
            else//y==0
            {
                dlambda = 0;
            }
            /******************************************公式7*****************************************/
            double lambda_k = input.dlambda_0 + dlambda;
            /******************************************公式8*****************************************/
            dTemp1 = Math.Pow(Constant.AE, 2) * Math.Pow(Math.Sin(dTheta_k), 2) + Math.Pow(Constant.BE, 2) * Math.Pow(Math.Cos(dTheta_k), 2);
            double dRk = Constant.AE * Constant.BE / Math.Sqrt(dTemp1);
            /******************************************公式9*****************************************/
            double dHk = drk_0 - dRk;
            /******************************************公式10*****************************************/
            //	计算矩阵Ge2nue
            double[,] dGe2nue = new double[,] { { 0.0, 0.0, 0.0 }, { 0.0, 0.0, 0.0 }, { 0.0, 0.0, 0.0 } };
            dGe2nue[0,0] =  Math.Cos(dBk);
            dGe2nue[0,1] = -Math.Sin(dBk) * Math.Cos(dlambda);
            dGe2nue[0,2] = -Math.Sin(dBk) * Math.Sin(dlambda);
            dGe2nue[1,0] =  Math.Sin(dBk);
            dGe2nue[1,1] =  Math.Cos(dBk) * Math.Cos(dlambda);
            dGe2nue[1,2] =  Math.Cos(dBk) * Math.Sin(dlambda);
            dGe2nue[2,0] =  0;
            dGe2nue[2,1] = -Math.Sin(dlambda);
            dGe2nue[2,2] =  Math.Cos(dlambda);
            /******************************************公式11*****************************************/
            //	计算矩阵Gfnue
            double[,] dGfnue=new double[,] { { 0.0, 0.0, 0.0 }, { 0.0, 0.0, 0.0 }, { 0.0, 0.0, 0.0 } };
            MatrixMul(dGe2nue, 3, 3, input.constantCalculateOutput.dGfe, 3, ref dGfnue);
            /******************************************公式12*****************************************/
            //	计算矩阵dV_nue
            double[,] dV_nue = new double[3,1];
            MatrixMul(dGfnue, 3, 3, new double[,] { { input.dhIn.dV_f[0] }, { input.dhIn.dV_f[1] }, {input.dhIn.dV_f[2] } }, 1, ref dV_nue);
            /******************************************公式13*****************************************/
            double[,] dtmpGnn = new double[,] { { 0.0 }, { 0.0 }, {0.0 } };
            double[] dV_XA = new double[] { 0.0, 0.0, 0.0 };
            //	计算向量叉乘a*b=（a.y*b.z-b.y*a.z , b.x*a.z-a.x*b.z  , a.x*b.y-b.x*a.y）
            dTemp[0,0] = input.constantCalculateOutput.dWef[1] * drk_v[2,0] - drk_v[1,0] * input.constantCalculateOutput.dWef[2];
            dTemp[1,0] = -input.constantCalculateOutput.dWef[0] * drk_v[2,0] + drk_v[0,0] * input.constantCalculateOutput.dWef[2];
            dTemp[2,0] = input.constantCalculateOutput.dWef[0] * drk_v[1,0] - drk_v[0,0] * input.constantCalculateOutput.dWef[1];
            //矩阵叉乘
            //	dtmpGnn[0] = dGfnue[0]*dWRk[0]+dGfnue[1]*dWRk[1]+dGfnue[2]*dWRk[2];//向量点乘
            MatrixMul(dGfnue, 3, 3, dTemp, 1, ref dtmpGnn);
            //	计算VnA,VuA,VeA
            dV_XA[0] = dV_nue[0, 0] + dtmpGnn[0, 0];
            dV_XA[1] = dV_nue[1, 0] + dtmpGnn[1, 0];
            dV_XA[2] = dV_nue[2, 0] + dtmpGnn[2, 0];
            /******************************************公式14*****************************************/
            if (Math.Abs(dV_XA[0]) < 1E-3)
            {//当dvnpow绝对值小于1E-3时
                dV_XA[0] += 1E-3;
            }
            //	计算VkA
            double dVkA = Math.Sqrt(Math.Pow(dV_XA[0], 2) + Math.Pow(dV_XA[1], 2) + Math.Pow(dV_XA[2], 2));
            /****************************************公式15*******************************************/
            double dthetakA = 0.0; 
            if (dVkA != 0.0)
            {
                dthetakA = Math.Asin(dV_XA[1] / dVkA);
            }
            else
            {
                dthetakA = Constant.PI / 2;
            }
            /****************************************公式16*******************************************/
            double dAkA = 0.0;
            if (dV_XA[0] > 0.0f)
            {
                dAkA = Math.Atan(dV_XA[2] / dV_XA[0]);
            }
            else if (dV_XA[2] < 0.0f)
            {
                dAkA = Math.Atan(dV_XA[2] / dV_XA[0]) - Constant.PI;
            }
            else
            {
                dAkA = Math.Atan(dV_XA[2] / dV_XA[0]) + Constant.PI;
            }
            /****************************************公式17*******************************************/
            double dRc = drk_0 - dHk + input.dh_end;//Hk为公式9输出参数，Hc为装订参数
            if (drk_0 < dRc)//步骤g
            {
                output.dt_range = 0.0;
                output.dt_z = 0.0;
                return false;
            }
            /****************************************公式18*******************************************/
            double dVk = Math.Pow(dVkA, 2) * drk_0 / Constant.miu;
            /****************************************公式19*******************************************/
            dTemp1 = dVk * (dVk - 2);
            dTemp2 = Math.Pow(Math.Cos(dthetakA), 2);
            dTemp3 = 1 + dTemp1 * dTemp2;
            double de = Math.Sqrt(dTemp3);

            if (de >= 1.0f)
            {
                output.dt_range = 0.0;
                output.dt_z = 0.0;
                return false;
            }
            /****************************************公式20*******************************************/
            dTemp1 = drk_0 * dVk * dTemp2;   //
            dTemp3 = dTemp1 / (1 + de);

            if (dTemp3 >= dRc)
            {
                output.dt_range = 0.0;
                output.dt_z = 0.0;
                return false;
            }
            /****************************************公式21*******************************************/
            dTemp1 = Math.Sin(dthetakA) / Math.Cos(dthetakA);
            double dA = 2.0f * dRc * (1.0f + Math.Pow(dTemp1, 2)) - dVk * (dRc + drk_0);//dthetakA公式20中得到, dVk公式22中得到 -
            double dB = 2.0f * dVk * dRc * dTemp1;
            double dC = dVk * (dRc - drk_0);
            double dD = dB + Math.Sqrt(Math.Pow(dB, 2) - 4 * dA * dC);
            /****************************************公式22*******************************************/
            dTemp1 = 4 * Math.Pow(dA, 2) + Math.Pow(dD, 2);
            dTemp2 = dD / Math.Sqrt(dTemp1);
            double dbeta_C = 2 * Math.Sin(dTemp2);
            /****************************************公式23*******************************************/
            double dA_T = Math.Sqrt(Math.Pow(drk_0, 3) / Constant.miu * Math.Pow(2 - dVk, 3));  //公式2得到rk,公式18得到Vk
            double dB_T = Math.Acos((dRc * (2 - dVk) - drk_0) / de * drk_0);
            double dC_T = Math.Acos(1 - dVk / de);
            /****************************************公式24*******************************************/
            dTemp1 = 1.0f * de * Math.Sin(dB_T) + dB_T;//e*sin(B_T)+B_T
            dTemp2 = 1.0f * de * Math.Sin(dC_T) + dC_T;//e*sin(C_T)+C_T
            //此处公式计算与Matlab的计算方式不一致
            dTemp3 = 1.0f * dA_T * dTemp1;//A_T *(e*sin(B_T)+B_T)
            dTemp4 = 1.0f * dA_T * dTemp2;
            if (dthetakA < 0.0f)//公式15得到Ka
            {
                output.dT = dTemp3 - dTemp4;
            }
            else
            {
                output.dT = dTemp3 + dTemp4;
            }
            /****************************************公式25*******************************************/
            double dBc = Math.Asin(Math.Cos(dbeta_C) * Math.Sin(dBk) + Math.Sin(dbeta_C) * Math.Cos(dBk) * Math.Cos(dAkA));//
            /****************************************公式26*******************************************/
            dTemp1 = Math.Sin(dbeta_C) * Math.Sin(dAkA) / Math.Cos(dBc);
            dTemp2 = Math.Sin(dTemp1);
            double delta_lambda_A0 = Math.Asin(Math.Sin(dbeta_C) * Math.Sin(dAkA) / Math.Cos(dBc));
            /****************************************公式27*******************************************/
            double dbeta_CC = Math.Acos(Math.Sin(dBk) * Math.Sin(dBc) + Math.Cos(dBk) * Math.Cos(dBc) * Math.Cos(delta_lambda_A0));//
            /****************************************公式28*******************************************/
            double delta_lambda_A = 0.0;
            if (Math.Abs(dbeta_CC - dbeta_C) >= 0.001f)
            {
                delta_lambda_A = Constant.PI - delta_lambda_A0;
            }
            else
            {
                delta_lambda_A = delta_lambda_A0;
            }
            /****************************************公式29*******************************************/
            dTemp1 = delta_lambda_A + lambda_k;//lambda_k为公式7输出
            double lambda_C = dTemp1 - Constant.we * output.dT;
            /****************************************公式30*******************************************/
            double dRnc = Constant.RC / Math.Sqrt(1 - Math.Pow(Constant.EE, 2) * Math.Pow(Math.Sin(dBc), 2));
            /****************************************公式31*******************************************/
            double[] d_xyz_eC = new double[]{ 0.0, 0.0, 0.0 };
            d_xyz_eC[0] = (dRnc + input.dh_end) * Math.Cos(dBc) * Math.Cos(lambda_C);//Bc来自于公式30
            d_xyz_eC[1] = (dRnc + input.dh_end) * Math.Cos(dBc) * Math.Sin(lambda_C);//Bc来自于公式30
            d_xyz_eC[2] = ((1 - Math.Pow(Constant.EE, 2)) * dRnc + input.dh_end) * Math.Sin(dBc);
            /****************************************公式32*******************************************/
            double[,] d_xyz_fC = new double[,] { { 0.0 }, { 0.0 }, { 0.0 } };
            dTemp[0,0] = d_xyz_eC[0] - input.constantCalculateOutput.xyz_e0[0];
            dTemp[1,0] = d_xyz_eC[1] - input.constantCalculateOutput.xyz_e0[1];
            dTemp[2,0] = d_xyz_eC[2] - input.constantCalculateOutput.xyz_e0[2];
            MatrixMul(input.constantCalculateOutput.dGe, 3, 3, dTemp, 1, ref d_xyz_fC);
            output.dt_z = d_xyz_fC[2,0];
            /****************************************公式33*******************************************/
            double[] drc_f = new double[]{ 0.0, 0.0, 0.0 };
            drc_f[0] = d_xyz_fC[0, 0] + input.constantCalculateOutput.dRf0[0];
            drc_f[1] = d_xyz_fC[1, 0] + input.constantCalculateOutput.dRf0[1];
            drc_f[2] = d_xyz_fC[2, 0] + input.constantCalculateOutput.dRf0[2];
            /****************************************公式34*******************************************/
            double dr = Math.Sqrt(Math.Pow(drc_f[0], 2) + Math.Pow(drc_f[1], 2) + Math.Pow(drc_f[2], 2));
            /****************************************公式35*******************************************/
            double dBetaOC = 0.0;
            //计算点积和
            dTemp1 = drc_f[0] * input.constantCalculateOutput.dRf0[0] + drc_f[1] * input.constantCalculateOutput.dRf0[1]
                    + drc_f[2] * input.constantCalculateOutput.dRf0[2];
            dTemp2 = dr * input.constantCalculateOutput.dR0;
            dTemp3 = dTemp1 / dTemp2;
            if (dTemp3 >= 1.0f)//此处参见maltab修改
            {
                dBetaOC = 0.0f;
            }
            else
            {
                dBetaOC = Math.Acos(dTemp3);
            }

            /****************************************公式36*******************************************/
            output.dt_range = dBetaOC * input.constantCalculateOutput.dR0;

            return true;
        }

        public static bool CalcResult(ConstantCalculateOutput constantCalculateOutput, double x, double y, double z,
            double vx, double vy, double vz, double dhEnd, double dlambda_0, out FallPoint fallPoint, out double fallTime)
        {
            CalculateInput input = new CalculateInput
            {
                dh_end = dhEnd,
                dlambda_0 = dlambda_0,
                dhIn = new DHINPUT
                {
                    dxyz_f = new double[]{x, y, z},
                    dV_f = new double[] {vx, vy, vz}
                },
                constantCalculateOutput = constantCalculateOutput
            };
            CalculateOutput output = new CalculateOutput();
            if (!CalculateFlyParams(input, ref output))
            {
                fallPoint = null;
                fallTime = 0;
                return false;
            }
            fallPoint = new FallPoint { 
                x = output.dxyz[0],
                y = output.dxyz[1]
            };
            fallTime = output.dT;
            return true;
        }

        public static FallPoint CalcIdeaPointOfFall(double shotLength)
        {
            return new FallPoint
            {
                x = 0,
                y = shotLength
            };
        }
    }
}
