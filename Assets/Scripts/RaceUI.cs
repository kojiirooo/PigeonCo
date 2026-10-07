using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private Button raceButton;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text resultText;

    void Start()
    {
        raceButton.onClick.AddListener(OnRaceClicked);
        resultText.text = "";
    }

    void Update()
    {
        coinsText.text = "Coins: " + pigeonCollection.coins;
        raceButton.gameObject.SetActive(FindRacer() != null);
    }

    private Pigeon FindRacer()
    {
        foreach (Pigeon p in pigeonCollection.ownedPigeons)
        {
            if (pigeonCollection.CanRace(p))
            {
                return p;
            }
        }
        return null;
    }

    private void OnRaceClicked()
    {
        Pigeon racer = FindRacer();

        if (racer == null)
        {
            return;
        }

        int place = RaceManager.RunRace(racer);
        int prize = RaceManager.GetPrize(place);
        pigeonCollection.AddCoins(prize);

        string racerName = string.IsNullOrWhiteSpace(racer.pigeonName) ? "Your pigeon" : racer.pigeonName;
        resultText.text = racerName + " finished " + Ordinal(place) + "! +" + prize + " coins";
    }

    private string Ordinal(int place)
    {
        switch (place)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            default: return place + "th";
        }
    }
}