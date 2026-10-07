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

    [Header("Saving")]
    [SerializeField] private PigeonCollection pigeonCollection;

    [Header("Naming Popup")]
    [SerializeField] private GameObject namePopup;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button freeFlyButton;
    [Header("Petting")]
    [SerializeField] private float bondPerPixel = 0.02f;
    [SerializeField] private float tapMoveLimit = 10f;
    [SerializeField] private float debugBond;

    private bool isPressing;
    private float dragDistance;
    private Vector3 lastMousePosition;

    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmClicked);
        freeFlyButton.onClick.AddListener(OnFreeFlyClicked);

        namePopup.SetActive(false);
        hungerBar.gameObject.SetActive(false);
        freeFlyButton.gameObject.SetActive(false);
    }

    public void SetupPigeon(Pigeon data)
    {
        pigeonData = data;
        Debug.Log($"Pigeon name: {pigeonData.pigeonName}, color : {pigeonData.pigeonColor}, gender : {pigeonData.gender}");

        UpdateSprite();
        if (pigeonData.stage != LifeStage.Egg && !pigeonData.isAway)
        {
            hungerBar.gameObject.SetActive(true);
        }

        if (pigeonData.isAway)
        {
            spriteRenderer.enabled = false;
        }
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
        pigeonCollection.SaveGame();
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
        pigeonCollection.SaveGame();
        Debug.Log($"Pigeon named: {pigeonData.pigeonName}");
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || pigeonData.stage == LifeStage.Egg || pigeonData.isAway)
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
            pigeonCollection.SaveGame();
        }
        else
        {
            pigeonCollection.SaveGame();
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
        else if (pigeonData.isAway)
        {
            if (pigeonData.IsBackFromFreeFly())
            {
                ReturnFromFreeFly();
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

            freeFlyButton.gameObject.SetActive(pigeonData.CanFreeFly());
        }
    }

    private void OnFreeFlyClicked()
    {
        pigeonData.StartFreeFly();
        freeFlyButton.gameObject.SetActive(false);
        hungerBar.gameObject.SetActive(false);
        spriteRenderer.enabled = false;
        Debug.Log($"{pigeonData.pigeonName} flew away!");
        pigeonCollection.SaveGame();
    }

    private void ReturnFromFreeFly()
    {
        pigeonData.ReturnFromFreeFly();
        spriteRenderer.enabled = true;
        spriteRenderer.color = Color.white;
        hungerBar.gameObject.SetActive(true);
        Debug.Log($"{pigeonData.pigeonName} came back!");
        BringMate();     
        pigeonCollection.SaveGame();
    }

    private void BringMate()
    {
        if (!string.IsNullOrWhiteSpace(pigeonData.mateID))
        {
            return;
        }

        Pigeon mate = new Pigeon("Mate", "Brown", "Common Pigeon", LifeStage.Adult, BreedRarity.Common);

        if (pigeonData.gender == Gender.Male)
        {
            mate.gender = Gender.Female;
        }
        else
        {
            mate.gender = Gender.Male;
        }

        mate.bond = Pigeon.MaxBond;
        mate.hasFreeFlown = true;
        mate.mateID = pigeonData.pigeonID;
        pigeonData.mateID = mate.pigeonID;

        pigeonCollection.AddPigeon(mate);
        Debug.Log($"A {mate.gender} mate arrived for {pigeonData.pigeonName}!");
    }
}
