using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebox.Core.Extensions.Exception
{
    public class ClientNotificationException : System.Exception
    {
        public ClientNotificationException(string message) : base(message)
        {

        }
    }
}
