// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using GenericActions;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizInDtoAsync : GenericActionToBizDtoAsync<BizDataIn, ServiceLayerBizInDtoAsync>
    {
        public int Num { get; set; }

        public int DtoControlNum { get; set; }

        public bool SetupSecondaryDataCalled { get; private set; }
        public bool CopyToBizDataCalled { get; private set; }

        protected internal override Task SetupSecondaryDataAsync(DbContext db)
        {
            SetupSecondaryDataCalled = true;
            return base.SetupSecondaryDataAsync(db);
        }

        protected internal override async Task<BizDataIn> CopyToBizDataAsync(DbContext db, IMapper mapper, ServiceLayerBizInDtoAsync source)
        {
            CopyToBizDataCalled = true;
            return await base.CopyToBizDataAsync(db, mapper, source);
        }
    }
}