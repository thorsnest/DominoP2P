using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominoForm.View;

namespace DominoForm.Controller
{
    internal class Controller_AllTiles
    {
        Client f;                //El tablero
        string[,] tiles;        //La lista con todas las fichas del juego
        string[,] pile;        //La lista con todas las fichas del juego
        Button[] hand;          //La mano del jugador
        int leftTile;           //El valor aceptado del lado izquierdo del tablero
        int rightTile;          //El valor aceptado del lado derecho del tablero
        public Controller_AllTiles()
        {
            f = new Client();
            Config();
            f.tauler.Text += tiles[leftTile, rightTile];
            Application.Run(f);
        }

        private void Config()
        {
            tiles = ConfigTiles();
            pile = ConfigTiles();
            hand = f.Controls.OfType<Button>().ToArray();
            ConfigButtons();
            EnableHand();
        }

        //Emmagatzema els caràcters dominó, omitin les fitxes repetides, dintre de l'array bidimensional 'tiles'
        //amb la ubicació dient quina peça és (La fitxa amb valors 1 i 4
        //s'emmagatzema a la casella [1,4] de l'array).
        //Totes les peces son emmagatazemades pels dos costats excepte les dobles, que s'emmagatzemen en vertical
        private string[,] ConfigTiles()
        {
            string unicodi0 = "\U0001F031";
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(unicodi0); ;
            string[,] tiles = new string[7, 7];
            for (int x = 0; x < 7; x++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (j != x)
                    {
                        tiles[x, j] = Encoding.Unicode.GetString(unicodeBytes);
                    }
                    unicodeBytes[2]++;
                }
            }
            unicodeBytes[2]++;
            for (int x = 0; x < 7; x++)
            {
                tiles[x, x] = Encoding.Unicode.GetString(unicodeBytes);
                unicodeBytes[2] += 8;
            }

            return tiles;
        }

        //Configura els 7 botons donan-lis a cadascún una caràcter de fitxa aleatori i el treu de l'array.

        private void ConfigButtons()
        {
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
                    if (pile[x, y] != "")
                    {
                        button.Text = tiles[x, y];
                        pile[x, y] = "";
                        pile[y, x] = "";
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
                        f.tauler.Text = tiles[i, leftTile] + f.tauler.Text;
                        leftTile = i;
                        button.Visible = false;
                        break;
                    }
                    else if (tiles[i, leftTile].Equals(button.Text))
                    {
                        f.tauler.Text = tiles[i, leftTile] + f.tauler.Text;
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
                        f.tauler.Text += tiles[rightTile, i];
                        rightTile = i;
                        button.Visible = false;
                        break;
                    }
                    else if (tiles[i, rightTile].Equals(button.Text))
                    {
                        f.tauler.Text += tiles[rightTile, i];
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
                    tiles[leftTile, i].Equals(button.Text) ||
                    tiles[i, rightTile].Equals(button.Text) ||
                    tiles[i, leftTile].Equals(button.Text))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
