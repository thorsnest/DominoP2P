using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DominoForm.Controller
{
    internal class Server
    {
        public Server(string ip)
        {
            TcpListener server = new TcpListener(IPAddress.Parse(ip), 443);

            server.Start();
            Console.WriteLine("Server started");

            TcpClient client = server.AcceptTcpClient();

            Console.WriteLine("A client connected.");
        }
    }
}
