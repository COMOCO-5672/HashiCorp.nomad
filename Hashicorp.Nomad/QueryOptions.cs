using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hashicorp.Nomad
{
    public class QueryOptions
    {
        public static readonly QueryOptions Default = new QueryOptions()
        {
            Consistency = ConsistencyMode.Default,
            Datacenter = string.Empty,
            Token = string.Empty,
            WaitIndex = 0
        };

        public string Datacenter { get; set; }

        public ConsistencyMode Consistency { get; set; }

        public ulong WaitIndex { get; set; }

        public TimeSpan? WaitTime { get; set; }

        public string Token { get; set; }

        public string Near { get; set; }
    }

    public enum ConsistencyMode
    {
        Default,
        Consistent,
        Stale,
    }
}