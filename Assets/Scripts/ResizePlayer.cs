using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResizePlayer : MonoBehaviour
{
    [SerializeField] private string[] sceneWherePlayerIsBig;
    [SerializeField] private string[] sceneWherePlayerIsSmall;

    private void Start()
    {
        EnlargeObject();
        ShrinkObject();
    }

    private void EnlargeObject()
    {
        foreach (var scene in sceneWherePlayerIsBig)
            if (scene == SceneManager.GetActiveScene().name)
                transform.localScale = new Vector3(1, 1);
    }

    private void ShrinkObject()
    {
        foreach (var scene in sceneWherePlayerIsSmall)
            if (scene == SceneManager.GetActiveScene().name)
                transform.localScale = new Vector3(.7f, .7f);
    }
}
