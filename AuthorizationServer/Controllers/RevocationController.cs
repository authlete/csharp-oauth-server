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
    /// An implementation of revocation endpoint
    /// (<a href="http://tools.ietf.org/html/rfc7009">RFC 7009</a>).
    /// </summary>
    [Route("api/[controller]")]
    public class RevocationController : BaseController
    {
        public RevocationController(IAuthleteApi api) : base(api)
        {
        }


        /// <summary>
        /// Revocation endpoint.
        /// </summary>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<HttpResponseMessage> Post()
        {
            // Request parameters.
            string parameters = await ReadRequestBodyAsString();

            // The value of the Authorization header.
            string auth = GetRequestHeaderValue("Authorization");

            // Call Authlete's /api/auth/revocation API.
            return await new RevocationRequestHandler(API)
                .Handle(parameters, auth);
        }
    }
}
