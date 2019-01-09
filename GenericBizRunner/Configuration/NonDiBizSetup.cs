// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericBizRunner.Configuration.Internal;
using GenericBizRunner.PublicButHidden;

namespace GenericBizRunner.Configuration
{
    public class NonDiBizSetup
    {
        /// <summary>
        /// We only create the IWrappedConfigAndMapper when someone needs it.
        /// This allows you to add multiple DTOs and the AutoMapper mapping is only worked out once they are all in.
        /// </summary>
        private IWrappedBizRunnerConfigAndMappings _configAndMapper;

        private readonly BizRunnerProfile _bizInProfile;
        private readonly BizRunnerProfile _bizOutProfile;

        /// <summary>
        /// This holds the global configuration and the AutoMapper data
        /// </summary>
        public IWrappedBizRunnerConfigAndMappings WrappedConfig => 
            _configAndMapper ?? (_configAndMapper =
               SetupDtoMappings.CreateWrappedConfig(PublicConfig, _bizInProfile, _bizOutProfile));

        /// <summary>
        /// This is the global config
        /// </summary>
        public IGenericBizRunnerConfig PublicConfig { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="publicConfig"></param>
        public NonDiBizSetup(IGenericBizRunnerConfig publicConfig = null)
        {
            PublicConfig = publicConfig ?? new GenericBizRunnerConfig();
            _bizInProfile = new BizRunnerProfile();
            _bizOutProfile = new BizRunnerProfile();
        }

        public static NonDiBizSetup SetupBizInDtoMapping<TBizInDto>(IGenericBizRunnerConfig publicConfig = null)
        {
            var nonDiConf = new NonDiBizSetup(publicConfig);
            SetupDtoMappings.SetupMappingForDto(typeof(TBizInDto), nonDiConf._bizInProfile, true);
            //SetupDtoMapping(typeof(TBizInDto), nonDiConf);
            return nonDiConf;
        }

        public void AddBizInDtoMapping<TBizInDto>()
        {
            SetupDtoMappings.SetupMappingForDto(typeof(TBizInDto), _bizInProfile, true);
            //SetupDtoMapping(typeof(TBizInDto), this);
        }

        public static NonDiBizSetup SetupBizOutDtoMapping<TBizOutDto>(IGenericBizRunnerConfig publicConfig = null)
        {
            var nonDiConf = new NonDiBizSetup(publicConfig);
            SetupDtoMappings.SetupMappingForDto(typeof(TBizOutDto), nonDiConf._bizOutProfile, false);
            return nonDiConf;
        }

        public void AddBizOutDtoMapping<TBizOutDto>(IGenericBizRunnerConfig publicConfig = null)
        {
            SetupDtoMappings.SetupMappingForDto(typeof(TBizOutDto), _bizOutProfile, false);
        }

        //---------------------------------------------------
        //private 

        private static void SetupDtoMapping(Type dtoType, NonDiBizSetup nonDiConf)
        {
            var bizIn = dtoType.GetInterface(nameof(IGenericActionToBizDto)) != null;
            if (!bizIn && dtoType.GetInterface(nameof(IGenericActionFromBizDto)) == null)
                throw new InvalidOperationException($"The class {dtoType.Name} doesn't inherit from ine of the Biz Runner Dto classes.");

            SetupDtoMappings.SetupMappingForDto(dtoType, bizIn ? nonDiConf._bizInProfile : nonDiConf._bizOutProfile, bizIn);
        }
    }
}