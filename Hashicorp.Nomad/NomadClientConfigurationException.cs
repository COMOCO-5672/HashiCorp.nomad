using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    [Serializable]
    public class NomadClientConfigurationException:Exception
    {
        public NomadClientConfigurationException()
        {
        }

        public NomadClientConfigurationException(string message)
            : base(message)
        {
        }

        public NomadClientConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NomadClientConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
