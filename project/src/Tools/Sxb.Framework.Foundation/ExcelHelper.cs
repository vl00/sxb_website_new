using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sxb.Framework.Foundation
{
    public static class ExcelHelper
    {
        public static Stream ToExcel(IDictionary<string, IEnumerable<dynamic>> sheets)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            foreach (var sheet in sheets)
            {
                var _sheet = workbook.CreateSheet(sheet.Key);
                var props = sheet.Value.First() as IDictionary<string, object>;
                if (props == null)
                {
                    throw new Exception("dynamic无法强转为IDictionary<string, object>,请对dynamic中实际类型添加强转IDictionary<string, object>实现或直接存储Dictionary<string, object>对象");
                }
                int rowIndex = 0;
                int cellIndex = 0;
                var rowi = _sheet.CreateRow(rowIndex++);
                foreach (var prop in props)
                {
                    var cell0i = rowi.CreateCell(cellIndex++);
                    cell0i.SetCellValue(prop.Key);
                }
                foreach (var item in sheet.Value)
                {
                    rowi = _sheet.CreateRow(rowIndex++);
                    props = item as IDictionary<string, object>;
                    cellIndex = 0;
                    foreach (var prop in props)
                    {
                        var cellii = rowi.CreateCell(cellIndex++);
                        cellii.SetCellValue(prop.Value?.ToString());
                    }

                }
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms, true);
            ms.Position = 0;
            return ms;
        }

        public static Stream ObjectsToExcel(IDictionary<string, IEnumerable<object>> sheets)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            foreach (var sheet in sheets)
            {
                var _sheet = workbook.CreateSheet(sheet.Key);
                var headers = sheet.Value.First().GetType().GetProperties().Select(p => p.Name);
                int rowIndex = 0;
                int cellIndex = 0;
                var rowi = _sheet.CreateRow(rowIndex++);
                foreach (var header in headers)
                {
                    var cell0i = rowi.CreateCell(cellIndex++);
                    cell0i.SetCellValue(header);
                }
                foreach (var item in sheet.Value)
                {
                    rowi = _sheet.CreateRow(rowIndex++);
                    var props = item.GetType().GetProperties();
                    cellIndex = 0;
                    foreach (var prop in props)
                    {
                        var cellii = rowi.CreateCell(cellIndex++);
                        cellii.SetCellValue(prop.GetValue(item)?.ToString());
                    }

                }
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms, true);
            ms.Position = 0;
            return ms;
        }
        public static List<(int RowIndex, IEnumerable<(int CellIndex, string CellName, string CellValue)> Cells)> ExcelToData(this Stream s, int sheetIndex = 0, int startRowNum = 1)
        {
            var excelFile = new XSSFWorkbook(s);
            var currentSheet = excelFile.GetSheetAt(sheetIndex);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            if (titleRow == default) return default;
            for (int i = 0; i < titleRow.Cells.Count; i++) titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            var rows = new List<(int RowIndex, IEnumerable<(int CellIndex, string CellName, string CellValue)> Cells)>();
            for (int i = startRowNum; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    if (!titleNames.ContainsKey(o)) continue;
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            if (HSSFDateUtil.IsCellDateFormatted(cell))
                            {
                                cellValue = cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                cellValue = cell.NumericCellValue.ToString();
                            }
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) cellValue = null;
                    rowData.Add((o, titleNames[o], cellValue));
                }
                rows.Add((i + 1, rowData));
            }
            return rows;
        }
    }
}
