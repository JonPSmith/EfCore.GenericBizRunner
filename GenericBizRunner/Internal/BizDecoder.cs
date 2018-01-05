#region licence
// =====================================================
// GenericActions Library - Library for running business actions
// Filename: BizDecoder.cs
// Date Created: 2015/01/28
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
[assembly: InternalsVisibleTo("ProfileApp")]

namespace GenericBizRunner.Internal
{

    /// <summary>
    /// This class generates the correct version of the ActionService. 
    /// I do this to make the service class signiture managable while still being able to call actions directly, i.e. no reflection.
    /// There are three types, with async and async versions:
    /// 1) InAndOut, sync/async
    /// 2) OutOnly, sync/async
    /// 3) InOnly, sync/async
    /// </summary>
    internal class BizDecoder
    {

        private static readonly ConcurrentDictionary<string, BizInfo> BizDecoderCache = new ConcurrentDictionary<string, BizInfo>(); 

        public BizInfo BizInfo { get; }

        /// <summary>
        /// This decodes the iBizType by looking at the in/out types and sets up the data
        /// to be able to call the business method with the right setup.
        /// It checking that the iBizType matches the requested In, Out, Async rules provided
        /// </summary>
        /// <param name="iBizType">Must be an interface linked to the class holding the business logic</param>
        /// <param name="requestedInOut">The details of how the developer has called the method. Used for checking.</param>
        /// <returns></returns>
        public BizDecoder(Type iBizType, RequestedInOut requestedInOut, bool turnOffCaching)
        {
            BizInfo = turnOffCaching
                ? LocalDecodeIBizType(iBizType, requestedInOut)
                : BizDecoderCache.GetOrAdd(FormCacheKey(iBizType, requestedInOut),
                    type => LocalDecodeIBizType(iBizType, requestedInOut));
        }

        //-------------------------------------------------------------------------
        //private helpers

        private static string FormCacheKey(Type bizType, RequestedInOut requestedInOut)
        {
            return bizType.FullName + "RequestedInOut=" + (byte)requestedInOut;
        }

        private static BizInfo LocalDecodeIBizType(Type iBizType, RequestedInOut requestedInOut)
        {
            if (!iBizType.IsInterface)
                throw new InvalidOperationException("The provided generic type for this action must be an interface, but was " + iBizType.Name + ".");
            var actionInterface = iBizType.GetInterfaces().FirstOrDefault();
            if (actionInterface == null || !actionInterface.IsGenericType)
                throw new InvalidOperationException("The interface must inherit from one of the IGenericActions interfaces.");
            var genericInterfacePart = actionInterface.GetGenericTypeDefinition();
            if (!ServiceBuilderLookup.ServiceLookup.ContainsKey(genericInterfacePart))
                throw new InvalidOperationException("The inherited interface '" + iBizType.Name +
                                                    "' was not one of the possible IGenericAction interfaces.");

            var serviceBaseInfo = ServiceBuilderLookup.ServiceLookup[genericInterfacePart];
            //now check things before we proceed
            CheckRequestFitsType(iBizType, requestedInOut, serviceBaseInfo);

            return new BizInfo(iBizType, actionInterface, serviceBaseInfo);
        }

        private static void CheckRequestFitsType(Type iBizType, RequestedInOut requestedInOut, ServiceBuilderLookup serviceBaseInfo)
        {

            //with async transactions the methods can be a mix of sync or async, so we mask to just check the non-async part
            var flagsMask = requestedInOut.HasFlag(RequestedInOut.OptionalAsync)
                ? RequestedInOut.NonAsyncFlagsMask
                : RequestedInOut.AllFlagsMask;

            if ((requestedInOut & flagsMask) == (serviceBaseInfo.TypeOfService & flagsMask))
                return;

            if (requestedInOut.HasFlag(RequestedInOut.InOrInOut))
            {
                if (!serviceBaseInfo.TypeOfService.HasFlag(RequestedInOut.In))
                    throw new InvalidOperationException("The action with interface of " + iBizType.Name + " does not have an input, but you called it with a method that needs an input.");

                //otherwise InOrInOut passes this test 
                return;
            }

            //Else it is another error
            throw new InvalidOperationException(
                string.Format(
                    (string) "Your call of {0} needed '{1}' but the Business class had a different setup of '{2}'",
                    (object) iBizType.Name, (object) requestedInOut, (object) serviceBaseInfo.TypeOfService));
        }

    }
}
