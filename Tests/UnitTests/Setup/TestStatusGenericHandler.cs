// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Setup
{
    public class TestStatusGenericHandler
    {
        [Fact]
        public void TestBaseStatus()
        {
            //SETUP

            //ATTEMPT
            var status = new StatusGenericHandler();

            //VERIFY
            status.HasErrors.ShouldBeFalse();
            status.Message.ShouldEqual("Success");
        }


        [Fact]
        public void TestStatusHasOneErrors()
        {
            //SETUP
            var status = new StatusGenericHandler();

            //ATTEMPT
            status.AddError("This is an error");

            //VERIFY
            status.HasErrors.ShouldBeTrue();
            status.Message.ShouldEqual("Failed with 1 error");
            status.GetAllErrors().ShouldEqual("This is an error");
        }

        [Fact]
        public void TestStatusHasTwoErrors()
        {
            //SETUP
            var status = new StatusGenericHandler();

            //ATTEMPT
            status.AddError("This is an error");
            status.AddError("This is another error");

            //VERIFY
            status.HasErrors.ShouldBeTrue();
            status.Message.ShouldEqual("Failed with 2 errors");
            status.GetAllErrors().ShouldEqual("This is an error\nThis is another error");
        }


        [Fact]
        public void TestCombineStatuses()
        {
            //SETUP
            var status1 = new StatusGenericHandler();
            var status2 = new StatusGenericHandler();

            //ATTEMPT
            status1.Message = "Status1";
            status2.CombineErrors(status1);

            //VERIFY
            status2.HasErrors.ShouldBeFalse();
            status2.Message.ShouldEqual("Status1");
        }

        [Fact]
        public void TestCombineStatusesWithErrors()
        {
            //SETUP
            var status1 = new StatusGenericHandler();
            var status2 = new StatusGenericHandler();

            //ATTEMPT
            status1.AddError("This is an error");
            status2.CombineErrors(status1);

            //VERIFY
            status2.HasErrors.ShouldBeTrue();
            status2.Message.ShouldEqual("Failed with 1 error");
            status2.GetAllErrors().ShouldEqual("This is an error");
        }

    }
}