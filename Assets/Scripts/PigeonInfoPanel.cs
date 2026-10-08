using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PigeonInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text ratingText;
    [SerializeField] private Button backButton;

    private Pigeon currentPigeon;

    void Awake()
    {
        backButton.onClick.AddListener(Hide);
        panel.SetActive(false);
    }

    public void Show(Pigeon pigeon)
    {
        if (pigeon == null)
        {
            return;
        }

        currentPigeon = pigeon;
        panel.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        currentPigeon = null;
        panel.SetActive(false);
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
        string displayName = string.IsNullOrWhiteSpace(currentPigeon.pigeonName)
            ? "Unnamed"
            : currentPigeon.pigeonName;

        nameText.text = displayName;
        stageText.text = currentPigeon.stage.ToString();
        ratingText.text = "Rating: " + Mathf.RoundToInt(currentPigeon.GetRating());
    }
}