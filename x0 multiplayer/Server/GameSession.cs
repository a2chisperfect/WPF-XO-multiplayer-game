using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Message;
using System.Threading;


namespace Server
{
    class GameSession
    {
        private GameClient winner;

        public GameClient Winner
        {
            get { return winner; }
            set { winner = value;}
        }
        
        Field field;
        GameServer server;
        List<GameClient> players;
        GameClient Xplayer;
        GameClient Oplayer;

        public GameSession(GameClient player1, GameClient player2, GameServer server)
        {
            this.server = server;
            player1.liveGame = this;
            player2.liveGame = this;
            players = new List<GameClient>();
            players.Add(player1);
            players.Add(player2);
            GameStarted();
        }
        private void GameStarted()
        {
            field = new Field();
            field.ClearField();

            Random rnd = new Random();
            if(rnd.Next(1) == 0)
            {
                players[1].info.Side = Side.O;
                var tmp = new Message.Message(MessageType.MatchReadyInfo, Side.X, Turn.Your, players[1].info);
                SendMessage(players[0], tmp);
                players[0].info.Side = Side.X;
                var tmp2 = new Message.Message(MessageType.MatchReadyInfo, Side.O, Turn.Enemy, players[0].info);
                SendMessage(players[1], tmp2);
            }
            else
            {
                players[0].info.Side = Side.O;
                var tmp = new Message.Message(MessageType.MatchReadyInfo, Side.X, Turn.Your, players[0].info);
                SendMessage(players[1], tmp);
                players[1].info.Side = Side.X;
                var tmp2 = new Message.Message(MessageType.MatchReadyInfo, Side.O, Turn.Enemy, players[1].info);
                SendMessage(players[0], tmp2);
            }
            Oplayer = players.FirstOrDefault(p => p.info.Side == Side.O);
            Xplayer = players.FirstOrDefault(p => p.info.Side == Side.X);

        }
        public void GameClosed()
        {
            foreach (var player in players)
            {
                player.liveGame = null;
            }
            server.CloseGameAsync(this);
           
        }
        public void UserLeft(GameClient player)
        {
            SendMessage(players.FirstOrDefault(p => p.info.Id != player.info.Id),new Message.Message(MessageType.ResultInfo, MatchResult.Win));
            GameClosed();
        }

        public void GotMessage(GameClient player, Message.Message msg ,byte[] message)
        {
            Task.Run(() =>
            {

                if (msg.MessageType == MessageType.CoordInfo)
                {
                    
                    field.SetCell((int)msg.Coord.X, (int)msg.Coord.Y, player.info.Side);
                    GameStatus();
                    SendMessage(player, new Message.Message(MessageType.TurnInfo, Turn.Enemy));
                    var tmp = players.FirstOrDefault(p => p.info.Id != player.info.Id);
                    SendMessage(tmp, message);
                    SendMessage(tmp, new Message.Message(MessageType.TurnInfo, Turn.Your));
                }
                if(msg.MessageType == MessageType.Quit)
                {
                    UserLeft(player);
                }

            });
               
        }
        private void SendMessage(GameClient player, byte[] message)
        {
            player.SendMessageAsync(message);
        }
        private void SendMessage(GameClient player, Message.Message message)
        {
            player.SendMessageAsync(message);
        }
        private void GameStatus()
        {
            if (field.GetCell(0, 0) == field.GetCell(0, 1) && field.GetCell(0, 1) == field.GetCell(0, 2) || field.GetCell(0, 0) == field.GetCell(1, 0) && field.GetCell(1, 0) == field.GetCell(2, 0) || field.GetCell(0, 0) == field.GetCell(1, 1) && field.GetCell(1, 1) == field.GetCell(2, 2))
            {
                if (field.GetCell(0, 0) == Side.X) Winner = Xplayer;
                if (field.GetCell(0, 0) == Side.O) Winner = Oplayer;
            }
            if (field.GetCell(2, 0) == field.GetCell(2, 1) && field.GetCell(2, 1) == field.GetCell(2, 2) || field.GetCell(2, 2) == field.GetCell(1, 2) && field.GetCell(1, 2) == field.GetCell(0, 2))
            {
                if (field.GetCell(2, 2) == Side.X) Winner = Xplayer;
                if (field.GetCell(2, 2) == Side.O) Winner = Oplayer;
            }
            if (field.GetCell(1, 0) == field.GetCell(1, 1) && field.GetCell(1, 1) == field.GetCell(1, 2) || field.GetCell(0, 1) == field.GetCell(1, 1) && field.GetCell(1, 1) == field.GetCell(2, 1) || field.GetCell(2, 0) == field.GetCell(1, 1) && field.GetCell(1, 1) == field.GetCell(0, 2))
            {
                if (field.GetCell(1, 1) == Side.X) Winner = Xplayer;
                if (field.GetCell(1, 1) == Side.O) Winner = Oplayer;
            }
            if (winner != null)
            {
                SendMessage(winner, new Message.Message(MessageType.ResultInfo, MatchResult.Win));
                SendMessage(players.FirstOrDefault(p => p.info.Side != winner.info.Side),new Message.Message(MessageType.ResultInfo, MatchResult.Lose));
                GameClosed();
                
            } 
            if(field.FreeCells() == 0)
            {
                foreach (var player in players)
	            {
                    SendMessage(player,new Message.Message(MessageType.ResultInfo, MatchResult.Draw));
	            }
                GameClosed();
            }

        }
    }
}
