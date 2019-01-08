// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using GenericBizRunner.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestBizLayer.BizDTOs;
using Tests.DTOs;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Setup
{
    public class TestAutoMapperConfig
    {
        [Fact]
        public void TestBizInMappingDto()
        {
            //SETUP
            var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>();

            //ATTEMPT
            var input = new ServiceLayerBizInDto { Num = 234 };
            var data = utData.WrappedConfig.ToBizIMapper.Map<BizDataIn>(input);

            //VERIFY
            data.Num.ShouldEqual(234);
        }


        [Fact]
        public void TestBizOutMappingDto()
        {
            //SETUP
            var utData = NonDiSetup.SetupBizOutDtoMapping<ServiceLayerBizOutDto>();

            //ATTEMPT
            var input = new BizDataOut { Output = "hello"};
            var data = utData.WrappedConfig.ToBizIMapper.Map<ServiceLayerBizOutDto>(input);

            //VERIFY
            data.Output.ShouldEqual("hello");
        }

        //---------------------------------------------------------------

        [Fact]
        public void TestDtoWithOverrideOfAutoMapperSetup()
        {
            //SETUP
            var utData = NonDiSetup.SetupBizOutDtoMapping<ServiceLayerBizOutWithMappingDto>();

            //ATTEMPT
            var input = new BizDataOut { Output = "Hello" };
            var data = utData.WrappedConfig.FromBizIMapper.Map<ServiceLayerBizOutWithMappingDto>(input);

            //VERIFY
            data.MappedOutput.ShouldEqual("Hello with suffix.");
        }
    }
}