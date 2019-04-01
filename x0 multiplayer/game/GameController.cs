using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using Message;

namespace game
{
 
    class GameController
    {

        public event EventHandler<MatchResultEventArgs> MatchResultHandler;
        //public event EventHandler<EventArgs> GameEndedHandler;

        public event EventHandler<TurnEventArgs> TurnChangedHandler;
        public event EventHandler<GameStartedEventArgs> GameStartedHandler;

        public event EventHandler<ChoiceEventArgs> ChoiceDoneHandler;

        private bool gameStarted;

        public bool GameStarted
        {
            get { return gameStarted; }
            set { 
                    gameStarted = value;
                    if (GameStartedHandler != null)
                    {
                        GameStartedHandler(this, new GameStartedEventArgs(you.info, enemy.info));
                    }
                }
        }
        //private bool gameEnded;
        //public bool GameEnded
        //{
        //    get { return gameEnded; }
        //    set
        //    {
        //        gameEnded = value;
        //        if (GameEndedHandler != null)
        //        {
        //            GameEndedHandler(this, new EventArgs());
        //        }
        //    }
        //}
        

        private MatchResult matchResult;
        public MatchResult MatchResult
        {
            get { return matchResult; }
            set
            {
                matchResult = value;
                if (MatchResultHandler != null && matchResult != MatchResult.None)
                {
                    MatchResultHandler(this, new MatchResultEventArgs(matchResult));
                }
            }
        }
        private Turn turn;
        public Turn Turn
        {
            get { return turn; }
            set
            {
                turn = value;
                if (TurnChangedHandler != null)
                {
                    TurnChangedHandler(this, new TurnEventArgs(turn));
                }
            }
        }
        
        private Player you;
        private Enemy enemy;
     
        private Field field;

        
        public GameController()
        {
            field = new Field();
            GameStarted = false;         
        }

        public void StartGame(UserInfo your, UserInfo enemy,Turn turn)
        {
            matchResult = MatchResult.None;
            you = new Player(your, your.Side);
            this.enemy = new Enemy(enemy, enemy.Side);
            field.ClearField();
            GameStarted = true;
            Turn = turn;
            
        }
        public bool PlayerChoice(int x, int y)
        {
            if(field.GetCell(x,y) == Side.empty && MatchResult == Message.MatchResult.None)
            {
                char symbol= Turn == Turn.Your?you.symbol:enemy.symbol;
                Side tmp = Turn == Turn.Your?you.info.Side:enemy.info.Side;
                field.SetCell(x, y, tmp);
                ChoiceDoneHandler(this, new ChoiceEventArgs(x, y,symbol));
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    public class TurnEventArgs : EventArgs
    {
        public Turn turn { get; set; }
        public TurnEventArgs(Turn t)
        {
            turn = t;
        }
    }

    public class ChoiceEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y  { get; set; }
        public char symbol { get; set; }
        public ChoiceEventArgs(int x , int y, char s)
        {
            X = x;
            Y = y;
            symbol = s;
        }
    }

    public class MatchResultEventArgs : EventArgs
    {
        public MatchResult result { get; set; }
        public MatchResultEventArgs(MatchResult res)
        {
            result = res;
        }
    }
    public class GameStartedEventArgs : EventArgs
    {
        public UserInfo Your { get; set; }
        public UserInfo Enemy { get; set; }
        public GameStartedEventArgs(UserInfo your, UserInfo enemy)
        {
            Your = your;
            Enemy = enemy;
        }
    }
}

