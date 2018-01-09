// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using GenericActions;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal.DtoAccessors;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericBizRunner.Internal
{
    /// <summary>
    /// This class creates an access to the dto functions. I do this because it means I don't have to know all the types at run time.
    /// There are five possible copy types:
    /// 1) Direct copy, i.e. the service level data type is the same as the biz level. Simply point to the original class
    /// 2) Input: 2a) DTO-to-Biz, 2b) DTO-to-Biz async
    /// 3) Ouput: 3a) Biz-to-DTO, 3b) Biz-to-DTO async
    /// </summary>
    internal class DtoAccessGenerator
    {
        private static readonly ConcurrentDictionary<string, DtoAccessGenerator> DtoAccessInstanceCache =
            new ConcurrentDictionary<string, DtoAccessGenerator>();

        /// <summary>
        /// This holds copier instance, or null if it is a direct copy
        /// </summary>
        private readonly dynamic _dtoAccessInstance;

        private readonly bool _isAsync;

        private DtoAccessGenerator(bool isAsync, dynamic dtoAccessInstance)
        {
            _isAsync = isAsync;
            _dtoAccessInstance = dtoAccessInstance;
        }

        private bool IsDirectCopy => _dtoAccessInstance == null;

        public static DtoAccessGenerator BuildCopier(Type fromType, Type toType, bool toBiz, bool asyncAllowed,
            IGenericBizRunnerConfig config)
        {
            if (fromType == toType)
                //simple case - just copy the data as is so do not need to create a copier
                return new DtoAccessGenerator(false, null);

            //else its a complex copier, so we create it
            if (!config.TurnOffCaching)
                return DtoAccessInstanceCache.GetOrAdd(FormCacheKey(fromType, toType, toBiz, asyncAllowed),
                    key => ActivateCopierInstance(fromType, toType, toBiz, asyncAllowed));

            return ActivateCopierInstance(fromType, toType, toBiz, asyncAllowed);
        }

        //---------------------------------------------------
        //InBiz methods

        public TBiz DoCopyToBiz<TBiz>(DbContext db, IMapper mapper, object inputDto)
        {
            if (IsDirectCopy) return (TBiz) inputDto;

            var result = (TBiz) _dtoAccessInstance.CopyToBiz(db, mapper, inputDto);
            return result;
        }

        public async Task<TBiz> DoCopyToBizAsync<TBiz>(DbContext db, IMapper mapper, object inputDto)
        {
            //NOTE: Async business methods can use either sync or async dto, so we need to handle both
            if (!_isAsync) return DoCopyToBiz<TBiz>(db, mapper, inputDto);

            var result = await ((Task<TBiz>) _dtoAccessInstance.CopyToBizAsync(db, mapper, inputDto)).ConfigureAwait(false);
            return result;
        }

        public void SetupSecondaryDataIfRequired(DbContext db, object inputDto)
        {
            if (!IsDirectCopy)
                //we need to call SetupSecondaryOutputData
                _dtoAccessInstance.SetupSecondaryData(db, inputDto);
        }

        public async Task SetupSecondaryDataIfRequiredAsync(DbContext db, object inputDto)
        {
            //NOTE: Async business methods can use either sync or async dto, so we need to handle both
            if (!IsDirectCopy)
            {
                //we need to call SetupSecondaryOutputData
                if (_isAsync)
                    await _dtoAccessInstance.SetupSecondaryDataAsync(db, inputDto).ConfigureAwait(false);
                else
                    _dtoAccessInstance.SetupSecondaryData(db, inputDto);
            }
        }

        public T CreateDataWithPossibleSetup<T>(DbContext db) where T : class, new()
        {
            var result = new T();
            SetupSecondaryDataIfRequired(db, result);
            return result;
        }

        public async Task<T> CreateDataWithPossibleSetupAsync<T>(DbContext db) where T : class, new()
        {
            var result = new T();
            await SetupSecondaryDataIfRequiredAsync(db, result).ConfigureAwait(false);
            return result;
        }

        //---------------------------------------------
        //OutBiz methods


        public TDto DoCopyFromBiz<TDto>(DbContext db, IMapper mapper, object bizOutput)
        {
            if (IsDirectCopy) return (TDto) bizOutput;

            var result = (TDto) _dtoAccessInstance.CopyFromBiz(db, mapper, bizOutput);
            //we need to call SetupSecondaryOutputData
            _dtoAccessInstance.SetupSecondaryOutputData(db, result);

            return result;
        }

        public async Task<TDto> DoCopyFromBizAsync<TDto>(DbContext db, IMapper mapper, object bizOutput)
        {
            //NOTE: Async business methods can use either sync or async dto, so we need to handle both
            if (!_isAsync) return DoCopyFromBiz<TDto>(db, mapper, bizOutput);

            var result = await ((Task<TDto>) _dtoAccessInstance.CopyFromBizAsync(db, mapper, bizOutput)).ConfigureAwait(false);
            await _dtoAccessInstance.SetupSecondaryOutputDataAsync(db, result).ConfigureAwait(false);
            return result;
        }

        //---------------------------------------------------------------
        //private helpers

        /// <summary>
        /// This must form a unique key for each BuildCoper call. It is used to lookup on the cache
        /// </summary>
        private static string FormCacheKey(Type fromType, Type toType, bool toBiz, bool asyncAllowed)
        {
            return fromType.FullName + toType.FullName + toBiz + asyncAllowed;
        }

        private static DtoAccessGenerator ActivateCopierInstance(Type fromType, Type toType, bool toBiz,
            bool asyncAllowed)
        {
            var dtoType = GenericTypeofDto(fromType, toType, toBiz);
            var isAsync = dtoType.Name.Contains("Async`");
            if (isAsync && !asyncAllowed)
                throw new InvalidOperationException(
                    "You cannot use an Async version of the DTO in a non-async action.");

            var genericArguments = dtoType.GetGenericArguments();
            var copyGenericType = GetCorrectCopyType(toBiz, isAsync);
            return new DtoAccessGenerator(isAsync, CreateCopyInstance(copyGenericType, genericArguments));
        }

        private static dynamic CreateCopyInstance(Type baseGenericType, Type[] genericArguments)
        {
            var typeToCreate = baseGenericType.MakeGenericType(genericArguments);
            return Activator.CreateInstance(typeToCreate);
        }

        private static Tuple<Type, Type> GetAppropriateGenericTypesToCheckAgainst(bool toBiz)
        {
            return (toBiz)
                ? new Tuple<Type, Type>(typeof(GenericActionToBizDto<,>), typeof(GenericActionToBizDtoAsync<,>))
                : new Tuple<Type, Type>(typeof(GenericActionFromBizDto<,>), typeof(GenericActionFromBizDtoAsync<,>));
        }

        private static Type GetCorrectCopyType(bool toBiz, bool isAsync)
        {
            return (toBiz)
                ? isAsync ? typeof(CopyToBizDataAsync<,>) : typeof(CopyToBizData<,>)
                : isAsync
                    ? typeof(CopyFromBizDataAsync<,>)
                    : typeof(CopyFromBizData<,>);
        }

        private static Type GenericTypeofDto(Type fromType, Type toType, bool toBiz)
        {
            var classType = toBiz ? fromType : toType;
            var expectedGeneric = GetAppropriateGenericTypesToCheckAgainst(toBiz);
            while (classType != null && (!classType.IsGenericType ||
                                         (classType.GetGenericTypeDefinition() != expectedGeneric.Item1 &&
                                          classType.GetGenericTypeDefinition() != expectedGeneric.Item2)))
                classType = classType.BaseType;

            if (classType == null)
                throw new InvalidOperationException(
                    string.Format(
                        "Indirect copy {0} biz action. from type = {1}, to type {2}. Expected a DTO of type {3}<{4},{5}>",
                        toBiz ? "to" : "from",
                        fromType.Name, toType.Name,
                        expectedGeneric.Item1.Name.Substring(0, expectedGeneric.Item1.Name.Length - 2), //remove `2 
                        toBiz ? toType.Name : fromType.Name,
                        toBiz ? fromType.Name : toType.Name));

            return classType;
        }
    }
}