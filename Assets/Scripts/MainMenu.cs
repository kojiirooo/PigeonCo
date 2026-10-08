using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string gameSceneName = "Loft";
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private Button closeInstructionsButton;

    private string SavePath
    {
        get { return Path.Combine(Application.persistentDataPath, "pigeonco_save.json"); }
    }

    void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        confirmYesButton.onClick.AddListener(OnConfirmYesClicked);
        confirmNoButton.onClick.AddListener(OnConfirmNoClicked);
        confirmPanel.SetActive(false);
        instructionsButton.onClick.AddListener(OnInstructionsClicked);
        closeInstructionsButton.onClick.AddListener(OnCloseInstructionsClicked);
        instructionsPanel.SetActive(false);

        // Continue only works if a save file exists
        continueButton.interactable = File.Exists(SavePath);
    }

    private void OnContinueClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnNewGameClicked()
    {
        // Only ask for confirmation if there is progress to lose
        if (File.Exists(SavePath))
        {
            confirmPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    private void OnInstructionsClicked()
    {
        instructionsPanel.SetActive(true);
    }

    private void OnCloseInstructionsClicked()
    {
        instructionsPanel.SetActive(false);
    }

    private void OnConfirmYesClicked()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnConfirmNoClicked()
    {
        confirmPanel.SetActive(false);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }


}