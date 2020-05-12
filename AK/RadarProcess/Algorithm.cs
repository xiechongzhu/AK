using Algorithm;
using MathNet.Numerics;
using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
     public class ConstLaunch
    {
        public double R0;
        public double[,] R0_f;
        public double[,] C_e2f;
        public double[,] C_fe2;
        public double[,] we_f;
        public double[,] xyz_e0;
    }


    public class CalculateOutput
    {
        public double t_x;
        public double t_y;
        public double t_z;
        public double t_range;
        public double flighttime;
        public double x;
        public double y;
        public double z;
        public double vx;
        public double vy;
        public double vz;
    }

    public class Algorithm
    {
        MatLab matlab = new MatLab();

        public ConstLaunch calc_const_launch_fsx(double B0, double lambda0, double h0, double A0)
        {
            double[] geo0 = new double[] { B0, lambda0, h0 };
            MWArray mwGeo0 = new MWNumericArray(geo0);
            double[] a0 = new double[] { A0 };
            MWArray mwA0 = new MWNumericArray(a0);
            MWArray[] argsOut = matlab.calc_const_launch(6, mwGeo0, mwA0);
            return new ConstLaunch
            {
                R0 = ((double[,])(argsOut[0].ToArray()))[0, 0],
                R0_f = (double[,])argsOut[1].ToArray(),
                C_e2f = (double[,])argsOut[2].ToArray(),
                C_fe2 = (double[,])argsOut[3].ToArray(),
                we_f = (double[,])argsOut[4].ToArray(),
                xyz_e0 = (double[,])argsOut[5].ToArray()
            };
        }

        private CalculateOutput calc_target_ld(double lambda0, double x, double y, double z, double vx, double vy, double vz, ConstLaunch launchFsx, double h_end)
        {
            MWArray mwLambda0 = new MWNumericArray(1, 1, new double[] { lambda0 / 180 * Math.PI });
            MWArray mwNavNow = new MWNumericArray(1, 6, new double[] { x, y, z, vx, vy, vz });
            MWArray[] argsOut = matlab.calc_target_ld(6, mwLambda0, mwNavNow, new MWNumericArray(launchFsx.R0), new MWNumericArray(launchFsx.R0_f),
                new MWNumericArray(launchFsx.C_e2f), new MWNumericArray(launchFsx.C_fe2), new MWNumericArray(launchFsx.we_f), 
                new MWNumericArray(launchFsx.xyz_e0), new MWNumericArray(h_end));

            return new CalculateOutput
            {
                t_x = ((double[,])(argsOut[0].ToArray()))[0, 0],
                t_y = ((double[,])(argsOut[1].ToArray()))[0, 0],
                t_z = ((double[,])(argsOut[4].ToArray()))[0, 0],
                t_range = ((double[,])(argsOut[3].ToArray()))[0, 0],
                flighttime = ((double[,])(argsOut[5].ToArray()))[0, 0]
            };
        }


        public void CalcResultLd(double lambda0, double x, double y, double z, double vx, double vy, double vz, ConstLaunch launchFsx, double h_end,
            out FallPoint fallPoint, out double fallTime, out double distance)
        {
            CalculateOutput output = calc_target_ld(lambda0, x, y, z, vx, vy, vz, launchFsx, h_end);
            fallPoint = new FallPoint {
                x = output.t_z,
                y = output.t_x
            };
            distance = output.t_range;
            fallTime = output.flighttime;
        }

        private CalculateOutput calc_target_yc(double latitude, double longitudey, double height, double speedEast, double speedNorth,
            double speedSky, ConstLaunch launchFsx, double h_end)
        {
            MWArray mwNavNow = new MWNumericArray(1, 6, new double[] { latitude, longitudey, height, speedEast, speedNorth, speedSky });
            MWArray[] argsOut = matlab.calc_target_yc(8, mwNavNow, new MWNumericArray(launchFsx.R0), new MWNumericArray(launchFsx.R0_f), 
                new MWNumericArray(launchFsx.xyz_e0), new MWNumericArray(launchFsx.C_e2f), new MWNumericArray(launchFsx.C_fe2), 
                new MWNumericArray(launchFsx.we_f), new MWNumericArray(h_end));
            return new CalculateOutput
            {
                t_x = ((double[,])(argsOut[2].ToArray()))[0, 0],
                t_y = ((double[,])(argsOut[3].ToArray()))[0, 0],
                t_z = ((double[,])(argsOut[6].ToArray()))[0, 0],
                t_range = ((double[,])(argsOut[5].ToArray()))[0, 0],
                flighttime = ((double[,])(argsOut[7].ToArray()))[0, 0],
                x = ((double[,])(argsOut[0].ToArray()))[0, 0],
                y = ((double[,])(argsOut[0].ToArray()))[0, 1],
                z = ((double[,])(argsOut[0].ToArray()))[0, 2],
                vx = ((double[,])(argsOut[1].ToArray()))[0, 0],
                vy = ((double[,])(argsOut[1].ToArray()))[1, 0],
                vz = ((double[,])(argsOut[1].ToArray()))[2, 0]
            };
        }

        public void CalcResultYc(double latitude, double longitudey, double height, double speedEast, double speedNorth,
            double speedSky, ConstLaunch launchFsx, double h_end, out FallPoint fallPoint, out double fallTime, out double distance,
            out double x, out double y, out double z, out double vx, out double vy, out double vz)
        {
            CalculateOutput output = calc_target_yc(latitude, longitudey, height, speedEast, speedNorth, speedSky, launchFsx, h_end);
            fallPoint = new FallPoint
            {
                x = output.t_z,
                y = output.t_x
            };
            distance = output.t_range;
            fallTime = output.flighttime;
            x = output.x;
            y = output.y;
            z = output.z;
            vx = output.vx;
            vy = output.vy;
            vz = output.vz;
        }

        public static FallPoint CalcIdeaPointOfFall()
        {
            return new FallPoint
            {
                x = 0,
                y = Config.GetInstance().flightshot
            };
        }

        public static double GetSpeed(double[] x, double[] y, double x0)
        {
            double[] polynomial = Fit.Polynomial(x, y, 3);
            return (3 * polynomial[3] * Math.Pow(x0, 2) + 2 * polynomial[2] * x0 + polynomial[1])*1000;
        }
    }
}
