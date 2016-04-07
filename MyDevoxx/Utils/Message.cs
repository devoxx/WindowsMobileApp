using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDevoxx.Utils
{
    public abstract class Message<T>
    {
        public abstract T Value { get; set; }
    }
}
