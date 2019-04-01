using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Message;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace Server
{
    class GameClient
    {
        TcpClient client;
        NetworkStream stream;
        public UserInfo info { get; set; }
        GameServer server;
        public GameSession liveGame { get; set; }

        public GameClient(TcpClient client, GameServer server)
        {
            this.client = client;
            this.server = server;
            stream = client.GetStream();
        }

        public void SendMessageAsync(byte[] msg)
        {
            try
            {

                stream.WriteAsync(msg, 0, msg.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void SendMessageAsync(Message.Message msg)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (var s = new MemoryStream())
                {
                    formatter.Serialize(s, msg);
                    stream.WriteAsync(s.ToArray(), 0, (int)s.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public async void ReciveMessage()
        {
            try
            {
                while (true)
                {
                    byte[] buff = new byte[2024];
                    await stream.ReadAsync(buff, 0, buff.Length);
                    using (var s = new MemoryStream(buff))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        Message.Message tmp = (Message.Message)formatter.Deserialize(s);
                        if (tmp.MessageType == MessageType.UserInfo)
                        {
                            info = tmp.UserData;
                        }
                        else if (tmp.MessageType == MessageType.SearchInfo)
                        {
                            if (tmp.SearchInfo == Search.Start)
                            {
                                server.AddInQueueAsync(this);
                            }
                            else
                            {
                                server.RemoveFromQueueAsync(this);
                            }
                        }
                        if (liveGame != null)
                        {
                            liveGame.GotMessage(this, tmp, buff);
                        }
                        if (tmp.MessageType == MessageType.Quit)
                        {
                            server.RemoveClientAsync(this);
                            break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                
                client.Close();
            }
                
        }
    }
}
