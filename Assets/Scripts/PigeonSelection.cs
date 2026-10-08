using UnityEngine;

public class PigeonSelection : MonoBehaviour
{
    public static PigeonSelection Instance { get; private set; }

    public PigeonController Selected { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Select(PigeonController pigeon)
    {
        // Tapping the same pigeon again deselects it
        if (Selected == pigeon)
        {
            Deselect();
            return;
        }

        Selected = pigeon;
        Debug.Log("Selected: " + pigeon.pigeonData.pigeonName);
    }

    public void Deselect()
    {
        Selected = null;
    }
}