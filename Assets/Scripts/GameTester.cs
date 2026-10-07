using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTester : MonoBehaviour
{
    [SerializeField] private PigeonController pigeonController;
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private PigeonController mateController;
    [SerializeField] private PigeonController nursery1;
    [SerializeField] private PigeonController nursery2;

    void Start()
    {
        List<Pigeon> loaded = saveManager.Load();

        if (loaded != null && loaded.Count > 0)
        {
            foreach (Pigeon p in loaded)
            {
                pigeonCollection.AddPigeon(p);
            }
            pigeonCollection.coins = saveManager.loadedCoins;
            pigeonController.SetupPigeon(loaded[0]);
            if (loaded.Count > 1)
            {
                mateController.SetupPigeon(loaded[1]);
            }
            if (loaded.Count > 2)
            {
                nursery1.SetupPigeon(loaded[2]);
            }
            if (loaded.Count > 3)
            {
                nursery2.SetupPigeon(loaded[3]);
            }

        }
        else
        {
            Pigeon starterEgg = new Pigeon("", "Grey", "Common Pigeon", LifeStage.Egg, BreedRarity.Common);
            pigeonCollection.AddPigeon(starterEgg);
            pigeonController.SetupPigeon(starterEgg);
            pigeonCollection.SaveGame();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Pigeon p in pigeonCollection.ownedPigeons)
            {
                Debug.Log(p.pigeonName + " (" + p.stage + ") can race: " + pigeonCollection.CanRace(p));
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (Pigeon p in pigeonCollection.ownedPigeons)
            {
                if (pigeonCollection.CanRace(p))
                {

                    int place = RaceManager.RunRace(p);
                    int prize = RaceManager.GetPrize(place);
                    pigeonCollection.AddCoins(prize);
                    Debug.Log("Finished in place: " + place);
                    break;
                }
            }
        }
    }

}