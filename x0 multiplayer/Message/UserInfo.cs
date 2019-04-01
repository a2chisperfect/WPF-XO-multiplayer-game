using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message
{
     [Serializable]
    public class UserInfo
    {
        public string Name { get; private set; }
        public Guid Id { get; private set; }
        public Side Side { get; set; }
        public UserInfo(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
