// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestDtoMappings
{
    public class TestBizActionNestedOutOnly
    {  
        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
        private readonly IGenericBizRunnerConfig _config= new GenericBizRunnerConfig { TurnOffCaching = true };

        [Fact]
        public void TestActionServiceNestedOutUsingGenericActionFromBizDtoOk()
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerNestedOutDto>(_config);
            utData.AddDtoMapping<ServiceLayerNestedOutChildDto>();
            var wrappedConfig = utData.WrappedConfig;
            var bizInstance = new BizActionNestedOutOnly();
            var runner = new ActionService<IBizActionNestedOutOnly>(_emptyDbContext, bizInstance, wrappedConfig);

            //ATTEMPT
            var data = runner.RunBizAction<ServiceLayerNestedOutDto>();

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Test");
            data.ChildData.ChildInt.ShouldEqual(123);
            data.ChildData.ChildString.ShouldEqual("Nested");
        }

        
    }
}