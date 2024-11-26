using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHelpButton : MonoBehaviour
{
    private Transform playerPos;

    private Vector3 startPos;

    [SerializeField] private GameObject helpObject;
    [SerializeField] private Vector3 offset;
    private void Start()
    {
        helpObject.SetActive(false);

        startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerPos = FindObjectOfType<Player>().transform;

            helpObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            helpObject.SetActive(false);

            helpObject.transform.position = startPos;
        } 
    }

    private void Update()
    {
        if (playerPos != null)
            helpObject.transform.position = playerPos.position + offset;
    }
}
