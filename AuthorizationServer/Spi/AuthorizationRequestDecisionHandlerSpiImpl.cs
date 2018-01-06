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


using Microsoft.AspNetCore.Mvc;
using Authlete.Handler.Spi;


namespace AuthorizationServer.Spi
{
    public class AuthorizationRequestDecisionHandlerSpiImpl
        : AuthorizationRequestHandlerSpiImpl,
          IAuthorizationRequestDecisionHandlerSpi
    {
        readonly bool _clientAuthorized;


        public AuthorizationRequestDecisionHandlerSpiImpl(
            Controller controller, bool clientAuthorized)
            : base(controller)
        {
            _clientAuthorized = clientAuthorized;
        }


        public bool IsClientAuthorized()
        {
            return _clientAuthorized;
        }
    }
}
