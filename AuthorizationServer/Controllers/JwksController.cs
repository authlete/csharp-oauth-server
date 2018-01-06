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
    /// An implementation of an endpoint to expose a JSON Web Key
    /// Set document
    /// (<a href="https://tools.ietf.org/html/rfc7517">RFC 7517</a>).
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// An OpenID Provider (OP) is required to expose its JSON Web
    /// Key Set document (JWK Set) so that client applications can
    /// (1) verify signatures by the OP and (2) encrypt their
    /// requests to the OP. The URI of a JWK Set document endpoint
    /// can be found as the value of the <c>"jwks_uri"</c> metadata
    /// defined in
    /// <a href="http://openid.net/specs/openid-connect-discovery-1_0.html#ProviderMetadata">OpenID
    /// Provider Metadata</a> if the OP supports
    /// <a href="http://openid.net/specs/openid-connect-discovery-1_0.html">OpenID
    /// Connect Discovery 1.0</a>.
    /// </para>
    /// </remarks>
    [Route("api/[controller]")]
    public class JwksController : BaseController
    {
        public JwksController(IAuthleteApi api) : base(api)
        {
        }


        /// <summary>
        /// JWK Set document endpoint.
        /// </summary>
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            // Call Authlete's /api/service/jwks/get API.
            return await new JwksRequestHandler(API).Handle();
        }
    }
}
