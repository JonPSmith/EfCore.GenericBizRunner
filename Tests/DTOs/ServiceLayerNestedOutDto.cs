// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerNestedOutDto : GenericActionFromBizDto<NestedBizDataOut, ServiceLayerNestedOutDto>
    {
        public string Output { get; set; }

        public ServiceLayerNestedOutChildDto ChildData { get; set; }
    }
}