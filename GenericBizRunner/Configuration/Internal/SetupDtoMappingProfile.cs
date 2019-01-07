// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericBizRunner.PublicButHidden;

namespace GenericBizRunner.Configuration.Internal
{
    internal class SetupDtoMappingProfile
    {
        public SetupDtoMappingProfile(Type dtoType, Type bizInOutType, BizRunnerProfile profile, bool bizIn)
        {
            var myGeneric = bizIn ? typeof(SetupToBizDtoProfile<,>) : typeof(SetupFromBizDtoProfile<,>);
            var setupType = myGeneric.MakeGenericType(dtoType, bizInOutType);
            Activator.CreateInstance(setupType, new object[]{ profile });
        }

        private class SetupFromBizDtoProfile<TBizOut, TDtoOut>
            where TBizOut : class
            where TDtoOut : GenericActionFromBizDtoSetup<TBizOut, TDtoOut>, new()
        {

            public SetupFromBizDtoProfile(BizRunnerProfile profile)
            {
                dynamic dto = Activator.CreateInstance(typeof(TDtoOut));
                var mappingConfig = dto.MappingConfig;
                if (mappingConfig == null)
                    profile.CreateMap<TBizOut, TDtoOut>();
                else
                {
                    mappingConfig(profile.CreateMap<TBizOut, TDtoOut>());
                }
            }
        }

        private class SetupToBizDtoProfile<TDtoIn, TBizIn> //NOTE: The TDtoIn, TBizIn are in a different order to elsewhere! That is on purpose
            where TDtoIn : GenericActionToBizDtoSetup<TBizIn, TDtoIn>, new()
            where TBizIn : class
        {
            public SetupToBizDtoProfile(BizRunnerProfile profile)
            {
                dynamic dto = Activator.CreateInstance(typeof(TDtoIn));
                var mappingConfig = dto.MappingConfig;
                if (mappingConfig == null)
                    profile.CreateMap<TBizIn, TDtoIn>();
                else
                {
                    mappingConfig(profile.CreateMap<TBizIn, TDtoIn>());
                }
            }
        }
    }
}