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
    public int reputation;
    public bool loft2Unlocked;
    public bool loft3Unlocked;
    
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
    public bool loadedLoft2Unlocked;
    public bool loadedLoft3Unlocked;
    public int loadedReputation;

    public void Save(List<Pigeon> pigeons, int coins, int premiumFeed, int rings, int wingbands, bool loft2, bool loft3, int reputation)
    {
        SaveData data = new SaveData();
        data.pigeons = pigeons;
        data.coins = coins;
        data.premiumFeed = premiumFeed;
        data.rings = rings;
        data.wingbands = wingbands;
        data.loft2Unlocked = loft2;
        data.loft3Unlocked = loft3;
        data.reputation = reputation;   

        foreach (Pigeon p in data.pigeons)
        {
            p.PrepareForSave();
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Saved to: " + SavePath);
    }


    public void SaveLoft(List<Pigeon> loftPigeons, int loftNumber, int coins, int premiumFeed, int rings, int wingbands, bool loft2, bool loft3, int reputation)
    {
        List<Pigeon> combined = new List<Pigeon>();

        // Keep the pigeons from other lofts exactly as they are
        if (File.Exists(SavePath))
        {
            SaveData existing = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            foreach (Pigeon p in existing.pigeons)
            {
                if (p.loftID != loftNumber)
                {
                    p.RestoreAfterLoad();
                    combined.Add(p);
                }
            }
        }

        // Then add this loft's current pigeons
        combined.AddRange(loftPigeons);

        Save(combined, coins, premiumFeed, rings, wingbands, loft2, loft3, reputation);
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
        loadedLoft2Unlocked = data.loft2Unlocked;
        loadedLoft3Unlocked = data.loft3Unlocked;
        loadedReputation = data.reputation;   
        Debug.Log("Loaded " + data.pigeons.Count + " pigeon(s).");
        return data.pigeons;
    }
}