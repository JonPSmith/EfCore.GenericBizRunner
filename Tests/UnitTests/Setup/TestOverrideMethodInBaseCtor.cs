// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
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
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ServiceLayerBizOutWithMappingDto());
            });
            var mapper = config.CreateMapper();

            //ATTEMPT
            var input = new BizDataOut { Output = "Hello" };
            var data = mapper.Map<ServiceLayerBizOutWithMappingDto>(input);

            //VERIFY
            data.MappedOutput.ShouldEqual("Hello with suffix.");
        }
    }
}