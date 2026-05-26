using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    bool gamehasEnded = false;

    public AudioSource sfc;
    public AudioClip sfx, sfx2;
    public float restartDelay = 1f;

    void Start()
    {
        sfc.clip = sfx2;
        sfc.Play();
    }

    public GameObject completeLevelUI;
    
   
    public void CompleteLevel()
    {
        switch (levelIndex)
        {
            case 1:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case 2:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case 3:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case 4:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case 5:
                completeLevelUI.SetActive(true);
                sfc.clip = sfx;
                sfc.Play();
                EndGame();
                break;
        }
                
    }
    public void EndGame()
    {
        if (gamehasEnded == false)
        {
            gamehasEnded = true;
            Debug.Log("Game Over");
            
        }
    }
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

  
}
