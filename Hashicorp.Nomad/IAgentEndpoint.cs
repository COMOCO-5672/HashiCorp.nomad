using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Hashicorp.Nomad
{
    public interface IAgentEndpoint
    {
        Task<QueryResult<string>> Members(bool wan, CancellationToken ct = default(CancellationToken));

    }
}
