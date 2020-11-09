using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public class Node : INodeEndpoint
    {

        private readonly NomadClient _client;

        internal Node(NomadClient c)
        {
            this._client = c;
        }

        public Task<QueryResult<NodeInfo[]>> Nodes(CancellationToken ct = default(CancellationToken))
        {
            return this.Nodes(QueryOptions.Default, ct);
        }

        public Task<QueryResult<NodeInfo[]>> Nodes(QueryOptions q, CancellationToken ct = default(CancellationToken))
        {
            return this._client.Get<NodeInfo[]>("/v1/nodes",q).Execute(ct);
        }

        public Task<QueryResult<NodeDetailInfo>> NodeDetailInfo(string NodeID, CancellationToken ct=default(CancellationToken))
        {
            return this.NodeDetailInfo(NodeID,QueryOptions.Default,ct);
        }
        public Task<QueryResult<NodeDetailInfo>> NodeDetailInfo(string NodeID, QueryOptions q, CancellationToken ct=default(CancellationToken))
        {
            return this._client.Get<NodeDetailInfo>(string.Format("/v1/node/{0}", (object) NodeID), q).Execute(ct);
        }
    }

}
