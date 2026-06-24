using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.API.Helpers
{
    /// <summary>
    /// An attribute helper to decorate what concrete class is to be used for an interface when serialising/deserialising.
    /// Library took from https://gist.github.com/teamaton/bba69cf95b9e6766f231 (or similar blog post).
    /// </summary>
    /// <typeparam name="TConcrete">Implementation type to use.</typeparam>
    public class ConcreteTypeConverter<TConcrete> : JsonConverter
    {
        /// <summary>
        /// Simple implementation can assume it can always convert.
        /// </summary>
        /// <param name="objectType">Not used in this case.</param>
        /// <returns>Always returns true.</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Deserialise a string to a an object.
        /// </summary>
        /// <param name="reader">A JSON reader to read the string from.</param>
        /// <param name="objectType">Not used in this case.</param>
        /// <param name="existingValue">The existing value. Not used in this case.</param>
        /// <param name="serializer">The JSON serialiser.</param>
        /// <returns>The deserialised object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<TConcrete>(reader);
        }

        /// <summary>
        /// Serialise an object to a string.
        /// </summary>
        /// <param name="writer">The writer to write the serialised object to.</param>
        /// <param name="value">The object to serialiser.</param>
        /// <param name="serializer">The JSON serialiser.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
