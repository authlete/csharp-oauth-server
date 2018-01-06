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
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Authlete.Api;
using Authlete.Dto;
using Authlete.Handler;
using Authlete.Types;
using Authlete.Util;
using Authlete.Web;
using AuthorizationServer.Models;
using AuthorizationServer.Spi;
using AuthorizationServer.Util;


namespace AuthorizationServer.Controllers
{
    /// <summary>
    /// An implementation of OAuth 2.0
    /// <a href="http://tools.ietf.org/html/rfc6749#section-3.1">authorization
    /// endpoint</a> with
    /// <a href="http://openid.net/connect/">OpenID Connect</a>
    /// support.
    /// </summary>
    [Route("api/[controller]")]
    public class AuthorizationController : BaseController
    {
        // View name of the authorization page. See "View discovery"
        // of "Views in ASP.NET Core MVC" for details.
        const string VIEW_NAME = "Page";


        readonly IViewEngine _viewEngine;


        public AuthorizationController(
            IAuthleteApi api, ICompositeViewEngine viewEngine)
            : base(api)
        {
            _viewEngine = viewEngine;
        }


        /// <summary>
        /// An authorization endpoint for HTTP GET method.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// 3.1. Authorization Endpoint (RFC 6749) says that the
        /// authorization endpoint MUST support GET method.
        /// </para>
        /// </remarks>
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            // Query parameters.
            string parameters = Request.QueryString.ToString();

            return await Process(parameters);
        }


        /// <summary>
        /// An authorization endpoint for HTTP POST method.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// 3.1. Authorization Endpoint (RFC 6749) says that the
        /// authorization endpoint MAY support POST method.
        /// </para>
        ///
        /// <para>
        /// In addition, 3.1.2.1. Authentication Request (OpenID
        /// Connect Core 1.0) says that the authorization endpoint
        /// MUST support POST method.
        /// </para>
        /// </remarks>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<HttpResponseMessage> Post()
        {
            // Form parameters.
            string parameters = await ReadRequestBodyAsString();

            return await Process(parameters);
        }


        async Task<HttpResponseMessage> Process(string parameters)
        {
            // Call Authlete's /api/auth/authorization API.
            AuthorizationResponse response =
                await CallAuthorizationApi(parameters);

            // 'action' in the response denotes the next action
            // which this authorization endpoint implementation
            // should take.
            AuthorizationAction action = response.Action;

            // Dispatch according to the action.
            switch (action)
            {
                case AuthorizationAction.INTERACTION:
                    // Process the authorization request with
                    // user interaction.
                    return await HandleInteraction(response);

                case AuthorizationAction.NO_INTERACTION:
                    // Process the authorization request without
                    // user interaction. The flow reaches here
                    // only when the authorization request contains
                    // 'prompt=none'.
                    return await HandleNoInteraction(response);

                default:
                    // Handle other error cases here.
                    return HandleError(response);
            }
        }


        async Task<AuthorizationResponse> CallAuthorizationApi(
            string parameters)
        {
            // Create a request for Authlete's
            // /api/auth/authorization API.
            var request = new AuthorizationRequest
            {
                Parameters = parameters
            };

            // Call Authlete's /api/auth/authorization API.
            return await API.Authorization(request);
        }


        async Task<HttpResponseMessage> HandleNoInteraction(
            AuthorizationResponse response)
        {
            // Make NoInteractionHandler handle the case of
            // 'prompt=none'. An implementation of the
            // INoInteractionHandlerSpi interface needs to be given
            // to the constructor of NoInteractionHandler.
            return await new NoInteractionHandler(
                API, new NoInteractionHandlerSpiImpl(this))
                    .Handle(response);
        }


        HttpResponseMessage HandleError(AuthorizationResponse response)
        {
            // Make AuthorizationRequestErrorHandler handle the
            // error case.
            return new AuthorizationRequestErrorHandler()
                .Handle(response);
        }


        async Task<HttpResponseMessage> HandleInteraction(
            AuthorizationResponse response)
        {
            // Store some variables into TempData so that they can
            // be referred to in AuthorizationDecisionController.
            var data = new UserTData(TempData);
            data.Set(      "ticket",       response.Ticket);
            data.SetObject("claimNames",   response.Claims);
            data.SetObject("claimLocales", response.ClaimsLocales);

            // Clear user information in TempData if necessary.
            ClearUserDataIfNecessary(response, data);

            // Prepare a model object which is needed to render
            // the authorization page.
            var model = new AuthorizationPageModel(
                response, data.GetUserEntity());

            // Render the authorization page manually.
            string html = await Render(VIEW_NAME, model);

            // Return "200 OK" with "text/html".
            return ResponseUtility.OkHtml(html);
        }


        void ClearUserDataIfNecessary(
            AuthorizationResponse response, UserTData data)
        {
            // Get the user information from TempData.
            var entity = data.GetUserEntity();

            // If user information does not exist in TempData.
            if (entity == null)
            {
                // Nothing to clear.
                return;
            }

            // If 'login' is required (= if the "prompt" parameter
            // of the authorization request includes "login").
            if (IsLoginRequired(response))
            {
                // Even if a user has already logged in, the user
                // needs to be re-authenticated. This simple
                // implementation forces the user to log out here.
                data.RemoveUserTData();
                return;
            }

            // If the max authentication age has been exceeded.
            if (IsMaxAgeExceeded(response, data))
            {
                // Much time has elapsed since the last login, so
                // re-authentication is needed. This simple
                // implementation forces the user to log out here.
                data.RemoveUserTData();
                return;
            }

            // No need to clear user data.
        }


        bool IsLoginRequired(AuthorizationResponse response)
        {
            // If the authorization request does not have
            // a "prompt" request parameter.
            if (response.Prompts == null)
            {
                // 'login' is not required.
                return false;
            }

            // For each value in the "prompt" request parameter.
            foreach (Prompt prompt in response.Prompts)
            {
                if (prompt == Prompt.LOGIN)
                {
                    // 'login' is required.
                    return true;
                }
            }

            // 'login' is not required.
            return false;
        }


        bool IsMaxAgeExceeded(
            AuthorizationResponse response, UserTData data)
        {
            if (response.MaxAge <= 0)
            {
                // Don't have to care about the maximum
                // authentication age.
                return false;
            }

            // Calculate the number of seconds that have elapsed
            // since the last login.
            long age =
                TimeUtility.CurrentTimeSeconds()
                           - data.GetUserAuthenticatedAt();

            if (age <= response.MaxAge)
            {
                // The max age is not exceeded yet.
                return false;
            }

            // The max age has been exceeded.
            return true;
        }


        async Task<string> Render(string viewName, object model)
        {
            // Render the view manually.
            return await new Renderer(this, _viewEngine)
                .Render(viewName, model);
        }
    }
}
