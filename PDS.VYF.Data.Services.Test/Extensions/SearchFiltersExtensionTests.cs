using FluentAssertions;
using PDS.VYF.Data.Services.Extensions;
using System.Text;

namespace PDS.VYF.Data.Services.Tests.Extensions
{
    /// <summary>
    /// The Search Filters Extension Tests.
    /// </summary>
    [TestClass]
    [TestCategory("Unit")]
    public class SearchFiltersExtensionTests
    {
        /// <summary>
        /// Adds the search in filter should add correct filter when items is null.
        /// </summary>
        [TestMethod]
        public void AddSearchInFilter_ShouldAddCorrectFilter_WhenItemsIsNull()
        {
            // Arrange
            var filter = new StringBuilder();
            List<string>? items = null;
            string columnName = "TestColumn";

            // Act
            filter.AddSearchInFilter(items, columnName);

            // Assert
            filter.ToString().Should().BeEmpty();
        }

        /// <summary>
        /// Adds the search in filter should add correct filter when items is empty.
        /// </summary>
        [TestMethod]
        public void AddSearchInFilter_ShouldAddCorrectFilter_WhenItemsIsEmpty()
        {
            // Arrange
            var filter = new StringBuilder();
            var items = new List<string>();
            string columnName = "TestColumn";

            // Act
            filter.AddSearchInFilter(items, columnName);

            // Assert
            filter.ToString().Should().BeEmpty();
        }

        /// <summary>
        /// Adds the search in filter should add correct filter when items has one element.
        /// </summary>
        [TestMethod]
        public void AddSearchInFilter_ShouldAddCorrectFilter_WhenItemsHasOneElement()
        {
            // Arrange
            var filter = new StringBuilder();
            var items = new List<string> { "Item1" };
            string columnName = "TestColumn";

            // Act
            filter.AddSearchInFilter(items, columnName);

            // Assert
            filter.ToString().Should().Be("TestColumn eq 'Item1'");
        }

        /// <summary>
        /// Adds the search in filter should add correct filter when items has one element.
        /// </summary>
        [TestMethod]
        public void AddSearchInFilter_ShouldAddCorrectFilter_WhenItemsHasOneElementAndIsCollectionColumn()
        {
            // Arrange
            var filter = new StringBuilder();
            var items = new List<string> { "Item1" };
            string columnName = "TestColumn";
            bool isCollectionColumn = true;

            // Act
            filter.AddSearchInFilter(items, columnName, isCollectionColumn);

            // Assert
            filter.ToString().Should().Be("TestColumn/ any(g: g eq 'Item1')");
        }

        /// <summary>
        /// Adds the search in filter should add correct filter when items has multiple elements and is collection column.
        /// </summary>
        [TestMethod]
        public void AddSearchInFilter_ShouldAddCorrectFilter_WhenItemsHasMultipleElementsAndIsCollectionColumn()
        {
            // Arrange
            var filter = new StringBuilder();
            var items = new List<string> { "Item1", "Item2" };
            string columnName = "TestColumn";
            bool isCollectionColumn = true;

            // Act
            filter.AddSearchInFilter(items, columnName, isCollectionColumn);

            // Assert
            filter.ToString().Should().Be("TestColumn/ any(g: search.in(g, 'Item1, Item2'))");
        }

        /// <summary>
        /// Adds the search in filter should add correct filter when items has multiple elements and is not collection column.
        /// </summary>
        [TestMethod]
        public void AddSearchInFilter_ShouldAddCorrectFilter_WhenItemsHasMultipleElementsAndIsNotCollectionColumn()
        {
            // Arrange
            var filter = new StringBuilder();
            var items = new List<string> { "Item1", "Item2" };
            string columnName = "TestColumn";

            // Act
            filter.AddSearchInFilter(items, columnName);

            // Assert
            filter.ToString().Should().Be("search.in(TestColumn, 'Item1, Item2')");
        }

        /// <summary>
        /// Appends the eq filter should add correct filter when date value is not null.
        /// </summary>
        [TestMethod]
        public void AppendEqFilter_ShouldAddCorrectFilter_WhenDateValueIsNotNull()
        {
            // Arrange
            var filter = new StringBuilder();
            DateTime? dateValue = new DateTime(2023, 1, 1);
            string columnName = "TestDateColumn";

            // Act
            filter.AppendEqFilter(dateValue, columnName);

            // Assert
            filter.ToString().Should().Be("TestDateColumn eq 2023-01-01T00:00:00.0000000Z");
        }

        /// <summary>
        /// Appends the eq filter should add correct filter when string value is not null or empty.
        /// </summary>
        [TestMethod]
        public void AppendEqFilter_ShouldAddCorrectFilter_WhenStringValueIsNotNullOrEmpty()
        {
            // Arrange
            var filter = new StringBuilder();
            string stringValue = "TestValue";
            string columnName = "TestStringColumn";

            // Act
            filter.AppendEqFilter(stringValue, columnName);

            // Assert
            filter.ToString().Should().Be("TestStringColumn eq 'TestValue'");
        }

        /// <summary>
        /// Appends the eq filter should add correct filter when bool value is not null.
        /// </summary>
        [TestMethod]
        public void AppendEqFilter_ShouldAddCorrectFilter_WhenBoolValueIsNotNull()
        {
            // Arrange
            var filter = new StringBuilder();
            bool? boolValue = true;
            string columnName = "TestBoolColumn";

            // Act
            filter.AppendEqFilter(boolValue, columnName);

            // Assert
            filter.ToString().Should().Be("TestBoolColumn eq true");
        }

        /// <summary>
        /// Appends the and if required should add and when filter is not empty and does not end with and.
        /// </summary>
        [TestMethod]
        public void AppendAndIfRequired_ShouldAddAnd_WhenFilterIsNotEmptyAndDoesNotEndWithAnd()
        {
            // Arrange
            var filter = new StringBuilder("ExistingFilter");

            // Act
            filter.AppendAndIfRequired();

            // Assert
            filter.ToString().Should().Be("ExistingFilter and ");
        }

        /// <summary>
        /// Appends the and if required should not add and when filter is empty.
        /// </summary>
        [TestMethod]
        public void AppendAndIfRequired_ShouldNotAddAnd_WhenFilterIsEmpty()
        {
            // Arrange
            var filter = new StringBuilder();

            // Act
            filter.AppendAndIfRequired();

            // Assert
            filter.ToString().Should().BeEmpty();
        }

        /// <summary>
        /// Appends the and if required should not add and when filter ends with and.
        /// </summary>
        [TestMethod]
        public void AppendAndIfRequired_ShouldNotAddAnd_WhenFilterEndsWithAnd()
        {
            // Arrange
            var filter = new StringBuilder("ExistingFilter and ");

            // Act
            filter.AppendAndIfRequired();

            // Assert
            filter.ToString().Should().Be("ExistingFilter and ");
        }
    }
}
