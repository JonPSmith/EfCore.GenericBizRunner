// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GenericBizRunner.Internal.Runners;

namespace GenericBizRunner.Internal
{
    internal class ServiceBuilderLookup
    {
        internal static readonly IReadOnlyDictionary<Type, ServiceBuilderLookup> ServiceLookup =
            new ReadOnlyDictionary<Type, ServiceBuilderLookup>(
                new Dictionary<Type, ServiceBuilderLookup>()
                {
                    { typeof(IGenericAction<,>), new ServiceBuilderLookup(typeof(ActionServiceInOut<,,>), RequestedInOut.InOut, WriteToDbStates.DoNotWrite)},
                    //{ typeof(IGenericActionAsync<,>), new ServiceBuilderLookup(typeof(ActionServiceInOutAsync<,,>), RequestedInOut.InOut | RequestedInOut.Async, false)},
                    //{ typeof(IGenericActionInOnly<>), new ServiceBuilderLookup(typeof(ActionServiceInOnly<,>), RequestedInOut.In, false)},
                    //{ typeof(IGenericActionInOnlyAsync<>), new ServiceBuilderLookup(typeof(ActionServiceInOnlyAsync<,>), RequestedInOut.In| RequestedInOut.Async, false)},
                    //{ typeof(IGenericActionOutOnly<>), new ServiceBuilderLookup(typeof(ActionServiceOutOnly<,>), RequestedInOut.Out, false)},
                    //{ typeof(IGenericActionOutOnlyAsync<>), new ServiceBuilderLookup(typeof(ActionServiceOutOnlyAsync<,>), RequestedInOut.Out | RequestedInOut.Async, false)},
                    ////Now the writeDb versions
                    //{ typeof(IGenericActionWriteDb<,>), new ServiceBuilderLookup(typeof(ActionServiceInOut<,,>), RequestedInOut.InOut, true)},
                    //{ typeof(IGenericActionWriteDbAsync<,>), new ServiceBuilderLookup(typeof(ActionServiceInOutAsync<,,>), RequestedInOut.InOut | RequestedInOut.Async, true)},
                    //{ typeof(IGenericActionInOnlyWriteDb<>), new ServiceBuilderLookup(typeof(ActionServiceInOnly<,>), RequestedInOut.In, true)},
                    //{ typeof(IGenericActionInOnlyWriteDbAsync<>), new ServiceBuilderLookup(typeof(ActionServiceInOnlyAsync<,>), RequestedInOut.In| RequestedInOut.Async, true)},
                    //{ typeof(IGenericActionOutOnlyWriteDb<>), new ServiceBuilderLookup(typeof(ActionServiceOutOnly<,>), RequestedInOut.Out, true)},
                    //{ typeof(IGenericActionOutOnlyWriteDbAsync<>), new ServiceBuilderLookup(typeof(ActionServiceOutOnlyAsync<,>), RequestedInOut.Out | RequestedInOut.Async, true)},
                });


        public ServiceBuilderLookup(Type serviceHandleType, RequestedInOut typeOfService, WriteToDbStates writeStates)
        {
            ServiceHandleType = serviceHandleType;
            TypeOfService = typeOfService;
            WriteStates = writeStates;
        }

        /// <summary>
        /// This holds the internal service to handle this type of biz action
        /// </summary>
        public Type ServiceHandleType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public RequestedInOut TypeOfService { get; private set; }

        /// <summary>
        /// True if the interface name contains "WriteDb"
        /// </summary>
        public WriteToDbStates WriteStates { get; private set; }

        public override string ToString()
        {
            return string.Format("ServiceHandleType: {0}, TypeOfService: {1}, WriteStates: {2}", ServiceHandleType.Name,
                TypeOfService, WriteStates);
        }
    }
}