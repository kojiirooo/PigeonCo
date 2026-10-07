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

public class Pigeon
{
    public string pigeonID;
    public string pigeonName;
    public string pigeonColor;
    public string breedType;
    public LifeStage stage;
    public BreedRarity rarity;
    public Gender gender;
    public float hunger;
    public float bond;
    public System.DateTime lastFedTime;
    public float pigeonRating;
    public System.DateTime creationTime;
    public const float HoursUntilEmpty = 8f;
    public const float FeedAmount = 40f;
    public const float HungryThreshold = 30f;
    public const float MaxBond = 100f;
    public const float BondToBeYoung = 40f;
    public const float BondToBeAdult = 100f;



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

    public bool IsReadyToHatch()
    {
        float requiredMinutes = 0f;

        switch (rarity)
        {
            case BreedRarity.Common:
                requiredMinutes = 2f;
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
        if (stage == LifeStage.Egg || IsHungry())
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
}