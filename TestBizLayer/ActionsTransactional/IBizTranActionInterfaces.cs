// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.ActionsTransactional
{
    public interface IBizTranActionFirst : IGenericAction<BizDataGuid, BizDataGuid>
    {}

    public interface IBizTranActionFirstNoIn : IGenericActionOutOnly<BizDataGuid>
    {}

    public interface IBizTranActionSecond : IGenericAction<BizDataGuid, BizDataGuid>
    {}

    public interface IBizTranActionSecondAsync : IGenericActionAsync<BizDataGuid, BizDataGuid>
    {}

    public interface IBizTranActionSecondWriteDbAsync : IGenericActionWriteDbAsync<BizDataGuid, BizDataGuid>
    {}

    public interface IBizTranActionFinal : IGenericAction<BizDataGuid, BizDataGuid>
    {}

    public interface IBizTranActionFinalAsync : IGenericActionAsync<BizDataGuid, BizDataGuid>
    {}

    public interface IBizTranActionFirstWriteDb : IGenericActionWriteDb<BizDataGuid, BizDataGuid>
    {}

}