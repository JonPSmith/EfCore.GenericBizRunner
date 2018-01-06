// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using GenericBizRunner.Configuration;

namespace GenericBizRunner.Internal
{
    [Flags]
    internal enum RequestedInOut : byte
    {
        None = 0,
        In = 1,
        Out = 2,
        InOut = In | Out,
        InOrInOut = 4,
        OptionalAsync = 8,
        Async = 16,
        NonAsyncFlagsMask = InOut | InOrInOut,
        AllFlagsMask = NonAsyncFlagsMask | Async
    }

    [Flags]
    internal enum WriteToDbStates
    {
        DoNotWrite = 0,
        WriteToDb = 1,
        ValidateWrite = 2
    }

    internal class BizInfo
    {
        private dynamic _bizInstance;

        private readonly Type _iBizType;

        private readonly Type _extractedActionInterface;

        private readonly ServiceBuilderLookup _matchingServiceType;

        /// <summary>
        /// True if an Async method
        /// </summary>
        public bool IsAsync => _matchingServiceType.TypeOfService.HasFlag(RequestedInOut.Async);

        /// <summary>
        /// True if the interface name contains "WriteDb"
        /// </summary>
        public WriteToDbStates WriteStates => _matchingServiceType.WriteStates;

        public BizInfo(Type iBizType, Type extractedActionInterface, ServiceBuilderLookup matchingServiceType)
        {
            _iBizType = iBizType;
            _extractedActionInterface = extractedActionInterface;
            _matchingServiceType = matchingServiceType;
        }

        public override string ToString()
        {
            return string.Format(
                "IBizType: {0}, ExtractedActionInterface: {1}, MatchingServiceType: {2}, IsAsync: {3}, WriteStates: {4}",
                _iBizType.Name, _extractedActionInterface.Name, _matchingServiceType, IsAsync, WriteStates);
        }

        /// <summary>
        /// This is the instance that can be called to run the service
        /// </summary>
        public dynamic GetServiceInstance(IGenericBizRunnerConfig config)
        {
            return _bizInstance ??
                   (_bizInstance =
                       CreateRequiredServiceInstance(_matchingServiceType, _iBizType, _extractedActionInterface, config));
        }

        public Type GetBizInType()
        {
            if (!_matchingServiceType.TypeOfService.HasFlag(RequestedInOut.In))
                throw new InvalidOperationException("This business logic does not have an input type");

            return _extractedActionInterface.GetGenericArguments().First();
        }

        public Type GetBizOutType()
        {
            if (!_matchingServiceType.TypeOfService.HasFlag(RequestedInOut.Out))
                throw new InvalidOperationException("This business logic does not have an output type");

            return _extractedActionInterface.GetGenericArguments().Last();
        }

        /// <summary>
        /// When running a transaction we need to set the type of the output dynamically. 
        /// This method uses reflection to create the method we need to invoke to call the service
        /// </summary>
        /// <returns></returns>
        public MethodInfo GetRunMethod()
        {
            //Now build the type of the class so we can create the right Method
            var genericAgruments = _extractedActionInterface.GetGenericArguments().ToList();
            genericAgruments.Insert(0, _iBizType);
            var genericType = _matchingServiceType.ServiceHandleType.MakeGenericType(genericAgruments.ToArray());
            var genericRunMethod =
                genericType.GetMethod(IsAsync ? "RunBizActionDbAndInstanceAsync" : "RunBizActionDbAndInstance");
            if (genericRunMethod == null)
                throw new NullReferenceException(
                    "Could not find a run method in the created internal service instance.");

            return _matchingServiceType.TypeOfService.HasFlag(RequestedInOut.Out)
                ? genericRunMethod.MakeGenericMethod(GetBizOutType())
                : genericRunMethod;
        }

        //---------------------------------------------------
        //private methods

        private dynamic CreateRequiredServiceInstance(ServiceBuilderLookup serviceBaseInfo,
            Type iBizType, Type genericInterfacePart, IGenericBizRunnerConfig config)
        {
            var genericAgruments = genericInterfacePart.GetGenericArguments().ToList();
            genericAgruments.Insert(0, iBizType);
            return Activator.CreateInstance(
                serviceBaseInfo.ServiceHandleType.MakeGenericType(genericAgruments.ToArray()),
                new object[] {WriteStates, config});
        }
    }
}