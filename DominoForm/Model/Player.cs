using System.Net.WebSockets;

namespace DominoForm.Model
{
    internal class Player
    {
        public WebSocket ws { get; set; }
        public int turn { get; set; }

        public Player(WebSocket ws, int turn)
        {
            this.ws = ws;
            this.turn = turn;
        }
    }
}