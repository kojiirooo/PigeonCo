using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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
    [SerializeField] private Button breedButton;

    [Header("Petting")]
    [SerializeField] private float bondPerPixel = 0.02f;
    [SerializeField] private float tapMoveLimit = 10f;
    [SerializeField] private float debugBond;
    [SerializeField] private float debugRating;

    [Header("Role")]
    [SerializeField] private bool isStarter = true;
    [SerializeField] private PigeonController mateController;
    [SerializeField] private PigeonController nursery1;
    [SerializeField] private PigeonController nursery2;

    private bool isPressing;
    private float dragDistance;
    private Vector3 lastMousePosition;

    void Start()
    {
        if (isStarter)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            freeFlyButton.onClick.AddListener(OnFreeFlyClicked);
            breedButton.onClick.AddListener(OnBreedClicked);
            breedButton.gameObject.SetActive(false);
            namePopup.SetActive(false);
            freeFlyButton.gameObject.SetActive(false);
        }
    }

    void Awake()
    {
        pigeonData = null;
        hungerBar.gameObject.SetActive(false);

        if (!isStarter)
        {
            spriteRenderer.enabled = false;
        }
    }

    public void SetupPigeon(Pigeon data)
    {
        // IMPORTANT:
        // Assign the data before trying to access pigeonData.
        pigeonData = data;

        spriteRenderer.enabled = !pigeonData.isAway;

        Debug.Log(
            $"Pigeon name: {pigeonData.pigeonName}, " +
            $"color: {pigeonData.pigeonColor}, " +
            $"gender: {pigeonData.gender}"
        );

        UpdateSprite();

        bool showBar = pigeonData.stage != LifeStage.Egg && !pigeonData.isAway;
        hungerBar.gameObject.SetActive(showBar);
        spriteRenderer.enabled = !pigeonData.isAway;
    }

    private void UpdateSprite()
    {
        if (pigeonData == null)
        {
            return;
        }

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

        // Only the starter pigeon should use the naming popup.
        if (isStarter)
        {
            namePopup.SetActive(true);
        }

        hungerBar.gameObject.SetActive(true);

        pigeonCollection.SaveGame();
    }

    private void OnConfirmClicked()
    {
        string typedName = nameInput.text.Trim();

        if (typedName == "")
        {
            return;
        }

        pigeonData.pigeonName = typedName;

        namePopup.SetActive(false);

        pigeonCollection.SaveGame();

        Debug.Log($"Pigeon named: {pigeonData.pigeonName}");
    }

    private void OnMouseDown()
    {
        // Don't interact if there is no pigeon data.
        if (pigeonData == null)
        {
            isPressing = false;
            return;
        }

        // Don't interact when clicking UI.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            isPressing = false;
            return;
        }

        // Eggs cannot be fed/petted yet.
        if (pigeonData.stage == LifeStage.Egg)
        {
            isPressing = false;
            return;
        }

        // Away pigeons cannot be interacted with.
        if (pigeonData.isAway)
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
        if (!isPressing || pigeonData == null)
        {
            return;
        }

        Vector3 currentMousePosition = Input.mousePosition;

        float moved = Vector3.Distance(
            currentMousePosition,
            lastMousePosition
        );

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
        if (!isPressing || pigeonData == null)
        {
            return;
        }

        isPressing = false;

        // Small click = feed
        if (dragDistance <= tapMoveLimit)
        {
            pigeonData.Feed();

            Debug.Log(
                $"Fed {pigeonData.pigeonName}. " +
                $"Hunger is now {pigeonData.GetCurrentHunger()}"
            );

            pigeonCollection.SaveGame();
        }
        // Drag = pet
        else
        {
            pigeonCollection.SaveGame();
        }
    }

    void Update()
    {
        // Mate-test starts with no data.
        if (pigeonData == null)
        {
            return;
        }

        // =========================
        // EGG
        // =========================
        if (pigeonData.stage == LifeStage.Egg)
        {
            if (pigeonData.IsReadyToHatch())
            {
                HatchEgg();
            }
        }

        // =========================
        // AWAY / FREE FLYING
        // =========================
        else if (pigeonData.isAway)
        {
            if (pigeonData.IsBackFromFreeFly())
            {
                ReturnFromFreeFly();
            }
        }

        // =========================
        // ACTIVE PIGEON
        // =========================
        else
        {
            float currentHunger = pigeonData.GetCurrentHunger();

            debugHunger = currentHunger;
            debugBond = pigeonData.bond;
            debugRating = pigeonData.GetRating();

            hungerBar.value = currentHunger;

            // Change color when hungry.
            if (pigeonData.IsHungry())
            {
                spriteRenderer.color = hungryColor;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }

            // ONLY THE STARTER CONTROLS THE SHARED FREE FLY BUTTON.
            if (isStarter)
            {
                Pigeon mate = pigeonCollection.FindByID(pigeonData.mateID);
                breedButton.gameObject.SetActive(pigeonData.CanBreedWith(mate));
                freeFlyButton.gameObject.SetActive(
                    pigeonData.CanFreeFly()

                );
            }
        }
    }

    private void OnFreeFlyClicked()
    {
        // Safety check.
        if (pigeonData == null)
        {
            return;
        }

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

        // When the starter pigeon comes back,
        // bring its mate with it.
        BringMate();

        pigeonCollection.SaveGame();
    }

    private void BringMate()
    {
        // Don't create another mate if this pigeon
        // already has one.
        if (!string.IsNullOrWhiteSpace(pigeonData.mateID))
        {
            return;
        }

        // Create the mate.
        Pigeon mate = new Pigeon(
            "Mate",
            "Brown",
            "Common Pigeon",
            LifeStage.Adult,
            BreedRarity.Common
        );

        // Give the mate the opposite gender.
        if (pigeonData.gender == Gender.Male)
        {
            mate.gender = Gender.Female;
        }
        else
        {
            mate.gender = Gender.Male;
        }

        // Give the mate max bond.
        mate.bond = Pigeon.MaxBond;

        // Mark the mate as having free-flown before.
        mate.hasFreeFlown = true;

        // Connect the two pigeons.
        mate.mateID = pigeonData.pigeonID;
        pigeonData.mateID = mate.pigeonID;

        // Save the new mate.
        pigeonCollection.AddPigeon(mate);

        // Show the mate in the Mate-test GameObject.
        if (mateController != null)
        {
            mateController.SetupPigeon(mate);
        }
        else
        {
            Debug.LogWarning(
                "Mate Controller is not assigned on the starter pigeon!"
            );
        }

        Debug.Log(
            $"A {mate.gender} mate arrived for {pigeonData.pigeonName}!"
        );
    }

    private void OnBreedClicked()
    {
        Pigeon mate = pigeonCollection.FindByID(pigeonData.mateID);

        if (pigeonData.CanBreedWith(mate))
        {
            pigeonCollection.Breed(pigeonData, mate);

            // The 2 new eggs are the last 2 in the collection.
            int count = pigeonCollection.ownedPigeons.Count;
            nursery1.SetupPigeon(pigeonCollection.ownedPigeons[count - 2]);
            nursery2.SetupPigeon(pigeonCollection.ownedPigeons[count - 1]);
            
            breedButton.gameObject.SetActive(false);
            Debug.Log("Breeding complete: 2 eggs added.");
        }
    }
}
