using UnityEngine;
using UnityEngine.UI;

// Clean button: only visible when a pigeon in this loft has a cold
public class CleanUI : MonoBehaviour
{
    [SerializeField] private PigeonCollection pigeonCollection;
    [SerializeField] private Button cleanButton;

    void Start()
    {
        cleanButton.onClick.AddListener(OnCleanClicked);
    }

    void Update()
    {
        cleanButton.gameObject.SetActive(LoftHasCold());
    }

    private bool LoftHasCold()
    {
        foreach (Pigeon p in pigeonCollection.ownedPigeons)
        {
            if (p.loftID == pigeonCollection.loftNumber && p.hasCold)
            {
                return true;
            }
        }
        return false;
    }

    private void OnCleanClicked()
    {
        pigeonCollection.CleanLoft();
        GameMessage.Instance?.Show("Loft cleaned! Colds cured.");
    }
}