using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu Instance { get; private set; }

    public bool pauseGame = false;
    [SerializeField] private GameObject optionsMenuUI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathScreen;

    public static event System.Action OnSaveGame;
    public static event System.Action OnLoadGame;
    public static event System.Action OnDeleteSave;

    [Header("Keybind")]
    public KeyCode pauseKey = KeyCode.Escape;

    private void Start()
    {
        Application.targetFrameRate = 60;

        if (!Instance)
        {
            Instance = this;
            //DontDestroyOnLoad(this); // название говорит само за себя
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey) && deathScreen.activeSelf == false)
        {
            if (pauseGame == false)
            {
                Pause();
            }
            else Resume();
        }
    }

    public void OpenOptionsMenu()
    {
        optionsMenuUI.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenuUI.SetActive(false);
    }

    public void DeleteSaves()
    {
        OnDeleteSave?.Invoke();
        optionsMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        pauseGame = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        pauseGame = false;
    }

    public void SaveGame()
    {
        OnSaveGame?.Invoke();
    }

    public void LoadGame()
    {
        OnLoadGame?.Invoke();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
