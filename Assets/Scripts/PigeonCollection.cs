using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonCollection : MonoBehaviour
{
    public List<Pigeon> ownedPigeons = new List<Pigeon>();
    [SerializeField] private SaveManager saveManager;

    public void AddPigeon(Pigeon newPigeon)
    {
        ownedPigeons.Add(newPigeon);
        Debug.Log($"Added pigeon to collection. Total owned: {ownedPigeons.Count}");
    }

    public void SaveGame()
    {
        saveManager.Save(ownedPigeons);
    }
}