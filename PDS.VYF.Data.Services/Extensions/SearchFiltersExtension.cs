namespace PDS.VYF.Data.Services.Extensions
{
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// The class of SearchFiltersExtension.
    /// </summary>
    public static class SearchFiltersExtension
    {
        /// <summary>
        /// Adds the search in filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="items">The items.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="isCollectionColumn">if set to <c>true</c> [is collection column].</param>
        /// <returns>The same object.</returns>
        public static StringBuilder AddSearchInFilter(this StringBuilder filter, List<string>? items, string columnName, bool isCollectionColumn = false)
        {
            var filterText = items switch
            {
                null or { Count: 0 } => string.Empty,
                { Count: 1 } when isCollectionColumn && !items[0].Contains(",") => $"{columnName}/ any(g: g eq '{items[0]}')",
                { Count: 1 } when !isCollectionColumn && !items[0].Contains(",") => $"{columnName} eq '{items[0]}'",
                { Count: 1 } when isCollectionColumn && items[0].Contains(",") => $"{columnName}/ any(g: search.in(g, '{string.Join(", ", items)}'))",
                { Count: 1 } when !isCollectionColumn && items[0].Contains(",") => $"search.in({columnName}, '{string.Join(", ", items)}')",
                _ when isCollectionColumn => $"{columnName}/ any(g: search.in(g, '{string.Join(", ", items)}'))",
                _ => $"search.in({columnName}, '{string.Join(", ", items)}')"
            };

            if (!string.IsNullOrWhiteSpace(filterText))
            {
                filter.AppendAndIfRequired();
                filter.Append(filterText);
            }

            return filter;
        }

        /// <summary>
        /// Appends the eq filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="dateValue">The date value.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The same object.</returns>
        public static StringBuilder AppendEqFilter(this StringBuilder filter, DateTime? dateValue, string columnName)
        {
            if (dateValue is not null)
            {
                filter.AppendAndIfRequired();
                filter.Append($"{columnName} eq {dateValue?.ToString("o", CultureInfo.InvariantCulture)}Z");
            }

            return filter;
        }

        /// <summary>
        /// Appends the eq filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="stringValue">The string value.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The same object.</returns>
        public static StringBuilder AppendEqFilter(this StringBuilder filter, string? stringValue, string columnName)
        {
            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                filter.AppendAndIfRequired();
                filter.Append($"{columnName} eq '{stringValue}'");
            }

            return filter;
        }

        /// <summary>
        /// Appends the eq filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="boolValue">The bool value.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The same object.</returns>
        public static StringBuilder AppendEqFilter(this StringBuilder filter, bool? boolValue, string columnName)
        {
            if (boolValue != null)
            {
                filter.AppendAndIfRequired();
                filter.Append($"{columnName} eq {boolValue.Value.ToString().ToLower()}");
            }

            return filter;
        }

        /// <summary>
        /// Appends the and if required.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <returns>The same object.</returns>
        public static StringBuilder AppendAndIfRequired(this StringBuilder stringBuilder)
        {
            if (!string.IsNullOrWhiteSpace(stringBuilder.ToString()) && !stringBuilder.ToString().EndsWith(" and "))
            {
                stringBuilder.Append(" and ");
            }

            return stringBuilder;
        }
    }
}
