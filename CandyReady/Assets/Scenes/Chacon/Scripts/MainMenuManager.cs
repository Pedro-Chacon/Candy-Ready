using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button buttonNewGame;
    [SerializeField] private Button buttonLoad;
    [SerializeField] private string gameSceneName = "Game"; 

    private void Start()
    {
        buttonNewGame.onClick.AddListener(NewGame);
        buttonLoad.onClick.AddListener(LoadGame);

      
        buttonLoad.interactable = DataManager.HasSave();
    }

    private void NewGame()
    {
        DataManager.NewGame();
        SceneManager.LoadScene(gameSceneName);
    }

    private void LoadGame()
    {
        if (!DataManager.HasSave()) return;

        
        SceneManager.LoadScene(gameSceneName);
    }
}