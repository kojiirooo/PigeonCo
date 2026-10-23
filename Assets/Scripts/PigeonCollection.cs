using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonCollection : MonoBehaviour
{
    public List<Pigeon> ownedPigeons = new List<Pigeon>();
    public int loftNumber = 1;
    public bool loft2Unlocked = false;
    public bool loft3Unlocked = false;
    public bool isLoaded = false;

    public bool IsLoftUnlocked(int loft)
    {
        if (loft == 1) return true;
        if (loft == 2) return loft2Unlocked;
        if (loft == 3) return loft3Unlocked;
        return false;
    }
    public bool TryUnlockLoft(int loft)
    {
        if (IsLoftUnlocked(loft))
        {
            return false;
        }

        int cost = 0;
        if (loft == 2) cost = Loft2Cost;
        if (loft == 3) cost = Loft3Cost;

        if (coins < cost)
        {
            return false;
        }

        coins -= cost;

        if (loft == 2) loft2Unlocked = true;
        if (loft == 3) loft3Unlocked = true;

        SaveGame();
        return true;
    }

    [SerializeField] private SaveManager saveManager;

    public int coins;
    public int premiumFeed;
    public bool premiumFeedArmed;
    public const int PremiumFeedCost = 30;
    public int rings;
    public int wingbands;
    public bool ringArmed;
    public bool wingbandArmed;
    public const int RingCost = 50;
    public const int WingbandCost = 80;
    public const int Loft2Cost = 500;
    public const int Loft3Cost = 1000;

    public int GetCapacity(int loft)
    {
        switch (loft)
        {
            case 1: return 4;
            case 2: return 6;
            case 3: return 8;
            default: return 4;
        }
    }

    public int CountPigeonsInLoft(int loft)
    {
        int count = 0;
        foreach (Pigeon p in ownedPigeons)
        {
            if (p.loftID == loft)
            {
                count++;
            }
        }
        return count;
    }

    public bool IsLoftFull(int loft)
    {
        return CountPigeonsInLoft(loft) >= GetCapacity(loft);
    }

    public bool TryMovePigeon(Pigeon p, int targetLoft)
    {
        if (p == null || p.loftID == targetLoft)
        {
            return false;
        }

        if (!IsLoftUnlocked(targetLoft) || IsLoftFull(targetLoft))
        {
            return false;
        }

        p.loftID = targetLoft;
        SaveGame();
        return true;
    }

    public bool CanMoveTo(Pigeon p, int targetLoft)
    {
        if (p == null || p.loftID == targetLoft)
        {
            return false;
        }

        if (p.stage == LifeStage.Egg || p.isAway)
        {
            return false;
        }

        return IsLoftUnlocked(targetLoft) && !IsLoftFull(targetLoft);
    }

    public void ReleasePigeon(Pigeon p)
    {
        if (p == null)
        {
            return;
        }

        // Clear the partner's link to this pigeon
        foreach (Pigeon other in ownedPigeons)
        {
            if (other.mateID == p.pigeonID)
            {
                other.mateID = "";
            }
        }

        ownedPigeons.Remove(p);
        SaveGame();
        Debug.Log("Released " + p.pigeonName + ".");
    }


    // NEW (Step 2): how many coins a pigeon is worth. Change the 2f to tune the economy.
    public int GetSellPrice(Pigeon p)
    {
        return Mathf.RoundToInt(p.GetRating() * 2f);
    }

    // NEW (Step 2): only adults that are home can be sold, and a loft always keeps at least one pigeon
    public bool CanSell(Pigeon p)
    {
        if (p == null || p.stage != LifeStage.Adult || p.isAway)
        {
            return false;
        }

        return ownedPigeons.Count > 1;
    }

    // NEW (Step 2): pay the player, then remove the pigeon (ReleasePigeon also saves)
    public bool SellPigeon(Pigeon p)
    {
        if (!CanSell(p))
        {
            return false;
        }

        coins += GetSellPrice(p);
        ReleasePigeon(p);
        return true;
    }

    public void AddPigeon(Pigeon newPigeon)
    {
        newPigeon.loftID = loftNumber;
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

    public bool Breed(Pigeon a, Pigeon b)
    {
        // Breeding makes 2 eggs, so the loft needs room for 2 more pigeons
        if (CountPigeonsInLoft(loftNumber) + 2 > GetCapacity(loftNumber))
        {
            Debug.Log("Not enough room in this loft for 2 eggs.");
            return false;
        }

        for (int i = 0; i < 2; i++)
        {
            string color = (Random.value < 0.5f) ? a.pigeonColor : b.pigeonColor;
            Pigeon egg = new Pigeon("", color, a.breedType, LifeStage.Egg, BreedRarity.Common);
            egg.parentAID = a.pigeonID;
            egg.parentBID = b.pigeonID;
            AddPigeon(egg);
        }

        a.lastBredTime = System.DateTime.Now;
        b.lastBredTime = System.DateTime.Now;
        a.hasBred = true;
        b.hasBred = true;
        SaveGame();
        return true;
    }

    public bool CanRace(Pigeon p)
    {
        if (p.stage != LifeStage.Adult || p.isAway || p.bond < Pigeon.MaxBond)
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

    public bool HasUnfinishedChildren(Pigeon p)
    {
        foreach (Pigeon other in ownedPigeons)
        {
            bool isChildOfP = other.parentAID == p.pigeonID || other.parentBID == p.pigeonID;

            if (isChildOfP && other.stage != LifeStage.Young && other.stage != LifeStage.Adult)
            {
                return true;
            }
        }

        return false;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveGame();
    }

    public bool TryBuyRing()
    {
        if (coins < RingCost)
        {
            return false;
        }

        coins -= RingCost;
        rings++;
        SaveGame();
        return true;
    }

    public bool TryBuyWingband()
    {
        if (coins < WingbandCost)
        {
            return false;
        }

        coins -= WingbandCost;
        wingbands++;
        SaveGame();
        return true;
    }

    public bool UseRing(Pigeon p)
    {
        if (rings <= 0 || p.hasRing || p.stage == LifeStage.Egg || p.isAway)
        {
            return false;
        }

        rings--;
        p.hasRing = true;
        ringArmed = false;
        return true;
    }

    public bool UseWingband(Pigeon p)
    {
        if (wingbands <= 0 || p.hasWingband || p.stage == LifeStage.Egg || p.isAway)
        {
            return false;
        }

        wingbands--;
        p.hasWingband = true;
        wingbandArmed = false;
        return true;
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
        // Don't save until GameTester has loaded the real values,
        // or we could overwrite the save with empty defaults.
        if (!isLoaded)
        {
            Debug.LogWarning("Save skipped: game has not finished loading yet.");
            return;
        }

        saveManager.SaveLoft(ownedPigeons, loftNumber, coins, premiumFeed, rings, wingbands, loft2Unlocked, loft3Unlocked);
    }


}