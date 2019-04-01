using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message
{
 
    public enum MessageType { UserInfo, SearchInfo, MatchReadyInfo, SideInfo, TurnInfo, CoordInfo, ResultInfo ,Quit };

    public enum Turn {Your, Enemy};
 
    public enum Search {Start,Stop}

    public enum MatchResult {Win, Lose, Draw, None}

    public enum Side {empty,X,O}
    [Serializable]
    public class Message
    {
        public UserInfo UserData { get; set; }
        public MessageType MessageType { get; set; }
        public Search SearchInfo { get; set; }
        public Side Side {get;set;}
        public Turn TurnInfo { get; set; }
        public Point Coord { get; set; }
        public MatchResult MatchResult { get; set; }

        public Message(MessageType type)
        {
            MessageType = type;
        }
        public Message(MessageType type, UserInfo info)
        {
            MessageType = type;
            UserData = info;
        }
        public Message(MessageType type, Search searchInfo)
        {
            MessageType = type;
            SearchInfo = searchInfo;
        }
        public Message(MessageType type, Side sideInfo, Turn turnInfo, UserInfo enemy)
        {
            MessageType = type;
            Side = sideInfo;
            TurnInfo = turnInfo;
            UserData = enemy;
            
        }
        public Message(MessageType type, Turn turnInfo)
        {
            MessageType = type;
            TurnInfo = turnInfo;
        }
        public Message(MessageType type, Point coord)
        {
            MessageType = type;
            Coord = coord;
        }
        public Message(MessageType type, MatchResult matchResult)
        {
            MessageType = type;
            MatchResult = matchResult;
        }
    }


}
