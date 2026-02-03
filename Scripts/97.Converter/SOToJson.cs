using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SOToJson
{
    public string soFolderPath;
    public string jsonOutputFolder;

    public void ConvertSOToJson()
    {
        if (string.IsNullOrEmpty(jsonOutputFolder))
        {
            Debug.LogError("JSON 출력 폴더가 지정되지 않았습니다.");
            return;
        }

        // 해당 폴더 내의 모든 ScriptableObject 에셋의 GUID를 찾습니다.
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { soFolderPath });

        if (guids.Length == 0)
        {
            Debug.Log($"{soFolderPath} 경로에 ScriptableObject가 없습니다.");
            return;
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (so == null) continue;

            object dataToSerialize = ExtractDataFromSO(so);

            if (dataToSerialize != null)
            {
                string json = JsonConvert.SerializeObject(dataToSerialize, Formatting.Indented);

                string outputPath = Path.Combine(jsonOutputFolder, $"{so.name}.json");
                File.WriteAllText(outputPath, json);

                Debug.Log($"[SO Export] {so.name} → {outputPath}");
            }
        }

        AssetDatabase.Refresh();
    }

    private object ExtractDataFromSO(ScriptableObject so)
    {
        // SO 필드 중 List 형태인 것을 찾아 그 값을 반환합니다.
        var fields = so.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var field in fields)
        {
            // 보통 SO 구조가 public List<T> data; 형식이므로 첫 번째 리스트를 반환
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return field.GetValue(so);
            }
        }

        // 리스트 필드가 없다면 SO 전체를 직렬화
        return so;
    }
}
