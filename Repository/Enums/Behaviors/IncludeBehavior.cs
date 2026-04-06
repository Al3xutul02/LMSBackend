using System.ComponentModel;

namespace Repository.Enums.Behaviors
{
    public enum IncludeBehavior
    {
        [Description("No inclusions should be made in the querry, only the data in the queried entity")]
        NoInclude,
        [Description("All inclusions are needed, this is more taxing on performance")]
        AllIncludes,
        [Description("Only include specified entities")]
        SelectedIncludes
    }
}

