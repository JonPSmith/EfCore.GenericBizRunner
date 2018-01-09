#region licence
// =====================================================
// GenericActions Library - Library for running business actions
// Filename: Test20RunBizActionInOut.cs
// Date Created: 2015/01/30
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using GenericActions.Services;
using GenericActions.Services.Internal.Services;
using GenericServices;
using NUnit.Framework;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using Tests.DTOs;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test20RunBizActionInOut
    {
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            GenericServicesConfig.AddToSqlErrorDict(-1,"SaveChanges.Error");
        }

        [Fact]
        public void Test00CallInternalInOutServiceOk()
        {
            //SETUP 
            var service = new ActionServiceInOut<IBizActionInOut, BizDataIn, BizDataOut>(false);
            var input = new BizDataIn { Num = 123 };

            //ATTEMPT
            var status = service.RunBizActionDbAndInstance<BizDataOut>(null, new BizActionInOut(), input);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldNotEqualNull();
            status.Result.Output.ShouldEqual("123");
        }

        [Fact]
        public void Test01RunBizActionInOutDirectOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = new BizDataIn {Num = 123};

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>(input);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldNotEqualNull();
            status.HasWarnings.ShouldEqual(false);
            status.Result.Output.ShouldEqual("123");
        }

        [Fact]
        public void Test02RunBizActionInOutDirectBizErrorOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = new BizDataIn { Num = -1 };

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>(input);

            //VERIFY
            status.ShouldBeValid(false);
            status.HasWarnings.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("RunAction.Error");
        }

        [Fact]
        public void Test03RunBizActionInOutDirectWarningsOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = new BizDataIn { Num = 0 };

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>(input);

            //VERIFY
            status.ShouldBeValid();
            status.Warnings.Count.ShouldEqual(1);
            status.Warnings[0].ShouldEqual("Warning: RunAction.Warning");
        }

        [Fact]
        public void Test10RunBizActionInOutViaDtoOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = new ServiceLayerBizInDto {Num = 123};

            //ATTEMPT
            var status = service.RunBizAction<ServiceLayerBizOutDto>(input);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldNotEqualNull();
            status.Result.Output.ShouldEqual("123");
        }

        [Fact]
        public void Test11RunBizActionInOutViaDtoBizErrorOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = new ServiceLayerBizInDto { Num = -1 };

            //ATTEMPT
            var status = service.RunBizAction<ServiceLayerBizOutDto>(input);

            //VERIFY
            status.ShouldBeValid(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("RunAction.Error");
        }

        //---------------------------------------------------------------
        //test WriteDb

        [Fact]
        public void Test40InOutWriteDbOk()
        {
            //SETUP 
            var db = new DummyIDbContextWithValidation(false);
            var service = new ActionService<IBizActionInOutWriteDb>(db, new BizActionInOutWriteDb());
            var input = new BizDataIn { Num = 123 };

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>(input);

            //VERIFY
            status.ShouldBeValid();
            db.SaveChangesCalled.ShouldEqual(true);
        }

        [Fact]
        public void Test41InOutErrorStopsWriteDbOk()
        {
            //SETUP 
            var db = new DummyIDbContextWithValidation(false);
            var service = new ActionService<IBizActionInOutWriteDb>(db, new BizActionInOutWriteDb());
            var input = new BizDataIn { Num = -1 };

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>(input);

            //VERIFY
            status.ShouldBeValid(false);
            db.SaveChangesCalled.ShouldEqual(false);
        }

        [Fact]
        public void Test41InOutErrorWhenWriteDbOk()
        {
            //SETUP 
            var db = new DummyIDbContextWithValidation(true);
            var service = new ActionService<IBizActionInOutWriteDb>(db, new BizActionInOutWriteDb());
            var input = new BizDataIn { Num = 123 };

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>(input);

            //VERIFY
            status.ShouldBeValid(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("SaveChanges.Error");
        }


        //---------------------------------------------------------------
        //error checking

        [Fact]
        public void Test90InputIsBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( () => service.RunBizAction<BizDataOut>(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = String, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,String>");
        }

        [Fact]
        public void Test91OutputIsBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = new BizDataIn { Num = 123 };

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy from biz action. from type = BizDataOut, to type String. Expected a DTO of type GenericActionFromBizDto<BizDataOut,String>");
        }

        [Fact]
        public void Test92CallHasNoOutputBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionInOut needed 'In' but the Business class had a different setup of 'InOut'");
        }

        [Fact]
        public void Test93CallHasNoInputBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction<BizDataOut>());

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionInOut needed 'Out' but the Business class had a different setup of 'InOut'");
        }

        [Fact]
        public void Test94UseAsyncBizInWithSyncBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOut>(null, new BizActionInOut());
            var input = new ServiceLayerBizInDtoAsync();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction<BizDataOut>(input));

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }
    }
}