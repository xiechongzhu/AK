/******************************************************************* 
* @brief : 算法类代码 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using Algorithm;
using MathNet.Numerics;
using MathWorks.MATLAB.NET.Arrays;
using System;

namespace RadarProcess
{
    /// <summary>
    /// 常量计算结果
    /// </summary>
     public class ConstLaunch
     {
        /// <summary>
        /// R0
        /// </summary>
        public double R0;
        /// <summary>
        /// R0_f
        /// </summary>
        public double[,] R0_f;
        /// <summary>
        /// C_e2f
        /// </summary>
        public double[,] C_e2f;
        /// <summary>
        /// C_fe2
        /// </summary>
        public double[,] C_fe2;
        /// <summary>
        /// we_f
        /// </summary>
        public double[,] we_f;
        /// <summary>
        /// xyz_e0
        /// </summary>
        public double[,] xyz_e0;
     }

    /// <summary>
    /// 落点计算结果
    /// </summary>
    public class CalculateOutput
    {
        /// <summary>
        /// t_x
        /// </summary>
        public double t_x;
        /// <summary>
        /// t_y
        /// </summary>
        public double t_y;
        /// <summary>
        /// t_z
        /// </summary>
        public double t_z;
        /// <summary>
        /// t_range
        /// </summary>
        public double t_range;
        /// <summary>
        /// flighttime
        /// </summary>
        public double flighttime;
        /// <summary>
        /// x
        /// </summary>
        public double x;
        /// <summary>
        /// y
        /// </summary>
        public double y;
        /// <summary>
        /// z
        /// </summary>
        public double z;
        /// <summary>
        /// vx
        /// </summary>
        public double vx;
        /// <summary>
        /// vy
        /// </summary>
        public double vy;
        /// <summary>
        /// vy
        /// </summary>
        public double vz;
    }

    /// <summary>
    /// 落点计算类
    /// </summary>
    public class Algorithm
    {
        /// <summary>
        /// matlab算法实例
        /// </summary>
        MatLab matlab = new MatLab();

        /// <summary>
        /// calc_const_launch_fsx
        /// </summary>
        /// <param name="B0"></param>
        /// <param name="lambda0"></param>
        /// <param name="h0"></param>
        /// <param name="A0"></param>
        /// <returns></returns>
        public ConstLaunch calc_const_launch_fsx(double B0, double lambda0, double h0, double A0)
        {
            //Array geo0
            double[] geo0 = new double[] { B0, lambda0, h0 };
            MWArray mwGeo0 = new MWNumericArray(geo0);
            //Array a0
            double[] a0 = new double[] { A0 };
            MWArray mwA0 = new MWNumericArray(a0);
            //Array argOut
            MWArray[] argsOut = matlab.calc_const_launch(6, mwGeo0, mwA0);
            //构造返回值
            return new ConstLaunch
            {
                R0 = ((double[,])(argsOut[0].ToArray()))[0, 0],     //R0
                R0_f = (double[,])argsOut[1].ToArray(),             //R0_f
                C_e2f = (double[,])argsOut[2].ToArray(),            //C_e2f
                C_fe2 = (double[,])argsOut[3].ToArray(),            //C_fe2
                we_f = (double[,])argsOut[4].ToArray(),             //we_f
                xyz_e0 = (double[,])argsOut[5].ToArray()            //xyz_e0
            };
        }

        /// <summary>
        /// calc_target_ld
        /// </summary>
        /// <param name="lambda0"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="vx"></param>
        /// <param name="vy"></param>
        /// <param name="vz"></param>
        /// <param name="launchFsx"></param>
        /// <param name="h_end"></param>
        /// <returns></returns>
        private CalculateOutput calc_target_ld(double lambda0, double x, double y, double z, double vx, double vy, double vz, ConstLaunch launchFsx, double h_end)
        {
            //mwLambda0
            MWArray mwLambda0 = new MWNumericArray(1, 1, new double[] { lambda0 / 180 * Math.PI });
            //mwNavNow
            MWArray mwNavNow = new MWNumericArray(1, 6, new double[] { x, y, z, vx, vy, vz });
            //argsOut
            MWArray[] argsOut = matlab.calc_target_ld(6, mwLambda0, mwNavNow, new MWNumericArray(launchFsx.R0), new MWNumericArray(launchFsx.R0_f),
                new MWNumericArray(launchFsx.C_e2f), new MWNumericArray(launchFsx.C_fe2), new MWNumericArray(launchFsx.we_f), 
                new MWNumericArray(launchFsx.xyz_e0), new MWNumericArray(h_end));

            //返回值
            return new CalculateOutput
            {
                t_x = ((double[,])(argsOut[0].ToArray()))[0, 0],        //t_x
                t_y = ((double[,])(argsOut[1].ToArray()))[0, 0],        //t_y
                t_z = ((double[,])(argsOut[4].ToArray()))[0, 0],        //t_z
                t_range = ((double[,])(argsOut[3].ToArray()))[0, 0],    //t_range
                flighttime = ((double[,])(argsOut[5].ToArray()))[0, 0]  //flighttime
            };
        }

        /// <summary>
        /// CalcResultLd
        /// </summary>
        /// <param name="lambda0"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="vx"></param>
        /// <param name="vy"></param>
        /// <param name="vz"></param>
        /// <param name="launchFsx"></param>
        /// <param name="h_end"></param>
        /// <param name="fallPoint"></param>
        /// <param name="fallTime"></param>
        /// <param name="distance"></param>
        public void CalcResultLd(double lambda0, double x, double y, double z, double vx, double vy, double vz, ConstLaunch launchFsx, double h_end,
            out FallPoint fallPoint, out double fallTime, out double distance)
        {
            //计算落点
            CalculateOutput output = calc_target_ld(lambda0, x, y, z, vx, vy, vz, launchFsx, h_end);
            fallPoint = new FallPoint {
                x = output.t_z,
                y = output.t_x
            };
            //射程
            distance = output.t_range;
            //落地时间
            fallTime = output.flighttime;
        }

        /// <summary>
        /// calc_target_yc
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitudey"></param>
        /// <param name="height"></param>
        /// <param name="speedEast"></param>
        /// <param name="speedNorth"></param>
        /// <param name="speedSky"></param>
        /// <param name="launchFsx"></param>
        /// <param name="h_end"></param>
        /// <returns></returns>
        private CalculateOutput calc_target_yc(double latitude, double longitudey, double height, double speedEast, double speedNorth,
            double speedSky, ConstLaunch launchFsx, double h_end)
        {
            //mwNavNow
            MWArray mwNavNow = new MWNumericArray(1, 6, new double[] { latitude, longitudey, height, speedEast, speedNorth, speedSky });
            //argsOut
            MWArray[] argsOut = matlab.calc_target_yc(8, mwNavNow, new MWNumericArray(launchFsx.R0), new MWNumericArray(launchFsx.R0_f), 
                new MWNumericArray(launchFsx.xyz_e0), new MWNumericArray(launchFsx.C_e2f), new MWNumericArray(launchFsx.C_fe2), 
                new MWNumericArray(launchFsx.we_f), new MWNumericArray(h_end));
            //计算遥测落点
            return new CalculateOutput
            {
                t_x = ((double[,])(argsOut[2].ToArray()))[0, 0],           //t_x
                t_y = ((double[,])(argsOut[3].ToArray()))[0, 0],           //t_y
                t_z = ((double[,])(argsOut[6].ToArray()))[0, 0],           //t_z
                t_range = ((double[,])(argsOut[5].ToArray()))[0, 0],       //t_range
                flighttime = ((double[,])(argsOut[7].ToArray()))[0, 0],    //flighttime
                x = ((double[,])(argsOut[0].ToArray()))[0, 0],             //x 
                y = ((double[,])(argsOut[0].ToArray()))[0, 1],             //y 
                z = ((double[,])(argsOut[0].ToArray()))[0, 2],             //z 
                vx = ((double[,])(argsOut[1].ToArray()))[0, 0],            //vx
                vy = ((double[,])(argsOut[1].ToArray()))[1, 0],            //vy
                vz = ((double[,])(argsOut[1].ToArray()))[2, 0]             //vz
            };
        }

        /// <summary>
        /// CalcResultYc
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitudey"></param>
        /// <param name="height"></param>
        /// <param name="speedEast"></param>
        /// <param name="speedNorth"></param>
        /// <param name="speedSky"></param>
        /// <param name="launchFsx"></param>
        /// <param name="h_end"></param>
        /// <param name="fallPoint"></param>
        /// <param name="fallTime"></param>
        /// <param name="distance"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="vx"></param>
        /// <param name="vy"></param>
        /// <param name="vz"></param>
        public void CalcResultYc(double latitude, double longitudey, double height, double speedEast, double speedNorth,
            double speedSky, ConstLaunch launchFsx, double h_end, out FallPoint fallPoint, out double fallTime, out double distance,
            out double x, out double y, out double z, out double vx, out double vy, out double vz)
        {
            //计算遥测落点
            CalculateOutput output = calc_target_yc(latitude, longitudey, height, speedEast, speedNorth, speedSky, launchFsx, h_end);
            fallPoint = new FallPoint
            {
                x = output.t_z,   //侧偏
                y = output.t_x    //射程
            };
            //射程
            distance = output.t_range;
            //剩余飞行时间
            fallTime = output.flighttime;
            //位置x
            x = output.x;
            //位置y
            y = output.y;
            //位置z
            z = output.z;
            //速度vx
            vx = output.vx;
            //速度vy
            vy = output.vy;
            //速度vz
            vz = output.vz;
        }

        /// <summary>
        /// CalcIdeaPointOfFall
        /// </summary>
        /// <returns></returns>
        public static FallPoint CalcIdeaPointOfFall()
        {
            return new FallPoint
            {
                x = 0,                              //侧偏
                y = Config.GetInstance().flightshot //射程
            };
        }

        /// <summary>
        /// GetSpeed
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x0"></param>
        /// <returns></returns>
        public static double GetSpeed(double[] x, double[] y, double x0)
        {
            double[] polynomial = Fit.Polynomial(x, y, 3);  //拟合曲线
            return (3 * polynomial[3] * Math.Pow(x0, 2) + 2 * polynomial[2] * x0 + polynomial[1])*1000; //计算X0点的值
        }
    }
}
