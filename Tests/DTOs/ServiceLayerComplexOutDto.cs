// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerComplexOutDto : GenericActionFromBizDto<ComplexBizDataOut, ServiceLayerComplexOutDto>
    {
        public string Output { get; set; }

        public ServiceLayerComplexOutChildDto ChildData { get; set; }
    }
}