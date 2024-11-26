using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public static event System.Action OnLoadGame;
    public static event System.Action OnLoadGameSettings;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            OnLoadGameSettings?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            OnLoadGameSettings?.Invoke();
            OnLoadGame?.Invoke();
            RankSystem.Instance.UpdateRank();
            Destroy(gameObject);
        }
    }
}
