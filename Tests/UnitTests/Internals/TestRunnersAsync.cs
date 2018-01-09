// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal.Runners;
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

namespace Tests.UnitTests.Internals
{
    public class TestRunnersAsync
    {

        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };
        private readonly DbContext _dbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());

        //-------------------------------------------------------
        //Async, no mapping, no database access

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutValuesOkAsync(int num, bool hasErrors)
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto>(); //doesn't need a mapper, but mapper msutn't be null
            var bizInstance = new BizActionInOutAsync();
            var runner = new ActionServiceInOutAsync<IBizActionInOutAsync, BizDataIn, BizDataOut>(false, _noCachingConfig);
            var input = new BizDataIn { Num = num };

            //ATTEMPT
            var data = await runner.RunBizActionDbAndInstanceAsync<BizDataOut>(_dbContext, bizInstance, mapper, input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
                data.ShouldBeNull();
            else
                data.Output.ShouldEqual(num.ToString());
        }
    }
}