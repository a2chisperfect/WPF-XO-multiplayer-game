using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Threading;
using System.Net.Sockets;

namespace Server
{
    class GameServer
    {
        TcpListener listener;
        List<GameClient> players;
        List<GameSession> liveGames;
        List<GameClient> queue;
        public GameServer(IPAddress serverIp, int serverPort)
        {
            listener = new TcpListener(serverIp, serverPort);
            players = new List<GameClient>();
            liveGames = new List<GameSession>();
            queue = new List<GameClient>();
        }
        public void Start(int maxClientCount)
        {
            listener.Start(maxClientCount);
            while(true)
            {
                var tcpClient = listener.AcceptTcpClient();
                var client = new GameClient(tcpClient, this);
                players.Add(client);
                Task.Run(()=>client.ReciveMessage());
            } 
        }
        public void AddInQueueAsync(GameClient client)
        {
            Task.Run(() =>
            {
                if (queue.Count == 0)
                {
                    queue.Add(client);
                }
                else
                {
                    StartGameAsync(client, queue[0]);
                    queue.RemoveAt(0);
                }
            });
            
        }
        public void StartGameAsync(params GameClient [] players)
        {
            Task.Run(() =>
            {
                GameSession session = new GameSession(players[0], players[1], this);
                liveGames.Add(session);
            });
        }
        public void RemoveFromQueueAsync(GameClient client)
        {
            Task.Run(() =>
            {
                queue.Remove(client);
            });
        }
        public void CloseGameAsync(GameSession session)
        {
            Task.Run(() =>
            {
                liveGames.Remove(session);
                Console.WriteLine("Live games:{0}",liveGames.Count);
            });
        }
        public void RemoveClientAsync(GameClient client)
        {
            Task.Run(() =>
            {
                players.Remove(client);
                queue.Remove(client);
                Console.WriteLine("Players:{0}",players.Count);
            });
        }
        
    }
}
