using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace Hashicorp.Nomad
{
    public abstract class NomadRequest
    {
        protected Stopwatch timer = new Stopwatch();
        internal NomadClient Client { get; set; }

        internal HttpMethod Method { get; set; }

        internal Dictionary<string, string> Params { get; set; }

        internal Stream ResponseStream { get; set; }

        internal string Endpoint { get; set; }
        internal NomadRequest(NomadClient client, string url, HttpMethod method)
        {
            this.Client = client;
            this.Method = method;
            this.Endpoint = url;
            this.Params = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(client.Config.Datacenter))
                this.Params["dc"] = client.Config.Datacenter;
            TimeSpan? waitTime = client.Config.WaitTime;
            if (!waitTime.HasValue)
                return;
            Dictionary<string, string> dictionary = this.Params;
            string index = "wait";
            waitTime = client.Config.WaitTime;
            string goDuration = waitTime.Value.ToGoDuration();
            dictionary[index] = goDuration;
        }

        protected abstract void ApplyOptions(NomadClientConfiguration clientConfig);

        protected abstract void ApplyHeaders(HttpRequestMessage message, NomadClientConfiguration clientConfig);

        protected Uri BuildNomadUri(string url, Dictionary<string, string> p)
        {
            UriBuilder uriBuilder = new UriBuilder(this.Client.Config.Address);
            uriBuilder.Path = url;
            this.ApplyOptions(this.Client.Config);
            List<string> stringList = new List<string>(this.Params.Count / 2);
            foreach (KeyValuePair<string, string> keyValuePair in this.Params)
            {
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                    stringList.Add(string.Format("{0}={1}", (object)Uri.EscapeDataString(keyValuePair.Key), (object)Uri.EscapeDataString(keyValuePair.Value)));
                else
                    stringList.Add(string.Format("{0}", (object)Uri.EscapeDataString(keyValuePair.Key)));
            }
            uriBuilder.Query = string.Join("&", (IEnumerable<string>)stringList);
            return uriBuilder.Uri;
        }

        protected TOut Deserialize<TOut>(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                using (JsonTextReader jsonTextReader = new JsonTextReader((TextReader)streamReader))
                    return this.Client.serializer.Deserialize<TOut>((JsonReader)jsonTextReader);
            }
        }

        protected byte[] Serialize(object value)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
        }

    }
}
