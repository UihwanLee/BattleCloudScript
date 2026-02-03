using System.IO;
using UnityEditor;
using UnityEngine;

// Unity Editor Tool에서 Excel->Json으로 변환하게 해주는 Tool 스크립트
// excelFilePath: Excel 파일 주소
// jsonOutputFolder: 저장할 Json 폴더 주소
// Json 저장 위치는 Resources 폴더 내 Data에 위치함
// 저장 위치 고정 시 Define -> 폴더 경로 상수화
public class ExcelToJsonConverterWindow : EditorWindow
{
    public string excelFilePath;
    private string jsonOutputFolder;

    [MenuItem("Tools/Excel to JSON Converter")]
    public static void ShowWindow()
    {
        ExcelToJsonConverterWindow window = GetWindow<ExcelToJsonConverterWindow>("Excel to JSON Converter");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Excel File Path", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Excel File"))
        {
            string path = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
            {
                excelFilePath = path;
            }
        }

        EditorGUILayout.TextField("Excel File Path", excelFilePath);

        GUILayout.Space(10);

        GUILayout.Label("Json Output Folder", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Json Output Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Json Output Folder", "", "");
            if (!string.IsNullOrEmpty(folderPath))
            {
                jsonOutputFolder = folderPath;
            }
        }

        if (GUILayout.Button("Set Json Output Folder To Resource/Data"))
        {
            jsonOutputFolder = Path.Combine(UnityEngine.Application.dataPath, "Resources", "Data");
        }

        if (GUILayout.Button("Set Json Output Folder To Resource/Data/Localization"))
        {
            jsonOutputFolder = Path.Combine(UnityEngine.Application.dataPath, "Resources", "Data", "Localization");
        }

        EditorGUILayout.TextField("Json Folder Path", jsonOutputFolder);

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
}

