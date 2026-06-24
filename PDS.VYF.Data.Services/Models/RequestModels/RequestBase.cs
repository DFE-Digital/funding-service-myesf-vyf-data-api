namespace PDS.VYF.Data.Services.Models.RequestModels
{
    /// <summary>
    /// The base class for request.
    /// </summary>
    /// <typeparam name="TResponseModel">The type of the response model.</typeparam>
    public class RequestBase<TResponseModel>
        where TResponseModel : class, new()
    {
        /// <summary>
        /// Gets or sets the funding stream period.
        /// </summary>
        /// <value>
        /// The funding stream period.
        /// </value>
        public string? FundingStreamCode { get; set; } = null;

        /// <summary>
        /// Gets or sets the list of funding stream periods.
        /// </summary>
        public List<string>? FundingStreamPeriods { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request has to be the latest funding.
        /// </summary>
        public bool HasToBeLatestFunding { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of IDs.
        /// </summary>
        public List<string>? ListOfIds { get; set; }

        /// <summary>
        /// Gets or sets the list of UKPRNs.
        /// </summary>
        public List<string>? ListOfUKPRNs { get; set; }

        /// <summary>
        /// Gets the select fields as a comma-separated string.
        /// </summary>
        public string? Select => this.SelectFields?.Count > 0 ? string.Join(", ", this.SelectFields) : null;

        /// <summary>
        /// Gets or sets the set of select fields.
        /// </summary>
        public HashSet<string>? SelectFields { get; set; }

        /// <summary>
        /// Sets the select fields based on the provided selector.
        /// </summary>
        /// <typeparam name="TSelector">The type of the selector.</typeparam>
        /// <param name="selector">The selector function.</param>
        public void SetSelectFields<TSelector>(Func<TResponseModel, TSelector> selector)
        {
            this.SelectFields = typeof(TSelector)
                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .Select(a => a.Name)
                    .ToHashSet();
        }

        /// <summary>
        /// Tries to add the select fields based on the provided selector.
        /// </summary>
        /// <typeparam name="TSelector">The type of the selector.</typeparam>
        /// <param name="selector">The selector function.</param>
        /// <returns>True if the select fields were added successfully, false otherwise.</returns>
        public bool TryAddSelectFields<TSelector>(Func<TResponseModel, TSelector> selector)
        {
            this.SelectFields ??= new HashSet<string>();

            var newSelectFields = typeof(TSelector)
                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .Where(a => !this.SelectFields.Contains(a.Name))
                    .Select(a => a.Name);

            foreach (var item in newSelectFields)
            {
                this.SelectFields.Add(item);
            }

            return true;
        }

        /// <summary>
        /// Sets the select fields excluding the fields specified by the provided selector.
        /// </summary>
        /// <typeparam name="TSelector">The type of the selector.</typeparam>
        /// <param name="selector">The selector function.</param>
        public void SetSelectFieldsExcept<TSelector>(Func<TResponseModel, TSelector> selector)
        {
            var excludedFields = typeof(TSelector)
                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .Select(a => a.Name)
                    .ToHashSet();

            this.SelectFields = typeof(TResponseModel)
                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .Where(prop => !excludedFields.Contains(prop.Name))
                    .Select(a => a.Name)
                    .ToHashSet();
        }

        /// <summary>
        /// Removes the fields specified by the provided selector from the select fields.
        /// </summary>
        /// <typeparam name="TSelector">The type of the selector.</typeparam>
        /// <param name="selector">The selector function.</param>
        public void RemoveFields<TSelector>(Func<TResponseModel, TSelector> selector)
        {
            if (this.SelectFields?.Count > 0)
            {
                var removableFields = typeof(TSelector)
                        .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                        .Select(a => a.Name)
                        .ToHashSet();

                this.SelectFields.RemoveWhere(selectField => removableFields.Any(removableField => selectField == removableField));
            }
        }
    }
}