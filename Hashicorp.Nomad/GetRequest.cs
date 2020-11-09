using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public partial class GetRequest<TOut> : NomadRequest
    {
        public QueryOptions Options { get; set; }

        public GetRequest(NomadClient client, string url, QueryOptions options = null)
          : base(client, url, HttpMethod.Get)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException(nameof(url));
            this.Options = options ?? QueryOptions.Default;
        }

        public async Task<QueryResult<TOut>> Execute(CancellationToken ct)
        {
            GetRequest<TOut> getRequest = this;
            getRequest.Client.CheckDisposed();
            getRequest.timer.Start();
            QueryResult<TOut> result = new QueryResult<TOut>();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, getRequest.BuildNomadUri(getRequest.Endpoint, getRequest.Params));
            getRequest.ApplyHeaders(httpRequestMessage, getRequest.Client.Config);
            HttpResponseMessage response = await getRequest.Client.HttpClient.SendAsync(httpRequestMessage, ct).ConfigureAwait(false);
            getRequest.ParseQueryHeaders(response, result);
            result.StatusCode = response.StatusCode;
            Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            getRequest.ResponseStream = stream;
            if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
            {
                if (getRequest.ResponseStream == null)
                    throw new NomadRequestException(string.Format("Unexpected response, status code {0}", (object)response.StatusCode), response.StatusCode);
                using (StreamReader streamReader = new StreamReader(getRequest.ResponseStream))
                    throw new NomadRequestException(string.Format("Unexpected response, status code {0}: {1}", (object)response.StatusCode, (object)streamReader.ReadToEnd()), response.StatusCode);
            }
            else
            {
                if (response.IsSuccessStatusCode)
                    result.Response = getRequest.Deserialize<TOut>(getRequest.ResponseStream);
                result.RequestTime = getRequest.timer.Elapsed;
                getRequest.timer.Stop();
                return result;
            }
        }

        public async Task<QueryResult<Stream>> ExecuteStreaming(CancellationToken ct)
        {
            GetRequest<TOut> getRequest = this;
            getRequest.Client.CheckDisposed();
            getRequest.timer.Start();
            QueryResult<Stream> result = new QueryResult<Stream>();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, getRequest.BuildNomadUri(getRequest.Endpoint, getRequest.Params));
            getRequest.ApplyHeaders(httpRequestMessage, getRequest.Client.Config);
            HttpResponseMessage response = await getRequest.Client.HttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            getRequest.ParseQueryHeaders(response, result as QueryResult<TOut>);
            result.StatusCode = response.StatusCode;
            Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            getRequest.ResponseStream = stream;
            result.Response = getRequest.ResponseStream;
            if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
                throw new NomadRequestException(string.Format("Unexpected response, status code {0}", (object)response.StatusCode), response.StatusCode);
            result.RequestTime = getRequest.timer.Elapsed;
            getRequest.timer.Stop();
            return result;
        }

        protected override void ApplyOptions(NomadClientConfiguration clientConfig)
        {
            if (this.Options == QueryOptions.Default)
                return;
            if (!string.IsNullOrEmpty(this.Options.Datacenter))
                this.Params["dc"] = this.Options.Datacenter;
            switch (this.Options.Consistency)
            {
                case ConsistencyMode.Consistent:
                    this.Params["consistent"] = string.Empty;
                    break;
                case ConsistencyMode.Stale:
                    this.Params["stale"] = string.Empty;
                    break;
            }
            if (this.Options.WaitIndex != 0UL)
                this.Params["index"] = this.Options.WaitIndex.ToString();
            TimeSpan? waitTime = this.Options.WaitTime;
            if (waitTime.HasValue)
            {
                Dictionary<string, string> dictionary = this.Params;
                string index = "wait";
                waitTime = this.Options.WaitTime;
                string goDuration = waitTime.Value.ToGoDuration();
                dictionary[index] = goDuration;
            }
            if (string.IsNullOrEmpty(this.Options.Near))
                return;
            this.Params["near"] = this.Options.Near;
        }

        protected void ParseQueryHeaders(HttpResponseMessage res, QueryResult<TOut> meta)
        {
            HttpResponseHeaders headers = res.Headers;
            if (headers.Contains("X-Nomad-Index"))
            {
                try
                {
                    meta.LastIndex = ulong.Parse(headers.GetValues("X-Nomad-Index").First<string>());
                }
                catch (Exception ex)
                {
                    throw new NomadRequestException("Failed to parse X-Nomad-Index", res.StatusCode, ex);
                }
            }
            if (headers.Contains("X-Nomad-LastContact"))
            {
                try
                {
                    meta.LastContact = TimeSpan.FromMilliseconds((double)ulong.Parse(headers.GetValues("X-Nomad-LastContact").First<string>()));
                }
                catch (Exception ex)
                {
                    throw new NomadRequestException("Failed to parse X-Nomad-LastContact", res.StatusCode, ex);
                }
            }
            if (headers.Contains("X-Nomad-KnownLeader"))
            {
                try
                {
                    meta.KnownLeader = bool.Parse(headers.GetValues("X-Nomad-KnownLeader").First<string>());
                }
                catch (Exception ex)
                {
                    throw new NomadRequestException("Failed to parse X-Nomad-KnownLeader", res.StatusCode, ex);
                }
            }
            if (!headers.Contains("X-Nomad-Translate-Addresses"))
                return;
            try
            {
                meta.AddressTranslationEnabled = bool.Parse(headers.GetValues("X-Nomad-Translate-Addresses").First<string>());
            }
            catch (Exception ex)
            {
                throw new NomadRequestException("Failed to parse X-Nomad-Translate-Addresses", res.StatusCode, ex);
            }
        }

        protected override void ApplyHeaders(HttpRequestMessage message, NomadClientConfiguration clientConfig)
        {
            if (string.IsNullOrEmpty(this.Options.Token))
                return;
            message.Headers.Add("X-Nomad-Token", this.Options.Token);
        }
    }
}
