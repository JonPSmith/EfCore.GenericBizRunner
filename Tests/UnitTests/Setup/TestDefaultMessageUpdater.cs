// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner;
using GenericBizRunner.Configuration;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Setup
{
    public class TestDefaultMessageUpdater
    {
        private class TestBizActionStatus : BizActionStatus { }

        [Fact]
        public void TestDefaultWriteMessage()
        {
            //SETUP
            var status = new TestBizActionStatus();

            //ATTEMPT
            DefaultMessageUpdater.UpdateSuccessMessageOnGoodWrite(status, new GenericBizRunnerConfig());

            //VERIFY
            status.HasErrors.ShouldBeFalse();
            status.Message.ShouldEqual("Successfully saved.");
        }

        [Fact]
        public void TestAddWriteMessage()
        {
            //SETUP
            var status = new TestBizActionStatus();
            status.Message = "My action";

            //ATTEMPT
            DefaultMessageUpdater.UpdateSuccessMessageOnGoodWrite(status, new GenericBizRunnerConfig());

            //VERIFY
            status.HasErrors.ShouldBeFalse();
            status.Message.ShouldEqual("My action saved.");
        }

        [Fact]
        public void TestAddWriteMessageNoAddedSaved()
        {
            //SETUP
            var status = new TestBizActionStatus();
            status.Message = "My action.";

            //ATTEMPT
            DefaultMessageUpdater.UpdateSuccessMessageOnGoodWrite(status, new GenericBizRunnerConfig());

            //VERIFY
            status.HasErrors.ShouldBeFalse();
            status.Message.ShouldEqual("My action.");
        }

    }
}