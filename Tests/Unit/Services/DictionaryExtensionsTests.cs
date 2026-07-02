using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.Data.Services.Extensions;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.Tests.Unit.Services
{
    /// <summary>
    /// The Dictionary Extensions tests.
    /// </summary>
    [TestClass]
    public class DictionaryExtensionsTests
    {
        [TestMethod, TestCategory("Unit")]
        public void Merge_Tests()
        {
            // Arrange
            var dictionary1 = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value3" },
                { "key4", "value4" }
            };

            var dictionary2 = new Dictionary<string, string>
            {
                { "key1", "value5" },
                { "key2", "value6" },
                { "key5", "value7" },
                { "key6", "value8" }
            };

            // Act
            var result = DictionaryExtensions.Merge(new List<Dictionary<string, string>> { dictionary1, dictionary2 });

            // Assert
            result.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value3" },
                { "key4", "value4" },
                { "key5", "value7" },
                { "key6", "value8" }
            });
        }
    }
}
