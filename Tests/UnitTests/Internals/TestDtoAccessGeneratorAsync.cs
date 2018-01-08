// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner.Internal;
using GenericBizRunner.Internal.DtoAccessors;
using TestBizLayer.BizDTOs;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Internals
{
    public class TestDtoAccessGeneratorAsync
    {
        //NOTE: async business methods can use either sync or async dtos, so we need to check both

        [Fact]
        public async Task Test01BizInCopyAsyncServiceInstanceOk()
        {
            //SETUP 
            var copier = new CopyToBizDataAsync<BizDataIn, ServiceLayerBizInDtoAsync>();
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };

            //ATTEMPT
            var status = await copier.CopyToBizAsync(null, input);

            //VERIFY    
            status.ShouldBeValid();
            status.Result.Num.ShouldEqual(234);
        }

        //--------------------------------------------------
        //BizIn

        [Fact]
        public async Task Test05BizInCopyAsyncMethodDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true);
            var input = new BizDataIn { Num = 234 };

            //ATTEMPT
            var status = await copier.DoCopyToBizAsync<BizDataIn>(null, input);

            //VERIFY    
            status.ShouldBeValid();
            status.Result.Num.ShouldEqual(234);
        }

        [Fact]
        public async Task Test06BizInCopyAsyncAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true);
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };

            //ATTEMPT
            var status = await copier.DoCopyToBizAsync<BizDataIn>(null, input);

            //VERIFY    
            status.ShouldBeValid();
            status.Result.Num.ShouldEqual(234);
        }

        [Fact]
        public async Task Test07BizInCopyAsyncSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true);
            var input = new ServiceLayerBizInDto { Num = 234 };

            //ATTEMPT
            var status = await copier.DoCopyToBizAsync<BizDataIn>(null, input);

            //VERIFY    
            status.ShouldBeValid();
            status.Result.Num.ShouldEqual(234);
        }

        //-----------------------
        //BizIn reset checking

        [Fact]
        public async Task Test15FailCopyBizInSuccessAsyncAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true);
            var input = new ServiceLayerBizInDtoAsync { DtoControlNum = 234 };

            //ATTEMPT
            var status = await copier.DoCopyToBizAsync<BizDataIn>(null, input);

            //VERIFY    
            status.ShouldBeValid();
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "CopyToBizDataAsync.Success").ShouldEqual(true);
            logs.Any(x => x.RenderedMessage == "SetupSecondaryDataAsync").ShouldEqual(false);
        }

        [Fact]
        public async Task Test16FailCopyBizInSuccessAsyncSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true);
            var input = new ServiceLayerBizInDto { DtoControlNum = 234 };

            //ATTEMPT
            var status = await copier.DoCopyToBizAsync<BizDataIn>(null, input);

            //VERIFY    
            status.ShouldBeValid();
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "CopyToBizData.Success").ShouldEqual(true);
            logs.Any(x => x.RenderedMessage == "SetupSecondaryData").ShouldEqual(false);
        }

        [Fact]
        public async Task Test17FailCopyBizInCallSetupSecondaryDataAsyncAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true);
            var input = new ServiceLayerBizInDtoAsync { DtoControlNum = -1 };

            //ATTEMPT
            var status = await copier.DoCopyToBizAsync<BizDataIn>(null, input);

            //VERIFY    
            status.ShouldBeValid(false);
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "CopyToBizDataAsync.Error").ShouldEqual(true);
            logs.Any(x => x.RenderedMessage == "SetupSecondaryDataAsync").ShouldEqual(true);
        }

        [Fact]
        public async Task Test18FailCopyBizInCallSetupSecondaryDataAsyncSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true);
            var input = new ServiceLayerBizInDto { DtoControlNum = -1 };

            //ATTEMPT
            var status = await copier.DoCopyToBizAsync<BizDataIn>(null, input);

            //VERIFY    
            status.ShouldBeValid(false);
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "CopyToBizData.Error").ShouldEqual(true);
            logs.Any(x => x.RenderedMessage == "SetupSecondaryData").ShouldEqual(true);
        }


        //-----------------------
        //BizOut

        [Fact]
        public async Task Test30BizOutCopyAsyncDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(BizDataOut), false, true);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<BizDataOut>(null, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
        }


        [Fact]
        public async Task Test31BizOutCopyAsyncAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDtoAsync), false, true);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<ServiceLayerBizOutDtoAsync>(null, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
        }

        [Fact]
        public async Task Test32BizOutCopyAsyncSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDto), false, true);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<ServiceLayerBizOutDto>(null, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
        }

        [Fact]
        public async Task Test35BizOutCheckSecondaryDataCalledAsyncAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDtoAsync), false, true);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<ServiceLayerBizOutDtoAsync>(null, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryOutputDataAsync").ShouldEqual(true);
        }


        [Fact]
        public async Task Test36BizOutCheckSecondaryDataCalledAsyncSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataOut), typeof(ServiceLayerBizOutDto), false, true);
            var input = new BizDataOut { Output = "test copy" };

            //ATTEMPT
            var data = await copier.DoCopyFromBizAsync<ServiceLayerBizOutDto>(null, input);

            //VERIFY    
            data.Output.ShouldEqual("test copy");
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryOutputData").ShouldEqual(true);
        }


        //-------------------------------------------------------------------
        //SetupSecondaryDataIfRequired

        [Fact]
        public async Task Test40SetupSecondaryAsyncDataDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true);
            var input = new BizDataIn { Num = 234 };

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, input);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any().ShouldEqual(false);
        }

        [Fact]
        public async Task Test41SetupSecondaryDataAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true);
            var input = new ServiceLayerBizInDtoAsync { Num = 234 };

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, input);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryDataAsync").ShouldEqual(true);
        }

        [Fact]
        public async Task Test42SetupSecondaryDataSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true);
            var input = new ServiceLayerBizInDto { Num = 234 };

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, input);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryData").ShouldEqual(true);
        }

        [Fact]
        public void Test45SetupSecondaryDataViaInterfaceOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataInSetupSecondaryData), typeof(BizDataInSetupSecondaryData), true, true);
            var input = new BizDataInSetupSecondaryData { Num = 234 };

            //ATTEMPT
            copier.SetupSecondaryDataIfRequired(null, input);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryData").ShouldEqual(true);
        }

        [Fact]
        public async Task Test46SetupSecondaryDataAsyncViaInterfaceOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataInSetupSecondaryDataAsync), typeof(BizDataInSetupSecondaryDataAsync), true, true);
            var input = new BizDataInSetupSecondaryDataAsync { Num = 234 };

            //ATTEMPT
            await copier.SetupSecondaryDataIfRequiredAsync(null, input);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryDataAsync").ShouldEqual(true);
        }

        //-------------------------------------------------------------
        //CreateDataWithPossibleSetup

        [Fact]
        public async Task Test50CreateDataWithPossibleSetupDirectOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataIn), typeof(BizDataIn), true, true);

            //ATTEMPT
            await copier.CreateDataWithPossibleSetupAsync<BizDataIn>(null);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any().ShouldEqual(false);
        }

        [Fact]
        public async Task Test51CreateDataWithPossibleSetupAsyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDtoAsync), typeof(BizDataIn), true, true);

            //ATTEMPT
            await copier.CreateDataWithPossibleSetupAsync<ServiceLayerBizInDtoAsync>(null);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryDataAsync").ShouldEqual(true);
        }

        [Fact]
        public async Task Test52CreateDataWithPossibleSetupSyncDtoOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(ServiceLayerBizInDto), typeof(BizDataIn), true, true);

            //ATTEMPT
            await copier.CreateDataWithPossibleSetupAsync<ServiceLayerBizInDto>(null);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryData").ShouldEqual(true);
        }

        [Fact]
        public void Test55CreateDataWithPossibleSetupInterfaceOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataInSetupSecondaryData), typeof(BizDataInSetupSecondaryData), true, true);

            //ATTEMPT
            copier.CreateDataWithPossibleSetup<BizDataInSetupSecondaryData>(null);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryData").ShouldEqual(true);
        }

        [Fact]
        public async Task Test56CreateDataWithPossibleSetupInterfaceAsyncOk()
        {
            //SETUP 
            var copier = DtoAccessGenerator.BuildCopier(typeof(BizDataInSetupSecondaryDataAsync), typeof(BizDataInSetupSecondaryDataAsync), true, true);

            //ATTEMPT
            await copier.CreateDataWithPossibleSetupAsync<BizDataInSetupSecondaryDataAsync>(null);

            //VERIFY    
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Any(x => x.RenderedMessage == "SetupSecondaryDataAsync").ShouldEqual(true);
        }
    }
}