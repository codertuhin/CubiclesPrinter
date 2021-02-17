using System;
using System.Windows.Forms;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesAPITest.View;

namespace CubiclesAPITest
{
    /// <summary>
    /// 
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                LogHelper.Init("test.log.txt");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new APITestForm());
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }
    }
}
