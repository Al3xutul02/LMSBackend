using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Repository.Enums.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BookGenreType
    {
       Action,
       Adventure,
       ArtAndPhotography,
       Biography,
       Children,
       ComingOfAge,
       ContemporaryFiction,
       CookBooks,
       Dystopian,
       Fantasy,
       GraphicNovel,
       GuideOrHowTo,
       HistoricalFiction,
       History,
       Horror,
       HumanitiesAndSocialSciences,
       Humor,
       Memoir,
       Mistery,
       ParentingAndFamilies,
       Philosophy,
       ReligionAndSpirituality,
       Romance,
       ScienceAndTechnology,
       ScienceFiction,
       SelfHelp,
       ShortStory,
       Thriller,
       Travel,
       TrueCrime,
       YoungAdult
    }
}
