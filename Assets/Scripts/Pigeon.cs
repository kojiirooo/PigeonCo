using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LifeStage
{
    Egg,
    Chick,
    Young,
    Adult
}

public enum BreedRarity
{
    Common,
    Uncommon,
    Rare
}

public enum Gender
{
    Male,
    Female
}

[System.Serializable]
public class Pigeon
{
    public string pigeonID;
    public string pigeonName;
    public string pigeonColor;
    public string breedType;
    public LifeStage stage;
    public BreedRarity rarity;
    public Gender gender;
    public bool isAway;
    public bool hasFreeFlown;
    public string mateID = "";
    public string parentAID = "";
    public string parentBID = "";
    public bool hasBred;
    public bool hasRing;
    public bool hasWingband;
    public int loftID = 1;
    [System.NonSerialized] public System.DateTime lastFedTime;
    [System.NonSerialized] public System.DateTime creationTime;
    [System.NonSerialized]public System.DateTime returnTime;
    [System.NonSerialized] public System.DateTime lastBredTime = System.DateTime.MinValue;
    [System.NonSerialized] public System.DateTime lastFreeFlyTime = System.DateTime.MinValue;
    public string lastBredTimeText;
    public string lastFreeFlyTimeText;
    public string creationTimeText;
    public string lastFedTimeText;
    public string returnTimeText;   
    public float hunger;
    public float bond;
    public float dirt;
    public bool hasCold;
    public bool isWild;        
    public int tameSteps;      
    public float pigeonRating;
    

    //Constants
    public const float HoursUntilEmpty = 0.1f;
    public const float FeedAmount = 40f;
    public const float HungryThreshold = 30f;
    public const float MaxBond = 100f;
    public const float BondToBeYoung = 40f;
    public const float BondToBeAdult = 100f;
    public const float FreeFlyMinutes = 0.25f;
    public const float PremiumBondBonus = 10f;
    public const float RingRatingBonus = 10f;
    public const float WingbandRaceBonus = 10f;
    public const float BreedCooldownMinutes = 0.5f;
    public const float FreeFlyCooldownMinutes = 0.5f;
    public const float DirtPerMinute = 20f;   
    public const float ColdThreshold = 75f;  



    public Pigeon(string name, string color, string breed, LifeStage startingStage, BreedRarity startingRarity)
    {

        pigeonID = System.Guid.NewGuid().ToString();
        pigeonName = name;
        pigeonColor = color;
        breedType = breed;
        stage = startingStage;
        rarity = startingRarity;

        hunger = 100f;
        bond = 0f;
        lastFedTime = System.DateTime.Now;
        pigeonRating = 0f;
        creationTime = System.DateTime.Now;
        gender = (Gender)UnityEngine.Random.Range(0, 2);


    }

    public System.TimeSpan GetBreedCooldownLeft()
    {
        System.DateTime readyAt = lastBredTime.AddMinutes(BreedCooldownMinutes);
        System.TimeSpan left = readyAt - System.DateTime.Now;
        return left > System.TimeSpan.Zero ? left : System.TimeSpan.Zero;
    }

    public System.TimeSpan GetFreeFlyCooldownLeft()
    {
        System.DateTime readyAt = lastFreeFlyTime.AddMinutes(FreeFlyCooldownMinutes);
        System.TimeSpan left = readyAt - System.DateTime.Now;
        return left > System.TimeSpan.Zero ? left : System.TimeSpan.Zero;
    }

    private System.DateTime ParseSavedTime(string text)
    {
        // Older saves have no cooldown text, so treat them as ready
        if (string.IsNullOrEmpty(text))
        {
            return System.DateTime.MinValue;
        }

        return System.DateTime.Parse(text, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }

    public bool IsReadyToHatch()
    {
        float requiredMinutes = 0f;

        switch (rarity)
        {
            case BreedRarity.Common:
                requiredMinutes = 0.1f;
                break;
            case BreedRarity.Uncommon:
                requiredMinutes = 4f;
                break;
            case BreedRarity.Rare:
                requiredMinutes = 6f;
                break;
        }

        // your job: use creationTime, requiredMinutes, and System.DateTime.Now
        // to return true or false
        System.DateTime hatchTime = creationTime.AddMinutes(requiredMinutes);
        return System.DateTime.Now >= hatchTime;
    }

    public float GetCurrentHunger()
    {
        double hoursPassed = (System.DateTime.Now - lastFedTime).TotalHours;
        float hungerLost = (float)(hoursPassed / HoursUntilEmpty) * 100f;
        return Mathf.Max(0f, hunger - hungerLost);
    }

    public void Feed()
    {
        float currentHunger = GetCurrentHunger();
        hunger = Mathf.Min(100f, currentHunger + FeedAmount);
        lastFedTime = System.DateTime.Now;
    }

    public bool IsHungry()
    {
        return GetCurrentHunger() <= HungryThreshold;
    }

    public void AddBond(float amount)
    {
        if (stage == LifeStage.Egg || IsHungry() || isWild)
        {
            return;
        }

        bond = Mathf.Min(MaxBond, bond + amount);
    }

    public bool TryGrow()
    {
        if (stage == LifeStage.Chick && bond >= BondToBeYoung)
        {
            stage = LifeStage.Young;
            return true;
        }

        if (stage == LifeStage.Young && bond >= BondToBeAdult)
        {
            stage = LifeStage.Adult;
            return true;
        }

        return false;
    }

    public float GetRating()
    {
        float rating = GetBaseRating();

        if (hasRing)
        {
            rating += RingRatingBonus;
        }

        return rating;
    }

    public float GetBaseRating()
    {
        float rarityPoints = 10f;

        switch (rarity)
        {
            case BreedRarity.Uncommon:
                rarityPoints = 20f;
                break;
            case BreedRarity.Rare:
                rarityPoints = 30f;
                break;
        }

        return rarityPoints + (bond * 0.5f);
    }

    public bool CanFreeFly()
    {
        return stage == LifeStage.Adult && bond >= MaxBond && !isAway && IsFreeFlyReady();
    }

    public bool IsFreeFlyReady()
    {
        return GetFreeFlyCooldownLeft() == System.TimeSpan.Zero;
    }

    public void StartFreeFly()
    {
        hunger = GetCurrentHunger();
        isAway = true;
        hasFreeFlown = true;
        returnTime = System.DateTime.Now.AddMinutes(FreeFlyMinutes);
    }

    public bool IsBackFromFreeFly()
    {
        return isAway && System.DateTime.Now >= returnTime;
    }

    public void ReturnFromFreeFly()
    {
        isAway = false;
        lastFedTime = System.DateTime.Now;
    }

    public void PrepareForSave()
    {
        pigeonRating = GetRating();
        creationTimeText = creationTime.ToString("o");
        lastFedTimeText = lastFedTime.ToString("o");
        returnTimeText = returnTime.ToString("o");
        lastBredTimeText = lastBredTime.ToString("o");
        lastFreeFlyTimeText = lastFreeFlyTime.ToString("o");
    }

    public void RestoreAfterLoad()
    {
        // Saves made before lofts existed have no loft, so they go to Loft 1
        if (loftID == 0) loftID = 1;
        creationTime = System.DateTime.Parse(creationTimeText, null, System.Globalization.DateTimeStyles.RoundtripKind);
        lastFedTime = System.DateTime.Parse(lastFedTimeText, null, System.Globalization.DateTimeStyles.RoundtripKind);
        returnTime = System.DateTime.Parse(returnTimeText, null, System.Globalization.DateTimeStyles.RoundtripKind);
        lastBredTime = ParseSavedTime(lastBredTimeText);
        lastFreeFlyTime = ParseSavedTime(lastFreeFlyTimeText);
    }

    public bool UpdateDirt(float seconds)
    {
        if (stage == LifeStage.Egg || isAway) return false;

        dirt = Mathf.Min(100f, dirt + DirtPerMinute * seconds / 60f);

        if (!hasCold && dirt >= ColdThreshold)
        {
            hasCold = true;
            return true;
        }

        return false;
    }

    
    public const int TameStepsNeeded = 3;

    public bool CanBreedWith(Pigeon other)
    {
        if (other == null) return false;

        return stage == LifeStage.Adult && other.stage == LifeStage.Adult
            && bond >= MaxBond && other.bond >= MaxBond
            && !isAway && !other.isAway
            && gender != other.gender
            && mateID == other.pigeonID
            && GetBreedCooldownLeft() == System.TimeSpan.Zero
            && other.GetBreedCooldownLeft() == System.TimeSpan.Zero;
    }

    public void PremiumFeed()
    {
        hunger = 100f;
        lastFedTime = System.DateTime.Now;
        bond = Mathf.Min(MaxBond, bond + PremiumBondBonus);
    }
}