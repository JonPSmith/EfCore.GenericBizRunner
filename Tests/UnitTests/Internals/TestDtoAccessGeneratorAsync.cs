// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal;
using GenericBizRunner.Internal.DtoAccessors;
using TestBizLayer.BizDTOs;
using Tests.DTOs;
using Tests.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Internals
{
    public class TestDtoAccessGeneratorAsync
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };

        [Fact]
        public async Task TestBizInCopyAsyncAsyncDtoOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDtoAsync>();
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true, _noCachingConfig);
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };

            //ATTEMPT
            var data = await copier.DoCopyToBizAsync<BizDataIn>(null, mapper, input);

            //VERIFY    
            data.Num.ShouldEqual(234);
        }

        //--------------------------------------------------
        //BizIn

        [Fact]
        public async Task TestBizInCopyAsyncMethodDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true, _noCachingConfig);
            var input = new BizDataIn { Num = 234 };

            //ATTEMPT
            var data = await copier.DoCopyToBizAsync<BizDataIn>(null, null, input);

            //VERIFY    
            data.Num.ShouldEqual(234);
        }

        //NOTE: async business methods can use either sync or async dtos, so we need to check both

        [Fact]
        public async Task TestBizInCopyAsyncServiceInstanceOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDtoAsync>();
            var copier = new CopyToBizDataAsync<BizDataIn, ServiceLayerBizInDtoAsync>();
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };

            //ATTEMPT
            var data = await copier.CopyToBizAsync(null, mapper, input);

            //VERIFY    
            data.Num.ShouldEqual(234);
        }

        [Fact]
        public async Task TestBizInCopyAsyncSyncDtoOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto>();
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true, _noCachingConfig);
            var input = new ServiceLayerBizInDto { Num = 234 };

            //ATTEMPT
            var data = await copier.DoCopyToBizAsync<BizDataIn>(null, mapper, input);

            //VERIFY    
            data.Num.ShouldEqual(234);
        }


        [Fact]
        public async Task TestBizOutCopyAsyncAsyncDtoOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizOutDtoAsync>();
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDtoAsync), false, true, _noCachingConfig);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<ServiceLayerBizOutDtoAsync>(null, mapper, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
            data.CopyFromBizDataCalled.ShouldBeTrue();
            data.SetupSecondaryOutputDataCalled.ShouldBeTrue();
        }

        //-----------------------
        //BizOut

        [Fact]
        public async Task TestBizOutCopyAsyncDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(BizDataOut), false, true, _noCachingConfig);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<BizDataOut>(null, null, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
        }

        [Fact]
        public async Task TestBizOutCopyAsyncSyncDtoOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizOutDto>();
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDto), false, true, _noCachingConfig);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<ServiceLayerBizOutDto>(null, mapper, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
            data.CopyFromBizDataCalled.ShouldBeTrue();
            data.SetupSecondaryOutputDataCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task TestCreateDataWithPossibleSetupAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true, _noCachingConfig);

            //ATTEMPT
            var data = await copier.CreateDataWithPossibleSetupAsync<ServiceLayerBizInDtoAsync>(null);

            //VERIFY    
            data.SetupSecondaryDataCalled.ShouldBeTrue();
        }

        //-------------------------------------------------------------
        //CreateDataWithPossibleSetup

        [Fact]
        public async Task TestCreateDataWithPossibleSetupDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true, _noCachingConfig);

            //ATTEMPT
            await copier.CreateDataWithPossibleSetupAsync<BizDataIn>(null);

            //VERIFY    
            //Should work with no exceptions
        }

        [Fact]
        public async Task TestCreateDataWithPossibleSetupSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true, _noCachingConfig);

            //ATTEMPT
            var data = await copier.CreateDataWithPossibleSetupAsync<ServiceLayerBizInDto>(null);

            //VERIFY    
            data.SetupSecondaryDataCalled.ShouldBeTrue();
        }


        //-------------------------------------------------------------------
        //SetupSecondaryDataIfRequired

        [Fact]
        public async Task TestSetupSecondaryAsyncDataDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true, _noCachingConfig);
            var input = new BizDataIn { Num = 234 };

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, input);

            //VERIFY
            //Should work with no expections
        }

        [Fact]
        public async Task TestSetupSecondaryDataAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true, _noCachingConfig);
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, input);

            //VERIFY    
            input.SetupSecondaryDataCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task TestSetupSecondaryDataSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true, _noCachingConfig);
            var input = new ServiceLayerBizInDto { Num = 234 };

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, input);

            //VERIFY    
            input.SetupSecondaryDataCalled.ShouldBeTrue();
        }
    }
}