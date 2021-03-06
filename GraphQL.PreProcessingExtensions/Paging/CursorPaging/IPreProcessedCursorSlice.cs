﻿using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public interface IPreProcessedCursorSlice<TEntity> : IList<TEntity>, IHavePreProcessedPagingInfo, IAmPreProcessedResult
    {
        public IEnumerable<IndexEdge<TEntity>> ToEdgeResults();
    }
}
