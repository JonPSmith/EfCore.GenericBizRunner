// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    [AutoMap(typeof(NestedBizDataOutChild))]
    public class AutoMapNestedOutChildDto : IBizOutAutoMap
    {
        public int ChildInt { get; set; }

        public string ChildString { get; set; }
    }
}