using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class EnumExtension
    {
        public static IEnumerable<SelectListItem> ToDropdown(this Type type)
        {
            foreach (var item in Enum.GetValues(type))
            {
                yield return new SelectListItem
                {
                    Text = item.ToString(),
                    Value = ((int)item).ToString()
                };
            }
        }
    }
}
