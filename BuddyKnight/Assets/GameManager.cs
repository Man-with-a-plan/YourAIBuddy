using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

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
        completeLevelUI.SetActive(true);
        sfc.clip = sfx;
        sfc.Play();
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
