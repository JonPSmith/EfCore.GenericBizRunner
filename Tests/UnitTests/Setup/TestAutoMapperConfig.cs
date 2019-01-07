// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
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
        public void TestProfileOnDto()
        {
            //SETUP
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ServiceLayerBizInDto());
            });
            var mapper = config.CreateMapper();

            //ATTEMPT
            var input = new ServiceLayerBizInDto { Num = 234 };
            var data = mapper.Map<BizDataIn>(input);

            //VERIFY
            data.Num.ShouldEqual(234);
        }

        [Fact]
        public void TestViaAddAutoMapper()
        {
            //SETUP
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(null, new List<Assembly>{Assembly.GetAssembly(typeof(ServiceLayerBizInDto))});

            var mapper = ((MapperConfiguration) services[0].ImplementationInstance).CreateMapper();

            //ATTEMPT
            var input = new ServiceLayerBizInDto { Num = 234 };
            var data = mapper.Map<BizDataIn>(input);

            //VERIFY
            data.Num.ShouldEqual(234);
        }
    }
}