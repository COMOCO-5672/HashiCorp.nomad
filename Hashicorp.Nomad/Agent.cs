using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public class Agent : IAgentEndpoint
    {

        private readonly NomadClient _client;

        internal Agent(NomadClient n)
        {
            this._client = n;
        }

        public Task<QueryResult<string>> Members(bool wan, CancellationToken ct = default(CancellationToken))
        {
            return null;
        }
    }
}
