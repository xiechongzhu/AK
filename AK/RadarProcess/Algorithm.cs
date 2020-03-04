using Algorithm;
using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
     public class ConstLaunchFsx
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
        public double x;
        public double y;
        public double z;
        public double range;
        public double t_range;
        public double t_z;
        public double flighttime;
    }

    public class Algorithm
    {
        MatLab matlab = new MatLab();

        public ConstLaunchFsx calc_const_launch_fsx(double B0, double lambda0, double h0, double A0)
        {
            double[] geo0 = new double[] { B0 / 180 * Math.PI, lambda0 / 180 * Math.PI, h0 };
            MWArray mwGeo0 = new MWNumericArray(geo0);
            double[] a0 = new double[] { A0 / 180 * Math.PI };
            MWArray mwA0 = new MWNumericArray(a0);
            MWArray[] argsOut = matlab.calc_const_launch_fsx(6, mwGeo0, mwA0);
            return new ConstLaunchFsx
            {
                R0 = ((double[,])(argsOut[0].ToArray()))[0, 0],
                R0_f = (double[,])argsOut[1].ToArray(),
                C_e2f = (double[,])argsOut[2].ToArray(),
                C_fe2 = (double[,])argsOut[3].ToArray(),
                we_f = (double[,])argsOut[4].ToArray(),
                xyz_e0 = (double[,])argsOut[5].ToArray()
            };
        }

        private CalculateOutput calc_target_fsx(double lambda0, double x, double y, double z, double vx, double vy, double vz, ConstLaunchFsx launchFsx, double h_end)
        {
            MWArray mwLambda0 = new MWNumericArray(1, 1, new double[] { lambda0 / 180 * Math.PI });
            MWArray mwNavNow = new MWNumericArray(1, 6, new double[] { x, y, z, vx, vy, vz });
            MWArray mwHEnd = new MWNumericArray(1, 1, new double[] { h_end });
            MWArray[] argsOut = matlab.calc_target_fsx(7, mwLambda0, mwNavNow, new MWNumericArray(launchFsx.R0), new MWNumericArray(launchFsx.R0_f),
                new MWNumericArray(launchFsx.C_e2f), new MWNumericArray(launchFsx.C_fe2), new MWNumericArray(launchFsx.we_f), 
                new MWNumericArray(launchFsx.xyz_e0), mwHEnd);

            return new CalculateOutput
            {
                x = ((double[,])(argsOut[0].ToArray()))[0, 0],
                y = ((double[,])(argsOut[1].ToArray()))[0, 0],
                z = ((double[,])(argsOut[2].ToArray()))[0, 0],
                range = ((double[,])(argsOut[3].ToArray()))[0, 0],
                t_range = ((double[,])(argsOut[4].ToArray()))[0, 0],
                t_z = ((double[,])(argsOut[5].ToArray()))[0, 0],
                flighttime = ((double[,])(argsOut[6].ToArray()))[0, 0]
            };
        }


        public void CalcResult(double lambda0, double x, double y, double z, double vx, double vy, double vz, ConstLaunchFsx launchFsx, double h_end,
            out FallPoint fallPoint, out double fallTime, out double distance)
        {
            CalculateOutput output = calc_target_fsx(lambda0, x, y, z, vx, vy, vz, launchFsx, h_end);
            fallPoint = new FallPoint {
                x = output.z,
                y = output.x
            };
            distance = output.t_range;
            fallTime = output.flighttime;
        }

        public static FallPoint CalcIdeaPointOfFall()
        {
            return new FallPoint
            {
                x = 0,
                y = Config.GetInstance().flightshot
            };
        }
    }
}
