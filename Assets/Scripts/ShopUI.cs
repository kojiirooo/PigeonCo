using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button useButton;
    [SerializeField] private TMP_Text useButtonText;
    [SerializeField] private TMP_Text messageText;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyClicked);
        useButton.onClick.AddListener(OnUseClicked);
        messageText.text = "";
    }

    void Update()
    {
        if (pigeonCollection.premiumFeed <= 0)
        {
            pigeonCollection.premiumFeedArmed = false;
        }

        if (pigeonCollection.premiumFeedArmed)
        {
            useButtonText.text = "Tap a pigeon...";
        }
        else
        {
            useButtonText.text = "Use Premium Feed (" + pigeonCollection.premiumFeed + ")";
        }

        useButton.interactable = pigeonCollection.premiumFeed > 0;
    }

    private void OnBuyClicked()
    {
        if (pigeonCollection.TryBuyPremiumFeed())
        {
            messageText.text = "Bought Premium Feed!";
        }
        else
        {
            messageText.text = "Not enough coins (need " + PigeonCollection.PremiumFeedCost + ")";
        }
    }

    private void OnUseClicked()
    {
        pigeonCollection.premiumFeedArmed = !pigeonCollection.premiumFeedArmed;
    }
}