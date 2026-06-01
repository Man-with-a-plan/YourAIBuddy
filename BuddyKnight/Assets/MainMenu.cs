using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject continueButton;
        void Start()
        {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        EnableContinueButton();
    }
    public void PlayGame()
    {
        

        PlayerPrefs.DeleteKey("SavedScene");
        PlayerPrefs.DeleteKey("HasSave");

        SceneManager.LoadScene("EntryIntoCastle");

    }
    void EnableContinueButton()
    {
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ContinueGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            string sceneName = PlayerPrefs.GetString("SavedScene");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("No save found");
        }
    }
    public void NewGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerPrefs.SetString("IsNewGame", "Yes");
        PlayerPrefs.DeleteKey("SavedScene");
        PlayerPrefs.DeleteKey("HasSave");

        SceneManager.LoadScene("EntryIntoCastle");
    }
}
