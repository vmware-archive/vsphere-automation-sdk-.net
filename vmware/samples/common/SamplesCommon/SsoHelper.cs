/**
 * *******************************************************
 * Copyright VMware, Inc. 2015, 2016.  All Rights Reserved.
 * SPDX-License-Identifier: MIT
 * *******************************************************
 *
 * DISCLAIMER. THIS PROGRAM IS PROVIDED TO YOU "AS IS" WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, WHETHER ORAL OR WRITTEN,
 * EXPRESS OR IMPLIED. THE AUTHOR SPECIFICALLY DISCLAIMS ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
 */
namespace vmware.samples.common
{
    using vmware.sso;
    using vmware.samples.common.authentication;
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;
    using vapi.util.security;

    public class SsoHelper
    {
        private static string dateFormat =
            "{0:yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'}";

        private static string strAssertionId = "ID";
        private static string strIssueInstant = "IssueInstant";
        private static string
            strSubjectConfirmationNode = "saml2:SubjectConfirmation";
        private static string
            strSubjectConfirmationMethodValueAttribute = "Method";
        private static string
            strSubjectConfirmationMethodValueTypeBearer =
            "urn:oasis:names:tc:SAML:2.0:cm:bearer";
        private static string strSubjectConfirmationMethodValueTypeHoK =
            "urn:oasis:names:tc:SAML:2.0:cm:holder-of-key";

        public static SamlToken GetSamlBearerToken(
            string ssoUrl, string ssoUserName, string ssoPassword)
        {
            var binding = VimAuthenticationHelper.GetCustomBinding();
            var address = new EndpointAddress(ssoUrl);

            var stsServiceClient =
                new STSService_PortTypeClient(binding, address);

            stsServiceClient.ClientCredentials.UserName.UserName = ssoUserName;
            stsServiceClient.ClientCredentials.UserName.Password = ssoPassword;

            RequestSecurityTokenType tokenType =
                new RequestSecurityTokenType();

            /**
            * For this request we need at least the following element in the
            * RequestSecurityTokenType set
            *
            * 1. Lifetime - represented by LifetimeType which specifies the
            * lifetime for the token to be issued
            *
            * 2. Tokentype - "urnoasisnamestcSAML20assertion", which is the
            * class that models the requested token
            *
            * 3. RequestType -
            * "httpdocsoasisopenorgwssxwstrust200512Issue", as we want
            * to get a token issued
            *
            * 4. KeyType -
            * "httpdocsoasisopenorgwssxwstrust200512Bearer",
            * representing the kind of key the token will have. There are two
            * options namely bearer and holder-of-key
            *
            * 5. SignatureAlgorithm -
            * "httpwwww3org200104xmldsigmorersasha256", representing the
            * algorithm used for generating signature
            *
            * 6. Renewing - represented by the RenewingType which specifies
            *  whether the token is renewable or not
            */
            tokenType.TokenType =
                TokenTypeEnum.urnoasisnamestcSAML20assertion;
            tokenType.RequestType =
                RequestTypeEnum.httpdocsoasisopenorgwssxwstrust200512Issue;
            tokenType.KeyType =
                KeyTypeEnum.httpdocsoasisopenorgwssxwstrust200512Bearer;
            tokenType.SignatureAlgorithm =
                SignatureAlgorithmEnum.httpwwww3org200104xmldsigmorersasha256;
            tokenType.Delegatable = true;
            tokenType.DelegatableSpecified = true;

            LifetimeType lifetime = new LifetimeType();
            AttributedDateTime created = new AttributedDateTime();
            String createdDate = String.Format(dateFormat,
                DateTime.Now.ToUniversalTime());
            created.Value = createdDate;
            lifetime.Created = created;

            AttributedDateTime expires = new AttributedDateTime();
            TimeSpan duration = new TimeSpan(1, 10, 10);
            String expireDate = String.Format(dateFormat,
                DateTime.Now.Add(duration).ToUniversalTime());
            expires.Value = expireDate;
            lifetime.Expires = expires;
            tokenType.Lifetime = lifetime;
            RenewingType renewing = new RenewingType();
            renewing.Allow = false;
            renewing.OK = true;
            tokenType.Renewing = renewing;

            RequestSecurityTokenResponseCollectionType responseToken =
                stsServiceClient.Issue(tokenType);
            RequestSecurityTokenResponseType rstResponse =
                responseToken.RequestSecurityTokenResponse;
            XmlElement samlTokenXml = rstResponse.RequestedSecurityToken;
            SamlToken samlToken = new SamlToken(samlTokenXml);
            return samlToken;
        }

        public static void PrintToken(XmlElement token)
        {
            if (token != null)
            {
                String assertionId =
                    token.Attributes.GetNamedItem(strAssertionId).Value;
                String issueInstanct =
                    token.Attributes.GetNamedItem(strIssueInstant).Value;
                String typeOfToken = "";
                XmlNode subjectConfirmationNode =
                    token.GetElementsByTagName(
                        strSubjectConfirmationNode).Item(0);
                String subjectConfirmationMethodValue =
                    subjectConfirmationNode.Attributes.GetNamedItem(
                        strSubjectConfirmationMethodValueAttribute).Value;
                if (subjectConfirmationMethodValue ==
                    strSubjectConfirmationMethodValueTypeHoK)
                {
                    typeOfToken = "Holder-Of-Key";
                }
                else if (subjectConfirmationMethodValue ==
                    strSubjectConfirmationMethodValueTypeBearer)
                {
                    typeOfToken = "Bearer";
                }
                Console.WriteLine("Token Details");
                Console.WriteLine("\tAssertionId =  " + assertionId);
                Console.WriteLine("\tToken Type =  " + typeOfToken);
                Console.WriteLine("\tIssued On =  " + issueInstanct);
            }
        }
    }
}
