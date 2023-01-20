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

        private static void PlaceButtonsValue(Button[] handButtons, string[] hand)
        {
            int i = 0;
            foreach (Button button in handButtons)
            {
                button.Text = hand[i];
                button.Click += Button_Click;
                i++;
            }
        }

        private static void Button_Click(object? sender, EventArgs e)
        {
            Button b = sender as Button;
            b.Visible = false;
        }

        private static string[] HandTiles(string[,] tiles)
        {
            Random r = new Random();
            string[] hand = new string[7];
            for (int i = 0; i < 7; i++)
            {
                bool placed = false;
                while (!placed)
                {
                    int x = r.Next(6);
                    int y = r.Next(6);
                    if (tiles[x, y] != "")
                    {
                        hand[i] = tiles[x, y];
                        tiles[x, y] = "";
                        tiles[y, x] = "";
                        placed = true;
                    }
                }
            }
            return hand;
        }

        private static string[,] ConfigTiles()
        {
            string unicodi0 = "\U0001F031";
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(unicodi0); ;
            string[,] tiles = new string[7, 7];
            for (int x = 7; x > 0; x--)
            {
                for (int j = x; j > 0; j--)
                {
                    tiles[7 - x, 7 - j] = Encoding.Unicode.GetString(unicodeBytes);
                    tiles[7 - j, 7 - x] = Encoding.Unicode.GetString(unicodeBytes);
                    unicodeBytes[2]++;
                }
                unicodeBytes[2] += (byte)(8 - x);
            }
            return tiles;
        }
    }
}