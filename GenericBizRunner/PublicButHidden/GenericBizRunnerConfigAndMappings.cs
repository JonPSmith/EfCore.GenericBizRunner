// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericBizRunner.Configuration;

namespace GenericBizRunner.PublicButHidden
{
    public interface IWrappedBizRunnerConfigAndMappings
    {
        /// <summary>
        /// This holds the config for BizRunner
        /// </summary>
        IGenericBizRunnerConfig Config { get; }

        /// <summary>
        /// This holds the mappings from DTOs input to the business logic
        /// </summary>
        IMapper ToBizIMapper { get; }

        /// <summary>
        /// This holds the mappings from the business logic output to DTOs
        /// </summary>
        IMapper FromBizIMapper { get; }
    }

    public class WrappedBizRunnerConfigAndMappings : IWrappedBizRunnerConfigAndMappings
    {
        public WrappedBizRunnerConfigAndMappings(IGenericBizRunnerConfig config, MapperConfiguration toBizMapping, MapperConfiguration fromBizMapping)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            ToBizIMapper = toBizMapping?.CreateMapper() ?? throw new ArgumentNullException(nameof(toBizMapping));
            FromBizIMapper = fromBizMapping?.CreateMapper() ?? throw new ArgumentNullException(nameof(fromBizMapping));
        }

        /// <inheritdoc />
        public IGenericBizRunnerConfig Config { get; }

        /// <inheritdoc />
        public IMapper ToBizIMapper { get; }

        /// <inheritdoc />
        public IMapper FromBizIMapper { get; }
    }
}