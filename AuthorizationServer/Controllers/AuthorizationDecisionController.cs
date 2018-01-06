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
using Authlete.Util;
using AuthorizationServer.Db;
using AuthorizationServer.Spi;
using AuthorizationServer.Util;


namespace AuthorizationServer.Controllers
{
    /// <summary>
    /// The endpoint that receives a request from the form in the
    /// authorization page.
    /// </summary>
    [Route("api/authorization/decision")]
    public class AuthorizationDecisionController : BaseController
    {
        public AuthorizationDecisionController(IAuthleteApi api)
            : base(api)
        {
        }


        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<HttpResponseMessage> Post()
        {
            // Wrap TempData.
            var data = new UserTData(TempData);

            // Authenticate the user if necessary.
            AuthenticateUserIfNecessary(data);

            // Flag which indicates whether the user has given
            // authorization to the client application or not.
            bool authorized = IsClientAuthorized();

            // Parameters contained in the authorization request.
            string   ticket       = (string)data.Get("ticket");
            string[] claimNames   = data.GetObject<string[]>("claimNames");
            string[] claimLocales = data.GetObject<string[]>("claimLocales");

            // Process the authorization request according to the
            // decision made by the user.
            return await HandleDecision(
                authorized, ticket, claimNames, claimLocales);
        }


        void AuthenticateUserIfNecessary(UserTData data)
        {
            // If user information is already stored in TempData.
            if (data.HasUserEntity())
            {
                // Already logged in. No need to authenticate the
                // user here again.
                return;
            }

            // Values input to the form in the authorization page.
            string loginId  = Request.Form["loginId"];
            string password = Request.Form["password"];

            // Search the user database for the user.
            UserEntity entity =
                UserDao.GetByCredentials(loginId, password);

            // If the user was found.
            if (entity != null)
            {
                // The user was authenticated successfully.
                data.SetUserEntity(entity);
                data.SetUserAuthenticatedAt(
                    TimeUtility.CurrentTimeSeconds());
            }
        }


        bool IsClientAuthorized()
        {
            // If the user pressed "Authorize" button, the request
            // contains an "authorized" parameter.
            return Request.Form.ContainsKey("authorized");
        }


        async Task<HttpResponseMessage> HandleDecision(
            bool authorized, string ticket,
            string[] claimNames, string[] claimLocales)
        {
            var spi = new AuthorizationRequestDecisionHandlerSpiImpl(this, authorized);
            var handler = new AuthorizationRequestDecisionHandler(API, spi);

            return await handler.Handle(ticket, claimNames, claimLocales);
        }
    }
}
