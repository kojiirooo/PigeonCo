using System.Collections;
using UnityEngine;
using TMPro;

public class GameMessage : MonoBehaviour
{
    public static GameMessage Instance { get; private set; }

    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject messageRoot;
    [SerializeField] private float showSeconds = 3f;

    private Coroutine running;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (messageText != null) messageText.text = "";
        if (messageRoot != null) messageRoot.SetActive(false);
    }

    // Shows a message for a few seconds. A new message replaces the old one.
    public void Show(string message)
    {
        Show(message, showSeconds);
    }

    public void Show(string message, float seconds)
    {
        if (messageText == null)
        {
            Debug.Log(message);
            return;
        }

        if (running != null)
        {
            StopCoroutine(running);
        }

        messageText.text = message;

        if (messageRoot != null) messageRoot.SetActive(true);

        running = StartCoroutine(Clear(seconds));
    }

    private IEnumerator Clear(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        messageText.text = "";

        if (messageRoot != null) messageRoot.SetActive(false);

        running = null;
    }
}