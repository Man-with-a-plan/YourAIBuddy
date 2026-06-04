using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;          // ← ADD THIS
using UnityEngine.Playables;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
     private int levelIndex;

    [Header("UI")]
    [SerializeField] private GameObject DeathScreen;
    [SerializeField] private CanvasGroup deathScreenCanvasGroup;
    public GameObject completeLevelUI;
    [SerializeField] private FallDamage fallDamage;

    [Header("Timelines")]
    [SerializeField] private PlayableDirector GameStartTimeline;
    [SerializeField] private VideoPlayer EndcutsceneVideoPlayer;
    [SerializeField] private GameObject playerFollowCamera;
    private PauseMenu pauseMenu;

    [Header("Death Timing")]
    public float delayBeforeUI = 2f;
    public float fadeDuration = 1.5f;
    public float delayBeforeRestart = 2f;

    [Header("Audio")]
    public AudioSource sfc;
    public AudioClip sfx, sfx2;

    private bool gamehasEnded = false;
    private bool isRestarting = false;

    void Awake()
    {
        levelIndex = SceneManager.GetActiveScene().buildIndex;
        if (GameStartTimeline != null && PlayerPrefs.GetString("IsNewGame") == "Yes")
        {
            playerFollowCamera.SetActive(false);
            GameStartTimeline.Play();
        }
        PlayerPrefs.SetString("IsNewGame", "No");
    }

    void Start()
    {
        sfc.clip = sfx2;
        sfc.Play();
        fallDamage.OnCriticalFallDamage += Restart;
        pauseMenu = GetComponent<PauseMenu>();

        if (deathScreenCanvasGroup != null)
        {
            deathScreenCanvasGroup.alpha = 0f;
            deathScreenCanvasGroup.interactable = false;
            deathScreenCanvasGroup.blocksRaycasts = false;
        }

        if (DeathScreen != null)
            DeathScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu != null)
            {
                if (pauseMenu.isPaused)
                    pauseMenu.Resume();
                else
                    pauseMenu.Pause();
            }
        }
    }

    // ===================== DEATH =====================

    public void Restart()
    {
        if (gamehasEnded || isRestarting) return;

        gamehasEnded = true;
        isRestarting = true;

        Debug.Log("Player Died");
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return new WaitForSecondsRealtime(delayBeforeUI);

        DeathScreen.SetActive(true);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            if (deathScreenCanvasGroup != null)
                deathScreenCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t / fadeDuration);
            yield return null;
        }

        if (deathScreenCanvasGroup != null)
        {
            deathScreenCanvasGroup.alpha = 1f;
            deathScreenCanvasGroup.interactable = true;
            deathScreenCanvasGroup.blocksRaycasts = true;
        }

        yield return new WaitForSecondsRealtime(delayBeforeRestart);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ===================== LEVEL COMPLETE =====================

    public void CompleteLevel()
    {
        if (gamehasEnded) return;      // ← guard rapid calls on all cases
        gamehasEnded = true;

        switch (levelIndex)
        {
            case 1:
            case 2:

            case 3:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;

            case 4:

                sfc.clip = sfx;
                sfc.Play();
                StartCoroutine(EndGameRoutine());  // ← coroutine instead of async Task
                break;
        }
    }

    // ← Reliable video wait via event, not Task.Delay
    private IEnumerator EndGameRoutine()
    {
        if (EndcutsceneVideoPlayer == null)
        {
            Debug.LogError("EndcutsceneVideoPlayer is not assigned!");
            yield break;
        }
        EndcutsceneVideoPlayer.Play();
        Debug.Log("Game Over");

        bool videoFinished = false;
        EndcutsceneVideoPlayer.loopPointReached += _ => videoFinished = true;

        yield return new WaitUntil(() => videoFinished);

        SceneManager.LoadScene(0);
    }

    void OnDestroy()
    {
        fallDamage.OnCriticalFallDamage -= Restart;
    }
}