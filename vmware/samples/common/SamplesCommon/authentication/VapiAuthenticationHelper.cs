/**
 * *******************************************************
 * Copyright VMware, Inc. 2016.  All Rights Reserved.
 * SPDX-License-Identifier: MIT
 * *******************************************************
 *
 * DISCLAIMER. THIS PROGRAM IS PROVIDED TO YOU "AS IS" WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, WHETHER ORAL OR WRITTEN,
 * EXPRESS OR IMPLIED. THE AUTHOR SPECIFICALLY DISCLAIMS ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
 */
namespace vmware.samples.common.authentication
{
    using System;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using vmware.cis;
    using vmware.vapi.bindings;
    using vmware.vapi.core;
    using vmware.vapi.dsig;
    using vmware.vapi.protocol;
    using vmware.vapi.security;
    using vmware.vapi.util.security;

    /// <summary>
    /// vAPI helper class which provides methods for authentication
    /// </summary>
    public class VapiAuthenticationHelper
    {
        private Session sessionSvc;
        public StubFactory StubFactory { get; private set; }
        public static readonly string VAPI_PATH = "/api";

        /// <summary>
        /// Creates a session with the server using username and password
        /// </summary>
        /// <param name="server">hostname of the server to login</param>
        /// <param name="username">username for login</param>
        /// <param name="password">password for login</param>
        /// <returns>the stub configuration configured with an authenticated
        ///          session
        /// </returns>
        public StubConfiguration LoginByUsernameAndPassword(string server,
            string username, string password)
        {
            if (this.sessionSvc != null)
            {
                throw new Exception("Session already created");
            }

            StubFactory = CreateApiStubFactory(server);

            // Create a security context for username/password authentication
            ExecutionContext.SecurityContext securityContext =
                new UserPassSecurityContext(
                    username, password.ToCharArray());

            /*
             * Create a stub configuration with username/password security
             * context
             */
            StubConfiguration stubConfig = new StubConfiguration();
            stubConfig.SetSecurityContext(securityContext);

            // Create a session stub using the stub configuration.
            Session session =
                    StubFactory.CreateStub<Session>(stubConfig);

            // Login and create a session
            char[] sessionId = session.Create();

            /*
             * Initialize a session security context from the generated
             * session id
             */
            SessionSecurityContext sessionSecurityContext =
                new SessionSecurityContext(sessionId);

            // Update the stub configuration to use the session id
            stubConfig.SetSecurityContext(sessionSecurityContext);

            /*
             * Create a stub for the session service using the authenticated
             * session
             */
            this.sessionSvc =
                StubFactory.CreateStub<Session>(stubConfig);
            return stubConfig;
        }

        /// <summary>
        /// Creates a session with the server using username and password
        /// </summary>
        /// <param name="server">hostname of the server to login</param>
        /// <param name="username">username for login</param>
        /// <param name="password">password for login</param>
        /// <returns>the stub configuration configured with an authenticated
        ///          session
        /// </returns>
        public async Task<StubConfiguration> LoginByUsernameAndPasswordAsync(
            string server, string username, string password)
        {
            if (this.sessionSvc != null)
            {
                throw new Exception("Session already created");
            }

            StubFactory = CreateApiStubFactory(server);

            // Create a security context for username/password authentication
            ExecutionContext.SecurityContext securityContext =
                new UserPassSecurityContext(
                    username, password.ToCharArray());

            /*
             * Create a stub configuration with username/password security
             * context
             */
            StubConfiguration stubConfig = new StubConfiguration();
            stubConfig.SetSecurityContext(securityContext);

            // Create a session stub using the stub configuration.
            Session session =
                    StubFactory.CreateStub<Session>(stubConfig);

            // Login and create a session
            char[] sessionId = await session.CreateAsync();

            /*
             * Initialize a session security context from the generated
             * session id
             */
            SessionSecurityContext sessionSecurityContext =
                new SessionSecurityContext(sessionId);

            // Update the stub configuration to use the session id
            stubConfig.SetSecurityContext(sessionSecurityContext);

            /*
             * Create a stub for the session service using the authenticated
             * session
             */
            this.sessionSvc =
                StubFactory.CreateStub<Session>(stubConfig);
            return stubConfig;
        }

        /// <summary>
        /// Creates a session with the server using SAML Bearer Token
        /// </summary>
        /// <param name="server">hostname of the server to login</param>
        /// <param name="username">username for login</param>
        /// <param name="password">password for login</param>
        /// <returns>the stub configuration configured with an authenticated
        ///          session
        /// </returns>
        public StubConfiguration LoginBySamlBearerToken(string server,
            SamlToken samlBearerToken)
        {
            if (this.sessionSvc != null)
            {
                throw new Exception("Session already created");
            }

            StubFactory = CreateApiStubFactory(server);

            // Create a SAML security context using SAML bearer token
            ExecutionContext.SecurityContext samlSecurityContext = new SamlTokenSecurityContext(
                samlBearerToken, null);

            /*
             * Create a stub configuration with username/password security
             * context
             */
            StubConfiguration stubConfig = new StubConfiguration();
            stubConfig.SetSecurityContext(samlSecurityContext);

            // Create a session stub using the stub configuration.
            Session session =
                    StubFactory.CreateStub<Session>(stubConfig);

            // Login and create a session
            char[] sessionId = session.Create();

            /*
             * Initialize a session security context from the generated
             * session id
             */
            SessionSecurityContext sessionSecurityContext =
                new SessionSecurityContext(sessionId);

            // Update the stub configuration to use the session id
            stubConfig.SetSecurityContext(sessionSecurityContext);

            /*
             * Create a stub for the session service using the authenticated
             * session
             */
            this.sessionSvc =
                StubFactory.CreateStub<Session>(stubConfig);
            return stubConfig;
        }

        /// <summary>
        /// Logs out of the current session
        /// </summary>
        public void Logout()
        {
            if (this.sessionSvc != null)
            {
                this.sessionSvc.Delete();
            }
        }

        /// <summary>
        /// Connects to the server using https protocol and returns the factory
        ///  instance that can be used for creating client side stubs.
        /// </summary>
        /// <param name="server">hostname of the server</param>
        /// <returns>factory for the client side stubs</returns>
        private StubFactory CreateApiStubFactory(string server)
        {
            // Create a https connection with the vapi url
            ProtocolConnectionFactory pf = new ProtocolConnectionFactory();
            string apiUrl = "https://" + server + VAPI_PATH;

            IProtocolConnection connection = pf.GetConnection(Protocol.Http,
                apiUrl, new CspParameters());

            // Initialize the stub factory with the api provider
            IApiProvider provider = connection.GetApiProvider();
            StubFactory stubFactory = new StubFactory(provider);
            return stubFactory;
        }
    }
}
