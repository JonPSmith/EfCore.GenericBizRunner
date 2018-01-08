// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerComplexOutChildDto : GenericActionFromBizDto<ComplexBizDataOutChild, ServiceLayerComplexOutChildDto>
    {
        public int ChildInt { get; set; }

        public string ChildString { get; set; }
    }
}