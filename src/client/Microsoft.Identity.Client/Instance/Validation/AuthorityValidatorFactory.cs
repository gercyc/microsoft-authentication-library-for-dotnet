﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Identity.Client.Internal;

namespace Microsoft.Identity.Client.Instance.Validation
{
    internal static class AuthorityValidatorFactory
    {
        public static IAuthorityValidator Create(AuthorityInfo authorityInfo, RequestContext requestContext)
        {
            switch (authorityInfo.AuthorityType)
            {
                case AuthorityType.Adfs:
                    return new AdfsAuthorityValidator(requestContext);
                case AuthorityType.Aad:
                    return new AadAuthorityValidator(requestContext);
                case AuthorityType.B2C:
                case AuthorityType.Dsts:
                    return new NullAuthorityValidator();
                case AuthorityType.PingId:
                    return new AadAuthorityValidator(requestContext);
                default:
                    throw new InvalidOperationException("Invalid AuthorityType");
            }
        }
    }
}
