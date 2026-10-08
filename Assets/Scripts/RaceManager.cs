using UnityEngine;

public static class RaceManager
{
    public const int OpponentCount = 3;

    // Returns the place the player finished in (1 = first).
    public static int RunRace(Pigeon player)
    {
        float wingbandBonus = player.hasWingband ? Pigeon.WingbandRaceBonus : 0f;
        float playerScore = player.GetRating() + Random.Range(0f, 20f);
        int place = 1;

        Debug.Log($"{player.pigeonName} race score: {playerScore}");

        for (int i = 0; i < OpponentCount; i++)
        {
            float opponentRating = player.GetBaseRating() + Random.Range(-15f, 15f);
            float opponentScore = opponentRating + Random.Range(0f, 20f);

            Debug.Log($"Opponent {i + 1} race score: {opponentScore}");

            if (opponentScore > playerScore)
            {
                place++;
            }
        }

        return place;
    }


    public static int GetPrize(int place)
    {
        switch (place)
        {
            case 1: return 100;
            case 2: return 50;
            case 3: return 20;
            default: return 0;
        }
    }
}