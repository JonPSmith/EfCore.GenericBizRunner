// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestActions
{
    public class TestGetDtoAndResetDto
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };

        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());

        //Beacause this is ValueInOut then there is no need for a mapper, but the ActionService checks that the Mapper isn't null
        private readonly IMapper _mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();

        [Fact]
        public void Test10ResetDtoDirectOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _mapper, _noCachingConfig);
            var data = new BizDataIn {Num = 123};

            //ATTEMPT
            service.ResetDto(data);

            //VERIFY
            data.ShouldNotBeNull();
            data.Num.ShouldEqual(123);
        }

        [Fact]
        public void Test11ResetDtoGenericActionsOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _mapper, _noCachingConfig);
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
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _mapper, _noCachingConfig);

            //ATTEMPT
            var data = service.GetDto<BizDataIn>();

            //VERIFY
            data.ShouldNotBeNull();
            data.ShouldBeType<BizDataIn>();
        }

        [Fact]
        public void TestGetDtoGenericActionsDtoAsyncBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _mapper, _noCachingConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.GetDto<ServiceLayerBizInDtoAsync>());

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }

        //------------------------------------------------------------
        //Errors

        //GetDto

        [Fact]
        public void TestGetDtoGenericActionsDtoBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _mapper, _noCachingConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.GetDto<ServiceLayerBizOutDto>());

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = ServiceLayerBizOutDto, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,ServiceLayerBizOutDto>");
        }

        [Fact]
        public void TestGetDtoGenericActionsDtoOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _mapper, _noCachingConfig);

            //ATTEMPT
            var data = service.GetDto<ServiceLayerBizInDto>();

            //VERIFY
            data.ShouldNotBeNull();
            data.ShouldBeType<ServiceLayerBizInDto>();
            data.SetupSecondaryDataCalled.ShouldEqual(true);
        }

        [Fact]
        public void TestGetDtoGenericActionsDtoOutOnlyBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionOutOnly>(_emptyDbContext, new BizActionOutOnly(), _mapper, _noCachingConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.GetDto<ServiceLayerBizInDto>());

            //VERIFY
            ex.Message.ShouldEqual("The action with interface of IBizActionOutOnly does not have an input, but you called it with a method that needs an input.");
        }

        [Fact]
        public void TestResetDtoGenericActionsAsyncDtoBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _mapper, _noCachingConfig);
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
            var service = new ActionService<IBizActionOutOnly>(_emptyDbContext, new BizActionOutOnly(), _mapper, _noCachingConfig);
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
            var service = new ActionService<IBizActionInOut>(_emptyDbContext, new BizActionInOut(), _mapper, _noCachingConfig);
            var data = new ServiceLayerBizOutDto();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.ResetDto(data));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = ServiceLayerBizOutDto, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,ServiceLayerBizOutDto>");
        }
    }
}