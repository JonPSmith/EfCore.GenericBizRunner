// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;

namespace GenericBizRunner.PublicButHidden
{
    /// <summary>
    /// This is the abstract class that is used by GenericActionToBizDto class
    /// It uses the AutoMapper Profile class to allow creation of the mapping in the ctor
    /// </summary>
    /// <typeparam name="TBizIn"></typeparam>
    /// <typeparam name="TDtoIn"></typeparam>
    public abstract class GenericActionToBizDtoSetup<TBizIn, TDtoIn> : Profile
        where TBizIn : class
        where TDtoIn : GenericActionToBizDtoSetup<TBizIn, TDtoIn>
    {
        /// <summary>
        /// ctor
        /// It is valid to call a method that has been overrriden in a derived class (see unit test TestOverrideMethodInBaseCtor)
        /// </summary>
        protected GenericActionToBizDtoSetup()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            AutoMapperSetup();
        }

        /// <summary>
        /// Override this to modify the AutoMapper mappings. For instance adding .ForMember or .BeforeMap/.AfterMap
        /// </summary>
        protected virtual void AutoMapperSetup()
        {
            CreateMap<TDtoIn, TBizIn>();
        }
    }
}