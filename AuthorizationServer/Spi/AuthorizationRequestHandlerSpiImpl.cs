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
using AuthorizationServer.Db;
using AuthorizationServer.Util;


namespace AuthorizationServer.Spi
{
    public class AuthorizationRequestHandlerSpiImpl
        : AuthorizationRequestHandlerSpiAdapter
    {
        readonly UserTData _userTData;


        public AuthorizationRequestHandlerSpiImpl(Controller controller)
        {
            // Wrap TempData.
            _userTData = new UserTData(controller.TempData);
        }


        public override object GetUserClaimValue(
            string subject, string claimName, string languageTag)
        {
            // Get the UserEntity from TempData.
            UserEntity entity = _userTData.GetUserEntity();

            // If TempData does not hold a UserEntity.
            if (entity == null)
            {
                // Claim value is not available.
                return null;
            }

            // Get the value of the claim.
            return entity.GetClaimValue(claimName);
        }


        public override long GetUserAuthenticatedAt()
        {
            // Get the authentication time from TempData.
            return _userTData.GetUserAuthenticatedAt();
        }


        public override string GetUserSubject()
        {
            // Get the UserEntity from TempData.
            UserEntity entity = _userTData.GetUserEntity();

            // If TempData does not hold a UserEntity.
            if (entity == null)
            {
                // User information is not available.
                return null;
            }

            // Return the subject (= unique identifier) of the user.
            return entity.Subject;
        }
    }
}
