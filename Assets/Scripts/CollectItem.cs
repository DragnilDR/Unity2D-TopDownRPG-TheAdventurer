using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectItem : MonoBehaviour
{
    [SerializeField] private InventorySlot inventorySlot;

    [SerializeField] private bool playerIsFound = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerIsFound)
        {
            SoundSystem.Instance.PlaySound("TakeItem");

            Inventory.Instance.AddItem(inventorySlot);

            Destroy(gameObject);
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
}
