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


using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Authlete.Api;


namespace AuthorizationServer.Controllers
{
    /// <summary>
    /// Base controller.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Constructor with an implementation of the
        /// <c>IAuthleteApi</c> interface. The given instance can
        /// be referred to as the value of the <c>API</c> property.
        /// </summary>
        public BaseController(IAuthleteApi api)
        {
            API = api;
        }


        /// <summary>
        /// The implementation of the <c>IAuthleteApi</c> interface
        /// that was given to the constructor.
        /// </summary>
        public IAuthleteApi API { get; }


        /// <summary>
        /// Read the request body as a string.
        /// </summary>
        public async Task<string> ReadRequestBodyAsString()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }


        /// <summary>
        /// Get the value of a request HTTP header.
        /// </summary>
        ///
        /// <returns>
        /// The value of the specified HTTP header in the request.
        /// </returns>
        ///
        /// <param name="headerName">
        /// The name of an HTTP header.
        /// </param>
        public string GetRequestHeaderValue(string headerName)
        {
            StringValues values = Request.Headers[headerName];

            // Return the value of the first entry.
            return values.GetEnumerator().Current;
        }
    }
}
