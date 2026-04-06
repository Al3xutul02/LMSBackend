using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Repository.Enums.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LoanStatus
    {
        Active,
        Overdue,
        Returned
    }
}
