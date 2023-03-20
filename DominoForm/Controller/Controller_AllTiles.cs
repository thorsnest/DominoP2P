using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominoForm.View;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Net;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace DominoForm.Controller
{
    internal class Controller_AllTiles
    {
        Client f;               //El tablero
        string[,] tiles;        //La lista con todas las fichas del juego
        string[,] pile;         //La lista con todas las fichas del juego
        Button[] hand;          //La mano del jugador
        int leftTile;           //El valor aceptado del lado izquierdo del tablero
        int rightTile;          //El valor aceptado del lado derecho del tablero
        WebApplication wsHost;  //Websocket Host
        ClientWebSocket wsClient = new ClientWebSocket();//Websocket Cliente
        string ip;

        public Controller_AllTiles(bool isHost, string? ip)
        {
            f = new Client();
            this.ip = ip!;
            if(isHost)
            {
                startAsHost();
            } 
            else
            {
                startAsPlayer();
            }
            Config();
            f.tauler.Text += tiles![leftTile, rightTile];
            
            f.Show();
        }

        private async void startAsPlayer()
        {
            await joinGame(ip);
        }

        private async void startAsHost()
        {
                f.ip_L.Text = getIP();
                createServerSocket();
                if(wsHost is not null)
                {
                    wsHost!.RunAsync("http://0.0.0.0:8080");
                }
                else
                {
                    Console.Error.WriteLine("Could't create the server, exiting...");
                    f.Dispose();
                }
                await joinGame(ip!);
        }

        private string getIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with a IPv4 address in the system!");
        }

        private async Task joinGame(string ip)
        {
            //Crear socket cliente 
            await wsClient.ConnectAsync(new Uri(ip), CancellationToken.None);
            byte[] buffer = new byte[1024];
            while(wsClient.State == WebSocketState.Open)
            {
                var result = await wsClient.ReceiveAsync(buffer, CancellationToken.None);
                Debug.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
            }
        }

        private void createServerSocket()
        {
            //Buscar alguna forma de creat un socket servidor
            wsHost = WebApplication.CreateBuilder().Build();
            wsHost.UseWebSockets();
            wsHost.MapGet("/host", async context =>
            {
                if(context.WebSockets.IsWebSocketRequest)
                {
                    using(WebSocket ws = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        while(ws.State == WebSocketState.Open)
                        {
                            byte[] data = Encoding.UTF8.GetBytes("Connected successfully!");
                            await ws.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                            await Task.Delay(1000);
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                }
            });
        }

        private void Config()
        {
            tiles = ConfigTiles();
            pile = ConfigTiles();
            hand = f.Controls.OfType<Button>().ToArray();
            ConfigButtons();
            EnableHand();
            InitListeners();
        }

        private void InitListeners()
        {
            f.SizeChanged += F_SizeChanged;
        }

        private void F_SizeChanged(object? sender, EventArgs e)
        {
            f.tauler.Font = new  Font(f.tauler.Font.Name, (40 * f.tauler.Width) / 796);
        }

            public Font GetAdjustedFont(Graphics g, string graphicString, Font originalFont, int containerWidth, int maxFontSize, int minFontSize, bool smallestOnFail)
            {
                Font testFont = null;
                // We utilize MeasureString which we get via a control instance           
                for (int adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
                {
                    testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);

                    // Test the string with the new size
                    SizeF adjustedSizeNew = g.MeasureString(graphicString, testFont);

                    if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width))
                    {
                        // Good font, return it
                        return testFont;
                    }
                }
                    
                if (smallestOnFail)
                {
                    return testFont;
                }
                else
                {
                    return originalFont;
                }
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
