using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ReleaseConfirmUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Pigeon pendingPigeon;

    void Start()
    {
        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);
        confirmPanel.SetActive(false);
    }

    // Called by PigeonSelection when the Let Go button is pressed
    public void Ask(Pigeon p)
    {
        if (p == null)
        {
            return;
        }

        pendingPigeon = p;

        string displayName = string.IsNullOrWhiteSpace(p.pigeonName) ? "this pigeon" : p.pigeonName;

        if (questionText != null)
        {
            questionText.text = "Do you want to let go of " + displayName + "? This cannot be undone.";
        }

        confirmPanel.SetActive(true);
    }

    private void OnYesClicked()
    {
        if (pendingPigeon != null)
        {
            pigeonCollection.ReleasePigeon(pendingPigeon);
        }

        pendingPigeon = null;
        confirmPanel.SetActive(false);

        // Reload the scene so the nursery and starter stop showing the released pigeon.
        // The save is already written, so nothing is lost.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnNoClicked()
    {
        pendingPigeon = null;
        confirmPanel.SetActive(false);
    }
}