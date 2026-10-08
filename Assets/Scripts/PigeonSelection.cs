using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PigeonSelection : MonoBehaviour
{
    public static PigeonSelection Instance { get; private set; }

    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private Button freeFlyButton;
    [SerializeField] private Button breedButton;
    [SerializeField] private Button raceButton;

    public PigeonController Selected { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        freeFlyButton.onClick.AddListener(OnFreeFlyPressed);
        breedButton.onClick.AddListener(OnBreedPressed);
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

        freeFlyButton.gameObject.SetActive(p.CanFreeFly());
        breedButton.gameObject.SetActive(p.CanBreedWith(mate));
        raceButton.gameObject.SetActive(pigeonCollection.CanRace(p));
    }

    private void OnFreeFlyPressed()
    {
        if (Selected == null) return;

        Selected.OnFreeFlyClicked();
        Deselect();
    }

    private void OnBreedPressed()
    {
        if (Selected == null) return;

        Selected.OnBreedClicked();
        Deselect();
    }
    private void HideAll()
    {
        freeFlyButton.gameObject.SetActive(false);
        breedButton.gameObject.SetActive(false);
        raceButton.gameObject.SetActive(false);
    }
}