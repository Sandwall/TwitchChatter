using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatter
{
    public class Message
    {
        public string Content;  //Text in message
        public string Sender;   //Author of message
        public string Color;    //In hex format

        public Message(string content, string sender, string color)
        {
            Content = content;
            Sender = sender;
            Color = color;
        }
    }
}
