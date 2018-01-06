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


using Authlete.Handler.Spi;
using AuthorizationServer.Db;


namespace AuthorizationServer.Spi
{
    public class TokenRequestHandlerSpiImpl
        : TokenRequestHandlerSpiAdapter
    {
        public override string AuthenticateUser(
            string username, string password)
        {
            // NOTE:
            // This method needs to be implemented only when you
            // want to support "Resource Owner Password Credentials"
            // flow (RFC 6749, 4.3).

            // Search the user database for the user.
            UserEntity entity =
                UserDao.GetByCredentials(username, password);

            // If not found.
            if (entity == null)
            {
                // There is no user who has the credentials.
                return null;
            }

            // Return the subject (= unique identifier) of the user.
            return entity.Subject;
        }
    }
}
