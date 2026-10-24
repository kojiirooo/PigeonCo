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
    public int reputation;
    public List<string> discoveredColors = new List<string>();   
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


    // how many coins a pigeon is worth.
    public int GetSellPrice(Pigeon p)
    {
        return Mathf.RoundToInt(p.GetRating() * 2f);
    }

    // only adults that are home can be sold, and a loft always keeps at least one pigeon
    public bool CanSell(Pigeon p)
    {
        if (p == null || p.stage != LifeStage.Adult || p.isAway)
        {
            return false;
        }

        return ownedPigeons.Count > 1;
    }

    //pay the player, then remove the pigeon (ReleasePigeon also saves)
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

    //parents, children and siblings can't be paired
    public bool AreRelated(Pigeon a, Pigeon b)
    {
        // Parent or child
        if (a.pigeonID == b.parentAID || a.pigeonID == b.parentBID) return true;
        if (b.pigeonID == a.parentAID || b.pigeonID == a.parentBID) return true;

        return false;
    }



    //can these two adults be paired as mates?
    public bool CanPair(Pigeon a, Pigeon b)
    {
        if (a == null || b == null || a == b) return false;
        if (a.stage != LifeStage.Adult || b.stage != LifeStage.Adult) return false;
        if (a.gender == b.gender) return false;
        if (a.isAway || b.isAway) return false;
        if (a.loftID != b.loftID) return false;
        if (AreRelated(a, b)) return false;   
        return true;
    }

    //link two pigeons, unlinking any old mates first
    public void Pair(Pigeon a, Pigeon b)
    {
        Unpair(a);
        Unpair(b);
        a.mateID = b.pigeonID;
        b.mateID = a.pigeonID;
        SaveGame();
    }

    // NEW (Step 3): remove a pigeon's mate link, on both sides
    public void Unpair(Pigeon p)
    {
        if (string.IsNullOrEmpty(p.mateID)) return;

        Pigeon old = FindByID(p.mateID);
        if (old != null && old.mateID == p.pigeonID)
        {
            old.mateID = "";
        }
        p.mateID = "";
    }

    // NEW (Step 3): the next eligible partner after the current one, or null if none
    public Pigeon NextPartnerFor(Pigeon p)
    {
        List<Pigeon> options = new List<Pigeon>();
        foreach (Pigeon other in ownedPigeons)
        {
            if (CanPair(p, other)) options.Add(other);
        }

        if (options.Count == 0) return null;

        int index = options.FindIndex(o => o.pigeonID == p.mateID);
        return options[(index + 1) % options.Count];
    }

  
    public void AddReputation(int amount)
    {
        reputation += amount;
        SaveGame();
    }

    public void CleanLoft()
    {
        foreach (Pigeon p in ownedPigeons)
        {
            if (p.loftID == loftNumber)
            {
                p.dirt = 0f;
                p.hasCold = false;
            }
        }
        SaveGame();
    }

    public Pigeon CreateWildPigeon()
    {
        Pigeon wild = new Pigeon("", "Grey", "Wild Pigeon", LifeStage.Adult, BreedRarity.Uncommon);
        wild.isWild = true;
        wild.tameSteps = 0;
        wild.bond = 0f;
        wild.hunger = 50f;
        return wild;
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

    // true if this loft has room for 2 more eggs
    public bool CanBreedInLoft()
    {
        return CountPigeonsInLoft(loftNumber) + 2 <= GetCapacity(loftNumber);
    }

   
    public static readonly string[] AllColors = { "Grey", "Brown", "White", "Black" };

   
    public void DiscoverColor(string color)
    {
        if (!discoveredColors.Contains(color))
        {
            discoveredColors.Add(color);
        }
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
            string color;
            if (Random.value < 0.8f)
            {
                color = (Random.value < 0.5f) ? a.pigeonColor : b.pigeonColor;
            }
            else
            {
                color = AllColors[Random.Range(0, AllColors.Length)];
            }
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
        if (p.stage != LifeStage.Adult || p.isAway || p.bond < Pigeon.MaxBond || p.hasCold || p.isWild)
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

        saveManager.SaveLoft(ownedPigeons, loftNumber, coins, premiumFeed, rings, wingbands, loft2Unlocked, loft3Unlocked, reputation); 
    }


}