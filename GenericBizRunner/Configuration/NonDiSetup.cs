// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner.Configuration.Internal;
using GenericBizRunner.PublicButHidden;

namespace GenericBizRunner.Configuration
{
    public class NonDiSetup
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
        public NonDiSetup(IGenericBizRunnerConfig publicConfig)
        {
            PublicConfig = publicConfig ?? new GenericBizRunnerConfig();
            _bizInProfile = new BizRunnerProfile();
            _bizOutProfile = new BizRunnerProfile();
        }

        public static NonDiSetup SetupBizInDtoMapping<TbizInDto>(IGenericBizRunnerConfig publicConfig = null)
        {
            var nonDiConf = new NonDiSetup(publicConfig);
            SetupDtoMappings.SetupMappingForDto(typeof(TbizInDto), nonDiConf._bizOutProfile, true);
            return nonDiConf;
        }

        public void AddBizInDtoMapping<TbizInDto>()
        {
            SetupDtoMappings.SetupMappingForDto(typeof(TbizInDto), _bizOutProfile, true);
        }

        public static NonDiSetup SetupBizOutDtoMapping<TbizOutDto>(IGenericBizRunnerConfig publicConfig = null)
        {
            var nonDiConf = new NonDiSetup(publicConfig);
            SetupDtoMappings.SetupMappingForDto(typeof(TbizOutDto), nonDiConf._bizOutProfile, false);
            return nonDiConf;
        }

        public void AddBizOutDtoMapping<TbizOutDto>(IGenericBizRunnerConfig publicConfig = null)
        {
            SetupDtoMappings.SetupMappingForDto(typeof(TbizOutDto), _bizOutProfile, false);
        }
    }
}