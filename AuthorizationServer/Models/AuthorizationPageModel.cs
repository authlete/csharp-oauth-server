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


using System;
using Authlete.Dto;
using AuthorizationServer.Db;


namespace AuthorizationServer.Models
{
    /// <summary>
    /// Model for the authorization page.
    /// See <c>Controllers/AuthorizationController.cs</c> and
    /// <c>Views/Authorization/Page.cshtml</c>.
    /// </summary>
    public class AuthorizationPageModel
    {
        public AuthorizationPageModel(
            AuthorizationResponse response, UserEntity user)
        {
            Client client = response.Client;

            ServiceName     = response.Service.ServiceName;
            ClientName      = client.ClientName;
            Description     = client.Description;
            LogoUri         = UriToString(client.LogoUri);
            ClientUri       = UriToString(client.ClientUri);
            PolicyUri       = UriToString(client.PolicyUri);
            TosUri          = UriToString(client.TosUri);
            Scopes          = response.Scopes;
            LoginId         = ComputeLoginId(response);
            LoginIdReadOnly = ComputeLoginIdReadOnly(response);
            User            = user;
        }


        /// <summary>
        /// The name of the service.
        /// </summary>
        public string ServiceName { get; }


        /// <summary>
        /// The name of the client application.
        /// </summary>
        public string ClientName { get; }


        /// <summary>
        /// The description of the client application.
        /// </summary>
        public string Description { get; }


        /// <summary>
        /// The URL of the logo image of the client application.
        /// </summary>
        public string LogoUri { get; }


        /// <summary>
        /// The URL of the homepage of the client application.
        /// </summary>
        public string ClientUri { get; }


        /// <summary>
        /// The URL of the policy page of the client application.
        /// </summary>
        public string PolicyUri { get; }


        /// <summary>
        /// The URL of "Terms of Service" page of the client
        /// application.
        /// </summary>
        public string TosUri { get; }


        /// <summary>
        /// Scopes requested by the authorization request.
        /// </summary>
        public Scope[] Scopes { get; }


        /// <summary>
        /// The login ID that should be used as the initial value
        /// for the login ID field in the authorization page.
        /// </summary>
        public string LoginId { get; }


        /// <summary>
        /// This property holds a string <c>"readonly"</c> whne the
        /// initial value of the login ID should not be changed.
        /// </summary>
        public string LoginIdReadOnly { get; }


        /// <summary>
        /// The current user. This is <c>null</c> if no user has
        /// logged in.
        /// </summary>
        public UserEntity User { get; }


        /// <summary>
        /// Get the string representation of the given URI.
        /// </summary>
        static string UriToString(Uri uri)
        {
            return (uri == null) ? null : uri.ToString();
        }


        /// <summary>
        /// Compute the initial value for the login ID field in the
        /// authorization page.
        /// </summary>
        static string ComputeLoginId(AuthorizationResponse response)
        {
            if (response.Subject != null)
            {
                return response.Subject;
            }

            return response.LoginHint;
        }


        /// <summary>
        /// Return <c>"readonly"</c> if the authorization request
        /// requires that a specific subject be used.
        /// </summary>
        static string ComputeLoginIdReadOnly(AuthorizationResponse response)
        {
            if (response.Subject != null)
            {
                return "readonly";
            }
            else
            {
                return null;
            }
        }
    }
}
