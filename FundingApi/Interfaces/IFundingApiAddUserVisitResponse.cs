namespace PDS.ViewYourFunding.Data.API.Interfaces
{
    /// <summary>
    /// An interface representing the response for adding user visit details.
    /// </summary>
    public interface IFundingApiAddUserVisitResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the add was a success or not.
        /// </summary>
        bool Success { get; set; }
    }
}