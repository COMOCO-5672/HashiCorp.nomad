using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    [Serializable]
    public class NomadRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public NomadRequestException()
        {
        }

        public NomadRequestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        public NomadRequestException(string message, HttpStatusCode statusCode, Exception inner)
            : base(message, inner)
        {
            this.StatusCode = statusCode;
        }

        protected NomadRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StatusCode", (object)this.StatusCode);
        }
    }
}
