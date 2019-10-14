using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public static class Helper
    {
        public static Random RandomNumberGenerator = new Random();

        public static void WaitSeconds(double seconds)
        {
            double durationTicks = Math.Round(seconds * Stopwatch.Frequency);

            Stopwatch sw = Stopwatch.StartNew();

            while (sw.ElapsedTicks < durationTicks) { };
        }
    }
}
