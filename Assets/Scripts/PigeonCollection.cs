using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonCollection : MonoBehaviour
{
    public List<Pigeon> ownedPigeons = new List<Pigeon>();
    [SerializeField] private SaveManager saveManager;
  
    public int coins;
    public int premiumFeed;
    public bool premiumFeedArmed;
    public const int PremiumFeedCost = 30;

    public void AddPigeon(Pigeon newPigeon)
    {
        ownedPigeons.Add(newPigeon);
        Debug.Log($"Added pigeon to collection. Total owned: {ownedPigeons.Count}");
    }

    public Pigeon FindByID(string id)
    {
        foreach (Pigeon p in ownedPigeons)
        {
            if (p.pigeonID == id) return p;
        }
        return null;
    }

    public void Breed(Pigeon a, Pigeon b)
    {
        for (int i = 0; i < 2; i++)
        {
            string color = (Random.value < 0.5f) ? a.pigeonColor : b.pigeonColor;
            Pigeon egg = new Pigeon("", color, a.breedType, LifeStage.Egg, BreedRarity.Common);
            egg.parentAID = a.pigeonID;
            egg.parentBID = b.pigeonID;
            AddPigeon(egg);
        }

        a.hasBred = true;
        b.hasBred = true;
        SaveGame();
    }

    public bool CanRace(Pigeon p)
    {
        // Only adults that are home can race.
        if (p.stage != LifeStage.Adult || p.isAway)
        {
            return false;
        }

        // A parent can't race until all of its chicks are Young or older.
        foreach (Pigeon other in ownedPigeons)
        {
            bool isChildOfP = other.parentAID == p.pigeonID || other.parentBID == p.pigeonID;

            if (isChildOfP)
            {
                if (other.stage != LifeStage.Young && other.stage != LifeStage.Adult)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveGame();
    }

    public bool TryBuyPremiumFeed()
    {
        if (coins < PremiumFeedCost)
        {
            return false;
        }

        coins -= PremiumFeedCost;
        premiumFeed++;
        SaveGame();
        return true;
    }

    public bool UsePremiumFeed(Pigeon p)
    {
        if (premiumFeed <= 0 || p.stage == LifeStage.Egg || p.isAway)
        {
            return false;
        }

        premiumFeed--;
        p.PremiumFeed();
        premiumFeedArmed = false;
        return true;
    }

    public void SaveGame()
    {
        saveManager.Save(ownedPigeons, coins, premiumFeed);
    }


}