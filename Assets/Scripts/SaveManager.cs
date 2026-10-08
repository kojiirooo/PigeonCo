using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<Pigeon> pigeons = new List<Pigeon>();
    public int coins;
    public int premiumFeed;
    public int rings;
    public int wingbands;
}

public class SaveManager : MonoBehaviour
{
    private string SavePath
    {
        get { return Path.Combine(Application.persistentDataPath, "pigeonco_save.json"); }
    }

    public int loadedCoins;
    public int loadedPremiumFeed;
    public int loadedRings;
    public int loadedWingbands;

    public void Save(List<Pigeon> pigeons, int coins, int premiumFeed, int rings, int wingbands)
    {
        SaveData data = new SaveData();
        data.pigeons = pigeons;
        data.coins = coins;
        data.premiumFeed = premiumFeed;
        data.rings = rings;
        data.wingbands = wingbands;

        foreach (Pigeon p in data.pigeons)
        {
            p.PrepareForSave();
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Saved to: " + SavePath);
    }

    public List<Pigeon> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (Pigeon p in data.pigeons)
        {
            p.RestoreAfterLoad();
        }



        loadedCoins = data.coins;
        loadedPremiumFeed = data.premiumFeed;
        loadedRings = data.rings;
        loadedWingbands = data.wingbands;
        Debug.Log("Loaded " + data.pigeons.Count + " pigeon(s).");
        return data.pigeons;
    }
}