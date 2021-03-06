using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.PreProcessingExtensions.Selections;
using HotChocolate;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Selections;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    [TestClass]
    public class ParamsContextSelectionTests : GraphQLTestBase
    {
        public ParamsContextSelectionTests() : base(new GraphQLTestServerFactory())
        {
        }

        [TestMethod]
        public async Task TestParamsContextCursorSelectTotalCountCursorPaging()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // Validate TotalCount Is Specified!
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersCursorPaginated(first:2) {
                    totalCount
                    nodes {
                        name
                        id
                    }
                }
            }");

            // assert
            var queryKey = "starWarsCharactersCursorPaginated";
            var paramsContext = server.GetParamsContext(queryKey);
            Assert.IsNotNull(paramsContext?.TotalCountSelection);
            Assert.IsTrue(paramsContext.IsTotalCountRequested);

            // Validate TotalCount is Not Specified!
            result = await server.PostQueryAsync(@"{
                starWarsCharactersCursorPaginated(first:2) {
                    nodes {
                        id
                    }
                }
            }");

            // assert
            paramsContext = server.GetParamsContext(queryKey);
            Assert.IsNull(paramsContext.TotalCountSelection);
            Assert.IsFalse(paramsContext.IsTotalCountRequested);
        }

        [TestMethod]
        public async Task TestParamsContextCursorSelectTotalCountOffsetPaging()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // Validate TotalCount Is Specified!
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersOffsetPaginated(skip: 2, take: 2) {
                    totalCount
                    items {
                        name
                        id
                    }
                }
            }");

            // assert
            var queryKey = "starWarsCharactersOffsetPaginated";
            var paramsContext = server.GetParamsContext(queryKey);
            Assert.IsNotNull(paramsContext?.TotalCountSelection);
            Assert.IsTrue(paramsContext.IsTotalCountRequested);

            // Validate TotalCount is Not Specified!
            result = await server.PostQueryAsync(@"{
                starWarsCharactersOffsetPaginated(skip: 2, take: 2) {
                    items {
                        id
                    }
                }
            }");

            // assert
            paramsContext = server.GetParamsContext(queryKey);
            Assert.IsNull(paramsContext.TotalCountSelection);
            Assert.IsFalse(paramsContext.IsTotalCountRequested);
        }



        //TODO: Add a few more tests for Cursor Paging with Edges, Offset Paging, and No Paging...
        [TestMethod]
        public async Task TestParamsContextSelectionsWithCursorPagingNodes()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersCursorPaginated(first:2) {
                    nodes {
                        id
                        name
                    }
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var queryKey = "starWarsCharactersCursorPaginated";
            var paramsContext = server.GetParamsContext(queryKey);

            var selectionNames = paramsContext?.AllSelectionNames;
            Assert.IsNotNull(selectionNames);

            Assert.AreEqual(selectionNames.Count, 2);
            Assert.AreEqual("id", selectionNames.FirstOrDefault());
            Assert.AreEqual("name", selectionNames.LastOrDefault());
        }

        [TestMethod]
        public async Task TestParamsContextSelectionsWithCursorPagingEdges()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersCursorPaginated(first:2) {
                    edges {
                        node {
                            id
                            name
                        }
                    }
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var queryKey = "starWarsCharactersCursorPaginated";
            var paramsContext = server.GetParamsContext(queryKey);

            var selectionNames = paramsContext?.AllSelectionNames;
            Assert.IsNotNull(selectionNames);

            Assert.AreEqual(selectionNames.Count, 2);
            Assert.AreEqual("id", selectionNames.FirstOrDefault());
            Assert.AreEqual("name", selectionNames.LastOrDefault());
        }

        [TestMethod]
        public async Task TestParamsContextSelectionsWithOffsetPaging()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersOffsetPaginated(skip:2, take:2) {
                    items {
                        id
                        name
                    }
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var queryKey = "starWarsCharactersOffsetPaginated";
            var paramsContext = server.GetParamsContext(queryKey);

            var selectionNames = paramsContext?.AllSelectionNames;
            Assert.IsNotNull(selectionNames);

            Assert.AreEqual(selectionNames.Count, 2);
            Assert.AreEqual("id", selectionNames.FirstOrDefault());
            Assert.AreEqual("name", selectionNames.LastOrDefault());
        }


    }
}
