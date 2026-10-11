using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private Button raceButton;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private RaceTrackUI raceTrack;

    void Start()
    {
        raceButton.onClick.AddListener(OnRaceClicked);
        resultText.text = "";
    }

    void Update()
    {
        coinsText.text = "Coins: " + pigeonCollection.coins;
    }

    private void OnRaceClicked()
    {
        Pigeon racer = PigeonSelection.Instance.Selected?.pigeonData;

        if (racer == null || !pigeonCollection.CanRace(racer))
        {
            return;
        }

        // Use the current loft's opponents and prize
        int loft = pigeonCollection.loftNumber;

        RaceResult full = RaceManager.RunFullRace(racer, loft);
        int place = full.place;
        int prize = RaceManager.GetPrize(place, loft);
        pigeonCollection.AddCoins(prize);

        // Open the track right away
        if (raceTrack != null)
        {
            raceTrack.ShowResult(full);
        }

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