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
using Authlete.Web;


namespace AuthorizationServer.Controllers
{
    /// <summary>
    /// An implementation of introspection endpoint
    /// (<a href="http://tools.ietf.org/html/rfc7662">RFC 7662</a>).
    /// </summary>
    [Route("api/[controller]")]
    public class IntrospectionController : BaseController
    {
        public IntrospectionController(IAuthleteApi api) : base(api)
        {
        }


        /// <summary>
        /// Introspection endpoint.
        /// </summary>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<HttpResponseMessage> Post()
        {
            // "2.1. Introspection Request" in RFC 7662 says as
            // follows:
            //
            //   To prevent token scanning attacks, the endpoint
            //   MUST also require some form of authorization to
            //   access this endpoint, such as client
            //   authentication as described in OAuth 2.0 [RFC6749]
            //   or a separate OAuth 2.0 access token such as the
            //   bearer token described in OAuth 2.0 Bearer Token
            //   Usage [RFC6750].  The methods of managing and
            //   validating these authentication credentials are
            //   out of scope of this specification.
            //
            // So, this API must be protected in some way or other.
            // Let's perform authentication of the API caller.
            bool authenticated = AuthenticateApiCaller();

            // If the API caller does not have necessary
            // privileges to call this API.
            if (authenticated == false)
            {
                // Return "401 Unauthorized".
                return GenerateUnauthorizedError();
            }

            // Request parameters.
            string parameters = await ReadRequestBodyAsString();

            // Call Authlete's /api/auth/introspection/standard API.
            return await new IntrospectionRequestHandler(API)
                .Handle(parameters);
        }


        bool AuthenticateApiCaller()
        {
            // NOTE:
            // THIS IMPLEMENTATION IS DEMONSTRATION PURPOSES ONLY.

            // Get the value of the Authorization header.
            string auth = GetRequestHeaderValue("Authorization");

            // Try to parse it as "Basic Authentication".
            BasicCredentials credentials =
                BasicCredentials.Parse(auth);

            // If the Authorization header does not contain
            // "Basic Authentication" or the user ID is not valid.
            if (credentials == null || credentials.UserId == null)
            {
                // Authentication of the API caller failed.
                return false;
            }

            // If the user ID is "nobody".
            if ("nobody".Equals(credentials.UserId))
            {
                // Reject the introspection request by "nobody".
                return false;
            }

            // Accept anybody except "nobody" regardless of
            // whatever the value of credentials.Password is.
            return true;
        }


        HttpResponseMessage GenerateUnauthorizedError()
        {
            // NOTE:
            // THIS IMPLEMENTATION IS DEMONSTRATION PURPOSES ONLY.

            // Prepare a value of the WWW-Authenticate header.
            // Note that the implementation of AuthenticateApiCaller()
            // requires Basic Authentication.
            string challenge = "Basic realm=\"/api/introspection\"";

            // Return "401 Unauthorized".
            return ResponseUtility.Unauthorized(challenge);
        }
    }
}
