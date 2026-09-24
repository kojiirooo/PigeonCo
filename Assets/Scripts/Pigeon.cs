using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


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


    public Pigeon (string id, string name, string color, string breed, LifeStage Stage, float Hunger, float Bond, System.DateTime lastfed, float rating )
    {
        pigeonID = id;
        pigeonName = name;
        pigeonColor = color;
        breedType = breed;
        stage = Stage;
        hunger = Hunger;
        bond = Bond;
        lastFedTime = lastfed;
        pigeonRating = rating;
    }

}
