using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public class NomadClient : IDisposable,INomadClient
    {

        internal readonly JsonSerializer serializer = new JsonSerializer();
        private NomadClient.NomadClientConfigurationContainer ConfigContainer;
        private bool disposedValue;
        private Lazy<Agent> _agent;
        private Lazy<Node> _node;
        private Lazy<Status> _status;

        public IAgentEndpoint Agent
        {
            get { return (IAgentEndpoint)this._agent.Value; }
        }

        public INodeEndpoint Node
        {
            get { return (INodeEndpoint) this._node.Value; }
        }

        public IStatusEndpoint Status
        {
            get { return (IStatusEndpoint) this._status.Value; }
        }

        internal HttpClient HttpClient
        {
            get
            {
                return this.ConfigContainer.HttpClient;
            }
        }



        public NomadClientConfiguration Config
        {
            get
            {
                return this.ConfigContainer.Config;
            }
        }
        public void CheckDisposed()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(typeof(NomadClient).FullName.ToString());
        }

        public NomadClient()
            : this((Action<NomadClientConfiguration>)null, (Action<HttpClient>)null, (Action<WebRequestHandler>)null)
        {
        }

        public NomadClient(Action<NomadClientConfiguration> configOverride)
            : this(configOverride, (Action<HttpClient>)null, (Action<WebRequestHandler>)null)
        {
        }

        public NomadClient(Action<NomadClientConfiguration> configOverride, Action<HttpClient> clientOverride)
            : this(configOverride, clientOverride, (Action<WebRequestHandler>)null)
        {
        }

        private void ApplyConfig(NomadClientConfiguration config, WebRequestHandler handler, HttpClient client)
        {
            if (config.HttpAuth != null)
                handler.Credentials = (ICredentials)config.HttpAuth;
            if (config.ClientCertificateSupported)
            {
                if (config.ClientCertificate != null)
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ClientCertificates.Add((X509Certificate)config.ClientCertificate);
                }
                else
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ClientCertificates.Clear();
                }
            }
            if (config.DisableServerCertificateValidation)
                handler.ServerCertificateValidationCallback += (RemoteCertificateValidationCallback)((certSender, cert, chain, sslPolicyErrors) => true);
            else
                handler.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)null;
            if (string.IsNullOrEmpty(config.Token))
                return;
            if (client.DefaultRequestHeaders.Contains("X-Nomad-Token"))
                client.DefaultRequestHeaders.Remove("X-Nomad-Token");
            client.DefaultRequestHeaders.Add("X-Nomad-Token", config.Token);
        }

        public NomadClient(Action<NomadClientConfiguration> configOverride, Action<HttpClient> clientOverride,
            Action<WebRequestHandler> handlerOverride)
        {
            NomadClient.NomadClientConfigurationContainer configurationContainer = new NomadClient.NomadClientConfigurationContainer();
            if (configOverride != null)
                configOverride(configurationContainer.Config);
            this.ApplyConfig(configurationContainer.Config, configurationContainer.HttpHandler, configurationContainer.HttpClient);
            if (handlerOverride != null)
                handlerOverride(configurationContainer.HttpHandler);
            if (clientOverride != null)
                clientOverride(configurationContainer.HttpClient);
            this.ConfigContainer = configurationContainer;
            this.InitializeEndpoints();
        }

      
        private void InitializeEndpoints()
        {
            this._agent=new Lazy<Agent>((Func<Agent>)(() => new Agent(this)));
            this._node=new Lazy<Node>((Func<Node>)(()=>new Node(this)));
            this._status=new Lazy<Status>((Func<Status>)(()=>new Status(this)));
        }

        internal GetRequest<TOut> Get<TOut>(string path, QueryOptions opts = null)
        {
            return new GetRequest<TOut>(this, path, opts ?? QueryOptions.Default);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private class NomadClientConfigurationContainer
        {
            internal readonly bool skipClientDispose;
            internal readonly HttpClient HttpClient;
            internal readonly WebRequestHandler HttpHandler;
            public readonly NomadClientConfiguration Config;
            private bool disposedValue;

            public NomadClientConfigurationContainer()
            {
                this.Config = new NomadClientConfiguration();
                this.HttpHandler = new WebRequestHandler();
                this.HttpClient = new HttpClient((HttpMessageHandler)this.HttpHandler);
                this.HttpClient.Timeout = TimeSpan.FromMinutes(15.0);
                this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                this.HttpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            }

            public NomadClientConfigurationContainer(NomadClientConfiguration config, HttpClient client)
            {
                this.skipClientDispose = true;
                this.Config = config;
                this.HttpClient = client;
            }

            public NomadClientConfigurationContainer(NomadClientConfiguration config)
            {
                this.Config = config;
                this.HttpHandler = new WebRequestHandler();
                this.HttpClient = new HttpClient((HttpMessageHandler)this.HttpHandler);
                this.HttpClient.Timeout = TimeSpan.FromMinutes(15.0);
                this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                this.HttpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            }

            protected virtual void Dispose(bool disposing)
            {
                if (this.disposedValue)
                    return;
                if (disposing)
                {
                    if (this.HttpClient != null && !this.skipClientDispose)
                        this.HttpClient.Dispose();
                    if (this.HttpHandler != null)
                        this.HttpHandler.Dispose();
                }
                this.disposedValue = true;
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize((object)this);
            }

            public void CheckDisposed()
            {
                if (this.disposedValue)
                    throw new ObjectDisposedException(typeof(NomadClient.NomadClientConfigurationContainer).FullName.ToString());
            }
        }

    }
}
