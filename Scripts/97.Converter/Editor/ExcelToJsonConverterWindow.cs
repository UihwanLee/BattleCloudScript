using System.IO;
using UnityEditor;
using UnityEngine;

// Unity Editor Tool에서 Excel->Json으로 변환하게 해주는 Tool 스크립트
// excelFilePath: Excel 파일 주소
// jsonOutputFolder: 저장할 Json 폴더 주소
// Json 저장 위치는 Resources 폴더 내 Data에 위치함
// 선택한 경로는 EditorPrefs에 저장해 창을 다시 열어도 유지
public class ExcelToJsonConverterWindow : EditorWindow
{
    private const string EXCEL_PATH_PREF = "ExcelToJson.ExcelFilePath";
    private const string OUTPUT_FOLDER_PREF = "ExcelToJson.JsonOutputFolder";

    private string excelFilePath;
    private string jsonOutputFolder;

    [MenuItem("Tools/Excel to JSON Converter")]
    public static void ShowWindow()
    {
        ExcelToJsonConverterWindow window = GetWindow<ExcelToJsonConverterWindow>("Excel to JSON Converter");
        window.Show();
    }

    private void OnEnable()
    {
        excelFilePath = EditorPrefs.GetString(EXCEL_PATH_PREF, "");
        jsonOutputFolder = EditorPrefs.GetString(OUTPUT_FOLDER_PREF, "");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Excel File Path", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Excel File"))
        {
            string path = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
            {
                SetExcelFilePath(path);
            }
        }

        SetExcelFilePath(EditorGUILayout.TextField("Excel File Path", excelFilePath));

        GUILayout.Space(10);

        GUILayout.Label("Json Output Folder", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Json Output Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Json Output Folder", "", "");
            if (!string.IsNullOrEmpty(folderPath))
            {
                SetJsonOutputFolder(folderPath);
            }
        }

        if (GUILayout.Button("Set Json Output Folder To Resource/Data"))
        {
            SetJsonOutputFolder(Path.Combine(Application.dataPath, "Resources", "Data"));
        }

        if (GUILayout.Button("Set Json Output Folder To Resource/Data/Localization"))
        {
            SetJsonOutputFolder(Path.Combine(Application.dataPath, "Resources", "Data", "Localization"));
        }

        SetJsonOutputFolder(EditorGUILayout.TextField("Json Folder Path", jsonOutputFolder));

        GUILayout.Space(10);

        if (GUILayout.Button("Convert ALL Sheets To Json"))
        {
            ExcelToJson converter = new ExcelToJson
            {
                excelFilePath = excelFilePath,
                jsonOutputFolder = jsonOutputFolder
            };

            converter.ConvertAllSheetsToJson();
        }
    }

    private void SetExcelFilePath(string path)
    {
        if (path == excelFilePath) return;
        excelFilePath = path;
        EditorPrefs.SetString(EXCEL_PATH_PREF, path);
    }

    private void SetJsonOutputFolder(string path)
    {
        if (path == jsonOutputFolder) return;
        jsonOutputFolder = path;
        EditorPrefs.SetString(OUTPUT_FOLDER_PREF, path);
    }
}
