using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatter
{
    public class MessagePool
    {
        public Queue<Message> messages { get; }

        public MessagePool(int messageLimit)
        {
            messages = new Queue<Message>(messageLimit);
        }

        public void ChangeCapacity(int c)
        {
            messages.EnsureCapacity(c);
        }

        public void Add(Message msg)
        {
            messages.Enqueue(msg);
        }
    }
}
