using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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

    [Header("Info Panel")]
    [SerializeField] private PigeonInfoPanel infoPanel;

    [Header("Petting")]
    [SerializeField] private float bondPerPixel = 0.02f;
    [SerializeField] private float tapMoveLimit = 10f;
    [SerializeField] private float petBondAmount = 10f;
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

    [Header("Wandering")]
    [SerializeField] private bool canWander = false;
    [SerializeField] private float wanderRadius = 2f;
    [SerializeField] private float wanderSpeed = 0.5f;
    [SerializeField] private float minPause = 1f;
    [SerializeField] private float maxPause = 3f;

    private Vector3 homePosition;
    private Vector3 wanderTarget;
    private float pauseTimer;

    [Header("Hunger Bar Follow")]
    [SerializeField] private Vector3 hungerBarOffset = new Vector3(0f, 0.8f, 0f);

    void Start()
    {
        homePosition = transform.position;
        wanderTarget = homePosition;
        if (isStarter)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            breedButton.gameObject.SetActive(false);
            namePopup.SetActive(false);
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

    public bool IsStarter => isStarter;

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
        else
        {
            int number = pigeonCollection.ownedPigeons.IndexOf(pigeonData) + 1;
            pigeonData.pigeonName = "Pigeon " + number;
        }

        
        pigeonCollection.DiscoverColor(pigeonData.pigeonColor);
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

    private void OnMouseOver()
    {
        // Right-click opens the info panel.
        if (!Input.GetMouseButtonDown(1))
        {
            return;
        }

        if (pigeonData == null || infoPanel == null)
        {
            return;
        }

        // Don't open when clicking through UI.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Away pigeons are not visible, so no info.
        if (pigeonData.isAway)
        {
            return;
        }

        infoPanel.Show(this);
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
        PigeonSelection.Instance.Select(this);
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
            if (pigeonCollection.ringArmed && pigeonCollection.UseRing(pigeonData))
            {
                Debug.Log(pigeonData.pigeonName + " got a ring!");
            }
            else if (pigeonCollection.wingbandArmed && pigeonCollection.UseWingband(pigeonData))
            {
                Debug.Log(pigeonData.pigeonName + " got a wingband!");
            }
            else if (pigeonCollection.premiumFeedArmed && pigeonCollection.UsePremiumFeed(pigeonData))
            {
                Debug.Log($"Premium fed {pigeonData.pigeonName}.");
            }
            else
            {
                // Normal feed
                FeedPigeon();
                Debug.Log($"Fed {pigeonData.pigeonName}. Hunger is now {pigeonData.GetCurrentHunger()}");
            }

            pigeonCollection.SaveGame();
        }
        // Drag = pet
        else
        {
            pigeonCollection.SaveGame();
        }
    }

    public void FeedPigeon()
    {
        if (pigeonData == null || pigeonData.stage == LifeStage.Egg || pigeonData.isAway)
        {
            return;
        }

        if (pigeonData.isWild)
        {
            pigeonData.hunger = 100f;
            pigeonData.lastFedTime = System.DateTime.Now;
            pigeonData.tameSteps++;
            if (pigeonData.tameSteps >= Pigeon.TameStepsNeeded)
            {
                pigeonData.isWild = false;
                pigeonData.bond = 0f;
                if (string.IsNullOrWhiteSpace(pigeonData.pigeonName)) pigeonData.pigeonName = "Wild " + pigeonCollection.ownedPigeons.IndexOf(pigeonData);
                GameMessage.Instance?.Show(pigeonData.pigeonName + " is tamed!", 4f);
            }
            else
            {
                GameMessage.Instance?.Show("Tame " + pigeonData.tameSteps + "/" + Pigeon.TameStepsNeeded, 2f);
            }
            pigeonCollection.SaveGame();
            return;
        }


        pigeonData.Feed();
        SoundManager.Instance?.PlayFeed();
        GameMessage.Instance?.Show(
            pigeonData.pigeonName + " is fed. Hunger " +
            Mathf.RoundToInt(pigeonData.GetCurrentHunger()) + "%"
        );
        pigeonCollection.SaveGame();
    }

    public void PetPigeon()
    {
        if (pigeonData == null || pigeonData.stage == LifeStage.Egg || pigeonData.isAway)
        {
            return;
        }

        float bondBefore = pigeonData.bond;
        pigeonData.AddBond(petBondAmount);

        // AddBond does nothing when the pigeon is hungry, so tell the player why
        if (pigeonData.bond > bondBefore)
        {
            GameMessage.Instance?.Show(
                pigeonData.pigeonName + " likes you. Bond " +
                Mathf.RoundToInt(pigeonData.bond) + "/100"
            );
        }
        else
        {
            GameMessage.Instance?.Show(pigeonData.pigeonName + " is too hungry to bond. Feed first.");
        }

        if (pigeonData.TryGrow())
        {
            UpdateSprite();
            GameMessage.Instance?.Show(
                pigeonData.pigeonName + " has grown into a " + pigeonData.stage + "!",
                4f
            );
        }

        pigeonCollection.SaveGame();
    }

    void Update()
    {
        // Mate-test starts with no data.
        if (pigeonData == null)
        {
            return;
        }

        // EGG
        if (pigeonData.stage == LifeStage.Egg)
        {
            if (pigeonData.IsReadyToHatch())
            {
                HatchEgg();
            }
        }

        // AWAY / FREE FLYING
        else if (pigeonData.isAway)
        {
            if (pigeonData.IsBackFromFreeFly())
            {
                ReturnFromFreeFly();
            }
        }

        // ACTIVE PIGEON
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

            
            if (pigeonData.UpdateDirt(Time.deltaTime))
            {
                GameMessage.Instance?.Show(pigeonData.pigeonName + " caught a cold! Clean the loft.", 4f);
                pigeonCollection.SaveGame();
            }
           
            if (!pigeonData.hasCold)
            {
                spriteRenderer.color = ColorTint(pigeonData.pigeonColor);
            }

            if (pigeonData.hasCold)
            {
                spriteRenderer.color = new Color(0.6f, 0.8f, 1f);
            }


            // Only chicks and up wander, and only when not being pressed
            if (canWander && !isPressing)
            {
                Wander();
            }
        }
    }

    private void Wander()
    {
        // Pause at a spot before walking again
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            wanderTarget,
            wanderSpeed * Time.deltaTime
        );

        // Reached the spot: pick a new one near home
        if (Vector3.Distance(transform.position, wanderTarget) < 0.01f)
        {
            wanderTarget = homePosition + (Vector3)(Random.insideUnitCircle * wanderRadius);
            pauseTimer = Random.Range(minPause, maxPause);
        }
    }

    private void FollowHungerBar()
    {
        // Only move the bar when it's visible
        if (hungerBar == null || !hungerBar.gameObject.activeSelf)
        {
            return;
        }

        // Turn the pigeon's world position into a screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + hungerBarOffset);

        RectTransform barRect = hungerBar.transform as RectTransform;
        barRect.position = screenPos;
    }

    void LateUpdate()
    {
        // LateUpdate runs after the wandering moves, so the bar doesn't lag behind
        if (pigeonData == null)
        {
            return;
        }

        FollowHungerBar();
    }

    public void OnFreeFlyClicked()
    {
        // Safety check.
        if (pigeonData == null)
        {
            return;
        }

        pigeonData.StartFreeFly();
        pigeonData.lastFreeFlyTime = System.DateTime.Now;

        // Only some controllers have these assigned, so check first.
        if (hungerBar != null)
        {
            hungerBar.gameObject.SetActive(false);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        GameMessage.Instance?.Show(pigeonData.pigeonName + " flew away. Back in about 15 seconds.");

        pigeonCollection.SaveGame();
    }

    private void ReturnFromFreeFly()
    {
        pigeonData.ReturnFromFreeFly();

        spriteRenderer.enabled = true;
        spriteRenderer.color = Color.white;

        hungerBar.gameObject.SetActive(true);

        GameMessage.Instance?.Show(pigeonData.pigeonName + " is back!", 4f);

        // When the starter pigeon comes back,
        // bring its mate with it.
        if (isStarter)
        {
            BringMate();
        }

       
        bool hasRoom = pigeonCollection.CountPigeonsInLoft(pigeonCollection.loftNumber)
                       < pigeonCollection.GetCapacity(pigeonCollection.loftNumber);

        if (hasRoom && Random.value < 0.25f)
        {
            Pigeon wild = pigeonCollection.CreateWildPigeon();
            pigeonCollection.AddPigeon(wild);
            pigeonCollection.SaveGame();

            // Reload so GameTester puts the wild pigeon on a visible slot
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
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

        // The mate starts with no bond. Pet it to build bond.
        mate.bond = 0f;
        mate.isWild = true;    
        mate.tameSteps = 0;    

        // Mark the mate as having free-flown before.
        mate.hasFreeFlown = false;

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

        GameMessage.Instance?.Show("A wild mate arrived for " + pigeonData.pigeonName + "!", 4f);
    }

    public void OnBreedClicked()
    {
        Pigeon mate = pigeonCollection.FindByID(pigeonData.mateID);

        if (!pigeonData.CanBreedWith(mate))
        {
            return;
        }

        if (!pigeonCollection.Breed(pigeonData, mate))
        {
            GameMessage.Instance?.Show("Loft full! Move a pigeon to another loft, or breed in a loft with room."); 
            return;
        }

        // The 2 new eggs are the last 2 in the collection.
        int count = pigeonCollection.ownedPigeons.Count;

        if (nursery1 != null)
        {
            nursery1.SetupPigeon(pigeonCollection.ownedPigeons[count - 2]);
        }

        if (nursery2 != null)
        {
            nursery2.SetupPigeon(pigeonCollection.ownedPigeons[count - 1]);
        }

        if (breedButton != null)
        {
            breedButton.gameObject.SetActive(false);
        }

        GameMessage.Instance?.Show(pigeonData.pigeonName + " and " + mate.pigeonName + " had 2 eggs!", 4f);
    }

    
    private Color ColorTint(string color)
    {
        switch (color)
        {
            case "Brown": return new Color(0.75f, 0.55f, 0.35f);
            case "White": return Color.white;
            case "Black": return new Color(0.35f, 0.35f, 0.4f);
            default: return new Color(0.85f, 0.85f, 0.9f);   // Grey
        }
    }

}