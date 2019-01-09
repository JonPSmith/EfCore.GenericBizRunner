// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericBizRunner.Configuration;
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
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>();

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
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizOutDto>();

            //ATTEMPT
            var input = new BizDataOut { Output = "hello"};
            var data = utData.WrappedConfig.FromBizIMapper.Map<ServiceLayerBizOutDto>(input);

            //VERIFY
            data.Output.ShouldEqual("hello");
        }

        //---------------------------------------------------------------

        [Fact]
        public void TestDtoWithOverrideOfAutoMapperSetup()
        {
            //SETUP
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizOutWithMappingDto>();

            //ATTEMPT
            var input = new BizDataOut { Output = "Hello" };
            var data = utData.WrappedConfig.FromBizIMapper.Map<ServiceLayerBizOutWithMappingDto>(input);

            //VERIFY
            data.MappedOutput.ShouldEqual("Hello with suffix.");
        }

        //---------------------------------------------------------------------
        //errors

        [Fact]
        public void TestNotValidDto()
        {
            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => NonDiBizSetup.SetupDtoMapping<string>());

            //VERIFY
            ex.Message.ShouldEqual("The class String doesn't inherit from one of the Biz Runner Dto classes.");
        }


        [Fact]
        public void TestMissingBizInMapThrowsError()
        {
            //SETUP
            var utData = new NonDiBizSetup();

            //ATTEMPT
            var input = new ServiceLayerBizInDto {Num = 1 };
            var ex = Assert.Throws<AutoMapperMappingException>(() => utData.WrappedConfig.FromBizIMapper.Map<BizDataIn>(input));

            //VERIFY
            ex.Message.ShouldStartWith("Missing type map configuration or unsupported mapping.");
        }

        [Fact]
        public void TestMissingBizOutMapThrowsError()
        {
            //SETUP
            var utData = new NonDiBizSetup();

            //ATTEMPT
            var input = new BizDataOut { Output = "hello"};
            var ex = Assert.Throws<AutoMapperMappingException>(() => utData.WrappedConfig.FromBizIMapper.Map<ServiceLayerBizOutDto>(input));

            //VERIFY
            ex.Message.ShouldStartWith("Missing type map configuration or unsupported mapping.");
        }
    }
}