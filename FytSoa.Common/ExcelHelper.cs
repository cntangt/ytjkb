using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Common
{
    public static class ExcelHelper
    {
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

        /// <summary>
        /// 创建Excel列组建器
        /// </summary>
        /// <typeparam name="T">集合对象类型</typeparam>
        /// <param name="list">数据源集合</param>
        /// <returns></returns>
        public static ExcelColumnBuilder<T> ExcelBuilder<T>(this IEnumerable<T> list)
        {
            return new ExcelColumnBuilder<T>(list);
        }
    }

    public class ExcelColumnBuilder<T>
    {
        private readonly IEnumerable<T> list;

        private readonly Dictionary<int, ExcelColumn<T>> cols = new Dictionary<int, ExcelColumn<T>>();

        public ExcelColumnBuilder(IEnumerable<T> list)
        {
            this.list = list;
        }

        /// <summary>
        /// 创建表格列
        /// </summary>
        /// <param name="header">列标题</param>
        /// <param name="val">列取值委托</param>
        /// <param name="format">列格式</param>
        /// <param name="funcType">列汇总函数</param>
        public ExcelColumnBuilder<T> Col(string header, Func<T, object> val, string format = null, FuncType? funcType = null)
        {
            // 列序号从1开始
            cols.Add(cols.Count + 1, new ExcelColumn<T>
            {
                FuncType = funcType,
                Value = val,
                Format = format,
                Header = header
            });

            return this;
        }

        /// <summary>
        /// 写入表格字节数据
        /// </summary>
        /// <param name="sheetName">表格Sheet名称</param>
        /// <returns></returns>
        public async Task<byte[]> Write(string sheetName = "导出数据")
        {
            var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add(sheetName);

            await Task.Run(() =>
            {
                // 添加标题
                foreach (var item in cols)
                {
                    var col = item.Value;

                    sheet.SetValue(1, item.Key, col.Header);

                    if (!string.IsNullOrEmpty(col.Format))
                    {
                        sheet.Column(item.Key).Style.Numberformat.Format = col.Format;
                    }
                }

                // 冻结标题
                sheet.View.FreezePanes(2, 1);

                // 添加数据
                // 数据行从2开始
                var rowIndex = 2;
                foreach (var row in list)
                {
                    foreach (var col in cols)
                    {
                        sheet.SetValue(rowIndex, col.Key, col.Value.Value(row));
                    }

                    rowIndex++;
                }

                // 设置汇总行
                var funs = cols.Where(p => p.Value.FuncType.HasValue).ToArray();
                if (funs.Length > 0)
                {
                    sheet.SetValue(rowIndex, 1, string.Join('/', funs.Select(p => p.Value.FuncType.Value.GetEnumText()).Distinct()));

                    foreach (var item in funs)
                    {
                        sheet.Cells[rowIndex, item.Key].Formula = $"={item.Value.FuncType}({sheet.Cells[2, item.Key].Address}:{sheet.Cells[rowIndex - 1, item.Key].Address})";
                    }
                }

                // 设置自动列宽
                foreach (var item in cols)
                {
                    sheet.Column(item.Key).AutoFit();
                }
            });

            var stream = new MemoryStream();

            await package.SaveAsAsync(stream);

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

    public class ExcelColumn<T>
    {
        public string Header { get; set; }
        public FuncType? FuncType { get; set; }
        public string Format { get; set; }
        public Func<T, object> Value { get; set; }
    }

    public enum FuncType
    {
        [Text("合计")]
        SUM,
        [Text("平均")]
        AVG
    }
}
