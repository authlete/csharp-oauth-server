//
// Copyright (C) 2018 Authlete, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific
// language governing permissions and limitations under the
// License.
//


using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Authlete.Api;
using Authlete.Handler;


namespace AuthorizationServer.Controllers
{
    /// <summary>
    /// An implementation of an OpenID Provider configuration
    /// endpoint.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// An OpenID Provider that supports
    /// <a href="http://openid.net/specs/openid-connect-discovery-1_0.html">OpenID
    /// Connect Discovery 1.0</a> must provide an endpoint that
    /// returns its configuration information in JSON format.
    /// Details about the format are described in
    /// <a href="http://openid.net/specs/openid-connect-discovery-1_0.html#ProviderMetadata">3.
    /// OpenID Provider Metadata</a>.
    /// </para>
    ///
    /// <para>
    /// Note that the URI of an OpenID Provider configuration
    /// endpoint is defined in
    /// <a href="http://openid.net/specs/openid-connect-discovery-1_0.html#ProviderConfigurationRequest">4.1.
    /// OpenID Provider Configuration Request</a>. In short, the
    /// URI must be:
    /// </para>
    ///
    /// <code>
    /// Issuer Identifier + /.well-known/openid-configuration
    /// </code>
    ///
    /// <para>
    /// <i>Issuer Identifier</i> is a URL to identify an OpenID
    /// Provider. For example, <c>https://example.com</c>. For
    /// details about Issuer Identifier, see the description of
    /// <c>"issuer"</c> in
    /// <a href="http://openid.net/specs/openid-connect-discovery-1_0.html#ProviderMetadata">3.
    /// OpenID Provider Metadata</a> (OpenID Connect Discovery 1.0)
    /// and the description of <c>"iss"</c> in
    /// <a href="http://openid.net/specs/openid-connect-core-1_0.html#IDToken">2.
    /// ID Token</a> (OpenID Connect Core 1.0).
    /// </para>
    ///
    /// <para>
    /// You can change the Issuer Identifier of your service using
    /// the management console (Serivce Owner Console). Note that
    /// the default value of Issuer Identifier is not appropriate
    /// for production use, so you should change it.
    /// </para>
    /// </remarks>
    [Route(".well-known/openid-configuration")]
    public class ConfigurationController : BaseController
    {
        public ConfigurationController(IAuthleteApi api) : base(api)
        {
        }


        /// <summary>
        /// OpenID Provider configuration endpoint.
        /// </summary>
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            // Call Authlete's /api/service/configuration API.
            return await new ConfigurationRequestHandler(API).Handle();
        }
    }
}
