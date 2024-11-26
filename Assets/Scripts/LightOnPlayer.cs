using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LightOnPlayer : MonoBehaviour
{
    private void Start()
    {
        Transform light = transform.Find("Light");
        light.GetComponent<Light2D>();

        if (SceneManager.GetActiveScene().name == "Cave")
            light.gameObject.SetActive(true);
        else light.gameObject.SetActive(false);
    }
}
