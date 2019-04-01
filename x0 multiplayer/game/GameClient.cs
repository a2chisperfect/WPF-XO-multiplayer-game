using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Message;
using System.Windows;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace game
{
    class GameClient
    {
        TcpClient client;
        NetworkStream dataStream;
        UserInfo info;
        //GameController controller;
        public GameController gameSession { get; set; }
        private Dispatcher dispatcher;
        private MainWindow window;

        public GameClient()
        {
            client = new TcpClient();
            gameSession = new GameController();
        }
        public void Connect(string username, IPAddress ip , int port, MainWindow window)
        {
            try
            {
                client.Connect(new IPEndPoint(ip, port));
                info = new UserInfo(username);
                dataStream = client.GetStream();
                this.window = window;
                dispatcher = Dispatcher.CurrentDispatcher;
                SendMyInfo();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Task SendMessage(byte[] msg)
        {
            return Task.Run(() =>
            {
                try
                {
                    dataStream.Write(msg,0,msg.Length);
                }
                catch
                {
                    client.Close();
                }

            });
        }

        public Task ReciveMessageAsync()
        {

            return Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        byte[] buf = new byte[2024];
                        using (var stream = new MemoryStream())
                        {
                            do
                            {
                                int byteRec = dataStream.Read(buf, 0, buf.Length);
                                stream.Write(buf, 0, byteRec);

                            } while (dataStream.DataAvailable);
                            if (stream.Length != 0)
                            {
                                stream.Position = 0;
                                var tmp = DeserializeMessage(stream);
                                ProceedMessage(tmp);
                            }

                        }
                    }
                }
                catch(Exception ex)
                {
                    dispatcher.BeginInvoke((ThreadStart)delegate() { MessageBox.Show(ex.Message); });
                }
                finally
                {
                    client.Close();
                }
            });
        }
        private void ProceedMessage(Message.Message tmp)
        {
            if (tmp.MessageType == MessageType.MatchReadyInfo)
            {
                info.Side = tmp.Side;
                gameSession.StartGame(info,tmp.UserData,tmp.TurnInfo);

            }
            if (tmp.MessageType == MessageType.TurnInfo)
            {
                gameSession.Turn = tmp.TurnInfo;
                
            }
            if (tmp.MessageType == MessageType.CoordInfo)
            {
                gameSession.PlayerChoice((int)tmp.Coord.X, (int)tmp.Coord.Y);
            }
            if (tmp.MessageType == MessageType.ResultInfo)
            {
                gameSession.MatchResult = tmp.MatchResult;

            }
        }

        private void SendMyInfo()
        {
            try
            {

                Message.Message tmp = new Message.Message(MessageType.UserInfo,info);
                SendMessage(SerializeMessage(tmp));
                
            }
            catch
            {
                throw;
            }
        }
        public void StartSearching()
        {
            try
            {

                Message.Message tmp = new Message.Message(MessageType.SearchInfo,Message.Search.Start);
                SendMessage(SerializeMessage(tmp));

            }
            catch
            {
                throw;
            }       
        }
        public void StopSearching()
        {
            try
            {

                Message.Message tmp = new Message.Message(MessageType.SearchInfo, Message.Search.Stop);
                SendMessage(SerializeMessage(tmp));

            }
            catch
            {
                throw;
            }
        }
        public void MyChoice(Point coord)
        {
            if(gameSession.PlayerChoice((int)coord.X,(int)coord.Y))
            {
                var tmp = new Message.Message(MessageType.CoordInfo, coord);
                SendMessage(SerializeMessage(tmp));
                
            }
        }

        private byte[] SerializeMessage(Message.Message msg)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, msg);
                return stream.ToArray();
            }
        }
        private Message.Message DeserializeMessage(MemoryStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (Message.Message)formatter.Deserialize(stream);
        }
        public void SendQuitMessage()
        {
            var tmp = new Message.Message(MessageType.Quit);
            SendMessage(SerializeMessage(tmp));
        }
    }
}
