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

public class Pigeon 
{
    public string pigeonID;
    public string pigeonName;
    public string pigeonColor;
    public string breedType;
    public LifeStage stage;
    public float hunger;
    public float bond;
    public System.DateTime lastFedTime;
    public float pigeonRating; 


    public Pigeon (string name, string color, string breed, LifeStage Stage)
    {
        pigeonID = System.Guid.NewGuid().ToString();
        pigeonName = name;
        pigeonColor = color;
        breedType = breed;
        stage = Stage;

        hunger = 100f;
        bond = 0f;
        lastFedTime = System.DateTime.Now;
        pigeonRating = 0f;
    }

}
