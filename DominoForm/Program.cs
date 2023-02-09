using System.Runtime.CompilerServices;
using System.Text;

namespace DominoForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            new Controller();
        }
    }
}