using UnityEngine.EventSystems;
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
    [Header("Debug")]
    [SerializeField] private float debugHunger;
    [Header("Hunger Display")]
    [SerializeField] private Slider hungerBar;
    [SerializeField] private Color hungryColor = new Color(0.7f, 0.7f, 0.7f);

    [Header("Naming Popup")]
    [SerializeField] private GameObject namePopup;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmButton;
    [Header("Petting")]
    [SerializeField] private float bondPerPixel = 0.02f;
    [SerializeField] private float tapMoveLimit = 10f;
    [SerializeField] private float debugBond;

    private bool isPressing;
    private float dragDistance;
    private Vector3 lastMousePosition;

    void Start()
    {
        namePopup.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmClicked);
        hungerBar.gameObject.SetActive(false);
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
        pigeonData.hunger = 100f;
        pigeonData.lastFedTime = System.DateTime.Now;
        UpdateSprite();
        namePopup.SetActive(true);
        hungerBar.gameObject.SetActive(true);
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

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || pigeonData.stage == LifeStage.Egg)
        {
            isPressing = false;
            return;
        }

        isPressing = true;
        dragDistance = 0f;
        lastMousePosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (!isPressing)
        {
            return;
        }

        Vector3 currentMousePosition = Input.mousePosition;
        float moved = Vector3.Distance(currentMousePosition, lastMousePosition);
        lastMousePosition = currentMousePosition;
        dragDistance += moved;

        if (dragDistance > tapMoveLimit)
        {
            pigeonData.AddBond(moved * bondPerPixel);

            if (pigeonData.TryGrow())
            {
                UpdateSprite();
            }
        }
    }

    private void OnMouseUp()
    {
        if (!isPressing)
        {
            return;
        }

        isPressing = false;

        if (dragDistance <= tapMoveLimit)
        {
            pigeonData.Feed();
            Debug.Log($"Fed {pigeonData.pigeonName}. Hunger is now {pigeonData.GetCurrentHunger()}");
        }
    }

    void Update()
    {
        if (pigeonData.stage == LifeStage.Egg)
        {
            if (pigeonData.IsReadyToHatch())
            {
                HatchEgg();
            }
        }
        else
        {
            float currentHunger = pigeonData.GetCurrentHunger();
            debugHunger = currentHunger;
            debugBond = pigeonData.bond;
            hungerBar.value = currentHunger;

            if (pigeonData.IsHungry())
            {
                spriteRenderer.color = hungryColor;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}