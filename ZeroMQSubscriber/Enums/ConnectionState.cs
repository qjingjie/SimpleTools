using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroMQSubscriber.Enums
{
    public enum ConnectionState
    {
        Unconfigured,
        Error,
        Connected,
        Bound
    }
}