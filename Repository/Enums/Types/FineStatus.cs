using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Repository.Enums.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FineStatus
    {
        Paid,
        Unpaid,
        Waived
    }
}
