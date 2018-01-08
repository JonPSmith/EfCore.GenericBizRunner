// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner
{
    public class ActionService<TBizInstance> : ActionService<DbContext, TBizInstance>
        where TBizInstance : class, IBizActionStatus
    {
        public ActionService(DbContext context, TBizInstance bizInstance, IMapper mapper, IGenericBizRunnerConfig config = null)
            : base(context, bizInstance, mapper, config)
        {
        }
    }

    public class ActionService<TContext, TBizInstance> : BizActionStatus, IActionService<TBizInstance>
        where TContext : DbContext
        where TBizInstance : class, IBizActionStatus
    {
        private readonly TBizInstance _bizInstance;
        private readonly IGenericBizRunnerConfig _config;
        private readonly IMapper _mapper;
        private readonly TContext _context;

        public ActionService(TContext context, TBizInstance bizInstance, IMapper mapper, IGenericBizRunnerConfig config = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _bizInstance = bizInstance ?? throw new ArgumentNullException(nameof(bizInstance));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _config = config ?? new GenericBizRunnerConfig();
        }

        /// <summary>
        /// This contains the Status after it has been run
        /// </summary>
        public IBizActionStatus Status => _bizInstance;

        /// <summary>
        /// This will run a business action that takes and input and produces a result
        /// </summary>
        /// <typeparam name="TOut">The type of the result to return. Should either be the Business logic output type or class which inherits fromm GenericActionFromBizDto</typeparam>
        /// <param name="inputData">The input data. It should be Should either be the Business logic input type or class which inherits form GenericActionToBizDto</param>
        /// <returns>The returned data after the run, or default value is thewre was an error</returns>
        public TOut RunBizAction<TOut>(object inputData)
        {
            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.InOut, _config.TurnOffCaching);
            return decoder.BizInfo.GetServiceInstance(_config).RunBizActionDbAndInstance<TOut>(_context, _bizInstance, _mapper, inputData);
        }

        /// <summary>
        /// This will run a business action that does not take an input but does produces a result
        /// </summary>
        /// <typeparam name="TOut">The type of the result to return. Should either be the Business logic output type or class which inherits fromm GenericActionFromBizDto</typeparam>
        /// <returns>The returned data after the run, or default value is thewre was an error</returns>
        public TOut RunBizAction<TOut>()
        {
            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.Out, _config.TurnOffCaching);
            return decoder.BizInfo.GetServiceInstance(_config).RunBizActionDbAndInstance<TOut>(_context, _bizInstance, _mapper);
        }

        /// <summary>
        /// This runs a business action which takes an input and returns just a status message
        /// </summary>
        /// <param name="inputData">The input data. It should be Should either be the Business logic input type or class which inherits form GenericActionToBizDto</param>
        /// <returns>status message with no result part</returns>
        public void RunBizAction(object inputData)
        {
            {
                var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.In, _config.TurnOffCaching);
                decoder.BizInfo.GetServiceInstance(_config)
                    .RunBizActionDbAndInstance(_context, _bizInstance, _mapper, inputData);
            }
        }

        /// <summary>
        /// This will return a new class for input. 
        /// If the type is based on a GenericActionsDto, or it has ISetupsecondaryData, it will run SetupSecondaryData on it before handing it back
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public TDto GetDto<TDto>() where TDto : class, new()
        {
            if (!typeof(TDto).IsClass)
                throw new InvalidOperationException("You should only call this on a primitive type. Its only useful for Dtos.");

            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.InOrInOut, _config.TurnOffCaching);
            var toBizCopier = DtoAccessGenerator.BuildCopier(typeof(TDto), decoder.BizInfo.GetBizInType(), true, true, _config);
            return toBizCopier.CreateDataWithPossibleSetup<TDto>(_context);
        }

        /// <summary>
        /// This should be called if a model error is found in the input data.
        /// If the provided data is a GenericActions dto, or it has ISetupsecondaryData, it will call SetupSecondaryData 
        /// to reset any data in the dto ready for reshowing the dto to the user.
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public TDto ResetDto<TDto>(TDto dto) where TDto : class
        {
            if (!typeof(TDto).IsClass)
                throw new InvalidOperationException("You should only call this on a primitive type. Its only useful for Dtos.");

            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.InOrInOut, _config.TurnOffCaching);
            var toBizCopier = DtoAccessGenerator.BuildCopier(typeof(TDto), decoder.BizInfo.GetBizInType(), true, true, _config);
            toBizCopier.SetupSecondaryDataIfRequired(_context, dto);
            return dto;
        }

    }
}