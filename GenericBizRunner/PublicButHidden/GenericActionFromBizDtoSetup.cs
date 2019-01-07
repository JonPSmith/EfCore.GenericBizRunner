// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;

namespace GenericBizRunner.PublicButHidden
{
    /// <summary>
    /// This is the abstract class that is used by GenericActionFromBizDto class
    /// It uses the AutoMapper Profile class to allow creation of the mapping in the ctor
    /// </summary>
    /// <typeparam name="TBizOut"></typeparam>
    /// <typeparam name="TDtoOut"></typeparam>
    public abstract class GenericActionFromBizDtoSetup<TBizOut, TDtoOut> : Profile
        where TBizOut : class
        where TDtoOut : GenericActionFromBizDtoSetup<TBizOut, TDtoOut>
    {
        /// <summary>
        /// ctor
        /// It is valid to call a method that has been overrriden in a derived class (see unit test TestOverrideMethodInBaseCtor)
        /// </summary>
        protected GenericActionFromBizDtoSetup()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            AutoMapperSetup();
        }

        /// <summary>
        /// Override this to modify the AutoMapper mappings. For instance adding .ForMember or .BeforeMap/.AfterMap
        /// </summary>
        protected virtual void AutoMapperSetup()
        {
            CreateMap<TBizOut, TDtoOut>();
        }
    }
}