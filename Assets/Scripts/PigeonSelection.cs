using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PigeonSelection : MonoBehaviour
{
    public static PigeonSelection Instance { get; private set; }

    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private Button freeFlyButton;
    [SerializeField] private Button breedButton;
    [SerializeField] private Button raceButton;
    [SerializeField] private TMP_Text freeFlyCooldownText;
    [SerializeField] private TMP_Text breedCooldownText;
    [SerializeField] private TMP_Text raceLockText;
    [SerializeField] private ReleaseConfirmUI releaseConfirm;
    [SerializeField] private TMP_Text freeFlyButtonText;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private LoftUI loftUI;   
    [SerializeField] private Button pairButton;   
    [SerializeField] private MoveUI moveUI;

    public PigeonController Selected { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        freeFlyButton.onClick.AddListener(OnFreeFlyPressed);
        breedButton.onClick.AddListener(OnBreedPressed);
        moveButton.onClick.AddListener(OnMovePressed);
        if (sellButton != null) sellButton.onClick.AddListener(OnSellPressed);
        if (pairButton != null) pairButton.onClick.AddListener(OnPairPressed);
        HideAll();
    }

    void Update()
    {
        // A click on empty space (not a pigeon, not a button) clears the selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!ClickedPigeon())
            {
                Deselect();
            }
        }

        RefreshButtons();
    }

    public void Select(PigeonController pigeon)
    {
        // Tapping the same pigeon again deselects it
        if (Selected == pigeon)
        {
            Deselect();
            return;
        }

        Selected = pigeon;
        Debug.Log("Selected: " + pigeon.pigeonData.pigeonName);
    }

    private bool ClickedPigeon()
    {
        Camera cam = Camera.main;
        Vector3 mouse = Input.mousePosition;

        // 2D colliders (sprites with Box Collider 2D)
        Vector2 world = cam.ScreenToWorldPoint(mouse);
        Collider2D hit2D = Physics2D.OverlapPoint(world);
        if (hit2D != null && hit2D.GetComponentInParent<PigeonController>() != null)
        {
            return true;
        }

        // 3D colliders
        Ray ray = cam.ScreenPointToRay(mouse);
        if (Physics.Raycast(ray, out RaycastHit hit3D) &&
            hit3D.collider.GetComponentInParent<PigeonController>() != null)
        {
            return true;
        }

        return false;
    }

    public void Deselect()
    {
        Selected = null;
    }

    private void RefreshButtons()
    {
        if (Selected == null || Selected.pigeonData == null)
        {
            HideAll();
            return;
        }

        Pigeon p = Selected.pigeonData;
        Pigeon mate = pigeonCollection.FindByID(p.mateID);

        bool isAdult = p.stage == LifeStage.Adult;
        bool childrenGrown = !pigeonCollection.HasUnfinishedChildren(p);
        bool hasBond = p.bond >= Pigeon.MaxBond;

        // Free Fly, or Let Go for pigeons under full bond
        bool lowBond = !hasBond;
        bool freeFlyAllowed = lowBond ? (isAdult && !p.isAway) : p.CanFreeFly();

        freeFlyButton.gameObject.SetActive(isAdult);
        freeFlyButton.interactable = freeFlyAllowed && childrenGrown;
        SetButtonLabel(freeFlyButtonText, lowBond ? "Let Go" : "Free Fly");

        string freeFlyStatus;
        if (lowBond)
        {
            freeFlyStatus = childrenGrown ? "" : "Chicks growing";
        }
        else
        {
            freeFlyStatus = GetStatusText(childrenGrown, true, "free fly", p.GetFreeFlyCooldownLeft());
        }
        SetStatusLabel(freeFlyCooldownText, freeFlyStatus);

        // Breed: both parents need full bond and finished cooldowns
        bool mateHasBond = mate == null || mate.bond >= Pigeon.MaxBond;
        System.TimeSpan breedLeft = p.GetBreedCooldownLeft();
        if (mate != null && mate.GetBreedCooldownLeft() > breedLeft)
        {
            breedLeft = mate.GetBreedCooldownLeft();
        }

        breedButton.gameObject.SetActive(isAdult);
        breedButton.interactable = p.CanBreedWith(mate) && childrenGrown;

        //if the loft has no room for 2 eggs, say so
        if (!pigeonCollection.CanBreedInLoft())
        {
            SetStatusLabel(breedCooldownText, "Loft full");
        }

        SetStatusLabel(breedCooldownText,
            GetStatusText(childrenGrown, hasBond && mateHasBond, "breed", breedLeft));

        // Race
        raceButton.gameObject.SetActive(isAdult);
        raceButton.interactable = pigeonCollection.CanRace(p);
        SetStatusLabel(raceLockText,
            GetStatusText(childrenGrown, hasBond, "race", System.TimeSpan.Zero));
        // Move: available for any pigeon that is not an egg and not away
        moveButton.gameObject.SetActive(isAdult);
        moveButton.interactable = p.stage != LifeStage.Egg && !p.isAway;

        
        if (sellButton != null)
        {
            sellButton.gameObject.SetActive(isAdult);
            sellButton.interactable = pigeonCollection.CanSell(p);
        }

        
        if (pairButton != null)
        {
            pairButton.gameObject.SetActive(isAdult);
        }
    }

    private string GetStatusText(bool childrenGrown, bool hasBond, string action, System.TimeSpan cooldownLeft)
    {
        if (!childrenGrown)
        {
            return "Chicks growing";
        }

        if (!hasBond)
        {
            return "Need " + Pigeon.MaxBond + " bond to " + action;
        }

        if (cooldownLeft > System.TimeSpan.Zero)
        {
            return string.Format("Cooldown {0:00}:{1:00}", (int)cooldownLeft.TotalMinutes, cooldownLeft.Seconds);
        }

        return "";
    }

    private void SetStatusLabel(TMP_Text label, string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    private void SetButtonLabel(TMP_Text label, string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    private void OnFreeFlyPressed()
    {
        if (Selected == null || Selected.pigeonData == null)
        {
            return;
        }

        Pigeon p = Selected.pigeonData;

        if (p.bond < Pigeon.MaxBond)
        {
            // Low bond: offer to let the pigeon go instead
            if (releaseConfirm != null)
            {
                releaseConfirm.Ask(p);
            }
            return;
        }

        Selected.OnFreeFlyClicked();
    }

    private void OnBreedPressed()
    {
        if (Selected == null) return;

        
        if (!pigeonCollection.CanBreedInLoft() && loftUI != null)
        {
            loftUI.OpenPanel();
            GameMessage.Instance?.Show("Loft full! Buy Loft 2, then move a pigeon there to breed.");
            return;
        }

        Selected.OnBreedClicked();
        Deselect();
    }

    private void OnMovePressed()
    {
        if (Selected == null || Selected.pigeonData == null)
        {
            return;
        }

        if (moveUI != null)
        {
            moveUI.Ask(Selected.pigeonData);
        }
    }

    // NEW (Step 2)
    private void OnSellPressed()
    {
        if (Selected == null || Selected.pigeonData == null)
        {
            return;
        }

        if (releaseConfirm != null)
        {
            releaseConfirm.AskSell(Selected.pigeonData);
        }
    }

   
    private void OnPairPressed()
    {
        if (Selected == null || Selected.pigeonData == null) return;

        Pigeon p = Selected.pigeonData;
        Pigeon next = pigeonCollection.NextPartnerFor(p);

        if (next == null)
        {
            pigeonCollection.Unpair(p);
            GameMessage.Instance?.Show("No partner available. Needs an adult of the other gender in this loft.");
            return;
        }

        pigeonCollection.Pair(p, next);
        GameMessage.Instance?.Show(p.pigeonName + " is paired with " + next.pigeonName + ".");
    }

    private void HideAll()
    {
        freeFlyButton.gameObject.SetActive(false);
        breedButton.gameObject.SetActive(false);
        raceButton.gameObject.SetActive(false);
        moveButton.gameObject.SetActive(false);
        if (sellButton != null) sellButton.gameObject.SetActive(false);
        if (pairButton != null) pairButton.gameObject.SetActive(false); 
    }
}