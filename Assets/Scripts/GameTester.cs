using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTester : MonoBehaviour
{
    [SerializeField] private PigeonController pigeonController;
    // Start is called before the first frame update
    void Start()
    {
        Pigeon testPigeon = new Pigeon("Test Name", "White", "Test Breed", LifeStage.Adult);
        pigeonController.SetupPigeon(testPigeon);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
