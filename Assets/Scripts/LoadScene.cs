using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadBar;
    [SerializeField] private TextMeshProUGUI loadPercent;

    [SerializeField] private string sceneName;

    [SerializeField] private bool playerIsFound = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerIsFound)
        {
            Menu.Instance.SaveGame();
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsFound = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsFound = false;
        }
    }

    public void LoadSavedGame(string loadSceneName)
    {
        string loadSaveScene = SaveLoadManager.Instance.LoadSaveScene();
        if (loadSaveScene == null)
        {
            Load(loadSceneName);
        }
        else
        {
            Load(loadSaveScene);
        }
    }

    public void Load(string loadSceneName)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadAsync(loadSceneName));
    }

    private IEnumerator LoadAsync(string loadSceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneName);

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            loadBar.value = asyncLoad.progress;
            loadPercent.text = (loadBar.value * 100).ToString("F0") + "%";

            if (asyncLoad.progress >= .9f && !asyncLoad.allowSceneActivation)
            {
                loadPercent.text = "Press Any Key";
                if (Input.anyKeyDown)
                {
                    loadingScreen.SetActive(false);
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
