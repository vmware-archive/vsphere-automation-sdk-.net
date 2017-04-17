/*
 * Copyright 2015, 2016 VMware, Inc.  All rights reserved.
 */

namespace vmware.samples.common
{
    using authentication;
    using CommandLine;
    using CommandLine.Text;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using vapi.bindings;

    /// <summary>
    /// Samples common implementation.
    /// </summary>
    public abstract class SamplesBase
    {
        /**
         * Common options across all samples.
         * Sample specific options are provided by the sample itself.
         */
        [Option("server", HelpText = "Hostname of vCenter Server",
            Required = true)]
        public virtual string Server { get; set; }

        [Option("username", HelpText = "Username to login to vCenter Server",
            Required = true)]
        public virtual string UserName { get; set; }

        [Option("password", HelpText = "Password to login to vCenter Server",
            Required = true)]
        public virtual string Password { get; set; }

        [Option("cleardata", HelpText = "Specify this option to undo all"
            + "persistent results of running the sample", Required = false)]
        public virtual bool ClearData { get; set; }

        [Option("skip-server-verification", HelpText = "Optional: Specify this"
            + " option if you do not want to perform SSL certificate "
            + "verification.\n\t\t\t\tNOTE: Circumventing SSL trust in this "
            + " manner is unsafe and should not be used with production code. "
            + "This is ONLY FOR THE PURPOSE OF DEVELOPMENT ENVIRONMENT.",
            DefaultValue = false, Required = false)]
        public virtual bool SkipServerVerification { get; set; }

        public VapiAuthenticationHelper VapiAuthHelper { get; private set;  }

        public VimAuthenticationHelper VimAuthHelper { get; private set; }

        public StubConfiguration SessionStubConfiguration { get; private set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText();
            help.AddDashesToOption = true;
            help.AddPreOptionsLine(this.GetType().Name + " [options]");
            help.AddOptions(this);
            return help;
        }

        /// <summary>
        /// Connects to service endpoints.
        /// </summary>
        public void Login()
        {
            System.Net.ServicePointManager.SecurityProtocol |=
                System.Net.SecurityProtocolType.Tls12;
            VapiAuthHelper = new VapiAuthenticationHelper();
            VimAuthHelper = new VimAuthenticationHelper();

            SessionStubConfiguration =
                VapiAuthHelper.LoginByUsernameAndPassword(
                    Server, UserName, Password);
            VimAuthHelper.LoginByUsernameAndPassword(
                Server, UserName, Password);
        }

        /// <summary>
        /// Runs the sample. Each sample will provider its own implementation
        /// for this method.
        /// </summary>
        public abstract void Run();

        public abstract void Cleanup();

        /// <summary>
        /// Logs out of the server
        /// </summary>
        public void Logout()
        {
            VapiAuthHelper.Logout();
            VimAuthHelper.Logout();
        }

        /// <summary>
        /// Sets up the server certificate validation callback method depending
        ///  on whether skip-server-verification option is specified. If not
        ///  specified, the certificate validation will be done in the
        ///  Validate method, else it will always be true, to ignore
        ///  certificate validation.
        ///
        /// Note: Below code circumvents SSL trust if the
        /// "skip-server-verification" option is specified. Circumventing SSL
        /// trust is unsafe and should not be used in production software. It
        ///  is ONLY FOR THE PURPOSE OF DEVELOPMENT ENVIRONMENT.
        /// </summary>
        public void SetupSslTrustForServer()
        {
            if (SkipServerVerification)
            {
                // ignores server certificate errors
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;

            }
            else
            {
                // validates server certificate
                ServicePointManager.ServerCertificateValidationCallback +=
                    Validate;
            }
        }

        private bool Validate(object sender,
            X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            var result = true;

            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return result;
            }

            if ((sslPolicyErrors &
                SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
            {
                Console.WriteLine("SSL policy error {0}." +
                    " Make sure that your application is using the correct" +
                    " server host name.",
                    SslPolicyErrors.RemoteCertificateNameMismatch);
                result = result && false;
            }

            if ((sslPolicyErrors &
                SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                var chainStatusList = new List<string>();
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (var status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer))
                        {
                            // Self signed certificates with an untrusted root
                            // are valid.
                            continue;
                        }
                        chainStatusList.Add(status.Status.ToString());
                    }
                }
                if (chainStatusList.Count > 0)
                {
                    Console.WriteLine(
                        "SSL policy error {0}. Fix the following errors {1}",
                        SslPolicyErrors.RemoteCertificateChainErrors,
                        string.Join(", ", chainStatusList));
                    result = result && false;
                }
            }

            if ((sslPolicyErrors &
                SslPolicyErrors.RemoteCertificateNotAvailable) != 0)
            {
                Console.WriteLine("SSL policy error {0}." +
                    " The server certificate is not available for validation.",
                    SslPolicyErrors.RemoteCertificateNotAvailable);
                result = result && false;
            }

            return result;
        }

        protected void Execute(string[] args)
        {

            if (Parser.Default.ParseArguments(args, this))
            {
                SetupSslTrustForServer();
                Login();
                Run();
                if (ClearData)
                {
                    Cleanup();
                }
                Logout();
                Console.ReadLine();
            }
        }
    }
}
