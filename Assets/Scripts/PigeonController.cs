using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonController : MonoBehaviour
{
    public Pigeon pigeonData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetupPigeon(Pigeon data)
    {
        pigeonData = data;
        Debug.Log($"Pigeon name: {pigeonData.pigeonName}, color : {pigeonData.pigeonColor}");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}
