using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FytSoa.Common
{
    public static class ExcelHelper
    {
        public static async Task Write<T>(this IEnumerable<T> source, Stream output, string sheetName = "导出数据", params Func<T, (string, object)>[] columns)
        {
            var ep = new ExcelPackage();

            var sheet = ep.Workbook.Worksheets.Add(sheetName);

            await Task.Run(() =>
            {
                var r = 2;
                foreach (var item in source)
                {
                    var c = 1;
                    foreach (var col in columns)
                    {
                        var col_data = col(item);

                        if (r == 2)
                        {
                            sheet.SetValue(r - 1, c, col_data.Item1);
                        }

                        sheet.SetValue(r, c, col_data.Item2);

                        c++;
                    }

                    r++;
                }
            });

            await ep.SaveAsAsync(output);
        }
    }

    public class ExcelColumn<T>
    {
        public string Header { get; set; }
        public Func<T, string> SetCell { get; set; }
    }
}
