﻿using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.RepoDb.SqlServer.Reflection;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.CursorPagination
{
    public static class BaseRepositoryOffsetPaginationCustomExtensions
    {
        /// <summary>
        /// Base Repository extension for Offset Paginated Batch Query capability.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDbConnection"></typeparam>
        /// <param name="baseRepo">Extends the RepoDb BaseRepository abstraction</param>
        /// <param name="afterCursor"></param>
        /// <param name="firstTake"></param>
        /// <param name="beforeCursor"></param>
        /// <param name="lastTake"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchOffsetPagingQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> where,
            int? page = null,
            int? rowsPerBatch = null,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        where TDbConnection : DbConnection
        {
            return await baseRepo.GraphQLBatchOffsetPagingQueryAsync<TEntity, TDbConnection>(
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where != null ? QueryGroup.Parse<TEntity>(where) : (QueryGroup)null,
                    hints: hints,
                    fields: fields,
                    tableName: tableName,
                    transaction: transaction,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Base Repository extension for Offset Paginated Batch Query capability.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDbConnection"></typeparam>
        /// <param name="baseRepo">Extends the RepoDb BaseRepository abstraction</param>
        /// <param name="afterCursor"></param>
        /// <param name="firstTake"></param>
        /// <param name="beforeCursor"></param>
        /// <param name="lastTake"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchOffsetPagingQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            int? page = null,
            int? rowsPerBatch = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        where TDbConnection : DbConnection
        {
            //Below Logic mirrors that of RepoDb Source for managing the Connection (PerInstance or PerCall)!
            var connection = (DbConnection)(transaction?.Connection ?? baseRepo.CreateConnection());

            try
            {
                var cursorPageResult = await connection.GraphQLBatchOffsetPagingQueryAsync<TEntity>(
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    hints: hints,
                    fields: fields,
                    tableName: tableName,
                    transaction: transaction,
                    cancellationToken: cancellationToken
                );

                return cursorPageResult;
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                baseRepo.DisposeConnectionForPerCallExtension(connection, transaction);
            }
        }

        /// <summary>
        /// Base DbConnection (SqlConnection) extension for Offset Batch Paginated Batch Query capability.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends DbConnection directly</param>
        /// <param name="afterCursor"></param>
        /// <param name="firstTake"></param>
        /// <param name="beforeCursor"></param>
        /// <param name="lastTake"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchOffsetPagingQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> where,
            int? page = null,
            int? rowsPerBatch = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        {
            return await dbConnection.GraphQLBatchOffsetPagingQueryAsync<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : (QueryGroup)null,
                hints: hints,
                fields: fields,
                transaction: transaction,
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// Base Repository extension for Offset Paginated Batch Query capability.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends DbConnection directly</param>
        /// <param name="afterCursor"></param>
        /// <param name="firstTake"></param>
        /// <param name="beforeCursor"></param>
        /// <param name="lastTake"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchOffsetPagingQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null, 
            int? page = null, 
            int? rowsPerBatch = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        {
            if (orderBy == null)
                throw new ArgumentNullException("A sort order must be specified to provide valid paging results.", nameof(orderBy));

            var dbTableName = string.IsNullOrWhiteSpace(tableName)
                                ? ClassMappedNameCache.Get<TEntity>()
                                : tableName;

            //Ensure we have default fields; default is to include All Fields...
            var selectFields = fields?.Any() == true
                ? fields
                : FieldCache.Get<TEntity>();

            //Retrieve only the select fields that are valid for the Database query!
            //NOTE: We guard against duplicate values as a convenience.
            var validSelectFields = await dbConnection.GetValidatedDbFields(dbTableName, selectFields.Distinct());

            //Dynamically hanlde RepoDb where filters (QueryGroup)...
            object whereParams = where != null
                ? RepoDbQueryGroupProxy.GetMappedParamsObject<TEntity>(where)
                : null;

            var batchResults = await dbConnection.BatchQueryAsync<TEntity>(
                tableName: tableName,
                page: page ?? 0,
                rowsPerBatch: rowsPerBatch ?? int.MaxValue,
                fields: validSelectFields,
                orderBy: orderBy,
                where: whereParams,
                cancellationToken: cancellationToken,
                transaction: transaction
            );

            //TODO: Implement Logic to determine HasNextPage, HasPreviousPage, and get Total Count or Not!
            var offsetPageResults = new OffsetPageResults<TEntity>(batchResults, true, true, 0);
            return offsetPageResults;
        }

        /// <summary>
        /// CLONED from RepoDb source code that is marked as 'internal' but needed to handle in a consistent way!
        /// Disposes an <see cref="IDbConnection"/> object if there is no <see cref="IDbTransaction"/> object connected
        /// and if the current <see cref="ConnectionPersistency"/> value is <see cref="ConnectionPersistency.PerCall"/>.
        /// </summary>
        /// <param name="connection">The instance of <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The instance of <see cref="IDbTransaction"/> object.</param>
        private static void DisposeConnectionForPerCallExtension<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IDbConnection connection,
            IDbTransaction transaction = null
        )
        where TEntity : class
        where TDbConnection : DbConnection
        {
            if (baseRepo.ConnectionPersistency == ConnectionPersistency.PerCall)
            {
                if (transaction == null)
                {
                    connection?.Dispose();
                }
            }
        }
  
    }

}