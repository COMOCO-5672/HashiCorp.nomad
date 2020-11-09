using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public interface INomadClient:IDisposable
    {
        IAgentEndpoint Agent { get; }
        IStatusEndpoint Status { get; }
        INodeEndpoint Node { get; }
    }
}
