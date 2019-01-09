// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
        [Fact]
        public async Task TestBizInCopyAsyncAsyncDtoOk()
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDtoAsync>();
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true, true);
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };

            //ATTEMPT
            var data = await copier.DoCopyToBizAsync<BizDataIn>(null, utData.WrappedConfig.ToBizIMapper, input);

            //VERIFY    
            data.Num.ShouldEqual(234);
        }

        //--------------------------------------------------
        //BizIn

        [Fact]
        public async Task TestBizInCopyAsyncMethodDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true, true);
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
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDtoAsync>();
            var copier = new CopyToBizDataAsync<BizDataIn, ServiceLayerBizInDtoAsync>();
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };

            //ATTEMPT
            var data = await copier.CopyToBizAsync(null, utData.WrappedConfig.ToBizIMapper, input);

            //VERIFY    
            data.Num.ShouldEqual(234);
        }

        [Fact]
        public async Task TestBizInCopyAsyncSyncDtoOk()
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>();
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true, true);
            var input = new ServiceLayerBizInDto { Num = 234 };

            //ATTEMPT
            var data = await copier.DoCopyToBizAsync<BizDataIn>(null, utData.WrappedConfig.ToBizIMapper, input);

            //VERIFY    
            data.Num.ShouldEqual(234);
        }


        [Fact]
        public async Task TestBizOutCopyAsyncAsyncDtoOk()
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizOutDtoAsync>();
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDtoAsync), false, true, true);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<ServiceLayerBizOutDtoAsync>(null, utData.WrappedConfig.FromBizIMapper, input);

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
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(BizDataOut), false, true, true);
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
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizOutDto>();
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDto), false, true, true);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<ServiceLayerBizOutDto>(null, utData.WrappedConfig.FromBizIMapper, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
            data.CopyFromBizDataCalled.ShouldBeTrue();
            data.SetupSecondaryOutputDataCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task TestCreateDataWithPossibleSetupAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true, true);
            var status = new TestBizActionStatus();

            //ATTEMPT
            var data = await copier.CreateDataWithPossibleSetupAsync<ServiceLayerBizInDtoAsync>(null, status, null);

            //VERIFY    
            data.SetupSecondaryDataCalled.ShouldBeTrue();
            data.Num.ShouldEqual(0);
        }

        [Fact]
        public async Task TestCreateDataWithPossibleSetupAsyncDtoWithActionOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true, true);
            var status = new TestBizActionStatus();

            //ATTEMPT
            var data = await copier.CreateDataWithPossibleSetupAsync<ServiceLayerBizInDtoAsync>(null, status, x => { x.Num = 2;});

            //VERIFY    
            data.SetupSecondaryDataCalled.ShouldBeTrue();
            data.Num.ShouldEqual(2);
        }

        //-------------------------------------------------------------
        //CreateDataWithPossibleSetup

        [Fact]
        public async Task TestCreateDataWithPossibleSetupDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true, true);
            var status = new TestBizActionStatus();

            //ATTEMPT
            await copier.CreateDataWithPossibleSetupAsync<BizDataIn>(null, status, null);

            //VERIFY    
            //Should work with no exceptions
        }

        [Fact]
        public async Task TestCreateDataWithPossibleSetupSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true, true);
            var status = new TestBizActionStatus();

            //ATTEMPT
            var data = await copier.CreateDataWithPossibleSetupAsync<ServiceLayerBizInDto>(null, status, null);

            //VERIFY    
            data.SetupSecondaryDataCalled.ShouldBeTrue();
            data.Num.ShouldEqual(0);
        }

        [Fact]
        public async Task TestCreateDataWithPossibleSetupSyncDtoWithActionOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true, true);
            var status = new TestBizActionStatus();

            //ATTEMPT
            var data = await copier.CreateDataWithPossibleSetupAsync<ServiceLayerBizInDto>(null, status, x => { x.Num = 2; });

            //VERIFY    
            data.SetupSecondaryDataCalled.ShouldBeTrue();
            data.Num.ShouldEqual(2);
        }

        //-------------------------------------------------------------------
        //SetupSecondaryDataIfRequired

        [Fact]
        public async Task TestSetupSecondaryAsyncDataDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true, true);
            var input = new BizDataIn { Num = 234 };
            var status = new TestBizActionStatus();

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, status, input);

            //VERIFY
            //Should work with no expections
        }

        [Fact]
        public async Task TestSetupSecondaryDataAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true, true);
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };
            var status = new TestBizActionStatus();

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, status, input);

            //VERIFY    
            input.SetupSecondaryDataCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task TestSetupSecondaryDataSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true, true);
            var input = new ServiceLayerBizInDto { Num = 234 };
            var status = new TestBizActionStatus();

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, status, input);

            //VERIFY    
            input.SetupSecondaryDataCalled.ShouldBeTrue();
        }
    }
}