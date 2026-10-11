using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceTrackUI : MonoBehaviour
{
    [SerializeField] private GameObject trackPanel;
    [SerializeField] private RectTransform trackArea;   // the bar the markers slide along
    [SerializeField] private RectTransform[] markers;   // index 0 = player, 1 and up = opponents
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button closeButton;

    void Start()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
        trackPanel.SetActive(false);
    }

    // Shows the racers on the track. Faster scores sit further right.
    public void ShowResult(RaceResult result)
    {
        if (result == null || result.scores.Count == 0)
        {
            return;
        }

        float min = float.MaxValue;
        float max = float.MinValue;
        foreach (float s in result.scores)
        {
            if (s < min) min = s;
            if (s > max) max = s;
        }
        float range = Mathf.Max(max - min, 1f);

        float width = trackArea.rect.width;

        for (int i = 0; i < markers.Length; i++)
        {
            bool active = i < result.scores.Count;
            markers[i].gameObject.SetActive(active);

            if (!active)
            {
                continue;
            }

            // 0 = slowest (far left), 1 = fastest (far right)
            float t = (result.scores[i] - min) / range;
            float x = (t - 0.5f) * width;
            markers[i].anchoredPosition = new Vector2(x, markers[i].anchoredPosition.y);
        }

        if (resultText != null)
        {
            resultText.text = "Place: " + result.place;
        }

        trackPanel.SetActive(true);
    }

    public void Hide()
    {
        trackPanel.SetActive(false);
    }
}