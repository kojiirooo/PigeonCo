using System.Collections.Generic;
using UnityEngine;

// Holds the result of one race, including every racer's score
public class RaceResult
{
    public int place;
    public List<float> scores = new List<float>(); // index 0 is the player
    public List<float> opponentRatings = new List<float>(); // fixed field, shown before the race
}

public static class RaceManager
{
    public const int OpponentCount = 3;

    // Fixed opponent ratings per loft. Edit these numbers to tune difficulty.
    // Each loft's field is always the same, so the player can read it.
    public static float[] GetOpponentRatings(int loft)
    {
        switch (loft)
        {
            case 1: return new float[] { 60f, 70f, 80f };
            case 2: return new float[] { 70f, 80f, 90f, 95f, 100f };
            case 3: return new float[] { 80f, 90f, 100f, 105f, 110f, 115f, 120f };
            default: return new float[] { 60f, 70f, 80f };
        }
    }

    public static int GetOpponentCount(int loft)
    {
        return GetOpponentRatings(loft).Length;
    }

    // Old call: returns only the place
    public static int RunRace(Pigeon player)
    {
        return RunRace(player, 1);
    }

    public static int RunRace(Pigeon player, int loft)
    {
        return RunFullRace(player, loft).place;
    }

    // Returns the place and every racer's score
    public static RaceResult RunFullRace(Pigeon player, int loft)
    {
        RaceResult result = new RaceResult();

        float wingbandBonus = player.hasWingband ? Pigeon.WingbandRaceBonus : 0f;

        // Your score: rating plus a small luck roll (0 to 10)
        float playerScore = player.GetRating() + wingbandBonus + Random.Range(0f, 15f);
        result.scores.Add(playerScore);

        int place = 1;
        float[] ratings = GetOpponentRatings(loft);

        for (int i = 0; i < ratings.Length; i++)
        {
            // Opponent score: fixed rating plus a small luck roll (-5 to +5)
            float opponentScore = ratings[i] + Random.Range(0f, 15f);

            result.scores.Add(opponentScore);
            result.opponentRatings.Add(ratings[i]);

            Debug.Log($"Opponent {i + 1} (rating {ratings[i]}) race score: {opponentScore}");

            if (opponentScore > playerScore)
            {
                place++;
            }
        }

        result.place = place;
        Debug.Log($"{player.pigeonName} race score: {playerScore}, place {place}");
        return result;
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