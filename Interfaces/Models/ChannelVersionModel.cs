using Newtonsoft.Json;

namespace PDS.ViewYourFunding.Data.Interfaces.Models
{
    /// <summary>
    /// An interface representing a channel version details.
    /// </summary>
    public class ChannelVersionModel
    {
        /// <summary>
        /// Gets or sets the type of the Channel.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the version Value of the Channel.
        /// </summary>
        [JsonProperty("value")]
        public int? Value { get; set; }
    }
}
