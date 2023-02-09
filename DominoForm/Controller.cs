using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoForm
{
    internal class Controller
    {
        Form1 f;                //El tablero
        string[,] tiles;        //La lista con todas las fichas del juego
        Button[] hand;          //La mano del jugador
        int leftTile;           //El valor aceptado del lado izquierdo del tablero
        int rightTile;          //El valor aceptado del lado derecho del tablero

        public Controller()
        {
            f = new Form1();
            Config();
            f.tauler.Text = tiles[0, 0];
            Application.Run(f);
        }

        private void Config()
        {
            tiles = ConfigTiles();
            hand = f.Controls.OfType<Button>().ToArray();
            ConfigButtons();
            EnableHand();
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
        private void ConfigButtons()
        {
            string[,] tempTiles = ConfigTiles();
            Random r = new Random();
            string[] hand = new string[7];
            foreach (Button button in this.hand)
            {
                button.MouseDown += Button_MouseDown;

                bool placed = false;
                while (!placed)
                {
                    int x = r.Next(7);
                    int y = r.Next(7);
                    if (tempTiles[x, y] != "")
                    {
                        button.Text = tempTiles[x, y];
                        tempTiles[x, y] = "";
                        tempTiles[y, x] = "";
                        placed = true;
                    }
                }
            }
        }

        private void Button_MouseDown(object? sender, MouseEventArgs e)
        {
            bool left;
            Button? b = sender as Button;
            left = e.Button == MouseButtons.Left;
            PlaceTile(b, left);
            EnableHand();
        }

        private void PlaceTile(Button button, bool left)
        {
            if (left)
            {
                for (int i = 0; i < 7; i++)
                    if (tiles[leftTile, i].Equals(button.Text))
                    {
                        f.tauler.Text = button.Text + f.tauler.Text;
                        leftTile = i;
                        button.Visible = false;
                        break;
                    }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                    if (tiles[rightTile, i].Equals(button.Text))
                    {
                        f.tauler.Text += button.Text;
                        rightTile = i;
                        button.Visible = false;
                        break;
                    }
            }
        }

        private void EnableHand()
        {
            foreach (Button button in hand)
            {
                button.Enabled = CheckPlaceableTile(button);
            }
        }

        private bool CheckPlaceableTile(Button button)
        {
            for (int i = 0; i < 7; i++)
            {
                if (tiles[rightTile, i].Equals(button.Text) ||
                    tiles[leftTile, i].Equals(button.Text))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
