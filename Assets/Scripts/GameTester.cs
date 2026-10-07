using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTester : MonoBehaviour
{
    [SerializeField] private PigeonController pigeonController;
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private PigeonController mateController;

    void Start()
    {
        List<Pigeon> loaded = saveManager.Load();

        if (loaded != null && loaded.Count > 0)
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

        }
        else
        {
            Pigeon starterEgg = new Pigeon("", "Grey", "Common Pigeon", LifeStage.Egg, BreedRarity.Common);
            pigeonCollection.AddPigeon(starterEgg);
            pigeonController.SetupPigeon(starterEgg);
            pigeonCollection.SaveGame();
        }
    }
}