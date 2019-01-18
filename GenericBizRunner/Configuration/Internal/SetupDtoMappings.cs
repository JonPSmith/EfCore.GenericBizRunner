// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using GenericBizRunner.PublicButHidden;

namespace GenericBizRunner.Configuration.Internal
{
    internal class SetupDtoMappings
    {
        readonly BizRunnerProfile _bizInProfile = new BizRunnerProfile();
        readonly BizRunnerProfile _bizOutProfile = new BizRunnerProfile();

        public IGenericBizRunnerConfig PublicConfig { get; }

        public SetupDtoMappings(IGenericBizRunnerConfig publicConfig)
        {
            PublicConfig = publicConfig ?? throw new ArgumentNullException(nameof(publicConfig));
        }

        public IWrappedBizRunnerConfigAndMappings BuildWrappedConfigByScanningForDtos(Assembly[] assembliesToScan, IGenericBizRunnerConfig config)
        {
            
            if (assembliesToScan == null || assembliesToScan.Length == 0)
                throw new ArgumentException("Needs assemblies to scan for DTOs. If not using DTOs use Assembly.GetExecutingAssembly()."
                    , nameof(assembliesToScan));
            foreach (var assembly in assembliesToScan)
            {
                ScanAssemblyAndAddToProfiles(assembly);
            }
            return CreateWrappedConfig(config, _bizInProfile, _bizOutProfile);
        }

        public static IWrappedBizRunnerConfigAndMappings CreateWrappedConfig(IGenericBizRunnerConfig config, 
            BizRunnerProfile bizInProfile, BizRunnerProfile bizOutProfile)
        {
            var bizInMapping = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMissingTypeMaps = false;
                cfg.AddProfile(bizInProfile);
            });
            var bizOutMapping = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMissingTypeMaps = false;
                cfg.AddProfile(bizOutProfile);
            });
            return new WrappedBizRunnerConfigAndMappings(config, bizInMapping, bizOutMapping);
        }

        //------------------------------------------------------
        //internal

        internal static void SetupMappingForDto(Type dtoType, BizRunnerProfile profile, bool bizIn)
        {
            var baseTypeName = (bizIn
                ? typeof(GenericActionToBizDtoSetup<,>)
                : typeof(GenericActionFromBizDtoSetup<,>)).FullName;
            baseTypeName = baseTypeName.Substring(0, baseTypeName.IndexOf('`'));
            Type setupType = null, loopType = dtoType.BaseType;
            while(setupType == null && loopType != null)
            {
                if (loopType.FullName.StartsWith(baseTypeName))
                    setupType = loopType;
                else
                {
                    loopType = loopType.BaseType;
                }
            }
            if (setupType == null)
                throw new InvalidOperationException(
                    $"You registered the DTO {dtoType.Name}, as a {(bizIn ? "bizInDto" : "bizOutDto")}, but it doesn't inherit from {baseTypeName}.");

            var bizInOutType = setupType.GetGenericArguments()[0];
            new SetupDtoMappingProfile(dtoType, bizInOutType, profile, bizIn);
        }

        //----------------------------------------------
        //private

        private void ScanAssemblyAndAddToProfiles(Assembly assemblyToScan)
        {
            var allTypesInAssembly = assemblyToScan.GetTypes();

            foreach (var bizOutDtoType in allTypesInAssembly.Where(x => x.GetInterface(nameof(IGenericActionFromBizDto)) != null))
            {
                SetupMappingForDto(bizOutDtoType, _bizOutProfile, false);
            }
            foreach (var bizOutDtoType in allTypesInAssembly.Where(x => x.GetInterface(nameof(IGenericActionToBizDto)) != null))
            {
                SetupMappingForDto(bizOutDtoType, _bizInProfile, true);
            }
        }


    }
}