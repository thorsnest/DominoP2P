using System.Text;
using DominoForm.View;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Net;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics;
using DominoForm.Model;
using System.Numerics;
using System.Windows.Forms;
using System.Reflection;

namespace DominoForm.Controller
{
    /*TODO
     * - Detectar quien ha ganado.
     *  · Cuando uno de los numeros de fichas llegue a 0
     *  · Cuando no se puedan jugar mas fichas.
     * - Mostrar de quien es el turno (poniendo sus fichas en rojo)
     * - Retoques a la vista
     * - Optimizar código
     */
    internal class Controller_AllTiles
    {
        Client f;                                                       //El tablero
        string[,] tiles;                                                //La lista con todas las fichas del juego
        string[,] pile;                                                 //La lista con todas las fichas jugándose
        Button[] hand;                                                  //La mano del jugador
        int[] playersHand = new int[4];                                 //Cantidad de fichas en cada jugador
        int[] playersPoints = new int[4];
        int leftTile = 6;                                               //El valor aceptado del lado izquierdo del tablero
        int rightTile = 6;                                              //El valor aceptado del lado derecho del tablero
        WebApplication wsHost;                                          //Websocket Host
        ClientWebSocket wsClient = new ClientWebSocket();               //Websocket Cliente
        string ip;                                                      //Dirección IP
        CancellationTokenSource cts = new CancellationTokenSource();    //Token de cancelación
        List<Player> players = new List<Player>();                      //Lista de jugadores
        int playerNum;                                                  //Número de cada jugador para el servidor
        int yourTurn = -1;                                              //Número del turno del cliente
        int turn;                                                       //Número del turno actual de la partida

        public Controller_AllTiles(bool isHost, string? ip)
        {
            f = new Client();
            this.ip = ip!;

            MainStart(isHost);
        }

        //Métode que crida als métodes per posar en marxa el servidor si cal, configuració general i comença la partida
        private void MainStart(bool isHost)
        {
            if (isHost) startHosting();
            startAsPlayer();
            Setup();
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
                int timesSkipped = 0;
                string prevMsg = "";
                List<string> playsLog = new List<string>();
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
                                else if (rcvMsg.Equals("/get"))
                                {
                                    char[] hand = getHand();
                                    await ws.SendAsync(Encoding.UTF8.GetBytes(hand),
                                        WebSocketMessageType.Text, true, CancellationToken.None);
                                    string handString = new string(hand);
                                    if (handString.IndexOf(tiles[6, 6]) != -1)
                                    {
                                        turn = p.turn;
                                    }
                                }
                                else if (rcvMsg.Equals("/left"))
                                {
                                    foreach (Player player in players)
                                    {
                                        msgBytes = Encoding.UTF8.GetBytes("Players left: " + (4 - playerNum));
                                        await player.ws.SendAsync(msgBytes,
                                            WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                }
                                else if (rcvMsg.StartsWith("/points"))
                                {
                                    bool filled = true;
                                    playersPoints[int.Parse(rcvMsg.Substring(7, 1))] = int.Parse(rcvMsg.Substring(8));
                                    foreach(int points in playersPoints)
                                    {
                                        if (points == 0) filled = false;
                                    }
                                    if (filled) {
                                        int lowest = 70;
                                        int winner = -1;
                                        for(int i = 0; i<playersPoints.Length;i++)
                                        {
                                            if (playersPoints[i] < lowest)
                                            {
                                                lowest = playersPoints[i];
                                                winner = i;
                                            }
                                        }
                                        msgBytes = Encoding.UTF8.GetBytes("The winner is Player " + (winner +1) + " by points!");
                                        foreach (Player player in players)
                                        {
                                            await player.ws.SendAsync(msgBytes,
                                            WebSocketMessageType.Text, true, CancellationToken.None);
                                        }
                                    }
                                }
                                else
                                {
                                    bool end = false;
                                    if (playerNum == 4)
                                    {
                                        for(int i = 0; i<4; i++)
                                        {
                                            if (int.Parse(rcvMsg.Substring(rcvMsg.Length-6+i,1)) == 0)
                                            {
                                                msgBytes = Encoding.UTF8.GetBytes("The winner is Player " + (i + 1) + "!");
                                                foreach (Player player in players)
                                                {
                                                    await player.ws.SendAsync(msgBytes,
                                                    WebSocketMessageType.Text, true, CancellationToken.None);
                                                    end = true;
                                                }
                                            }
                                        }
                                        //En canvi si es una jugada normal, al missatge se li afegeix el torn de la partida per saber a quin jugador li toca, la variable augmenta i es retornen les dades a tots els jugadors
                                        if (prevMsg.Length-1 <= rcvMsg.Length && !end)
                                        {
                                            Debug.WriteLine(rcvMsg);
                                            foreach (string play in playsLog){
                                                if (play.Equals(rcvMsg))
                                                {
                                                    timesSkipped++;
                                                }
                                            }
                                            if (timesSkipped == 3)
                                            {
                                                rcvMsg ="Domino closed!";
                                                foreach (Player player in players)
                                                {
                                                    msgBytes = Encoding.UTF8.GetBytes(rcvMsg);
                                                    await player.ws.SendAsync(msgBytes,
                                                        WebSocketMessageType.Text, true, CancellationToken.None);
                                                }
                                            }
                                            else
                                            {
                                                timesSkipped = 0;
                                                playsLog.Add(rcvMsg);
                                                rcvMsg += turn % 4;
                                                turn++;
                                                foreach (Player player in players)
                                                {
                                                    //rcvMsg = Encoding.UTF8.GetString(msgBytes).Substring(0, rcvMsg.Length - 1);
                                                    if (rcvMsg.StartsWith("P")) rcvMsg = "66" + ((turn - 1) % 4);
                                                    msgBytes = Encoding.UTF8.GetBytes(rcvMsg);
                                                    await player.ws.SendAsync(msgBytes,
                                                        WebSocketMessageType.Text, true, CancellationToken.None);
                                                }
                                                prevMsg = rcvMsg;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //Si hi ha ja 4 jugadors s'avisarà i es tancara l'aplicació
                        else
                        {
                            DialogResult d = MessageBox.Show("Aquesta partida ja té 4 jugadors");
                            if (d == DialogResult.OK)
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

        private char[] getHand()
        {
            Random r = new Random();
            string hand = "";
            for (int i = 0; i < 7; i++)
            {
                bool placed = false;
                while (!placed)
                {
                    int x = r.Next(7);
                    int y = r.Next(7);
                    if (pile[x, y] != "")
                    {
                        hand += tiles[x, y];
                        pile[x, y] = "";
                        pile[y, x] = "";
                        placed = true;
                    }
                }
            }
            hand += 'h';
            return hand.ToCharArray();
        }

        #endregion

        #region Client

        #region Setup
        //Métode principal per començar a jugar, cridant el métode per connectar-se i emmagatzemant el torn del jugador a la seva respectiva variable
        private async void startAsPlayer()
        {
            if (f.ip_L.Text == "")
            {
                f.ipText_L.Text = "IP Connected:";
                f.ip_L.Text = ip.Substring(5);
            }
            await joinGame(ip);
            getPlayerHand();
            getTurn();
            getPlayersLeft();
        }

        private async void getPlayersLeft()
        {
            string? missatge = "/left";
            byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
            var sendBuffer = new ArraySegment<byte>(sendBytes);
            await wsClient.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
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
                            Control.CheckForIllegalCrossThreadCalls = false;
                            int tempPlayerOrder;
                            if (!int.TryParse(rcvMsg, out tempPlayerOrder))
                            {
                                // Si el missatge acaba en 'h' voldrà dir que es tracta de la mà del jugador
                                if (rcvMsg.EndsWith("h"))
                                {
                                    SetupButtons(rcvMsg);
                                    if (rcvMsg.IndexOf(tiles[6, 6]) == -1) DisableHand();
                                    else DisableHand6();
                                }
                                else if (rcvMsg.StartsWith("P"))
                                {
                                    f.tauler.Text = rcvMsg;
                                    if (rcvMsg.EndsWith("0"))
                                    {
                                        f.tauler.Text = "";
                                        DisableHand6();
                                    }
                                    else DisableHand();
                                }
                                else if (rcvMsg.StartsWith("D"))
                                {
                                    EnviaPunts();
                                }
                                else if (rcvMsg.StartsWith("T"))
                                {
                                    f.tauler.Text = rcvMsg;
                                    DisableHand();
                                    f.player1tiles_L.Text = "";
                                    f.player2tiles_L.Text = "";
                                    f.player3tiles_L.Text = "";
                                }
                                else
                                {
                                    /* Exemple resposta: 🁓🂓6677460
                                     * La resposta está estructurada de la següent manera:
                                     * - "🁓🂓" és el tauler actual, que es mostrarà al Label "tauler"
                                     * - "6677" son les fitxes que li queden a cada jugador
                                     * - "4" és el valor del tauler per l'esquerra
                                     * - "6" és el valor del tauler per la dreta
                                     * - 0 és el jugador a qui li toca
                                    */

                                    f.tauler.Text = rcvMsg[..^7];
                                    switch (yourTurn) {
                                        case 0:
                                            f.player1tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 6, 1))));
                                            f.player2tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 5, 1))));
                                            f.player3tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 4, 1))));
                                            break;
                                        case 1:
                                            f.player1tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 5, 1))));
                                            f.player2tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 4, 1))));
                                            f.player3tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 7, 1))));
                                            break;
                                        case 2:
                                            f.player1tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 4, 1))));
                                            f.player2tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 7, 1))));
                                            f.player3tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 6, 1))));
                                            break;
                                        case 3:
                                            f.player1tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 7, 1))));
                                            f.player2tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 6, 1))));
                                            f.player3tiles_L.Text = string.Concat(Enumerable.Repeat("🁢", int.Parse(rcvMsg.Substring(rcvMsg.Length - 5, 1))));
                                            break;
                                    }
                                    for(int i = 0; i < playersHand.Length; i++)
                                    {
                                        playersHand[i] = int.Parse(rcvMsg.Substring(rcvMsg.Length - 7 + i, 1));
                                    }
                                    leftTile = int.Parse(rcvMsg.Substring(rcvMsg.Length - 3, 1));
                                    rightTile = int.Parse(rcvMsg.Substring(rcvMsg.Length - 2, 1));
                                    if (yourTurn == (int.Parse(rcvMsg.Substring(rcvMsg.Length - 1, 1)) + 1) % 4)
                                    {
                                        EnableHand();
                                    }
                                    else DisableHand();
                                }
                            }
                            else
                            {
                                yourTurn = tempPlayerOrder;
                                f.playerNum_L.Text = "Player " + (yourTurn + 1);
                            }
                        }
                    }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        //Métode que quan es crida retorna els punts totals de cada jugador, junt amb el seu torn per que el servidor el reconeixi.
        private async void EnviaPunts()
        {
            int punts = 0;
            foreach (Button button in hand)
            {
                if (button.Visible)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            if (tiles[i, j] == button.Text)
                            {
                                punts += i + j;
                            }
                        }
                    }
                }
            }
            byte[] sendBytes = Encoding.UTF8.GetBytes("/points" + yourTurn + punts.ToString());
            var sendBuffer = new ArraySegment<byte>(sendBytes);
            await wsClient.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
        }

        //Métode de configuració principal que crida a totes les configuracions inicials
        private void Setup()
        {
            tiles = SetupTiles();
            pile = SetupTiles();
            hand = f.Controls.OfType<Button>().ToArray();
            //SetupButtons();
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
        private void SetupButtons(string rcvMsg)
        {
            //Random r = new Random();
            //string[] hand = new string[7];
            int i = 0;
            foreach (Button button in hand)
            {
                button.MouseDown += Button_MouseDown;
                button.Text = rcvMsg.Substring(i, 2);
                /*
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
                }*/
                i += 2;
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
            f.tauler.Font = new Font(f.tauler.Font.Name, (23 * f.tauler.Width) / 842);
        }

        private void Button_MouseDown(object? sender, MouseEventArgs e)
        {
            bool left;
            Button? b = sender as Button;
            left = e.Button == MouseButtons.Left;
            PlaceTile(b, left);
//            EnableHand();
        }

        #endregion

        #region Jugabilitat

        //Métode que demana al servidor el torn del jugador per després emmagatzemar-ho a una variable
        private async void getPlayerHand()
        {
            string? missatge = "/get";
            byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
            var sendBuffer = new ArraySegment<byte>(sendBytes);
            await wsClient.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
        }

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
            string missatge = "";
            if (button.Text == tiles[6, 6])
            {
                missatge = tiles[6, 6];
                switch (yourTurn)
                {
                    case 0:
                        missatge += "6777";
                        break;
                    case 1:
                        missatge += "7677";
                        break;
                    case 2:
                        missatge += "7767";
                        break;
                    case 3:
                        missatge += "7776";
                        break;
                }
                missatge += "66";
                byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
                var sendBuffer = new ArraySegment<byte>(sendBytes);
                await wsClient.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
                button.Visible = false;
            }
            else
            {
                if (left)
                {
                    for (int i = 0; i < 7; i++)
                        if (tiles[leftTile, i].Equals(button.Text) || tiles[i, leftTile].Equals(button.Text))
                        {
                            missatge = tiles[i, leftTile] + f.tauler.Text;
                            for (int j = 0; j < playersHand.Length; j++)
                            {
                                if (yourTurn == j) playersHand[j]--;
                                missatge += playersHand[j];
                            }
                            missatge += i.ToString() + rightTile.ToString();
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
                            missatge = f.tauler.Text + tiles[rightTile, i];
                            for (int j = 0; j < playersHand.Length; j++) {
                                if (yourTurn == j) playersHand[j]--;
                                missatge += playersHand[j];
                            }
                            missatge += leftTile.ToString() + i.ToString();
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
        }

        //Métode que habilita la mà del jugador
        private void EnableHand()
        {
            bool cantPlay = true;
            bool invisible = true;
            foreach (Button button in hand)
            {
                if (button.Visible)
                {
                    invisible = false;
                    button.Enabled = CheckPlaceableTile(button);
                    if (button.Enabled) cantPlay = false;
                }
            }
            if (cantPlay)
            {
                SkipTurn();
            }
        }

        //Métode que pasa torn
        private async void SkipTurn()
        {
            string? missatge = f.tauler.Text;
            for (int j = 0; j < playersHand.Length; j++)
            {
                missatge += playersHand[j];
            }
            missatge += leftTile.ToString() + rightTile.ToString();
            byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
            var sendBuffer = new ArraySegment<byte>(sendBytes);
            await wsClient.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
        }

        //Métode que deshabilita la mà del jugador
        private void DisableHand()
        {
            foreach (Button button in hand)
            {
                button.Enabled = false;
            }
        }

        private void DisableHand6()
        {
            foreach (Button button in hand)
            {
                button.Enabled = button.Text == tiles[6, 6];
            }

        }

        //Métode que comproba si la fitxa de la teva mà pot ser col·locada
        private bool CheckPlaceableTile(Button button)
        {
            if (button.Text == tiles[6, 6]) return true;
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
