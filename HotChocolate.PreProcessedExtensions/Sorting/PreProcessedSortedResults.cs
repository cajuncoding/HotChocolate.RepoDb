﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HotChocolate.PreProcessedExtensions.Sorting
{
    /// <summary>
    /// Payload response container for HotChocolate pipeline that contains all deatils needed for pre-processed
    /// results to integrate with the existing OOTB paging pipeline, but with results that are already completely
    /// processed by the Resolver (or lower layer).
    /// 
    /// We must inherit from List<TEntity> to ensure that HotChocolate can correctly Infer the proper Schema from 
    /// the base TEntity generic type; simply providing IEnumerable<TEntity> isn't enough.  
    /// As a real List<> the PureCode Schema inference works as expected!
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PreProcessedSortedResults<TEntity> : List<TEntity>, IEnumerable<TEntity>, IAmPreProcessedResult
        where TEntity : class
    {
        public PreProcessedSortedResults(IEnumerable<TEntity> results)
        {
            if(results != null)
                this.AddRange(results);
        }

    }

    public static class PreprocessedSortedResultsExtnsions
    {
        /// <summary>
        /// Conveniene method to Wrap the current Enumeable Result Items as a PreProcessedSortResults; to eliminate
        /// cermenonial code for new'ing up the results.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="enumerableItems"></param>
        /// <returns></returns>
        public static PreProcessedSortedResults<TEntity> AsPreProcessedSortResults<TEntity>(this IEnumerable<TEntity> enumerableItems)
            where TEntity : class
        {
            if (enumerableItems == null) 
                return null;

            return new PreProcessedSortedResults<TEntity>(enumerableItems);
        }
    }
}
