using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SOToJsonConverterWindow : EditorWindow
{
    public string soFilePath;
    private string jsonOutputFolder;

    [MenuItem("Tools/SO to JSON Converter")]
    public static void ShowWindow()
    {
        SOToJsonConverterWindow window = GetWindow<SOToJsonConverterWindow>("SO to JSON Converter");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("SO Folder Path", EditorStyles.boldLabel);

        if (GUILayout.Button("Select SO Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select SO Folder", "", "");
            if (!string.IsNullOrEmpty(folderPath))
            {
                soFilePath = folderPath;
            }
        }

        if (GUILayout.Button("Set SO Folder To Resource/SO"))
        {
            soFilePath = "Assets/Resources/SO";
        }

        EditorGUILayout.TextField("SO File Path", soFilePath);

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

        EditorGUILayout.TextField("Json Folder Path", jsonOutputFolder);

        GUILayout.Space(10);

        if (GUILayout.Button("Convert ALL SO To Json"))
        {
            SOToJson soConverter = new SOToJson
            {
                soFolderPath = soFilePath,
                jsonOutputFolder = jsonOutputFolder
            };

            soConverter.ConvertSOToJson();
        }
    }
}
