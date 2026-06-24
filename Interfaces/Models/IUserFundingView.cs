using Azure.Data.Tables;
using System;

namespace PDS.ViewYourFunding.Data.Interfaces.Models
{
    /// <summary>
    /// An interface representing visit information of users to funding statements.
    /// </summary>
    public interface IUserFundingView : ITableEntity
    {
        /// <summary>
        /// Gets or sets the datetime when the user visited the funding statement.
        /// </summary>
        public DateTime ViewedAt { get; set; }
    }
}