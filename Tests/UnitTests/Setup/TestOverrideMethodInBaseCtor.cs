// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner.Configuration;
using TestBizLayer.BizDTOs;
using Tests.DTOs;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Setup
{
    public class TestOverrideMethodInBaseCtor
    {
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