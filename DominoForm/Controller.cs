using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoForm
{
    internal class Controller
    {
        Form1 f;
        string[,] tiles;
        string[] hand;
        Button[] handButtons;
        int leftTile;
        int rightTile;
        public Controller()
        {
            f = new Form1();
            tiles = ConfigTiles();
            hand = HandTiles(tiles);
            tiles = ConfigTiles();
            leftTile = 6;
            rightTile = 6;
            handButtons = f.Controls.OfType<Button>().ToArray();
            PlaceButtonsValue(handButtons, hand);
            EnableHand();
            f.tauler.Text = tiles[6, 6];
            Application.Run(f);
        }

        private void PlaceButtonsValue(Button[] handButtons, string[] hand)
        {
            int i = 0;
            foreach (Button button in handButtons)
            {
                button.Text = hand[i];
                button.MouseDown += Button_MouseDown;
                i++;
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
            }else{
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
            foreach (Button button in handButtons)
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

        private static string[] HandTiles(string[,] tiles)
        {
            Random r = new Random();
            string[] hand = new string[7];
            for (int i = 0; i < 7; i++)
            {
                bool placed = false;
                while (!placed)
                {
                    int x = r.Next(7);
                    int y = r.Next(7);
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
