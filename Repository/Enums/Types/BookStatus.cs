using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Repository.Enums.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BookStatus
    {
        InStock,
        OutOfStock,
        Discontinued,
        TemporarilyUnavailable
    }
}
