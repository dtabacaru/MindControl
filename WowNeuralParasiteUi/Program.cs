using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassicWowNeuralParasite
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainUI());
        }

        public static List<double> ExtractCommaDelimitedDoubles(string doubleString)
        {
            List<double> output = new List<double>();

            string[] doubleStringParts = doubleString.Split(new char[] { ',' });

            foreach (string doubleStringPart in doubleStringParts)
            {
                if (doubleStringPart == string.Empty || string.IsNullOrWhiteSpace(doubleStringPart))
                    continue;

                double decodedDouble = 0;

                try
                {
                    decodedDouble = double.Parse(doubleStringPart);
                    output.Add(decodedDouble);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return output;
        }

    }
}
