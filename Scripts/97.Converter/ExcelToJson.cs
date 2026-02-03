using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Excel을 Json으로 변환하는 클래스
// ProjectSetting->Player->OtherSetting->ApiCompatibilityLevel->.NET Framework 변경해야함
// 모든 데이터 타입은 ID로 구분할 수 있게 설정 -> Excel의 첫번째 열에는 ID값이 적혀져 있어야 함. 
// case1: 기본 String 값으로 가져온 후 순서 별 데이터 타입 캐스팅
// case2: Excel 구조 Name:Value 타입으로 가져갈 경우 Valeu로 타입 캐스팅
// 이미지, 프리팹 리소스 -> Resources 폴더 내 배치하여 Excel은 그 리소스 이름을 가지고 있도록 함
//                     -> 이후 Resources.Load<>가 아닌 Addressable Asset을 '무조건'적으로 사용할 수 있도록 함
public class ExcelToJson
{
    public string excelFilePath;
    public string jsonOutputFolder;

    public void ConvertAllSheetsToJson()
    {
        if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(jsonOutputFolder))
        {
            Debug.LogError("Excel Path or Json Output Folder is NULL");
            return;
        }

        using (FileStream stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new XSSFWorkbook(stream);

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);
                string sheetName = sheet.SheetName;

                var rowsData = new List<Dictionary<string, object>>();
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow currentRow = sheet.GetRow(row);

                    bool isRowEmpty = true;
                    for (int col = 0; col < cellCount; col++)
                    {
                        ICell cell = currentRow?.GetCell(col);
                        if (cell != null && !string.IsNullOrEmpty(cell.ToString()))
                        {
                            isRowEmpty = false;
                            break;
                        }
                    }

                    if (isRowEmpty)
                        continue;

                    var rowData = new Dictionary<string, object>();

                    for (int col = 0; col < cellCount; col++)
                    {
                        string columnName = headerRow.GetCell(col).ToString();
                        ICell cell = currentRow.GetCell(col);
                        object value = GetValueFromCell(cell);

                        // ID 컬럼은 무조건 int로 형태 변환
                        if (columnName.Contains("ID") && value != null)
                        {
                            try
                            {
                                value = (int)Convert.ToDouble(value);
                            }
                            catch
                            {
                                Debug.LogError($"[Convert Error] ID 변환 실패 - Sheet: {sheetName}, Row: {row}");
                            }
                        }

                        // Type:enum 구조를 사용할 때
                        //string pureColumn = columnName.Contains(":")
                        //                    ? columnName.Split(':')[0]
                        //                    : columnName;

                        rowData[columnName] = value;
                    }
                    rowsData.Add(rowData);
                }

                string json = JsonConvert.SerializeObject(rowsData, Newtonsoft.Json.Formatting.Indented);

                string outputPath = Path.Combine(jsonOutputFolder, $"{sheetName}.json");
                File.WriteAllText(outputPath, json);

                Debug.Log($"[{sheetName}] JSON Exported → {outputPath}");
            }
        }

        AssetDatabase.Refresh();
    }

    private object GetValueFromCell(ICell cell)
    {
        if (cell == null) return null;

        return cell.CellType switch
        {
            CellType.String => cell.StringCellValue,
            CellType.Numeric => (cell.NumericCellValue % 1 == 0)
                                ? (int)cell.NumericCellValue
                                : cell.NumericCellValue,
            CellType.Boolean => cell.BooleanCellValue,
            CellType.Formula => cell.ToString(),
            _ => cell.ToString(),
        };
    }

    /// <summary>
    /// Type:enum 구조
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private object ParseValueByColumnType(string columnName, object value)
    {
        if (value == null) return null;

        if (!columnName.Contains(":"))
            return value;

        var split = columnName.Split(':');
        string type = split[1];

        return type switch
        {
            "int" => Convert.ToInt32(value),
            "float" => Convert.ToSingle(value),
            "bool" => Convert.ToBoolean(value),
            "enum" => value.ToString(),   // 저장은 string으로 유지
            _ => value
        };
    }
}


