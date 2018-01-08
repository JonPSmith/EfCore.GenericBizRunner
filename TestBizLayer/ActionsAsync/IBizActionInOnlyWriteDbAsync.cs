// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.ActionsAsync
{
    public interface IBizActionInOnlyWriteDbAsync : IGenericActionInOnlyWriteDbAsync<BizDataIn>
    {}
}