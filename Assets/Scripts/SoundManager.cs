using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;     // slot: MainMenu background track
    [SerializeField] private AudioClip loftMusic;     // slot: Loft background track

    [Header("UI")]
    [SerializeField] private AudioClip buttonClick;   // slot: plays on every button press

    [Header("Game (for later)")]
    [SerializeField] private AudioClip feedSound;
    [SerializeField] private AudioClip bondSound;
    [SerializeField] private AudioClip hatchSound;
    [SerializeField] private AudioClip raceStartSound;
    [SerializeField] private AudioClip coinSound;

    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioClip currentMusic;
    private readonly HashSet<Button> hookedButtons = new HashSet<Button>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        SetupScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupScene(scene.name);
    }

    private void SetupScene(string sceneName)
    {
        // Forget buttons from scenes that have been unloaded
        hookedButtons.RemoveWhere(b => b == null);

        PlayMusic(sceneName == "MainMenu" ? menuMusic : loftMusic);
        HookButtons();
    }

    private void HookButtons()
    {
        foreach (Button b in FindObjectsOfType<Button>(true))
        {
            if (hookedButtons.Contains(b)) continue;

            hookedButtons.Add(b);
            b.onClick.AddListener(PlayClick);
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        // Don't restart the track if it's already playing
        if (clip == null || clip == currentMusic) return;

        currentMusic = clip;
        musicSource.clip = clip;
        musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    // Called by buttons automatically
    public void PlayClick() { PlaySFX(buttonClick); }

    // Call these from your game scripts later, e.g. SoundManager.Instance?.PlayFeed();
    public void PlayFeed() { PlaySFX(feedSound); }
    public void PlayBond() { PlaySFX(bondSound); }
    public void PlayHatch() { PlaySFX(hatchSound); }
    public void PlayRaceStart() { PlaySFX(raceStartSound); }
    public void PlayCoin() { PlaySFX(coinSound); }
}