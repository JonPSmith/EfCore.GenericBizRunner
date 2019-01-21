// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;

namespace GenericBizRunner.Configuration.Internal
{
    internal class BizRunnerProfile : Profile
    {
        public BizRunnerProfile()
        {
            CreateMissingTypeMaps = true;
        }
    }
}