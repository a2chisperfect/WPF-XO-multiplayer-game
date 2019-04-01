using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer server = new GameServer(IPAddress.Loopback, 11000);
            server.Start(10);
        }
    }
}
