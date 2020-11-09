using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    public class NomadClientConfiguration
    {
        internal static Lazy<bool> _clientCertSupport = new Lazy<bool>((Func<bool>)(() => Type.GetType("Mono.Runtime") == (Type)null));
        private NetworkCredential _httpauth;
        private bool _disableServerCertificateValidation;
        private X509Certificate2 _clientCertificate;

        internal event EventHandler Updated;

        internal bool ClientCertificateSupported
        {
            get
            {
                return NomadClientConfiguration._clientCertSupport.Value;
            }
        }

        [Obsolete("Use of DisableServerCertificateValidation should be converted to setting the WebRequestHandler's ServerCertificateValidationCallback in the NomadClient constructorThis property will be removed when 0.8.0 is released.", false)]
        internal bool DisableServerCertificateValidation
        {
            get
            {
                return this._disableServerCertificateValidation;
            }
            set
            {
                this._disableServerCertificateValidation = value;
                this.OnUpdated(new EventArgs());
            }
        }

        public Uri Address { get; set; }

        public string Datacenter { get; set; }

        [Obsolete("Use of HttpAuth should be converted to setting the WebRequestHandler's Credential property in the NomadClient constructorThis property will be removed when 0.8.0 is released.", false)]
        public NetworkCredential HttpAuth
        {
            internal get
            {
                return this._httpauth;
            }
            set
            {
                this._httpauth = value;
                this.OnUpdated(new EventArgs());
            }
        }

        [Obsolete("Use of ClientCertificate should be converted to adding to the WebRequestHandler's ClientCertificates list in the NomadClient constructor.This property will be removed when 0.8.0 is released.", false)]
        public X509Certificate2 ClientCertificate
        {
            internal get
            {
                return this._clientCertificate;
            }
            set
            {
                if (!this.ClientCertificateSupported)
                    throw new PlatformNotSupportedException("Client certificates are not supported on this platform");
                this._clientCertificate = value;
                this.OnUpdated(new EventArgs());
            }
        }

        public string Token { get; set; }

        public TimeSpan? WaitTime { get; set; }

        public NomadClientConfiguration()
        {
            UriBuilder nomadAddress = new UriBuilder("http://127.0.0.1:4646");
            this.ConfigureFromEnvironment(nomadAddress);
            this.Address = nomadAddress.Uri;
        }

        private void ConfigureFromEnvironment(UriBuilder nomadAddress)
        {
            string lowerInvariant1 = (Environment.GetEnvironmentVariable("NOMAD_HTTP_ADDR") ?? string.Empty).Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(lowerInvariant1))
            {
                string[] strArray = lowerInvariant1.Split(':');
                for (int index = 0; index < strArray.Length; ++index)
                    strArray[index] = strArray[index].Trim();
                if (!string.IsNullOrEmpty(strArray[0]))
                    nomadAddress.Host = strArray[0];
                if (strArray.Length > 1)
                {
                    if (!string.IsNullOrEmpty(strArray[1]))
                    {
                        try
                        {
                            nomadAddress.Port = (int)ushort.Parse(strArray[1]);
                        }
                        catch (Exception ex)
                        {
                            throw new NomadClientConfigurationException("Failed parsing port from environment variable NOMAD_HTTP_ADDR", ex);
                        }
                    }
                }
            }
            string lowerInvariant2 = (Environment.GetEnvironmentVariable("NOMAD_HTTP_SSL") ?? string.Empty).Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(lowerInvariant2))
            {
                try
                {
                    if (!(lowerInvariant2 == "1"))
                    {
                        if (!bool.Parse(lowerInvariant2))
                            goto label_15;
                    }
                    nomadAddress.Scheme = "https";
                }
                catch (Exception ex)
                {
                    throw new NomadClientConfigurationException("Could not parse environment variable NOMAD_HTTP_SSL", ex);
                }
            }
        label_15:
            string lowerInvariant3 = (Environment.GetEnvironmentVariable("NOMAD_HTTP_SSL_VERIFY") ?? string.Empty).Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(lowerInvariant3))
            {
                try
                {
                    if (!(lowerInvariant3 == "0"))
                    {
                        if (!bool.Parse(lowerInvariant3))
                            goto label_21;
                    }
                    this.DisableServerCertificateValidation = true;
                }
                catch (Exception ex)
                {
                    throw new NomadClientConfigurationException("Could not parse environment variable NOMAD_HTTP_SSL_VERIFY", ex);
                }
            }
        label_21:
            string environmentVariable = Environment.GetEnvironmentVariable("NOMAD_HTTP_AUTH");
            if (!string.IsNullOrEmpty(environmentVariable))
            {
                NetworkCredential networkCredential = new NetworkCredential();
                if (environmentVariable.Contains(":"))
                {
                    string[] strArray = environmentVariable.Split(':');
                    networkCredential.UserName = strArray[0];
                    networkCredential.Password = strArray[1];
                }
                else
                    networkCredential.UserName = environmentVariable;
                this.HttpAuth = networkCredential;
            }
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NOMAD_HTTP_TOKEN")))
                return;
            this.Token = Environment.GetEnvironmentVariable("NOMAD_HTTP_TOKEN");
        }

        internal virtual void OnUpdated(EventArgs e)
        {
            // ISSUE: reference to a compiler-generated field
            EventHandler updated = this.Updated;
            if (updated == null)
                return;
            updated((object)this, e);
        }
    }
}
