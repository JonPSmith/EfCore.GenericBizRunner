// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner
{
    public class ActionService<TBizInterface> : ActionService<DbContext, TBizInterface>
    {
        public ActionService(DbContext context, TBizInterface bizInterface, IGenericBizRunnerConfig config = null)
            : base(context, bizInterface, config)
        {
        }
    }


    public class ActionService<TContext, TBizInterface> : BizActionStatus, IActionService<TBizInterface>
        where TContext : DbContext
    {
        private TContext _context;
        private TBizInterface _bizInterface;
        private IGenericBizRunnerConfig _config;

        public IBizActionStatus Status { get; private set; }

        public ActionService(TContext context, TBizInterface bizInterface, IGenericBizRunnerConfig config = null)
        {
            _context = context;
            _bizInterface = bizInterface;
            _config = config ?? new GenericBizRunnerConfig();
        }

        public TOut RunBizAction<TOut>(object inputData)
        {
            throw new NotImplementedException();
        }

        public TOut RunBizAction<TOut>()
        {
            throw new NotImplementedException();
        }

        public void RunBizAction(object inputData)
        {
            throw new NotImplementedException();
        }

        public TDto GetDto<TDto>() where TDto : class, new()
        {
            throw new NotImplementedException();
        }

        public TDto ResetDto<TDto>(TDto dto) where TDto : class
        {
            throw new NotImplementedException();
        }
    }
}