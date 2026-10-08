using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private TMP_Text messageText;

    [Header("Shop Panel")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button closeShopButton;

    [Header("Premium Feed")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Button useButton;
    [SerializeField] private TMP_Text useButtonText;

    [Header("Ring")]
    [SerializeField] private Button buyRingButton;
    [SerializeField] private Button useRingButton;
    [SerializeField] private TMP_Text useRingText;

    [Header("Wingband")]
    [SerializeField] private Button buyWingbandButton;
    [SerializeField] private Button useWingbandButton;
    [SerializeField] private TMP_Text useWingbandText;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyFeedClicked);
        useButton.onClick.AddListener(OnUseFeedClicked);
        buyRingButton.onClick.AddListener(OnBuyRingClicked);
        useRingButton.onClick.AddListener(OnUseRingClicked);
        buyWingbandButton.onClick.AddListener(OnBuyWingbandClicked);
        useWingbandButton.onClick.AddListener(OnUseWingbandClicked);
        messageText.text = "";
        shopButton.onClick.AddListener(OpenShop);
        closeShopButton.onClick.AddListener(CloseShop);
        shopPanel.SetActive(false);
    }

    void Update()
    {
        // Turn off an armed item when the stock runs out.
        if (pigeonCollection.premiumFeed <= 0) pigeonCollection.premiumFeedArmed = false;
        if (pigeonCollection.rings <= 0) pigeonCollection.ringArmed = false;
        if (pigeonCollection.wingbands <= 0) pigeonCollection.wingbandArmed = false;

        useButtonText.text = pigeonCollection.premiumFeedArmed
            ? "Tap a pigeon..." : "Use Premium Feed (" + pigeonCollection.premiumFeed + ")";
        useRingText.text = pigeonCollection.ringArmed
            ? "Tap a pigeon..." : "Use Ring (" + pigeonCollection.rings + ")";
        useWingbandText.text = pigeonCollection.wingbandArmed
            ? "Tap a pigeon..." : "Use Wingband (" + pigeonCollection.wingbands + ")";

        useButton.interactable = pigeonCollection.premiumFeed > 0;
        useRingButton.interactable = pigeonCollection.rings > 0;
        useWingbandButton.interactable = pigeonCollection.wingbands > 0;
    }

    private void OpenShop()
    {
        shopPanel.SetActive(true);
    }

    private void CloseShop()
    {
        shopPanel.SetActive(false);
    }
    private void DisarmAll()
    {
        pigeonCollection.premiumFeedArmed = false;
        pigeonCollection.ringArmed = false;
        pigeonCollection.wingbandArmed = false;
    }

    private void OnBuyFeedClicked()
    {
        if (pigeonCollection.TryBuyPremiumFeed())
            messageText.text = "Bought Premium Feed!";
        else
            messageText.text = "Not enough coins (need " + PigeonCollection.PremiumFeedCost + ")";
    }

    private void OnBuyRingClicked()
    {
        if (pigeonCollection.TryBuyRing())
            messageText.text = "Bought a Ring!";
        else
            messageText.text = "Not enough coins (need " + PigeonCollection.RingCost + ")";
    }

    private void OnBuyWingbandClicked()
    {
        if (pigeonCollection.TryBuyWingband())
            messageText.text = "Bought a Wingband!";
        else
            messageText.text = "Not enough coins (need " + PigeonCollection.WingbandCost + ")";
    }

    private void OnUseFeedClicked()
    {
        bool turnOn = !pigeonCollection.premiumFeedArmed;
        DisarmAll();
        pigeonCollection.premiumFeedArmed = turnOn;
    }

    private void OnUseRingClicked()
    {
        bool turnOn = !pigeonCollection.ringArmed;
        DisarmAll();
        pigeonCollection.ringArmed = turnOn;
    }

    private void OnUseWingbandClicked()
    {
        bool turnOn = !pigeonCollection.wingbandArmed;
        DisarmAll();
        pigeonCollection.wingbandArmed = turnOn;
    }
}