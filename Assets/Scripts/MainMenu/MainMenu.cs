using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;
    
    [Header("scene name")]
    [SerializeField] private UnityEditor.SceneAsset sceneToLoad;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// start the game
    /// </summary>
    public void StartGame()
    {
        // need to add game mod selection menu
        SceneManager.LoadScene(sceneToLoad.name);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void Settings()
    {
        if (settingsMenu == false)
        { 
            mainMenu.SetActive(false); 
            settingsMenu.SetActive(true);
        }
        else
        {
            mainMenu.SetActive(true);
            settingsMenu.SetActive(false); 
        }
    }
}
