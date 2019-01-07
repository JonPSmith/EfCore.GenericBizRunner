// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal;
using GenericBizRunner.Internal.Runners;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.ActionsAsync;
using TestBizLayer.BizDTOs;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Internals
{
    public class TestBizDecode
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig {TurnOffCaching = true};

        [Theory]
        [InlineData(typeof(IBizActionInOut), "Out")]
        [InlineData(typeof(IBizActionInOut), "In")]
        [InlineData(typeof(IBizActionOutOnly), "In")]
        [InlineData(typeof(IBizActionInOnly), "Out")]
        [InlineData(typeof(IBizActionInOut), "InOut,Async")]
        public void TestDecodeBizActionBad(Type bizInterfaceType, string requestedInOutString)
        {
            //SETUP 
            var requestedInOut = (RequestedInOut) Enum.Parse(typeof (RequestedInOut), requestedInOutString); 

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( () => new BizDecoder(bizInterfaceType, requestedInOut, true));

            //VERIFY
        }

        [Theory]
        [InlineData(typeof(IBizActionInOutAsync), "Out,Async")]
        [InlineData(typeof(IBizActionInOutAsync), "In,Async")]
        [InlineData(typeof(IBizActionOutOnlyAsync), "In,Async")]
        [InlineData(typeof(IBizActionInOnlyAsync), "Out,Async")]
        [InlineData(typeof(IBizActionInOnlyAsync), "In")]
        public void TestDecodeBizActionAsyncBad(Type bizInterfaceType, string requestedInOutString)
        {
            //SETUP 
            var requestedInOut = (RequestedInOut)Enum.Parse(typeof(RequestedInOut), requestedInOutString);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => new BizDecoder(bizInterfaceType, requestedInOut, true));

            //VERIFY
        }

        //---------------------------------------------------------------------
        //Check Interface lookup

        [Fact]
        public void TestCheckAllInterfacesAreInLookup()
        {
            //SETUP 
            var genericActionInterfaces =
                Assembly.GetAssembly(typeof (IGenericAction<,>))
                    .GetTypes()
                    .Where(x => x.IsInterface && x.Name.StartsWith("IGenericAction") && x != typeof(DbContext))
                    .ToList();

            //ATTEMPT

            //VERIFY
            genericActionInterfaces.Count.ShouldEqual(ServiceBuilderLookup.ServiceLookup.Count);
            foreach (var foundInterface in genericActionInterfaces)
            {
                ServiceBuilderLookup.ServiceLookup.ContainsKey(foundInterface.GetGenericTypeDefinition()).ShouldEqual(true);
            }
        }

        [Fact]
        public void TestDecodeBizAction()
        {
            //SETUP 
            
            //ATTEMPT
            var decoder = new BizDecoder(typeof (IBizActionInOut), RequestedInOut.InOut, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceInOut<,,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(3);
            args[0].ShouldEqual(typeof (IBizActionInOut));   
            args[1].ShouldEqual(typeof (BizDataIn));
            args[2].ShouldEqual(typeof (BizDataOut));
            decoder.BizInfo.IsAsync.ShouldEqual(false);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(false);
        }

        [Fact]
        public void TestDecodeBizActionAsync()
        {
            //SETUP 

            //ATTEMPT
            var decoder = new BizDecoder(typeof(IBizActionInOutAsync), RequestedInOut.InOut | RequestedInOut.Async, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceInOutAsync<,,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(3);
            args[0].ShouldEqual(typeof(IBizActionInOutAsync));
            args[1].ShouldEqual(typeof (BizDataIn));
            args[2].ShouldEqual(typeof (BizDataOut));
            decoder.BizInfo.IsAsync.ShouldEqual(true);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(false);
        }

        [Fact]
        public void TestDecodeBizActionInOnly()
        {
            //SETUP 

            //ATTEMPT
            var decoder = new BizDecoder(typeof(IBizActionInOnly), RequestedInOut.In, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceInOnly<,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(2);
            args[0].ShouldEqual(typeof(IBizActionInOnly));
            args[1].ShouldEqual(typeof (BizDataIn));
            decoder.BizInfo.IsAsync.ShouldEqual(false);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(false);
        }

        [Fact]
        public void TestDecodeBizActionInOnlyAsync()
        {
            //SETUP 

            //ATTEMPT
            var decoder = new BizDecoder(typeof(IBizActionInOnlyAsync), RequestedInOut.In | RequestedInOut.Async, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceInOnlyAsync<,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(2);
            args[0].ShouldEqual(typeof(IBizActionInOnlyAsync));
            args[1].ShouldEqual(typeof (BizDataIn));
            decoder.BizInfo.IsAsync.ShouldEqual(true);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(false);
        }

        [Fact]
        public void TestDecodeBizActionOutOnly()
        {
            //SETUP 

            //ATTEMPT
            var decoder = new BizDecoder(typeof(IBizActionOutOnly), RequestedInOut.Out, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceOutOnly<,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(2);
            args[0].ShouldEqual(typeof(IBizActionOutOnly));
            args[1].ShouldEqual(typeof (BizDataOut));
            decoder.BizInfo.IsAsync.ShouldEqual(false);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(false);
        }

        [Fact]
        public void TestDecodeBizActionOutOnlyAsync()
        {
            //SETUP 

            //ATTEMPT
            var decoder = new BizDecoder(typeof(IBizActionOutOnlyAsync), RequestedInOut.Out | RequestedInOut.Async, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceOutOnlyAsync<,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(2);
            args[0].ShouldEqual(typeof(IBizActionOutOnlyAsync));
            args[1].ShouldEqual(typeof (BizDataOut));
            decoder.BizInfo.IsAsync.ShouldEqual(true);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(false);
        }

        [Fact]
        public void TestDecodeBizActionValueInOutOk()
        {
            //SETUP 

            //ATTEMPT
            var decoder = new BizDecoder(typeof(IBizActionValueInOut), RequestedInOut.InOut, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceInOut<,,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(3);
            args[0].ShouldEqual(typeof(IBizActionValueInOut));
            args[1].ShouldEqual(typeof(int));
            args[2].ShouldEqual(typeof(string));
            decoder.BizInfo.IsAsync.ShouldEqual(false);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(false);
        }

        //-----------------------------------------------------------
        //writeDb

        [Fact]
        public void TestDecodeBizActionWriteDb()
        {
            //SETUP 

            //ATTEMPT
            var decoder = new BizDecoder(typeof(IBizActionInOutWriteDb), RequestedInOut.InOut, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceInOut<,,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(3);
            args[0].ShouldEqual(typeof(IBizActionInOutWriteDb));
            args[1].ShouldEqual(typeof (BizDataIn));
            args[2].ShouldEqual(typeof (BizDataOut));
            decoder.BizInfo.IsAsync.ShouldEqual(false);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(true);
        }

        [Fact]
        public void TestDecodeBizActionWriteDbAsync()
        {
            //SETUP 

            //ATTEMPT
            var decoder = new BizDecoder(typeof(IBizActionInOutWriteDbAsync), RequestedInOut.InOut | RequestedInOut.Async, true);

            //VERIFY
            ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericTypeDefinition().ShouldEqual(typeof(ActionServiceInOutAsync<,,>));
            var args = ((Type)decoder.BizInfo.GetServiceInstance(_noCachingConfig).GetType()).GetGenericArguments();
            args.Length.ShouldEqual(3);
            args[0].ShouldEqual(typeof(IBizActionInOutWriteDbAsync));
            args[1].ShouldEqual(typeof (BizDataIn));
            args[2].ShouldEqual(typeof (BizDataOut));
            decoder.BizInfo.IsAsync.ShouldEqual(true);
            decoder.BizInfo.RequiresSaveChanges.ShouldEqual(true);
        }

        //-------------------------------------------------------------
        //GetBizIn/Out Types

        [Fact]
        public void TestGetBizInType()
        {
            //SETUP 
            var decoder = new BizDecoder(typeof(IBizActionInOut), RequestedInOut.InOut, true);

            //ATTEMPT
            var inType = decoder.BizInfo.GetBizInType();

            //VERIFY
            inType.ShouldEqual(typeof(BizDataIn));
        }

        [Fact]
        public void TestGetBizOutType()
        {
            //SETUP 
            var decoder = new BizDecoder(typeof(IBizActionInOut), RequestedInOut.InOut, true);

            //ATTEMPT
            var outType = decoder.BizInfo.GetBizOutType();

            //VERIFY
            outType.ShouldEqual(typeof(BizDataOut));
        }

        [Fact]
        public void TestGetRunMethodNoOutputTypeAsyncOk()
        {
            //SETUP 
            var decoder = new BizDecoder(typeof(IBizActionInOnlyAsync), RequestedInOut.In | RequestedInOut.Async, true);

            //ATTEMPT
            var runMethod = decoder.BizInfo.GetRunMethod();

            //VERIFY
            runMethod.Name.ShouldEqual("RunBizActionDbAndInstanceAsync");
            runMethod.ReturnType.ShouldEqual(typeof(Task));
            runMethod.GetParameters().Select(x => x.ParameterType).ShouldEqual(
                new[] { typeof(DbContext), typeof(IBizActionInOnlyAsync), typeof(IMapper), typeof(object)});
        }

        [Fact]
        public void TestGetRunMethodNoOutputTypeOk()
        {
            //SETUP 
            var decoder = new BizDecoder(typeof(IBizActionInOnly), RequestedInOut.In, true);

            //ATTEMPT
            var runMethod = decoder.BizInfo.GetRunMethod();

            //VERIFY
            runMethod.Name.ShouldEqual("RunBizActionDbAndInstance");
            runMethod.ReturnType.ShouldEqual(typeof(void));
            runMethod.GetParameters().Select(x => x.ParameterType).ShouldEqual(
                new[] { typeof(DbContext), typeof(IBizActionInOnly), typeof(IMapper), typeof(object) });
        }

        [Fact]
        public void TestGetRunMethodWithOutputTypeAsyncOk()
        {
            //SETUP 
            var decoder = new BizDecoder(typeof(IBizActionInOutAsync), RequestedInOut.InOut | RequestedInOut.Async, true);

            //ATTEMPT
            var runMethod = decoder.BizInfo.GetRunMethod();

            //VERIFY
            runMethod.Name.ShouldEqual("RunBizActionDbAndInstanceAsync");
            runMethod.ReturnType.ShouldEqual(typeof(Task<BizDataOut>));
            runMethod.GetParameters().Select(x => x.ParameterType).ShouldEqual(
                new[] { typeof(DbContext), typeof(IBizActionInOutAsync), typeof(IMapper), typeof(object) });
        }

        //---------------------------------------------------------------------
        //GetRunMethod

        [Fact]
        public void TestGetRunMethodWithOutputTypeOk()
        {
            //SETUP 
            var decoder = new BizDecoder(typeof(IBizActionInOut), RequestedInOut.InOut, true);

            //ATTEMPT
            var runMethod = decoder.BizInfo.GetRunMethod();

            //VERIFY
            runMethod.Name.ShouldEqual("RunBizActionDbAndInstance");
            runMethod.ReturnType.ShouldEqual(typeof(BizDataOut));
            runMethod.GetParameters().Select(x => x.ParameterType).ShouldEqual(
                new[] {typeof (DbContext), typeof (IBizActionInOut), typeof(IMapper), typeof (object)});
        }
    }
}