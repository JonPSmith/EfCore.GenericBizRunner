// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using GenericActions;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizInDtoAsync : GenericActionToBizDtoAsync<BizDataIn, ServiceLayerBizInDtoAsync>
    {
        public int Num { get; set; }

        public bool RaiseErrorInSetupSecondaryData { get; set; }

        public bool SetupSecondaryDataCalled { get; private set; }
        public bool CopyToBizDataCalled { get; private set; }

        protected internal override Task SetupSecondaryDataAsync(DbContext db, IBizActionStatus status)
        {
            SetupSecondaryDataCalled = true;
            if (RaiseErrorInSetupSecondaryData)
                status.AddError("Error in SetupSecondaryData");
            return base.SetupSecondaryDataAsync(db, status);
        }

        protected internal override async Task<BizDataIn> CopyToBizDataAsync(DbContext db, IMapper mapper, ServiceLayerBizInDtoAsync source)
        {
            CopyToBizDataCalled = true;
            return await base.CopyToBizDataAsync(db, mapper, source);
        }
    }
}