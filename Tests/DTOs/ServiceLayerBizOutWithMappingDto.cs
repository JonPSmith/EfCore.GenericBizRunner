// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizOutWithMappingDto : GenericActionFromBizDto<BizDataOut, ServiceLayerBizOutWithMappingDto>
    {
        public string Output { get; set; }

        public string MappedOutput { get; set; }


        public bool SetupSecondaryOutputDataCalled { get; private set; }
        public bool CopyFromBizDataCalled { get; private set; }

        protected override void AutoMapperSetup()
        {
            CreateMap<BizDataOut, ServiceLayerBizOutWithMappingDto>()
                .ForMember(p => p.MappedOutput, opt => opt.MapFrom(x => x.Output + " with suffix."));
        }

        protected internal override void SetupSecondaryOutputData(DbContext db)
        {
            SetupSecondaryOutputDataCalled = true;
            base.SetupSecondaryOutputData(db);
        }

        protected internal override ServiceLayerBizOutWithMappingDto CopyFromBizData(DbContext db, IMapper mapper,
            BizDataOut source)
        {
            CopyFromBizDataCalled = true;
            return base.CopyFromBizData(db, mapper, source);
        }
    }
}