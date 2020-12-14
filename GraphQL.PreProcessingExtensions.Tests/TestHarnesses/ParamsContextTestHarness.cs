﻿using System;
using System.Collections.Generic;
using System.Text;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using OffsetPagingArguments = HotChocolate.PreProcessingExtensions.Pagination.OffsetPagingArguments;

namespace GraphQL.PreProcessingExtensions.Tests
{
    public class ParamsContextTestHarness : IParamsContext
    {
        private IParamsContext _internalParamsContext;

        public ParamsContextTestHarness(IParamsContext paramsContext)
        {
            _internalParamsContext = paramsContext;

            //BBernard
            //THIS will force initialization of all data for Test cases
            //  to then have access to even if out of scope, since we have our own
            //  references here!
            this.ResolverContext = paramsContext.ResolverContext;
            this.AllSelectionFields = paramsContext.AllSelectionFields;
            this.AllSelectionNames = paramsContext.AllSelectionNames;
            this.SelectionDependencies = paramsContext.SelectionDependencies;
            this.SortArgs = paramsContext.SortArgs;
            this.PagingArgs = paramsContext.PagingArgs;
            this.CursorPagingArgs = paramsContext.CursorPagingArgs;
            this.OffsetPagingArgs = paramsContext.OffsetPagingArgs;
        }

        public IResolverContext ResolverContext { get; }
        public IReadOnlyList<IPreProcessingSelection> AllSelectionFields { get; }
        public IReadOnlyList<string> AllSelectionNames { get; }
        public IReadOnlyList<PreProcessingDependencyLink> SelectionDependencies { get; }
        public IReadOnlyList<ISortOrderField> SortArgs { get; }
        public CursorPagingArguments PagingArgs { get; }
        public CursorPagingArguments CursorPagingArgs { get; }
        public OffsetPagingArguments OffsetPagingArgs { get; }

        public IReadOnlyList<IPreProcessingSelection> GetSelectionFieldsFor<TObjectType>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSelectionMappedNames(SelectionNameFlags flags = SelectionNameFlags.DependencyNames)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSelectionMappedNamesFor<TObjectType>(SelectionNameFlags flags = SelectionNameFlags.DependencyNames)
        {
            throw new NotImplementedException();
        }

    }
}