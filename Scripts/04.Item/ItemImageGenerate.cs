using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class LevelUpRewardImageInfo
{
    [Header("리스트 인덱스")]
    public int index;
    [Header("레벨업 아이콘")]
    public Sprite icon;
    [Header("티어 리스트")]
    public List<int> tier;
}

// 아이템 이미지를 새로 생성하는 스크립트
// Resouces 폴더 내 같은 포멧으로 생성하여 이미지를 매칭시킬 수 있도록 한다.
public class ItemImageGenerate : MonoBehaviour
{
    [Header("아이템 이미지 리스트")]
    [SerializeField] private List<Sprite> itemSprites = new List<Sprite>();

    [Header("레벨업 보상 이미지 리스트")]
    [SerializeField] private List<LevelUpRewardImageInfo> rewardSprites = new List<LevelUpRewardImageInfo>();

    public void GenerateImage()
    {
        if (itemSprites.Count < 0) return;

        UpdateItemResources(itemSprites);
    }

    public void GenerateLevelUpImage()
    {
        if(rewardSprites.Count < 0) return;

        UpdateLevelUpRewardInfoResources();
    }

    private void UpdateItemResources(List<Sprite> sprites)
    {
        string folderPath = "Assets/Resources/Sprites/Item";

        // 폴더 확인 및 기존 파일 삭제
        if (Directory.Exists(folderPath))
        {
            // 폴더 내 파일 모두 삭제
            Directory.Delete(folderPath, true); 
        }

        // 폴더 재생성
        Directory.CreateDirectory(folderPath); 

        // 조언 이미지 생성
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i] == null) continue;

            string originalPath = AssetDatabase.GetAssetPath(sprites[i]);
            string extension = Path.GetExtension(originalPath);

            // 네이밍
            string newFileName = $"IMG_ITEM_BASE_{(i + 1).ToString("D3")}{extension}";
            string newPath = Path.Combine(folderPath, newFileName);

            AssetDatabase.CopyAsset(originalPath, newPath);
        }

        // 에디터 데이터베이스 새로고침
        AssetDatabase.Refresh();
        Debug.Log($"총 {sprites.Count}개의 아이템 이미지가 Resources 폴더에 갱신되었습니다.");
    }

    public void UpdateLevelUpRewardInfoResources()
    {
        string folderPath = "Assets/Resources/Sprites/Enhancement";

        int count = 0;

        // 폴더 확인 및 기존 파일 삭제
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }

        // 폴더 재생성
        Directory.CreateDirectory(folderPath);

        AssetDatabase.StartAssetEditing();

        try
        {
            for (int i = 0; i < rewardSprites.Count; i++)
            {
                var info = rewardSprites[i];
                if (info == null || info.icon == null) continue;

                string originalPath = AssetDatabase.GetAssetPath(info.icon);
                string extension = Path.GetExtension(originalPath);

                // 한 아이콘이 가진 여러 티어를 순회
                foreach (int tierValue in info.tier)
                {
                    // 네이밍
                    string newFileName = $"IMG_ENHANCEMENT_PLAYER_TIER{tierValue}_{(info.index + 1).ToString("D2")}{extension}";
                    string newPath = Path.Combine(folderPath, newFileName);

                    count++;

                    // 파일 복사
                    AssetDatabase.CopyAsset(originalPath, newPath);
                }
            }
        }
        finally
        {
            // 작업 완료 후 에디터 DB 업데이트
            AssetDatabase.StopAssetEditing();
        }

        // 에디터 데이터베이스 새로고침
        AssetDatabase.Refresh();
        Debug.Log($"총 {count}개의 레벨업 보상 이미지가 Resources 폴더에 갱신되었습니다.");
    }
}

[CustomEditor(typeof(ItemImageGenerate))]
public class ItemImageGenerateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemImageGenerate component = (ItemImageGenerate)target;
        if (GUILayout.Button("리소스 폴더 내 조언 이미지 생성"))
            component.GenerateImage();

        if (GUILayout.Button("리소스 폴더 내 레벨업 보상 이미지 생성"))
            component.GenerateLevelUpImage();
    }   
}
