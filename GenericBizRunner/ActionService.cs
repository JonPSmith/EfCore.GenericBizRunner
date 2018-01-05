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


    public class ActionService<TContext, TBizInterface> : BizActionStatus
        where TContext : DbContext
    {
        private TContext _context;
        private TBizInterface _bizInterface;
        private IGenericBizRunnerConfig _config;

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
    }
}