using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonController : MonoBehaviour
{
    public Pigeon pigeonData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite eggSprite;
    [SerializeField] private Sprite chickSprite;
    [SerializeField] private Sprite youngSprite;
    [SerializeField] private Sprite adultSprite;

    public void SetupPigeon(Pigeon data)
    {
        pigeonData = data;
        Debug.Log($"Pigeon name: {pigeonData.pigeonName}, color : {pigeonData.pigeonColor}");

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        switch (pigeonData.stage)
        {
            case LifeStage.Egg:
                spriteRenderer.sprite = eggSprite;
                break;
            case LifeStage.Chick:
                spriteRenderer.sprite = chickSprite;
                break;
            case LifeStage.Young:
                spriteRenderer.sprite = youngSprite;
                break;
            case LifeStage.Adult:
                spriteRenderer.sprite = adultSprite;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pigeonData.stage == LifeStage.Egg && pigeonData.IsReadyToHatch())
        {
            pigeonData.stage = LifeStage.Chick;
            UpdateSprite();
        }
    }
}