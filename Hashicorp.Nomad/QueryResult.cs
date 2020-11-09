using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public class QueryResult:NomadResult
    {
        public ulong LastIndex { get; set; }

        public TimeSpan LastContact { get; set; }

        public bool KnownLeader { get; set; }

        public bool AddressTranslationEnabled { get; set; }

        public QueryResult()
        {
        }
        public QueryResult(QueryResult other)
            : base((NomadResult)other)
        {
            this.LastIndex = other.LastIndex;
            this.LastContact = other.LastContact;
            this.KnownLeader = other.KnownLeader;
        }
    }
    public class QueryResult<T> : QueryResult
    {
        public T Response { get; set; }

        public QueryResult()
        {
        }

        public QueryResult(QueryResult other)
            : base(other)
        {
        }

        public QueryResult(QueryResult other, T value)
            : base(other)
        {
            this.Response = value;
        }
    }
}
