using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTester : MonoBehaviour
{
    [SerializeField] private PigeonController pigeonController;
    [SerializeField] private PigeonCollection pigeonCollection;

    // Start is called before the first frame update
    void Start()
    {
        Pigeon starterEgg = new Pigeon("", "Grey", "Common Pigeon", LifeStage.Egg, BreedRarity.Common);
        pigeonCollection.AddPigeon(starterEgg);
        pigeonController.SetupPigeon(starterEgg);
    }

    // Update is called once per frame
    void Update()
    {

    }
}