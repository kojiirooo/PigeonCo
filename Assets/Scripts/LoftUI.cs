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

    [SerializeField] private Button buyLoft2Button;
    [SerializeField] private Button buyLoft3Button;
    [SerializeField] private TMP_Text loft2Text;
    [SerializeField] private TMP_Text loft3Text;
    [SerializeField] private TMP_Text messageText;

    [SerializeField] private GameObject loadingPanel;

    void Start()
    {
        if (openLoftsButton != null) openLoftsButton.onClick.AddListener(OpenPanel);
        if (closeLoftsButton != null) closeLoftsButton.onClick.AddListener(ClosePanel);
        if (buyLoft2Button != null) buyLoft2Button.onClick.AddListener(OnLoft2Clicked);
        if (buyLoft3Button != null) buyLoft3Button.onClick.AddListener(OnLoft3Clicked);

        loftPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        SetText(messageText, "");
    }

    private void OpenPanel()
    {
        RefreshText();
        loftPanel.SetActive(true);
    }

    private void ClosePanel()
    {
        loftPanel.SetActive(false);
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
        SetText(messageText, "Loading Loft " + loft + "...");

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // Scenes are named Loft1, Loft2, Loft3
        SceneManager.LoadScene("Loft" + loft);
    }

    private void RefreshText()
    {
        SetText(loft2Text, pigeonCollection.loft2Unlocked
            ? "Loft 2: Open"
            : "Loft 2: " + PigeonCollection.Loft2Cost + " coins");

        SetText(loft3Text, pigeonCollection.loft3Unlocked
            ? "Loft 3: Open"
            : "Loft 3: " + PigeonCollection.Loft3Cost + " coins");

        SetButtonLabel(buyLoft2Button, pigeonCollection.loft2Unlocked ? "Go to Loft 2" : "Buy Loft 2");
        SetButtonLabel(buyLoft3Button, pigeonCollection.loft3Unlocked ? "Go to Loft 3" : "Buy Loft 3");
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