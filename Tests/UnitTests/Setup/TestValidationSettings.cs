// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Setup
{
    public class TestValidationSettings
    {
        readonly IMapper _mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void TestNormalValidateActionsBasedOnConfig(bool doNotValidate, bool hasErrors)
        {
            //SETUP  
            var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = doNotValidate };
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var bizInstance = new BizActionInOnlyWriteDb(context);
                var runner =
                    new ActionService<IBizActionInOnlyWriteDb>(context, bizInstance, _mapper, config);
                var input = new BizDataIn { Num = 1 };

                //ATTEMPT
                runner.RunBizAction(input);

                //VERIFY
                bizInstance.HasErrors.ShouldEqual(hasErrors);
                if (hasErrors)
                {
                    context.LogEntries.Any().ShouldBeFalse();
                }
                else
                {
                    context.LogEntries.Single().LogText.ShouldEqual("1");
                }
            }
        }

        [Fact]
        public void ForceValidateOff()
        {
            //SETUP  
            var config = new GenericBizRunnerConfig { TurnOffCaching = true};
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var bizInstance = new BizActionInOnlyWriteDbForceValidateOff(context);
                var runner =
                    new ActionService<IBizActionInOnlyWriteDbForceValidateOff>(context, bizInstance, _mapper, config);
                var input = new BizDataIn {Num = 1};

                //ATTEMPT
                runner.RunBizAction(input);

                //VERIFY
                runner.Status.HasErrors.ShouldBeFalse();
                context.LogEntries.Single().LogText.ShouldEqual("1");
            }
        }

        [Fact]
        public void ForceValidateOn()
        {
            //SETUP  
            var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = true};
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var bizInstance = new BizActionInOnlyWriteDbForceValidateOn(context);
                var runner =
                    new ActionService<IBizActionInOnlyWriteDbForceValidateOn>(context, bizInstance, _mapper, config);
                var input = new BizDataIn { Num = 1 };

                //ATTEMPT
                runner.RunBizAction(input);

                //VERIFY
                runner.Status.HasErrors.ShouldBeTrue();
                context.LogEntries.Any().ShouldBeFalse();
            }
        }
    }
}