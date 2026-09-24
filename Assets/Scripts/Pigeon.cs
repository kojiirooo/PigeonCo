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
    public string color;
    public string breedType;
    public LifeStage stage;
    public float hunger;
    public float bond;
    public System.DateTime lastFedTime;
    public float pigeonRating; 

}
