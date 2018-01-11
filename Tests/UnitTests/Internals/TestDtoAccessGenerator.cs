// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal;
using TestBizLayer.BizDTOs;
using Tests.DTOs;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Internals
{
    public class TestDtoAccessGenerator
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };

        [Fact]
        public void Test01DirectCopyOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, false, _noCachingConfig);
            var input = new BizDataIn { Num = 234 };

            //ATTEMPT
            var data = copier.DoCopyToBiz<BizDataIn>(null, null, input);        

            //VERIFY    
            data.Num.ShouldEqual(234);
        }

        [Fact]
        public void TestBizInCopyOk()
        {
            //SETUP
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ServiceLayerBizInDto());
            });
            var mapper = config.CreateMapper();
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, false, _noCachingConfig);
            var input = new ServiceLayerBizInDto { Num = 234 };

            //ATTEMPT
            var data = copier.DoCopyToBiz<BizDataIn>(null, mapper, input);

            //VERIFY    
            data.Num.ShouldEqual(234);
            input.CopyToBizDataCalled.ShouldBeTrue();
            input.SetupSecondaryDataCalled.ShouldBeFalse();
        }

        [Fact]
        public void TestBizOutCopyOk()
        {
            //SETUP 
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ServiceLayerBizOutDto());
            });
            var mapper = config.CreateMapper();
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDto), false, false, _noCachingConfig);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = copier.DoCopyFromBiz<ServiceLayerBizOutDto>(null, mapper, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
            data.CopyFromBizDataCalled.ShouldBeTrue();
            data.SetupSecondaryOutputDataCalled.ShouldBeTrue();
        }

        //-----------------------
        //BizOut

        [Fact]
        public void TestBizOutDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(BizDataOut), false, false, _noCachingConfig);
            var input = new BizDataOut { Output = "test copy"};

            //ATTEMPT
            var data = copier.DoCopyFromBiz<BizDataOut>(null, null, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
        }

        //-------------------------------------------------------------
        //CreateDataWithPossibleSetup

        [Fact]
        public void TestCreateDataWithPossibleSetupDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, false, _noCachingConfig);

            //ATTEMPT
            var data = copier.CreateDataWithPossibleSetup<BizDataIn>(null, null);

            //VERIFY    
            //Should not fail
        }

        [Fact]
        public void TestCreateDataWithPossibleSetupDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, false, _noCachingConfig);

            //ATTEMPT
            var data = copier.CreateDataWithPossibleSetup<ServiceLayerBizInDto>(null, null);

            //VERIFY    
            data.SetupSecondaryDataCalled.ShouldBeTrue();
            data.Num.ShouldEqual(0);
        }

        [Fact]
        public void TestCreateDataWithPossibleSetupDtoWithActionOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, false, _noCachingConfig);

            //ATTEMPT
            var data = copier.CreateDataWithPossibleSetup<ServiceLayerBizInDto>(null, x => { x.Num = 2;});

            //VERIFY    
            data.SetupSecondaryDataCalled.ShouldBeTrue();
            data.Num.ShouldEqual(2);
        }

        //-------------------------------------------------------------------
        //SetupSecondaryDataIfRequired

        [Fact]
        public void TestSetupSecondaryDataDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, false, _noCachingConfig);
            var input = new BizDataIn { Num = 234 };

            //ATTEMPT
            copier.SetupSecondaryDataIfRequired(null, input);

            //VERIFY    
            //Should not do anything, but musn't fail
        }

        [Fact]
        public void TestSetupSecondaryDataDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, false, _noCachingConfig);
            var input = new ServiceLayerBizInDto { Num = 234 };

            //ATTEMPT
            copier.SetupSecondaryDataIfRequired(null, input);

            //VERIFY    
            input.SetupSecondaryDataCalled.ShouldBeTrue();
        }
    }
}