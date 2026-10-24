using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private GameObject collectionPanel;
    [SerializeField] private Button collectionButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text listText;

    void Start()
    {
        collectionButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);
        collectionPanel.SetActive(false);
    }

    private void OpenPanel()
    {
        string text = "";

        foreach (Pigeon p in pigeonCollection.ownedPigeons)
        {
            string pigeonName = string.IsNullOrWhiteSpace(p.pigeonName) ? "(unnamed)" : p.pigeonName;
            text += pigeonName + "  -  " + p.stage + "  -  " + p.gender
                    + "  -  Rating " + Mathf.RoundToInt(p.GetRating())
                    + (p.isAway ? "  (away)" : "") + "\n"
                    + (p.hasCold ? "  -  COLD" : "") + "\n";
        }

        
        text += "\nColors found: " + pigeonCollection.discoveredColors.Count + " / " + PigeonCollection.AllColors.Length + "\n";
        foreach (string c in PigeonCollection.AllColors)
        {
            text += (pigeonCollection.discoveredColors.Contains(c) ? "[x] " : "[ ] ") + c + "\n";
        }

        listText.text = text;
        collectionPanel.SetActive(true);
    }

    private void ClosePanel()
    {
        collectionPanel.SetActive(false);
    }
}