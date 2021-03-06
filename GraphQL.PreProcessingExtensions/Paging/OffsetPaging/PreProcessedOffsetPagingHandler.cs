﻿# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.PreProcessingExtensions.Paging;
using HotChocolate.Utilities;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public class PreProcessedOffsetPagingHandler<TEntity> : OffsetPagingHandler
    {
        public PagingOptions PagingOptions { get; protected set; }

        public PreProcessedOffsetPagingHandler(PagingOptions pagingOptions)
            : base(pagingOptions)
        {
            this.PagingOptions = pagingOptions.ClonePagingOptions();
        }

        /// <summary>
        /// Provides a No-Op (no post-processing) implementation so that results are unchanged from
        /// what was returned by the Resolver (or lower layers); assumes that the results are correctly
        /// pre-processed as a IPreProcessedPagedResult
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected override ValueTask<CollectionSegment> SliceAsync(IResolverContext context, object source, OffsetPagingArguments arguments)
        {
            //If Appropriate we handle the values here to ensure that no post-processing is done other than
            //  correctly mapping the results into a GraphQL Collection Segment with appropriate Paging Details...
            if (source is IPreProcessedOffsetPageResults<TEntity> pagedResults)
            {
                bool includeTotalCountEnabled = this.PagingOptions.IncludeTotalCount ?? PagingDefaults.IncludeTotalCount;
                var graphQLParamsContext = new GraphQLParamsContext(context);

                //Optimized to only require TotalCount value if the query actually requested it!
                if (includeTotalCountEnabled && graphQLParamsContext.IsTotalCountRequested && pagedResults.TotalCount == null)
                    throw new InvalidOperationException($"Total Count is requested in the query, but was not provided with the results [{this.GetType().GetTypeName()}] from the resolvers pre-processing logic; TotalCount is null.");

                int? totalCount = pagedResults.TotalCount;

                //Ensure we are null safe and return a valid empty list by default.
                var segmentResults = pagedResults?.ToList() ?? new List<TEntity>();

                var collectionSegmentInfo = new CollectionSegmentInfo(
                    hasNextPage: pagedResults?.HasNextPage ?? false,
                    hasPreviousPage: pagedResults?.HasPreviousPage ?? false
                );

                var graphQLConnection = new CollectionSegment(
                    (IReadOnlyCollection<object>)segmentResults,
                    collectionSegmentInfo,
                    ct => new ValueTask<int>(totalCount ?? throw new InvalidOperationException())
                );

                return new ValueTask<CollectionSegment>(graphQLConnection);
            }

            throw new GraphQLException($"[{nameof(PreProcessedOffsetPagingHandler<TEntity>)}] cannot handle the specified data source of type [{source.GetType().Name}].");
        }
    }
}
