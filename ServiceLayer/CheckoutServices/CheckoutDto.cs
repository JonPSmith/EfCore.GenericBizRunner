// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace ServiceLayer.CheckoutServices
{
    public class CheckoutDto
    {
        public CheckoutDto(string userId, List<CheckoutItemDto> lineItems)
        {
            UserId = userId;
            LineItems = lineItems.ToImmutableList();
        }

        public string UserId { get; }

        public ImmutableList<CheckoutItemDto> LineItems { get; }

    }
}