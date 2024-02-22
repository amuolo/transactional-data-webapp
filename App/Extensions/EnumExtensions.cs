using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Model.Extensions;

public static class HtmlEnumExtensions
{
    public static HtmlString EnumToString<T>(this IHtmlHelper helper)
    {
        var values = (T[])Enum.GetValues(typeof(T)) ?? Array.Empty<T>();
        var integers = values.Select(x => Convert.ToInt32(x));
        var dict = integers.ToDictionary(x => x, x => Enum.GetName(typeof(T), x)); 
        return new HtmlString(JsonConvert.SerializeObject(dict));
    }
}
