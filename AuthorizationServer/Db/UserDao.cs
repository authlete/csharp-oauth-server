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


using Authlete.Dto;


namespace AuthorizationServer.Db
{
    /// <summary>
    /// Dummy database operations for a user table.
    /// </summary>
    public class UserDao
    {
        // Dummy user database table.
        static readonly UserEntity[] USER_DB =
        {
            new UserEntity
            {
                Subject     = "1001",
                LoginId     = "john",
                Password    = "john",
                Name        = "John Smith",
                Email       = "john@example.com",
                Address     = new Address { Country = "USA"},
                PhoneNumber = "+1 (425) 555-1212"
            },
            new UserEntity
            {
                Subject     = "1002",
                LoginId     = "jane",
                Password    = "jane",
                Name        = "Jane Smith",
                Email       = "jane@example.com",
                Address     = new Address { Country = "Chile"},
                PhoneNumber = "+56 (2) 687 2400"
            }
        };


        /// <summary>
        /// Get a user entity by a pair of login ID ans password.
        /// </summary>
        public static UserEntity GetByCredentials(
            string loginId, string password)
        {
            // For each record in the dummy user database table.
            foreach (UserEntity entity in USER_DB)
            {
                // If the login credentials are valid.
                if (entity.LoginId.Equals(loginId) &&
                    entity.Password.Equals(password))
                {
                    // Found the user who has the login credentials.
                    return entity;
                }
            }

            // Not found any user who has the login credentials.
            return null;
        }
    }
}
