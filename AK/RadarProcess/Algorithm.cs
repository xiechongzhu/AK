﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
    public class Algorithm
    {
        public static FallPoint CalcIdealPointOfFall()
        {
            Random random = new Random();
            return new FallPoint
            {
                x = random.NextDouble() * 100,
                y = random.NextDouble() * 100
            };
        }
    }
}