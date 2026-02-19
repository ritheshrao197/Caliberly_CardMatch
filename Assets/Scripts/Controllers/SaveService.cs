using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int HighScore;
}

public static class SaveService
{
    private const string Key = "save_data";

    public static void Save(int highScore)
    {
        SaveData data = new SaveData { HighScore = highScore };
        PlayerPrefs.SetString(Key, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static int Load()
    {
        if (!PlayerPrefs.HasKey(Key))
            return 0;

        var json = PlayerPrefs.GetString(Key);
        return JsonUtility.FromJson<SaveData>(json).HighScore;
    }
}
