using System.Text;
using DominoForm.View;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Net;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics;
using DominoForm.Model;

namespace DominoForm.Controller
{
    /*TODO
     * - No dejar jugar a menos que haya 4 jugadores.
     * - Repartir fichas únicas desde el servidor.
     * - Darle el primer turno a quien tenga el doble seis.
     * - Detectar quien ha ganado.
     */
    internal class Controller_AllTiles
    {
        Client f;                                                   //El tablero
        string[,] tiles;                                            //La lista con todas las fichas del juego
        string[,] pile;                                             //La lista con todas las fichas del juego
        Button[] hand;                                              //La mano del jugador
        int leftTile;                                               //El valor aceptado del lado izquierdo del tablero
        int rightTile;                                              //El valor aceptado del lado derecho del tablero
        WebApplication wsHost;                                      //Websocket Host
        ClientWebSocket wsClient = new ClientWebSocket();           //Websocket Cliente
        string ip;                                                  //Dirección IP
        CancellationTokenSource cts = new CancellationTokenSource();//Token de cancelación
        List<Player> players = new List<Player>();                  //Lista de jugadores
        int playerNum;                                              //Número de cada jugador para el servidor
        int yourTurn;                                               //Número del turno del cliente
        int turn;                                                   //Número del turno actual de la partida

        public Controller_AllTiles(bool isHost, string? ip)
        {
            f = new Client();
            this.ip = ip!;

            MainStart(isHost);
        }

        //Métode que crida als métodes per posar en marxa el servidor si cal, configuració general i comença la partida
        private void MainStart(bool isHost)
        {
            if (isHost)
            {
                startHosting();
            }
            startAsPlayer();
            Setup();
            f.tauler.Text += tiles![leftTile, rightTile];
            f.Show();
        }

        #region Servidor

        //Métode que crida la creació del servidor i l'engega
        private async void startHosting()
        {
            f.ip_L.Text = getIP();
            createServerSocket();
            if (wsHost is not null)
            {
                wsHost!.RunAsync("http://0.0.0.0:8080");
            }
            else
            {
                Console.Error.WriteLine("Could't create the server, exiting...");
                f.Dispose();
            }
        }

        //Métode que agafa la direcció IP de l'ordinador
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

        //Métode que crea i configura el servidor
        private void createServerSocket()
        {
            wsHost = WebApplication.CreateBuilder().Build();
            wsHost.UseWebSockets();
            wsHost.MapGet("/host", async context =>
            {
                var rcvBytes = new byte[256];
                var rcvBuffer = new ArraySegment<byte>(rcvBytes);
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (WebSocket ws = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        //Es crea un jugador per cada connexió fins a 4 i els afegeix a una llista.
                        if (playerNum < 4)
                        {
                            Player p = new Player(ws, playerNum);
                            playerNum++;
                            players.Add(p);

                            while (ws.State == WebSocketState.Open)
                            {
                                WebSocketReceiveResult rcvResult = await ws.ReceiveAsync(rcvBuffer, CancellationToken.None);
                                byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                                string rcvMsg = Encoding.UTF8.GetString(msgBytes);

                                //Si el missatge enviat es /num tornarà el torn corresponent del jugador
                                if (rcvMsg.Equals("/num"))
                                {
                                    await ws.SendAsync(Encoding.UTF8.GetBytes(p.turn.ToString()),
                                        WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                                //En canvi si es una jugada normal, al missatge se li afegeix el torn de la partida per saber a quin jugador li toca, la variable augmenta i es retornen les dades a tots els jugadors
                                else
                                {
                                    rcvMsg += turn % 4;
                                    turn++;
                                    foreach (Player player in players)
                                    {
                                        //rcvMsg = Encoding.UTF8.GetString(msgBytes).Substring(0, rcvMsg.Length - 1);
                                        msgBytes = Encoding.UTF8.GetBytes(rcvMsg);
                                        await player.ws.SendAsync(msgBytes,
                                            WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                }
                            }
                        }
                        //Si hi ha ja 4 jugadors s'avisarà i es tancara l'aplicació
                        else
                        {
                            DialogResult d = MessageBox.Show("Aquesta partida ja té 4 jugadors");
                            if(d == DialogResult.OK)
                            {
                                Application.Exit();
                            }
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                }
            });
        }

        #endregion

        #region Client

            #region Setup
        //Métode principal per començar a jugar, cridant el métode per connectar-se i emmagatzemant el torn del jugador a la seva respectiva variable
        private async void startAsPlayer()
        {
            await joinGame(ip);
            getTurn();
        }

        //Métode que connecta el client al servidor
        private async Task joinGame(string ip)
        {
            await wsClient.ConnectAsync(new Uri(ip), CancellationToken.None);
            if (wsClient.State == WebSocketState.Open)
            {
                await Task.Factory.StartNew(
                    async () =>
                    {

                        byte[] buffer = new byte[1024];
                        var rcvBuffer = new ArraySegment<byte>(buffer);
                        while (true)
                        {
                            WebSocketReceiveResult result = await wsClient.ReceiveAsync(buffer, cts.Token);

                            byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(result.Count).ToArray();
                            string rcvMsg = Encoding.UTF8.GetString(msgBytes);
                            var resultText = Encoding.UTF8.GetString(buffer, 0, result.Count);               
                            Debug.WriteLine(resultText);
                            Control.CheckForIllegalCrossThreadCalls = false;
                            int tempPlayerOrder;
                            if (!int.TryParse(rcvMsg, out tempPlayerOrder))
                            {
                                /* Exemple resposta: 🁓🂓460
                                 * La resposta está estructurada de la següent manera:
                                 * - "🁓🂓" és el tauler actual, que es mostrarà al Label "tauler"
                                 * - "4" és el valor del tauler per l'esquerra
                                 * - "6" és el valor del tauler per la dreta
                                 * - 0 és el jugador a qui li toca
                                */

                                f.tauler.Text = rcvMsg.Substring(0, rcvMsg.Length - 3);
                                leftTile = int.Parse(rcvMsg.Substring(rcvMsg.Length - 3, 1));
                                rightTile = int.Parse(rcvMsg.Substring(rcvMsg.Length - 2, 1));
                                if (yourTurn == (int.Parse(rcvMsg.Substring(rcvMsg.Length - 1, 1)) + 1) %4)
                                {
                                    EnableHand();
                                }
                                else DisableHand();
                            }
                            else
                            {
                                yourTurn = tempPlayerOrder;
                            }
                        }
                    }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }
        //Métode de configuració principal que crida a totes les configuracions inicials
        private void Setup()
        {
            tiles = SetupTiles();
            pile = SetupTiles();
            hand = f.Controls.OfType<Button>().ToArray();
            SetupButtons();
            EnableHand();
            InitListeners();
        }

        //Emmagatzema els caràcters dominó, omitin les fitxes repetides, dintre de l'array bidimensional 'tiles'
        //amb la ubicació dient quina peça és (La fitxa amb valors 1 i 4
        //s'emmagatzema a la casella [1,4] de l'array).
        //Totes les peces son emmagatazemades pels dos costats excepte les dobles, que s'emmagatzemen en vertical
        private string[,] SetupTiles()
        {
            string unicodi0 = "\U0001F031";
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(unicodi0);
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
        private void SetupButtons()
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

        //Métode que inicialitza els listeners
        private void InitListeners()
        {
            f.SizeChanged += F_SizeChanged;
        }

        //´Métode que canvia el tamany del tauler dinàmicament segons la mida de la finestra
        private void F_SizeChanged(object? sender, EventArgs e)
        {
            f.tauler.Font = new Font(f.tauler.Font.Name, (40 * f.tauler.Width) / 796);
        }

        private void Button_MouseDown(object? sender, MouseEventArgs e)
        {
            bool left;
            Button? b = sender as Button;
            left = e.Button == MouseButtons.Left;
            PlaceTile(b, left);
            EnableHand();
        }

        #endregion

            #region Jugabilitat

        //Métode que demana al servidor el torn del jugador per després emmagatzemar-ho a una variable
        private async void getTurn()
        {
            string? missatge = "/num";
            byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
            var sendBuffer = new ArraySegment<byte>(sendBytes);
            await wsClient.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
        }

        //Métode que comproba si la fitxa clicada pot ser col·locada i la col·loca
        private async void PlaceTile(Button button, bool left)
        {
            if (left)
            {
                for (int i = 0; i < 7; i++)
                    if (tiles[leftTile, i].Equals(button.Text) || tiles[i, leftTile].Equals(button.Text))
                    {
                        string? missatge = tiles[i, leftTile] + f.tauler.Text + i + rightTile;
                        byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
                        var sendBuffer = new ArraySegment<byte>(sendBytes);
                        await wsClient.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
                       // f.tauler.Text = tiles[i, leftTile] + f.tauler.Text;
                        
                        leftTile = i;
                        button.Visible = false;
                        break;
                    }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                    if (tiles[rightTile, i].Equals(button.Text) || tiles[i, rightTile].Equals(button.Text))
                    {
                        string? missatge = f.tauler.Text + tiles[rightTile, i] + leftTile + i;
                        byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
                        var sendBuffer = new ArraySegment<byte>(sendBytes);
                        await wsClient.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
                        // f.tauler.Text += tiles[rightTile, i];
                        rightTile = i;
                        button.Visible = false;
                        break;
                    }
            }
        }

        //Métode que habilita la mà del jugador
        private void EnableHand()
        {
            foreach (Button button in hand)
            {
                button.Enabled = CheckPlaceableTile(button);
            }
        }

        //Métode que deshabilita la mà del jugador
        private void DisableHand()
        {
            foreach (Button button in hand)
            {
                button.Enabled = false;
            }
        }

        //Métode que comproba si la fitxa de la teva mà pot ser col·locada
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

        #endregion

        #endregion

    }
}
