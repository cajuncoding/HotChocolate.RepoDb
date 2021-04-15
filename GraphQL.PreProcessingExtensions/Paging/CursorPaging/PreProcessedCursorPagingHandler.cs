﻿# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Utilities;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public class PreProcessedCursorPagingHandler<TEntity> : CursorPagingHandler
    {
        public PagingOptions PagingOptions { get; protected set; }

        public PreProcessedCursorPagingHandler(PagingOptions pagingOptions)
            : base(pagingOptions)
        {
            this.PagingOptions = pagingOptions;
        }

        /// <summary>
        /// Provides a No-Op (no post-processing) impelementation so that results are unchanged from
        /// what was returned by the Resolver (or lower layers); assumes that the results are correctly
        /// pre-processed as a IPreProcessedPagedResult
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async ValueTask<Connection> SliceAsync(IResolverContext context, object source, CursorPagingArguments arguments)
        #pragma warning restore CS1998 
        {
            //If Appropriate we handle the values here to ensure that no post-processing is done other than
            //  correctly mapping the results into a GraphQL Connection as Edges with Cursors...
            if (source is IPreProcessedCursorSlice<TEntity> pagedResults)
            {
                bool includeTotalCount = this.PagingOptions.IncludeTotalCount ?? false;
                //TODO: Optimize this to only require only if the query actually requested it (for both Offset & Cursor Paging)!
                if (includeTotalCount && pagedResults.TotalCount == null)
                    throw new InvalidOperationException($"Total Count is required by configuration, but was not provided with the results [{this.GetType().GetTypeName()}] by resolvers pre-processing logic; TotalCount is null.");

                int? totalCount = pagedResults.TotalCount;

                //Ensure we are null safe and return a valid empty list by default.
                IReadOnlyList<IndexEdge<TEntity>> selectedEdges = 
                    pagedResults?.ToEdgeResults().ToList() ?? new List<IndexEdge<TEntity>>(); ;

                IndexEdge<TEntity>? firstEdge = selectedEdges.FirstOrDefault();
                IndexEdge<TEntity>? lastEdge = selectedEdges.LastOrDefault();

                var connectionPageInfo = new ConnectionPageInfo(
                    hasNextPage: pagedResults?.HasNextPage ?? false,
                    hasPreviousPage: pagedResults?.HasPreviousPage ?? false,
                    startCursor: firstEdge?.Cursor,
                    endCursor: lastEdge?.Cursor,
                    totalCount: totalCount ?? 0
                );

                var graphQLConnection = new Connection<TEntity>(
                    selectedEdges,
                    connectionPageInfo,
                    ct => new ValueTask<int>(connectionPageInfo.TotalCount ?? 0)
                );

                return graphQLConnection;
            }

            throw new GraphQLException($"[{nameof(PreProcessedCursorPagingHandler<TEntity>)}] cannot handle the specified data source of type [{source.GetType().Name}].");
        }


    }
}
