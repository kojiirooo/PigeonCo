using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoftUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;

    [SerializeField] private GameObject loftPanel;
    [SerializeField] private Button openLoftsButton;
    [SerializeField] private Button closeLoftsButton;

    [SerializeField] private Button loft1Button;
    [SerializeField] private Button buyLoft2Button;
    [SerializeField] private Button buyLoft3Button;
    [SerializeField] private TMP_Text loft1Text;
    [SerializeField] private TMP_Text loft2Text;
    [SerializeField] private TMP_Text loft3Text;
    [SerializeField] private TMP_Text messageText;

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Button mainMenuButton;

    void Start()
    {
        if (openLoftsButton != null) openLoftsButton.onClick.AddListener(OpenPanel);
        if (closeLoftsButton != null) closeLoftsButton.onClick.AddListener(ClosePanel);
        if (loft1Button != null) loft1Button.onClick.AddListener(OnLoft1Clicked);
        if (buyLoft2Button != null) buyLoft2Button.onClick.AddListener(OnLoft2Clicked);
        if (buyLoft3Button != null) buyLoft3Button.onClick.AddListener(OnLoft3Clicked);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);

        loftPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        SetText(messageText, "");
    }

    public void OpenPanel()
    {
        RefreshText();
        loftPanel.SetActive(true);
    }

    private void ClosePanel()
    {
        loftPanel.SetActive(false);
    }

    private void OnLoft1Clicked()
    {
        HandleLoftButton(1);
    }

    private void OnLoft2Clicked()
    {
        HandleLoftButton(2);
    }

    private void OnLoft3Clicked()
    {
        HandleLoftButton(3);
    }

    // If the loft is open, go to it. If not, try to buy it.
    private void HandleLoftButton(int loft)
    {
        if (pigeonCollection.IsLoftUnlocked(loft))
        {
            GoToLoft(loft);
        }
        else
        {
            BuyLoft(loft);
        }
    }

    private void BuyLoft(int loft)
    {
        if (pigeonCollection.TryUnlockLoft(loft))
        {
            SetText(messageText, "Loft " + loft + " unlocked!");
        }
        else
        {
            int cost = (loft == 2) ? PigeonCollection.Loft2Cost : PigeonCollection.Loft3Cost;
            SetText(messageText, "Not enough coins (need " + cost + ")");
        }

        RefreshText();
    }

    private void GoToLoft(int loft)
    {
        if (pigeonCollection.loftNumber == loft)
        {
            SetText(messageText, "You are already in Loft " + loft + ".");
            return;
        }

        SetText(messageText, "Loading Loft " + loft + "...");

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // Scenes are named Loft1, Loft2, Loft3
        SceneManager.LoadScene("Loft" + loft);
    }

    private void GoToMainMenu()
    {
        // Scene name must match Build Settings exactly
        SceneManager.LoadScene("MainMenu");
    }

    private void RefreshText()
    {
        RefreshLoft(loft1Text, loft1Button, 1, 0);
        RefreshLoft(loft2Text, buyLoft2Button, 2, PigeonCollection.Loft2Cost);
        RefreshLoft(loft3Text, buyLoft3Button, 3, PigeonCollection.Loft3Cost);
    }

    private void RefreshLoft(TMP_Text label, Button button, int loft, int cost)
    {
        bool unlocked = pigeonCollection.IsLoftUnlocked(loft);
        bool isCurrent = pigeonCollection.loftNumber == loft;

        string status;
        if (isCurrent)
        {
            status = "Loft " + loft + ": Current";
        }
        else if (unlocked)
        {
            status = "Loft " + loft + ": Open";
        }
        else
        {
            status = "Loft " + loft + ": " + cost + " coins";
        }
        SetText(label, status);

        string buttonLabel;
        if (isCurrent)
        {
            buttonLabel = "You are here";
        }
        else if (unlocked)
        {
            buttonLabel = "Go to Loft " + loft;
        }
        else
        {
            buttonLabel = "Buy Loft " + loft;
        }
        SetButtonLabel(button, buttonLabel);

        // The loft you're standing in can't be pressed
        if (button != null)
        {
            button.interactable = !isCurrent;
        }
    }

    private void SetButtonLabel(Button button, string label)
    {
        if (button == null)
        {
            return;
        }

        SetText(button.GetComponentInChildren<TMP_Text>(), label);
    }

    private void SetText(TMP_Text target, string value)
    {
        if (target != null)
        {
            target.text = value;
        }
    }
}