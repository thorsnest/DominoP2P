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
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Form1 f = new Form1();
            string unicodi0 = "\U0001F031";
            byte[] b = Encoding.Unicode.GetBytes(unicodi0);
            byte[] b2 = b;
            String[,] fitxes = new string[7, 7];
            for(int x = 7; x > 0; x--)
            {
                for (int j = x; j > 0; j--)
                {
                    fitxes[7-x,7-j] = Encoding.Unicode.GetString(b2);
                    b2[2]++;
                }
                b2[2] += (byte) (8 - x);
            }
            f.label1.Text  = fitxes[0,0];
            f.label1.Text += fitxes[0,1];
            f.label1.Text += fitxes[0,2];
            f.label1.Text += fitxes[0,3];
            f.label1.Text += fitxes[0,4];
            f.label1.Text += fitxes[0,5];
            f.label1.Text += fitxes[0,6];
            f.label1.Text += fitxes[1,0];
            f.label1.Text += fitxes[1,1];
            f.label1.Text += fitxes[1,2];
            f.label1.Text += fitxes[1,3];
            f.label1.Text += fitxes[1,4];
            f.label1.Text += fitxes[1,5];
            f.label1.Text += fitxes[1,6];
            f.label1.Text += fitxes[2,0];
            f.label1.Text += fitxes[2,1];
            f.label1.Text += fitxes[2,2];
            f.label1.Text += fitxes[2,3];
            f.label1.Text += fitxes[2,4];
            f.label1.Text += fitxes[2,5];
            f.label1.Text += fitxes[2,6];
            f.label1.Text += fitxes[3,0];
            f.label1.Text += fitxes[3,1];
            f.label1.Text += fitxes[3,2];
            f.label1.Text += fitxes[3,3];
            f.label1.Text += fitxes[3,4];
            f.label1.Text += fitxes[3,5];
            f.label1.Text += fitxes[3,6];
            f.label1.Text += fitxes[4,0];
            f.label1.Text += fitxes[4,1];
            f.label1.Text += fitxes[4,2];
            f.label1.Text += fitxes[4,3];
            f.label1.Text += fitxes[4,4];
            f.label1.Text += fitxes[4,5];
            f.label1.Text += fitxes[4,6];
            f.label1.Text += fitxes[5,0];
            f.label1.Text += fitxes[5,1];
            f.label1.Text += fitxes[5,2];
            f.label1.Text += fitxes[5,3];
            f.label1.Text += fitxes[5,4];
            f.label1.Text += fitxes[5,5];
            f.label1.Text += fitxes[5,6];
            f.label1.Text += fitxes[6,0];
            f.label1.Text += fitxes[6,1];
            f.label1.Text += fitxes[6,2];
            f.label1.Text += fitxes[6,3];
            f.label1.Text += fitxes[6,4];
            f.label1.Text += fitxes[6,5];
            f.label1.Text += fitxes[6,6];
            Application.Run(f);
        }
    }
}