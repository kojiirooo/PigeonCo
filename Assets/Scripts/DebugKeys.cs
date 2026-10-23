using UnityEngine;

// TEMPORARY testing shortcuts. We will delete this before you submit the game.
//   ]  = give 1000 coins
//   [  = fully bond and grow the selected pigeon (click a pigeon first)
public class DebugKeys : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            pigeonCollection.AddCoins(1000);
            GameMessage.Instance?.Show("DEBUG: +1000 coins");
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            GrowSelected();
        }
    }

    private void GrowSelected()
    {
        PigeonController selected = PigeonSelection.Instance.Selected;

        if (selected == null || selected.pigeonData == null)
        {
            GameMessage.Instance?.Show("DEBUG: click a pigeon first");
            return;
        }

        Pigeon p = selected.pigeonData;
        p.bond = Pigeon.MaxBond;

        // TryGrow only moves up one stage at a time, so repeat until it stops growing
        while (p.TryGrow())
        {
        }

        // Refresh the sprite and hunger bar for the new stage
        selected.SetupPigeon(p);
        pigeonCollection.SaveGame();

        GameMessage.Instance?.Show("DEBUG: " + p.pigeonName + " is now " + p.stage);
    }
}
