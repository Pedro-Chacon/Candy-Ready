using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HudManager : MonoBehaviour
{
    [Header("Painel de Configurações")]
    [SerializeField] private GameObject settingsPanel;   

    [Header("Botões")]
    [SerializeField] private Button buttonOpenSettings;  
    [SerializeField] private Button buttonCloseSettings; 
    [SerializeField] private Button buttonMainMenu;     
    [SerializeField] private Button buttonResetGame;    

    [Header("Cenas")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; 

    private void Start()
    {
        settingsPanel.SetActive(false);

        buttonOpenSettings.onClick.AddListener(OpenSettings);
        buttonCloseSettings.onClick.AddListener(CloseSettings);
        buttonMainMenu.onClick.AddListener(GoToMainMenu);
        buttonResetGame.onClick.AddListener(ResetGame);
    }

    private void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void GoToMainMenu()
    {
       
        DataManager.SaveMoney(Player.moneyScore);

        Time.timeScale = 1f; 
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void ResetGame()
    {
       
        DataManager.NewGame();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}