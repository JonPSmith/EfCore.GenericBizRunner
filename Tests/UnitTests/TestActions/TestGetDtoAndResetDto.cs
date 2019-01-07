// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestActions
{
    public class TestGetDtoAndResetDto
    {
        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
        private readonly IWrappedBizRunnerConfigAndMappings _wrappedConfig;

        public TestGetDtoAndResetDto()
        {
            var config = new GenericBizRunnerConfig { TurnOffCaching = true };
            var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(config);
            utData.AddBizOutDtoMapping<ServiceLayerBizOutDto>();
            _wrappedConfig = utData.WrappedConfig;
        }

        [Fact]
        public void TestResetDtoDirectOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);
            var data = new BizDataIn {Num = 123};

            //ATTEMPT
            service.ResetDto(data);

            //VERIFY
            data.ShouldNotBeNull();
            data.Num.ShouldEqual(123);
        }

        [Fact]
        public void TestResetDtoGenericActionsOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);
            var data = new ServiceLayerBizInDto {Num = 123};

            //ATTEMPT
            service.ResetDto(data);

            //VERIFY
            data.ShouldNotBeNull();
            data.Num.ShouldEqual(123);
            data.SetupSecondaryDataCalled.ShouldBeTrue();
        }

        [Fact]
        public void TestGetDtoDirectOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);

            //ATTEMPT
            var data = service.GetDto<BizDataIn>();

            //VERIFY
            data.ShouldNotBeNull();
            data.ShouldBeType<BizDataIn>();
        }

        [Fact]
        public void TestGetDtoGenericActionsDtoOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);

            //ATTEMPT
            var data = service.GetDto<ServiceLayerBizInDto>();

            //VERIFY
            data.ShouldNotBeNull();
            data.ShouldBeType<ServiceLayerBizInDto>();
            data.SetupSecondaryDataCalled.ShouldEqual(true);
        }

        [Fact]
        public void TestGetDtoGenericActionsWithParamDtoOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);

            //ATTEMPT
            var data = service.GetDto<ServiceLayerBizInDto>(x => { x.Num = 2;});

            //VERIFY
            data.Num.ShouldEqual(2);
            data.SetupSecondaryDataCalled.ShouldEqual(true);
        }

        //------------------------------------------------------------
        //Errors

        //GetOriginal

        [Fact]
        public void TestGetDtoGenericActionsDtoBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.GetDto<ServiceLayerBizOutDto>());

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = ServiceLayerBizOutDto, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,ServiceLayerBizOutDto>");
        }

        [Fact]
        public void TestGetDtoGenericActionsDtoOutOnlyBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionOutOnly>(_emptyDbContext, new BizActionOutOnly(), _wrappedConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.GetDto<ServiceLayerBizInDto>());

            //VERIFY
            ex.Message.ShouldEqual("The action with interface of IBizActionOutOnly does not have an input, but you called it with a method that needs an input.");
        }

        [Fact]
        public void TestGetDtoGenericActionsDtoAsyncBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.GetDto<ServiceLayerBizInDtoAsync>());

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }

        [Fact]
        public void TestResetDtoGenericActionsAsyncDtoBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);
            var data = new ServiceLayerBizInDtoAsync();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.ResetDto(data));

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }

        [Fact]
        public void TestResetDtoGenericActionsDtoBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionOutOnly>(_emptyDbContext, new BizActionOutOnly(), _wrappedConfig);
            var data = new ServiceLayerBizInDto();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.ResetDto(data));

            //VERIFY
            ex.Message.ShouldEqual("The action with interface of IBizActionOutOnly does not have an input, but you called it with a method that needs an input.");
        }

        //-----------------------
        //ResetDto

        [Fact]
        public void TestResetDtoGenericActionsDtoOutNotInBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _wrappedConfig);
            var data = new ServiceLayerBizOutDto();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.ResetDto(data));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = ServiceLayerBizOutDto, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,ServiceLayerBizOutDto>");
        }
    }
}