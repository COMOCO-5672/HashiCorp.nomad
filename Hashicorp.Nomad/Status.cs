using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public class Status:IStatusEndpoint
    {
        private readonly NomadClient _client;
        public Status(NomadClient client)
        {
            this._client = client;
        }

        public Task<QueryResult<string>> GetStatusLeader(CancellationToken ct = default(CancellationToken))
        {
            return this.GetStatusLeader(QueryOptions.Default, ct);
        }

        public Task<QueryResult<string>> GetStatusLeader(QueryOptions q, CancellationToken ct = default(CancellationToken))
        {
            return this._client.Get<string>("/v1/status/leader",q).Execute(ct);
        }

        public Task<QueryResult<string[]>> GetStatusServers(CancellationToken ct=default(CancellationToken))
        {
            return this.GetStatusServers(QueryOptions.Default, ct);
        }

        public Task<QueryResult<string[]>> GetStatusServers(QueryOptions q, CancellationToken ct = default(CancellationToken))
        {
            return this._client.Get<string[]>("/v1/status/peers", q).Execute(ct);
        }
    }
}
