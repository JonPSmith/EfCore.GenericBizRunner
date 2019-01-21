// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericBizRunner.PublicButHidden;

namespace GenericBizRunner.Configuration.Internal
{
    internal class SetupDtoMappingProfile
    {
        public SetupDtoMappingProfile(Type dtoType, Type bizInOutType, BizRunnerProfile profile, bool bizIn)
        {
            var myGeneric = bizIn ? typeof(SetupToBizDtoProfile<,>) : typeof(SetupFromBizDtoProfile<,>);
            var setupType = myGeneric.MakeGenericType(bizInOutType, dtoType);
            Activator.CreateInstance(setupType, new object[]{ profile });
        }

        private class SetupFromBizDtoProfile<TBizOut, TDtoOut>
            where TBizOut : class
            where TDtoOut : GenericActionFromBizDtoSetup<TBizOut, TDtoOut>, new()
        {

            public SetupFromBizDtoProfile(BizRunnerProfile profile)
            {
                dynamic dto = Activator.CreateInstance(typeof(TDtoOut));
                var alterMapExp = dto.AlterDtoMapping;
                if (alterMapExp == null)
                    profile.CreateMap<TBizOut, TDtoOut>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                else
                {
                    alterMapExp(profile.CreateMap<TBizOut, TDtoOut>().IgnoreAllPropertiesWithAnInaccessibleSetter());
                }
            }
        }

        private class SetupToBizDtoProfile<TBizIn, TDtoIn>
            where TBizIn : class
            where TDtoIn : GenericActionToBizDtoSetup<TBizIn, TDtoIn>, new()
        {
            public SetupToBizDtoProfile(BizRunnerProfile profile)
            {
                dynamic dto = Activator.CreateInstance(typeof(TDtoIn));
                var alterMapExp = dto.AlterDtoMapping;
                if (alterMapExp == null)
                    profile.CreateMap<TDtoIn, TBizIn>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                else
                {
                    alterMapExp(profile.CreateMap<TDtoIn, TBizIn>().IgnoreAllPropertiesWithAnInaccessibleSetter());
                }
            }
        }
    }
}