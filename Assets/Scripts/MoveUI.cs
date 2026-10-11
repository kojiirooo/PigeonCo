using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MoveUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private GameObject movePanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button loft1Button;
    [SerializeField] private Button loft2Button;
    [SerializeField] private Button loft3Button;
    [SerializeField] private Button cancelButton;

    private Pigeon pendingPigeon;

    void Start()
    {
        if (loft1Button != null) loft1Button.onClick.AddListener(() => MoveTo(1));
        if (loft2Button != null) loft2Button.onClick.AddListener(() => MoveTo(2));
        if (loft3Button != null) loft3Button.onClick.AddListener(() => MoveTo(3));
        if (cancelButton != null) cancelButton.onClick.AddListener(Close);

        movePanel.SetActive(false);
    }

    // Called by PigeonSelection when the Move button is pressed
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
            questionText.text = "Move " + displayName + " to which loft?";
        }

        // Only unlocked, non-full lofts can be pressed
        SetLoftButton(loft1Button, 1);
        SetLoftButton(loft2Button, 2);
        SetLoftButton(loft3Button, 3);

        movePanel.SetActive(true);
    }

    private void SetLoftButton(Button button, int loft)
    {
        if (button != null)
        {
            button.interactable = pigeonCollection.CanMoveTo(pendingPigeon, loft);
        }
    }

    private void MoveTo(int loft)
    {
        if (pendingPigeon == null)
        {
            return;
        }

        bool moved = pigeonCollection.TryMovePigeon(pendingPigeon, loft);
        pendingPigeon = null;
        movePanel.SetActive(false);

        if (moved)
        {
            // Reload so the moved pigeon disappears from this loft's view
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Close()
    {
        pendingPigeon = null;
        movePanel.SetActive(false);
    }
}