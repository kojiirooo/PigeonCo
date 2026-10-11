using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PigeonInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text ratingText;
    [SerializeField] private TMP_Text hungerText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button feedButton;

    [SerializeField] private Button petButton;
    [SerializeField] private TMP_Text bondText;

    private PigeonController currentController;
    private Pigeon currentPigeon;

    void Awake()
    {
        backButton.onClick.AddListener(Hide);
        feedButton.onClick.AddListener(OnFeedClicked);
        petButton.onClick.AddListener(OnPetClicked);
        panel.SetActive(false);
    }

    public void Show(PigeonController controller)
    {
        if (controller == null || controller.pigeonData == null)
        {
            return;
        }

        currentController = controller;
        currentPigeon = controller.pigeonData;
        panel.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        currentController = null;
        currentPigeon = null;
        panel.SetActive(false);
    }

    private void OnFeedClicked()
    {
        if (currentController == null)
        {
            return;
        }

        currentController.FeedPigeon();
        Refresh();
    }

    void Update()
    {
        // Keep the numbers live while the panel is open.
        if (currentPigeon != null)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        if (currentPigeon == null)
        {
            return;
        }

        string displayName = string.IsNullOrWhiteSpace(currentPigeon.pigeonName)
            ? "Unnamed"
            : currentPigeon.pigeonName;

        nameText.text = displayName;
        stageText.text = currentPigeon.stage.ToString();
        ratingText.text = "Rating: " + Mathf.RoundToInt(currentPigeon.GetRating());
        hungerText.text = "Hunger: " + Mathf.RoundToInt(currentPigeon.GetCurrentHunger()) + "%";
        bondText.text = "Bond: " + Mathf.RoundToInt(currentPigeon.bond) + "/" + Mathf.RoundToInt(Pigeon.MaxBond);
    }

    private void OnPetClicked()
    {
        if (currentController == null)
        {
            return;
        }

        currentController.PetPigeon();
        Refresh();
    }
}