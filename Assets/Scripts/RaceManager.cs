using UnityEngine;

public static class RaceManager
{
    // Kept for the old call, which uses Loft 1 settings
    public const int OpponentCount = 3;

    public static int GetOpponentCount(int loft)
    {
        switch (loft)
        {
            case 1: return 3;
            case 2: return 5;
            case 3: return 7;
            default: return 3;
        }
    }

    // Opponents in higher lofts are stronger
    public static float GetOpponentBonus(int loft)
    {
        switch (loft)
        {
            case 2: return 10f;
            case 3: return 20f;
            default: return 0f;
        }
    }

    // The old call still works and uses Loft 1 settings
    public static int RunRace(Pigeon player)
    {
        return RunRace(player, 1);
    }

    // Returns the place the player finished in (1 = first)
    public static int RunRace(Pigeon player, int loft)
    {
        int opponents = GetOpponentCount(loft);
        float opponentBonus = GetOpponentBonus(loft);

        float wingbandBonus = player.hasWingband ? Pigeon.WingbandRaceBonus : 0f;
        float playerScore = player.GetRating() + wingbandBonus + Random.Range(0f, 20f);
        int place = 1;

        Debug.Log($"{player.pigeonName} race score: {playerScore}");

        for (int i = 0; i < opponents; i++)
        {
            float opponentRating = player.GetBaseRating() + opponentBonus + Random.Range(-15f, 15f);
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

    public static int GetPrize(int place, int loft)
    {
        // Higher lofts pay more: 1st place is 100 / 200 / 400
        int multiplier = 1;
        if (loft == 2) multiplier = 2;
        if (loft == 3) multiplier = 4;

        return GetPrize(place) * multiplier;
    }
}