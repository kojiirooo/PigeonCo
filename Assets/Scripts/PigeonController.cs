using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PigeonController : MonoBehaviour
{
    public Pigeon pigeonData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite eggSprite;
    [SerializeField] private Sprite chickSprite;
    [SerializeField] private Sprite youngSprite;
    [SerializeField] private Sprite adultSprite;

    [Header("Naming Popup")]
    [SerializeField] private GameObject namePopup;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmButton;

    void Start()
    {
        namePopup.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

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

    private void HatchEgg()
    {
        pigeonData.stage = LifeStage.Chick;
        UpdateSprite();
        namePopup.SetActive(true);
    }

    private void OnConfirmClicked()
    {
        string typedName = nameInput.text.Trim();

        if (typedName == "")
        {
            return; // don't accept an empty name
        }

        pigeonData.pigeonName = typedName;
        namePopup.SetActive(false);
        Debug.Log($"Pigeon named: {pigeonData.pigeonName}");
    }

    void Update()
    {
        if (pigeonData.stage == LifeStage.Egg && pigeonData.IsReadyToHatch())
        {
            HatchEgg();
        }
    }
}