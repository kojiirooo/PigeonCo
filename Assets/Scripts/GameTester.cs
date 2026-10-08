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

    [Header("Loft")]
    [SerializeField] private int loftNumber = 1;

    void Start()
    {
        pigeonCollection.loftNumber = loftNumber;
        List<Pigeon> all = saveManager.Load();

        // Shared resources load for every loft
        if (all != null)
        {
            pigeonCollection.coins = saveManager.loadedCoins;
            pigeonCollection.premiumFeed = saveManager.loadedPremiumFeed;
            pigeonCollection.rings = saveManager.loadedRings;
            pigeonCollection.wingbands = saveManager.loadedWingbands;
        }

        // Only pigeons that belong to this loft
        List<Pigeon> loaded = new List<Pigeon>();
        if (all != null)
        {
            foreach (Pigeon p in all)
            {
                if (p.loftID == loftNumber)
                {
                    loaded.Add(p);
                }
            }
        }

        if (loaded.Count > 0)
        {
            foreach (Pigeon p in loaded)
            {
                pigeonCollection.AddPigeon(p);
            }
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
        else if (loftNumber == 1)
        {
            // Only Loft 1 starts with the starter egg
            Pigeon starterEgg = new Pigeon("", "Grey", "Common Pigeon", LifeStage.Egg, BreedRarity.Common);
            starterEgg.loftID = loftNumber;
            pigeonCollection.AddPigeon(starterEgg);
            pigeonController.SetupPigeon(starterEgg);
            pigeonCollection.SaveGame();
        }
    }



}