using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Message;

namespace game
{
    
    interface IPlayer
    {
        UserInfo info { get; set; }
        char symbol { get; set; }
    }
    class Player :IPlayer
    {
        public UserInfo info { get; set; }
        public char symbol { get; set; }
        public Player(UserInfo player,Side side)
        {
            info = player;
            if(side == Side.X)
            {
                symbol = 'X';
            }
            else
            {
                symbol = 'O';
            }
        }
    }
    class Enemy :IPlayer
    {
        public UserInfo info { get; set; }
        public char symbol { get; set; }
        public Enemy(UserInfo player,Side side)
        {
            info = player;
            if(side == Side.X)
            {
                symbol = 'X';
            }
            else
            {
                symbol = 'O';
            }
        }
    }
}

