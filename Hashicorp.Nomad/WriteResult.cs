using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public class WriteResult<T> : WriteResult
    {
        public T Response { get; set; }

        public WriteResult()
        {
        }

        public WriteResult(WriteResult other)
            : base(other)
        {
        }

        public WriteResult(WriteResult other, T value)
            : base(other)
        {
            this.Response = value;
        }
    }

    public class WriteResult : NomadResult
    {
        public WriteResult()
        {
        }

        public WriteResult(WriteResult other)
            : base((NomadResult)other)
        {
        }
    }
    public abstract class NomadResult
    {
        public TimeSpan RequestTime { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public NomadResult()
        {
        }

        public NomadResult(NomadResult other)
        {
            this.RequestTime = other.RequestTime;
            this.StatusCode = other.StatusCode;
        }
    }
}
