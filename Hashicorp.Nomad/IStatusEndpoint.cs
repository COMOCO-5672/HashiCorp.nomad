using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public interface IStatusEndpoint
    {
        Task<QueryResult<string[]>> GetStatusServers(System.Threading.CancellationToken ct = default(System.Threading.CancellationToken));
        Task<QueryResult<string[]>> GetStatusServers(QueryOptions q,System.Threading.CancellationToken ct=default(System.Threading.CancellationToken));
        Task<QueryResult<string>> GetStatusLeader(System.Threading.CancellationToken ct = default(System.Threading.CancellationToken));
        Task<QueryResult<string>> GetStatusLeader(QueryOptions q, System.Threading.CancellationToken ct = default(System.Threading.CancellationToken));
    }
}
