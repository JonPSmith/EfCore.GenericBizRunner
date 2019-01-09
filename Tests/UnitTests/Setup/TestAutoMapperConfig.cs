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
            var utData = NonDiBizSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>();

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
            var utData = NonDiBizSetup.SetupBizOutDtoMapping<ServiceLayerBizOutDto>();

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
            var utData = NonDiBizSetup.SetupBizOutDtoMapping<ServiceLayerBizOutWithMappingDto>();

            //ATTEMPT
            var input = new BizDataOut { Output = "Hello" };
            var data = utData.WrappedConfig.FromBizIMapper.Map<ServiceLayerBizOutWithMappingDto>(input);

            //VERIFY
            data.MappedOutput.ShouldEqual("Hello with suffix.");
        }

        //---------------------------------------------------------------------
        //errors

        [Fact]
        public void TestDtoInWrongMapperSetup()
        {
            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => NonDiBizSetup.SetupBizOutDtoMapping<ServiceLayerBizInDto>());

            //VERIFY
            ex.Message.ShouldEqual("You registered the DTO ServiceLayerBizInDto, as a bizOutDto, but it doesn't inherit from GenericBizRunner.PublicButHidden.GenericActionFromBizDtoSetup.");
        }

        [Fact]
        public void TestDtoOutWrongMapperSetup()
        {
            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => NonDiBizSetup.SetupBizInDtoMapping<ServiceLayerBizOutDto>());

            //VERIFY
            ex.Message.ShouldEqual("You registered the DTO ServiceLayerBizOutDto, as a bizInDto, but it doesn't inherit from GenericBizRunner.PublicButHidden.GenericActionToBizDtoSetup.");
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