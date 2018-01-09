#region licence
// =====================================================
// GenericActions Library - Library for running business actions
// Filename: Test21RunBizActionIn.cs
// Date Created: 2015/01/30
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using GenericActions.Services;
using GenericActions.Services.Internal.Services;
using GenericLibsBase.Core;
using GenericServices;
using NUnit.Framework;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using Tests.DTOs;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test21RunBizActionIn
    {
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            GenericServicesConfig.AddToSqlErrorDict(-1, "SaveChanges.Error");
        }

        [Fact]
        public void Test01CallInternalInOnlyServiceOk()
        {
            //SETUP 
            var service = new ActionServiceInOnly<IBizActionInOnly, BizDataIn>(false);
            var input = new BizDataIn { Num = 123 };

            //ATTEMPT
            var status = service.RunBizActionDbAndInstance(null, new BizActionInOnly(), input);

            //VERIFY
            status.ShouldBeValid();
            status.IsA<SuccessOrErrors>();
        }

        [Fact]
        public void Test01RunBizActionInOnlyDirectOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOnly>(null, new BizActionInOnly());
            var input = new BizDataIn {Num = 123};

            //ATTEMPT
            var status = service.RunBizAction(input);

            //VERIFY
            status.ShouldBeValid();
            status.IsA<SuccessOrErrors>();
        }

        [Fact]
        public void Test02RunBizActionInOnlyDirectBizErrorOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOnly>(null, new BizActionInOnly());
            var input = new BizDataIn { Num = -1 };

            //ATTEMPT
            var status = service.RunBizAction(input);

            //VERIFY
            status.ShouldBeValid(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("RunAction.Error");
        }

        [Fact]
        public void Test03RunBizActionInOutDirectWarningsOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOnly>(null, new BizActionInOnly());
            var input = new BizDataIn { Num = 0 };

            //ATTEMPT
            var status = service.RunBizAction(input);

            //VERIFY
            status.ShouldBeValid();
            status.Warnings.Count.ShouldEqual(1);
            status.Warnings[0].ShouldEqual("Warning: RunAction.Warning");
        }

        [Fact]
        public void Test10RunBizActionInOnlyViaDtoOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOnly>(null, new BizActionInOnly());
            var input = new ServiceLayerBizInDto {Num = 123};

            //ATTEMPT
            var status = service.RunBizAction(input);

            //VERIFY
            status.ShouldBeValid();
            status.IsA<SuccessOrErrors>();
        }

        [Fact]
        public void Test11RunBizActionInOnlyViaDtoBizErrorOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOnly>(null, new BizActionInOnly());
            var input = new ServiceLayerBizInDto { Num = -1 };

            //ATTEMPT
            var status = service.RunBizAction(input);

            //VERIFY
            status.ShouldBeValid(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("RunAction.Error");
        }

        //---------------------------------------------------------------
        //test WriteDb

        [Fact]
        public void Test40InOnlyWriteDbOk()
        {
            //SETUP 
            var db = new DummyIDbContextWithValidation(false);
            var service = new ActionService<IBizActionInOnlyWriteDb>(db, new BizActionInOnlyWriteDb());
            var input = new BizDataIn { Num = 123 };

            //ATTEMPT
            var status = service.RunBizAction(input);

            //VERIFY
            status.ShouldBeValid();
            db.SaveChangesCalled.ShouldEqual(true);
        }

        [Fact]
        public void Test41InOnlyErrorStopsWriteDbOk()
        {
            //SETUP 
            var db = new DummyIDbContextWithValidation(false);
            var service = new ActionService<IBizActionInOnlyWriteDb>(db, new BizActionInOnlyWriteDb());
            var input = new BizDataIn { Num = -1 };

            //ATTEMPT
            var status = service.RunBizAction(input);

            //VERIFY
            status.ShouldBeValid(false);
            db.SaveChangesCalled.ShouldEqual(false);
        }

        [Fact]
        public void Test41InOnlyErrorWhenWriteDbOk()
        {
            //SETUP 
            var db = new DummyIDbContextWithValidation(true);
            var service = new ActionService<IBizActionInOnlyWriteDb>(db, new BizActionInOnlyWriteDb());
            var input = new BizDataIn { Num = 123 };

            //ATTEMPT
            var status = service.RunBizAction(input);

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
            var service = new ActionService<IBizActionInOnly>(null, new BizActionInOnly());
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = String, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,String>");
        }

        [Fact]
        public void Test91CallHasOutputBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionInOnly>(null, new BizActionInOnly());
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction<BizDataOut>(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionInOnly needed 'InOut' but the Business class had a different setup of 'In'");
        }

    }
}