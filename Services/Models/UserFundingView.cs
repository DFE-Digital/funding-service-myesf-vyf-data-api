using Azure;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using System;

namespace PDS.ViewYourFunding.Data.Services.Models
{
    /// <summary>
    /// Table entity to store visit information of users to allocation statements.
    /// </summary>
    public class UserFundingView : IUserFundingView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserFundingView"/> class.
        /// This table entity uses user id as the partition key and funding id as the row key.
        /// User id and funding id uniquely identify rows of this table entity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="fundingId">The funding id.</param>
        public UserFundingView(string userId, string fundingId)
        {
            PartitionKey = userId;
            RowKey = fundingId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFundingView"/> class.
        /// The default constructor for deserialization.
        /// </summary>
        public UserFundingView()
        {
        }

        /// <summary>
        /// Gets or sets the datetime when the user visited the allocation statement.
        /// </summary>
        public DateTime ViewedAt { get; set; }

        /// <inheritdoc />
        public string PartitionKey { get; set; }

        /// <inheritdoc />
        public string RowKey { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? Timestamp { get; set; }

        /// <inheritdoc />
        public ETag ETag { get; set; }
    }
}