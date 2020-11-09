using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public class NodeInfo
    {
        [Description("IP地址")]
        public string Address { get; set; }

        [Description("唯一ID")]
        public string ID { get; set; }

        [Description("数据中心")]
        public string Datacenter { get; set; }
        [Description("节点名称")]
        public string Name { get; set; }

        [Description("")]
        public string NodeClass { get; set; }
        [Description("运行版本号")]
        public string Version { get; set; }
        [Description("是否排空")]
        public bool Drain { get; set; }
        [Description("是否限制分配")]
        public SchedulingEligibility SchedulingEligibility { get; set; }
        [Description("节点状态")]
        public string Status { get; set; }
        [Description("节点状态描述")]
        public string StatusDescription { get; set; }
        [Description("")]
        public string HostVolumes { get; set; }
        [Description("")]
        public string CreateIndex { get; set; }
        [Description("")]
        public string ModifyIndex { get; set; }
    }

    public enum SchedulingEligibility
    {
        eligible,
        ineligible
    }

    public class NodeDetailInfo
    {
        public NodeDetailInfo()
        {
            Meta=new Dictionary<string, string>();
        }
        [Description("是否支持TLS")]
        public bool TLSEnabled { get; set; }
        [Description("HTTP访问地址")]
        public string HTTPAddr { get; set; }
        [Description("主机名")]
        public string Name { get; set; }
        [Description("计算机Meta，只有客户端有")]
        public Dictionary<string,string> Meta { get; set; }

        [Description("数据中心")]
        public string Datacenter { get; set; }

        [Description("秘钥ID")]
        public string SecretID { get; set; }

        [Description("计算机ID")]
        public string ID { get; set; }
        [Description("")]
        public string ComputedClass { get; set; }

        [Description("")]
        public string NodeClass { get; set; }

        [Description("是否排空")]
        public bool Drain { get; set; }

        [Description("是否限制分配")]
        public string DrainStrategy { get; set; }

        [Description("计算机状态")]
        public string Status { get; set; }

        [Description("计算机状态描述")]
        public string StatusDescription { get; set; }

        [Description("状态更新事件")]
        public string StatusUpdatedAt { get; set; }

    }   
}
