#region licence
// =====================================================
// GenericActions Library - Library for running business actions
// Filename: Test22RunBizActionOut.cs
// Date Created: 2015/01/30
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using GenericActions.Services;
using GenericServices;
using NUnit.Framework;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using Tests.DTOs;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test22RunBizActionOut
    {
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            GenericServicesConfig.AddToSqlErrorDict(-1, "SaveChanges.Error");
        }

        [Fact]
        public void Test01BasicRunBizActionOutOnlyDirectOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionOutOnly>(null, new BizActionOutOnly());

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>();

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldNotEqualNull();
            status.Result.Output.ShouldEqual("Out Only");
        }

        [Fact]
        public void Test10BasicRunBizActionOutOnlyViaDtoOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionOutOnly>(null, new BizActionOutOnly());

            //ATTEMPT
            var status = service.RunBizAction<ServiceLayerBizOutDto>();

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldNotEqualNull();
            status.Result.Output.ShouldEqual("Out Only");
        }

        //---------------------------------------------------------------
        //test WriteDb

        [Fact]
        public void Test40OutOnlyWriteDbOk()
        {
            //SETUP 
            var db = new DummyIDbContextWithValidation(false);
            var service = new ActionService<IBizActionOutOnlyWriteDb>(db, new BizActionOutOnlyWriteDb());;

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>();

            //VERIFY
            status.ShouldBeValid();
            db.SaveChangesCalled.ShouldEqual(true);
        }


        [Fact]
        public void Test41OutOnlyErrorWhenWriteDbOk()
        {
            //SETUP 
            var db = new DummyIDbContextWithValidation(true);
            var service = new ActionService<IBizActionOutOnlyWriteDb>(db, new BizActionOutOnlyWriteDb());;

            //ATTEMPT
            var status = service.RunBizAction<BizDataOut>();

            //VERIFY
            status.ShouldBeValid(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("SaveChanges.Error");
        }

        //---------------------------------------------------------------
        //error checking


        [Fact]
        public void Test91CallHasNoOutputBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionOutOnly>(null, new BizActionOutOnly());
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionOutOnly needed 'In' but the Business class had a different setup of 'Out'");
        }

    }
}