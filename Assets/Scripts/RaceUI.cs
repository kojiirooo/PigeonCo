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
        
    }

    private Pigeon FindRacer()
    {
        Pigeon best = null;

        foreach (Pigeon p in pigeonCollection.ownedPigeons)
        {
            if (pigeonCollection.CanRace(p))
            {
                if (best == null || p.GetRating() > best.GetRating())
                {
                    best = p;
                }
            }
        }

        return best;
    }

    private void OnRaceClicked()
    {
        Pigeon racer = PigeonSelection.Instance.Selected?.pigeonData;

        if (racer == null || !pigeonCollection.CanRace(racer))
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