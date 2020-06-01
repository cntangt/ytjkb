using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FytSoa.Common
{
    public static class ExcelHelper
    {
        public static async Task<byte[]> Write<T>(this IEnumerable<T> source, string sheetName, params Func<T, (string header, object value)>[] columns)
        {
            var stream = new MemoryStream();

            await Write(source, stream, sheetName, columns);

            return stream.ToArray();
        }

        public static async Task Write<T>(this IEnumerable<T> source, Stream output, string sheetName = "导出数据", params Func<T, (string header, object value)>[] columns)
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
                            sheet.SetValue(r - 1, c, col_data.header);
                        }

                        sheet.SetValue(r, c, col_data.value);

                        c++;
                    }

                    r++;
                }
            });

            await ep.SaveAsAsync(output);
        }

        public static async Task<byte[]> Write<T>(this IEnumerable<T> source, string sheetName = "导出数据", params Func<T, EC>[] columns)
        {
            var stream = new MemoryStream();
            var ep = new ExcelPackage();
            var sheet = ep.Workbook.Worksheets.Add(sheetName);

            await Task.Run(() =>
            {
                var r = 2;
                var sums = new List<int>();
                foreach (var item in source)
                {
                    var c = 1;
                    foreach (var col in columns)
                    {
                        var ec = col(item);

                        if (r == 2)
                        {
                            if (ec.Sum)
                            {
                                sums.Add(c);
                            }

                            sheet.SetValue(r - 1, c, ec.Header);
                            if (!string.IsNullOrEmpty(ec.Formatter))
                            {
                                sheet.Column(c).Style.Numberformat.Format = ec.Formatter;
                            }
                        }

                        sheet.SetValue(r, c, ec.Value);

                        c++;
                    }

                    r++;
                }
                if (sums.Count > 0)
                {
                    sheet.SetValue(r, 1, "合计");
                    foreach (var c in sums)
                    {
                        sheet.Cells[r, c].Formula = $"=sum({sheet.Cells[2, c].Address}:{sheet.Cells[r - 1, c].Address})";
                    }
                    //for (int i = 1; i <= columns.Length; i++)
                    //{
                    //    sheet.Cells[r, i].Style.Fill.BackgroundColor= 
                    //}
                }
                for (int i = 1; i <= columns.Length; i++)
                {
                    sheet.Column(i).AutoFit();
                }
                sheet.View.FreezePanes(2, 1);
            });

            await ep.SaveAsAsync(stream);

            return stream.ToArray();
        }
    }

    public class EC
    {
        public EC(string header, object val, string formatter = null, bool sum = false)
        {
            this.Header = header;
            this.Value = val;
            this.Formatter = formatter;
            this.Sum = sum;
        }

        public bool Sum { get; set; }
        public string Header { get; set; }
        public object Value { get; set; }
        public string Formatter { get; set; }
    }
}
