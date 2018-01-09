#region licence
// =====================================================
// GenericActions Library - Library for running business actions
// Filename: Test21RunBizActionIn.cs
// Date Created: 2015/01/30
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GenericActions.Services;
using GenericLibsBase.Core;
using NUnit.Framework;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test24RunBizActionInGeneric
    {

        [Fact]
        public void Test01RunBizActionInOnlyDirectOk()
        {
            //SETUP 
            var service = new ActionService<IBizActionGenericInOnly>(null, new BizActionGenericInOnly());
            var input = new Collection<int> {1, 2, 3};

            //ATTEMPT
            var status = service.RunBizAction(input);

            //VERIFY
            status.ShouldBeValid();
            status.IsA<SuccessOrErrors>();
        }

        //---------------------------------------------------------------
        //error checking

        [Fact]
        public void Test90InputIsBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionGenericInOnly>(null, new BizActionGenericInOnly());
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = String, to type Collection`1. Expected a DTO of type GenericActionToBizDto<Collection`1,String>");
        }

        [Fact]
        public void Test91CallHasOutputBad()
        {
            //SETUP 
            var service = new ActionService<IBizActionGenericInOnly>(null, new BizActionGenericInOnly());
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => service.RunBizAction<BizDataOut>(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionGenericInOnly needed 'InOut' but the Business class had a different setup of 'In'");
        }

    }
}