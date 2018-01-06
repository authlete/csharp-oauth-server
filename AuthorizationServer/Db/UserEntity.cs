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
using Authlete.Types;


namespace AuthorizationServer.Db
{
    /// <summary>
    /// Dummy user entity that represents a user record.
    /// </summary>
    public class UserEntity
    {
        /// <summary>
        /// The subject (= unique identifier) of the user.
        /// </summary>
        public string Subject { get; set; }


        /// <summary>
        /// Login ID.
        /// </summary>
        public string LoginId { get; set; }


        /// <summary>
        /// Login password.
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// Postal address.
        /// </summary>
        public Address Address { get; set; }


        /// <summary>
        /// Phone number.
        /// </summary>
        public string PhoneNumber { get; set; }


        /// <summary>
        /// Get the claim value of a claim.
        /// </summary>
        public object GetClaimValue(string claimName)
        {
            // See "OpenID Connect Core 1.0, 5. Claims".
            switch (claimName)
            {
                case StandardClaims.NAME:
                    // "name" claim. This claim can be requested by
                    // including "profile" in the "scope" parameter
                    // of an authorization request.
                    return Name;

                case StandardClaims.EMAIL:
                    // "email" claim. This claim can be requested
                    // by including "email" in the "scope"
                    // parameter of an authorization request.
                    return Email;

                case StandardClaims.ADDRESS:
                    // "address" claim. This claim can be requested
                    // by including "address" in the "scope"
                    // parameter of an authorization request.
                    return Address;

                case StandardClaims.PHONE_NUMBER:
                    // "phone_number" claim. This claim can be
                    // requested by including "phone" in the "scope"
                    // parameter of an authorization request.
                    return PhoneNumber;

                default:
                    // Unsupported claim.
                    return null;
            }
        }
    }
}
