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
using AuthorizationServer.Spi;


namespace AuthorizationServer.Controllers
{
    /// <summary>
    /// An implementation of OAuth 2.0
    /// <a href="http://tools.ietf.org/html/rfc6749#section-3.2">token
    /// endpoint</a> with <a href="http://openid.net/connect/">OpenID
    /// Connect</a> support.
    /// </summary>
    [Route("api/[controller]")]
    public class TokenController : BaseController
    {
        public TokenController(IAuthleteApi api) : base(api)
        {
        }


        /// <summary>
        /// Token endpoint.
        /// </summary>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<HttpResponseMessage> Post()
        {
            // Request parameters.
            string parameters = await ReadRequestBodyAsString();

            // The value of the Authorization header.
            string auth = GetRequestHeaderValue("Authorization");

            // Handle the token request.
            return await new TokenRequestHandler(
                API, new TokenRequestHandlerSpiImpl())
                    .Handle(parameters, auth);
        }
    }
}
