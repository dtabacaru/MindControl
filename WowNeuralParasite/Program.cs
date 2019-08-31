using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WoWControlLib;

namespace WowNeuralParasite
{
    static class Program
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        static void Main(string[] args)
        {
            //System.Threading.Thread.Sleep(5000);

            Process p = Process.GetProcessesByName("wow").FirstOrDefault();
            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);
            }

            #region Night Elf Rogue

            // Alterac turtles
            //List<double> xCoordinates = new List<double>(new double[] {36.2, 35.3, 34.2, 32.4, 31.3, 30.8, 29.8 });
            //List<double> yCoordinates = new List<double>(new double[] {23.4, 26.2, 27.8, 30.3, 32, 35.2, 38.7 });

            // Dustwallow turtles
            //List<double> xCoordinates = new List<double>(new double[] {60.4, 60.8,61.2,61.7,61.8 , 61.9, 61.7, 61.5, 61.8, 61.9,61.8, 62.7, 63,63.0, 63.2, 63.6, 64, 64.7, 64.8, 64.7 });
            //List<double> yCoordinates = new List<double>(new double[] {18.7, 19.4,20.6, 21.7,22.9, 25, 27.6, 29.1 ,30.7,  31,31.9, 34.4, 36.3,36.5, 37.6,38.3,  38.6, 40.3, 42, 43.1});


            //List<double> xCoordinates = new List<double>(new double[] { 52.7,51.7,51.0,50.3,50.0,50.1,50.6,51.5,52.0,52.3,52.9,52.7,52.8,53.2,54.0,54.6});
            //List<double> yCoordinates = new List<double>(new double[] { 69.1,69.9,70.6,71.4,72.1,72.9,73.5,73.6,73.5,74.0,73.8,73.0,72.2,70.9,70.3,69.5});

            // 10-14

            //List<double> xCoordinates = new List<double>(new double[] {43.9,44.0, 44.5, 45.7, 45.1, 44.9, 43.8,43.0,42.5,43.4,43.2,42.8,41.9,40.4, 39.3});
            //List<double> yCoordinates = new List<double>(new double[] {39.2,39.0, 37.8, 37.8, 36.6, 35.2, 34.7,33.6,32.6,32.0,30.2,31.7,34.0,34.7, 35.4});

            // 14-19

            //List<double> xCoordinates = new List<double>(new double[] {53.7,52.8,53.2,53.2,52.8,53.8,53.3,52.7,52.6,52.1,51.4,50.5,49.7,49.5,48.5,48.4,48.7,49.7,50.3});
            //List<double> yCoordinates = new List<double>(new double[] {25.6,25.6,26.5,27.0,27.6,27.7,28.9,29.5,28.6,28.2,28.3,27.8,28.4,27.6,27.8,26.1,25.7,25.6,25.6});

            //double[] xs = new double[] { 12.3, 11.9, 13.9, 14.9, 15.5, 17.1, 20.3, 19.3, 19.0, 18.8, 19.2, 20.5, 20.7, 21.9, 23.6, 25.9, 27.3, 28.4, 29.9, 31.7, 33.1, 35.5, 37.8, 39.6, 40.9, 41.9, 43.1, 45.4, 47.7, 51.1, 52.8, 54.7, 57.5, 58.7, 61.8, 63.0, 62.9, 64.4, 67.0, 69.3, 71.7, 71.0, 73.8, 75.2, 76.0 };
            //double[] ys = new double[] { 31.6, 28.9, 28.0, 29.9, 25.4, 25.5, 25.7, 23.6, 25.3, 27.2, 27.8, 27.5, 28.8, 28.8, 27.6, 27.6, 26.3, 25.5, 26.1, 27.0, 23.6, 21.6, 19.5, 20.6, 20.0, 21.0, 18.3, 17.5, 16.7, 17.2, 15.7, 18.7, 20.0, 20.8, 22.8, 22.2, 17.6, 18.6, 20.0, 22.5, 23.1, 21.7, 20.4, 18.8, 18.2 };

            //double[] xs = new double[] { 40.9, 41.9, 43.1, 45.4, 47.4, 47.7, 51.1, 52.8, 54.7, 57.9, 58.7, 61.8, 63.0, 62.9,64.5, 64.4, 67.0, 69.3, 71.7, 71.0, 73.8, 75.2, 76.0, 77.3 };
            //double[] ys = new double[] { 20.0, 21.0, 18.3, 17.5, 18.5, 16.7, 17.2, 15.7, 18.7, 19.8, 20.8, 22.8, 22.2, 17.6,18.9, 19.5, 20.0, 22.5, 23.1, 21.7, 20.4, 18.8, 18.2, 18.1 };


            // new spiders

            //double[] xs = new double[] { 74.0,72.5,70.7,69.8,68.7,67.6,66.7,65.0 };
            //double[] ys = new double[] { 19.6,21.1,21.5,18.5,20.0,20.1,20.9,22.4 };


            //double[] xs = new double[] {68.2, 67.3, 68.2, 67.4, 69.3, 67.5, 66.3, 65.6, 66.0, 66.9, 68.4, 67.4, 65.3, 64.1};
            //d a   aouble[] ys = new double[] {36.3, 35.5, 33.4, 31.9, 30.0, 28.7, 28.8, 31.0, 32.5, 34.6, 33.2, 32.0, 33.6, 33.5};

            // New rogue turtles

            //double[] xs = new double[] { 12.4, 13.9, 14.3, 15.3, 16.3, 17.5, 18.9, 19.1, 20.0, 21.3, 22.7, 23.5, 24.3, 24.9, 27.0, 27.9, 28.7, 29.2, 29.9, 29.8, 30.5, 31.1, 31.2, 31.2, 31.8, 32.0, 32.9, 34.2, 35.4, 35.9 };
            //double[] ys = new double[] { 55.0, 55.0, 53.4, 52.6, 52.4, 52.8, 52.3, 50.8, 49.4, 47.3, 45.9, 45.6, 43.9, 43.6, 42.3, 40.5, 39.7, 38.7, 38.2, 37.3, 36.4, 35.8, 33.8, 32.5, 31.5, 30.1, 29.0, 27.7, 25.7, 23.8};

            // New theramore turtles

            //double[] xs = new double[] {64.6, 64.9, 64.8, 64.7, 64.2, 63.6, 63.2, 63.0, 62.3, 61.9, 61.6, 61.8, 61.9, 61.9, 61.6, 61.7, 60.7, 60.1};
            //double[] ys = new double[] {42.6, 42.1, 40.8, 39.6, 39.3, 38.2, 37.4, 35.5, 33.3, 32.0, 30.1, 28.2, 25.7, 24.3, 22.7, 21.4, 19.2, 18.4};


            // NE Starer 1-6 ~1.5 hr
            //double[] xs = new double[] {58.9, 57.3, 56.0, 55.5, 54.8, 55.8, 55.5, 54.5, 55.4, 56.5, 56.7, 56.8, 57.7, 58.2, 59.6, 62.8};
            //double[] ys = new double[] {45.8, 45.2, 45.8, 44.8, 44.2, 41.4, 39.8, 39.3, 38.0, 36.1, 34.1, 31.9, 32.9, 33.3, 33.6, 37.1};

            //double[] xs = new double[] { 58.8, 59.6, 60.3, 60.0, 59.6, 60.1, 59.7, 59.3, 59.5, 59.6, 60.0, 60.3, 60.3, 61.0, 61.6, 60.6, 60.6, 61.4, 62.3, 61.4, 60.7, 59.8};
            //double[] ys = new double[] { 58.2, 58.8, 58.4, 59.1, 59.7, 60.3, 61.1, 61.7, 63.2, 64.1, 64.6, 65.2, 66.0, 65.6, 66.5, 68.1, 69.1, 69.4, 68.5, 69.2, 71.1, 72.2};

            #endregion

            // Undead rogue

            // 1-6

            //double[] xs = new double[] { 29.0, 29.6, 29.4, 30.1, 29.3, 27.7, 29.0, 31.6, 33.2, 34.9, 35.0 };
            //double[] ys = new double[] { 67.7, 66.0, 64.7, 62.1, 59.5, 59.0, 57.4, 58.1, 56.9, 58.7, 56.5 };

            // 6-11

            //double[] xs = new double[] { 56.8, 56.4, 55.7, 55.0, 54.4, 54.1, 53.7, 53.6, 53.0, 52.7, 52.7, 52.1, 51.7, 51.9, 51.9, 50.8, 51.4};
            //double[] ys = new double[] { 55.8, 57.3, 57.2, 57.3, 55.9, 55.9, 55.8, 56.4, 56.5, 57.3, 57.6, 58.1, 57.7, 58.5, 59.2, 59.5, 60.0};

            // 11-16

            //double[] xs = new double[] { 79.1, 80.8, 82.2, 83.0, 82.0, 81.4, 82.9, 83.2, 84.5, 85.0, 86.6, 87.3, 87.9, 87.9, 86.6, 87.4, 89.0, 89.8, 89.7, 90.3, 91.1, 91.6 };
            //double[] ys = new double[] { 48.6, 48.3, 48.3, 48.7, 47.0, 45.3, 46.1, 47.4, 47.2, 48.4, 48.4, 49.3, 48.6, 50.2, 50.6, 51.2, 49.7, 50.4, 51.5, 49.4, 50.7, 49.5 };

            // 16-21

            //double[] xs = new double[] {54.6, 54.8, 55.0, 54.7, 54.6, 54.3, 54.6, 55.3, 55.8, 56.0, 56.4, 56.8, 57.0, 56.3, 56.7, 57.0, 57.3, 57.9, 58.3, 58.9, 59.2, 59.4, 59.9, 60.1};
            //double[] ys = new double[] {10.7, 11.3, 11.9, 12.3, 12.8, 12.8, 13.5, 13.6, 14.1, 14.7, 15.4, 15.7, 15.9, 16.4, 17.4, 17.5, 18.2, 18.1, 18.2, 17.7, 18.2, 17.6, 17.3, 17.8};

            // 21-27

            double[] xs = new double[] { 44.3, 44.6, 44.9, 45.1, 44.4, 44.7, 45.4, 45.3 };
            double[] ys = new double[] { 62.8, 62.0, 62.5, 63.1, 63.9, 64.6, 65.7, 66.3 };


            List<double> xCoordinates = new List<double>(xs);
            List<double> yCoordinates = new List<double>(ys);

            xCoordinates.Reverse();
            yCoordinates.Reverse();

            WoWBot wwf = new WoWBot();

            //wwf.RunRogueBot(xCoordinates, yCoordinates);
        }

    }

}
