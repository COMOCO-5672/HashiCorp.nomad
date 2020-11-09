using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public interface INodeEndpoint
    {
        Task<QueryResult<NodeInfo[]>> Nodes(CancellationToken ct = default(CancellationToken));
        Task<QueryResult<NodeInfo[]>> Nodes(QueryOptions q, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<NodeDetailInfo>> NodeDetailInfo(string NodeID, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<NodeDetailInfo>> NodeDetailInfo(string NodeID,QueryOptions q, CancellationToken ct = default(CancellationToken));
    } 

}
