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
// ID 컬럼("ID", "ID_xxx")은 int로 변환하고, 변환에 실패하는 값이 있으면 해당 시트는 저장하지 않는다.
// (GameRule 시트처럼 ID를 그룹 키로 중복해서 쓰는 시트가 있으므로 중복 검사는 DataManager에서 Dictionary로 읽을 때 한다)
// 이미지, 프리팹 리소스 -> Resources 폴더 내 배치하여 Excel은 그 리소스 이름을 가지고 있도록 함
//                     -> 이후 Resources.Load<>가 아닌 Addressable Asset을 '무조건'적으로 사용할 수 있도록 함
public class ExcelToJson
{
    private const string ID_COLUMN = "ID";
    private const string ID_COLUMN_PREFIX = "ID_";

    public string excelFilePath;
    public string jsonOutputFolder;

    public void ConvertAllSheetsToJson()
    {
        if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(jsonOutputFolder))
        {
            Debug.LogError("Excel Path or Json Output Folder is NULL");
            return;
        }

        int exported = 0;
        List<string> failedSheets = new List<string>();

        // 엑셀에서 파일을 열어둔 상태여도 읽을 수 있도록 FileShare.ReadWrite 지정
        using (FileStream stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            IWorkbook workbook = new XSSFWorkbook(stream);

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);

                if (TryConvertSheet(sheet, out List<Dictionary<string, object>> rowsData))
                {
                    string json = JsonConvert.SerializeObject(rowsData, Formatting.Indented);
                    string outputPath = Path.Combine(jsonOutputFolder, $"{sheet.SheetName}.json");
                    File.WriteAllText(outputPath, json);

                    Debug.Log($"[{sheet.SheetName}] JSON Exported → {outputPath}");
                    exported++;
                }
                else
                {
                    failedSheets.Add(sheet.SheetName);
                }
            }
        }

        AssetDatabase.Refresh();

        if (failedSheets.Count > 0)
            Debug.LogError($"[ExcelToJson] {exported}개 시트 변환 완료, {failedSheets.Count}개 시트 실패 (기존 JSON 유지): {string.Join(", ", failedSheets)}");
        else
            Debug.Log($"[ExcelToJson] {exported}개 시트 변환 완료");
    }

    /// <summary>
    /// 시트 하나를 행 데이터로 변환. 잘못된 데이터가 있으면 false를 반환하고 저장하지 않는다.
    /// </summary>
    private bool TryConvertSheet(ISheet sheet, out List<Dictionary<string, object>> rowsData)
    {
        rowsData = new List<Dictionary<string, object>>();
        string sheetName = sheet.SheetName;

        IRow headerRow = sheet.GetRow(0);
        if (headerRow == null)
        {
            Debug.LogWarning($"[{sheetName}] 헤더 행이 없어 건너뜁니다.");
            return false;
        }

        // 헤더가 비어있는 열은 변환하지 않는다
        int cellCount = headerRow.LastCellNum;
        string[] columnNames = new string[cellCount];
        for (int col = 0; col < cellCount; col++)
        {
            string name = headerRow.GetCell(col)?.ToString().Trim();
            columnNames[col] = string.IsNullOrEmpty(name) ? null : name;
        }

        bool isValid = true;

        for (int row = 1; row <= sheet.LastRowNum; row++)
        {
            IRow currentRow = sheet.GetRow(row);
            if (IsRowEmpty(currentRow, cellCount))
                continue;

            var rowData = new Dictionary<string, object>();

            for (int col = 0; col < cellCount; col++)
            {
                string columnName = columnNames[col];
                if (columnName == null) continue;

                object value = GetValueFromCell(currentRow.GetCell(col));

                // ID 컬럼은 무조건 int로 형태 변환
                if (IsIdColumn(columnName))
                {
                    if (!TryConvertId(value, out int id))
                    {
                        // 엑셀 행 번호는 1부터 시작하므로 +1
                        Debug.LogError($"[{sheetName}] {row + 1}행 {columnName} 값 '{value}'을(를) int로 변환할 수 없습니다.");
                        isValid = false;
                        continue;
                    }

                    value = id;
                }

                rowData[columnName] = value;
            }
            rowsData.Add(rowData);
        }

        return isValid;
    }

    private static bool IsRowEmpty(IRow row, int cellCount)
    {
        if (row == null) return true;

        for (int col = 0; col < cellCount; col++)
        {
            ICell cell = row.GetCell(col);
            if (cell != null && !string.IsNullOrEmpty(cell.ToString()))
                return false;
        }
        return true;
    }

    // "ID", "ID_STAT", "ID_WEAPON" 처럼 ID로 시작하는 컬럼만 ID로 본다 ("VALID" 같은 컬럼 제외)
    private static bool IsIdColumn(string columnName)
    {
        return columnName == ID_COLUMN || columnName.StartsWith(ID_COLUMN_PREFIX, StringComparison.Ordinal);
    }

    private static bool TryConvertId(object value, out int id)
    {
        id = 0;
        if (value == null) return false;

        try
        {
            double number = Convert.ToDouble(value);
            if (number % 1 != 0 || number < int.MinValue || number > int.MaxValue) return false;

            id = (int)number;
            return true;
        }
        catch (Exception e) when (e is FormatException || e is InvalidCastException)
        {
            return false;
        }
    }

    private static object GetValueFromCell(ICell cell)
    {
        if (cell == null) return null;

        // 수식 셀은 수식 문자열이 아니라 계산된 결과값을 사용
        CellType type = cell.CellType == CellType.Formula ? cell.CachedFormulaResultType : cell.CellType;

        return type switch
        {
            CellType.String => cell.StringCellValue,
            CellType.Numeric => ToNumber(cell.NumericCellValue),
            CellType.Boolean => cell.BooleanCellValue,
            _ => cell.ToString(),
        };
    }

    // 정수는 int (범위를 넘으면 long), 소수는 double 그대로
    private static object ToNumber(double value)
    {
        if (value % 1 != 0) return value;
        if (value >= int.MinValue && value <= int.MaxValue) return (int)value;
        return (long)value;
    }
}
