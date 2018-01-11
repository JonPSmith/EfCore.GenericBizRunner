// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.ActionsAsync;
using TestBizLayer.ActionsAsync.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestActionsAsync
{
    public class TestGetDtoAndResetDtoAsync
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };

        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());

        //Beacause this is ValueInOut then there is no need for a mapper, but the ActionService checks that the Mapper isn't null
        private readonly IMapper _mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDtoAsync, ServiceLayerBizInDto>();

        [Fact]
        public async Task TestResetDtoDirectOk()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);
            var data = new BizDataIn {Num = 123};

            //ATTEMPT
            await service.ResetDtoAsync(data);

            //VERIFY
            data.ShouldNotBeNull();
            data.Num.ShouldEqual(123);
        }

        [Fact]
        public async Task TestResetDtoAsyncOk()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);
            var data = new ServiceLayerBizInDtoAsync { Num = 123 };

            //ATTEMPT
            await service.ResetDtoAsync(data);

            //VERIFY
            data.ShouldNotBeNull();
            data.Num.ShouldEqual(123);
            data.SetupSecondaryDataCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task Test11ResetDtoGenericActionsOk()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);
            var data = new ServiceLayerBizInDto {Num = 123};

            //ATTEMPT
            await service.ResetDtoAsync(data);

            //VERIFY
            data.ShouldNotBeNull();
            data.Num.ShouldEqual(123);
            data.SetupSecondaryDataCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task TestGetDtoDirectOk()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);

            //ATTEMPT
            var data = await service.GetDtoAsync<BizDataIn>();

            //VERIFY
            data.ShouldNotBeNull();
            data.ShouldBeType<BizDataIn>();
        }

        [Fact]
        public async Task TestGetDtoGenericActionsDtoAsyncOk()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);

            //ATTEMPT
            var data = await service.GetDtoAsync<ServiceLayerBizInDtoAsync>();

            //VERIFY
            data.ShouldNotBeNull();
            data.ShouldBeType<ServiceLayerBizInDtoAsync>();
            data.SetupSecondaryDataCalled.ShouldEqual(true);
            data.Num.ShouldEqual(0);
        }

        [Fact]
        public async Task TestGetDtoGenericActionsDtoAsyncOkAithAction()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);

            //ATTEMPT
            var data = await service.GetDtoAsync<ServiceLayerBizInDtoAsync>(x => { x.Num = 2;});

            //VERIFY
            data.ShouldNotBeNull();
            data.ShouldBeType<ServiceLayerBizInDtoAsync>();
            data.SetupSecondaryDataCalled.ShouldEqual(true);
            data.Num.ShouldEqual(2);
        }

        //------------------------------------------------------------
        //Errors

        //GetDto

        [Fact]
        public async Task TestGetDtoGenericActionsDtoBad()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetDtoAsync<ServiceLayerBizOutDto>());

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = ServiceLayerBizOutDto, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,ServiceLayerBizOutDto>");
        }

        [Fact]
        public async Task TestGetDtoGenericActionsDtoOk()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);

            //ATTEMPT
            var data = await service.GetDtoAsync<ServiceLayerBizInDto>();

            //VERIFY
            data.ShouldNotBeNull();
            data.ShouldBeType<ServiceLayerBizInDto>();
            data.SetupSecondaryDataCalled.ShouldEqual(true);
        }

        [Fact]
        public async Task TestGetDtoGenericActionsDtoOutOnlyBad()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionOutOnly>(_emptyDbContext, new BizActionOutOnly(), _mapper, _noCachingConfig);

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetDtoAsync<ServiceLayerBizInDto>());

            //VERIFY
            ex.Message.ShouldEqual("The action with interface of IBizActionOutOnly does not have an input, but you called it with a method that needs an input.");
        }

        [Fact]
        public async Task TestResetDtoGenericActionsDtoBad()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionOutOnly>(_emptyDbContext, new BizActionOutOnly(), _mapper, _noCachingConfig);
            var data = new ServiceLayerBizInDto();

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.ResetDtoAsync(data));

            //VERIFY
            ex.Message.ShouldEqual("The action with interface of IBizActionOutOnly does not have an input, but you called it with a method that needs an input.");
        }

        //-----------------------
        //ResetDto

        [Fact]
        public async Task TestResetDtoGenericActionsDtoOutNotInBad()
        {
            //SETUP 
            var service = new ActionServiceAsync<IBizActionInOutAsync>(_emptyDbContext, new BizActionInOutAsync(), _mapper, _noCachingConfig);
            var data = new ServiceLayerBizOutDto();

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.ResetDtoAsync(data));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = ServiceLayerBizOutDto, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,ServiceLayerBizOutDto>");
        }
    }
}