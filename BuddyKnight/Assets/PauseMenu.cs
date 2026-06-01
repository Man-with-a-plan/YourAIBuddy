using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
  
   public void Resume()
    {
       
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void SaveGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetString("SavedScene", currentScene);
        PlayerPrefs.SetInt("HasSave", 1);

        PlayerPrefs.Save();

        Debug.Log("Game Saved: " + currentScene);
    }
    public void LoadMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreenSpecialLevel");
       
    }
}
