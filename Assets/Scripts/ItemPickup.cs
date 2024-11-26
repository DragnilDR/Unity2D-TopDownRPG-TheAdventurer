using UnityEngine;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public InventorySlot inventorySlot;
    public Transform target;

    [SerializeField] private TextMeshPro itemName;

    [SerializeField] private float pickupDist;
    private float playerDist;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        target = FindAnyObjectByType<Player>().transform;

        spriteRenderer.sprite = inventorySlot.item.itemSprite;
        itemName.text = $"{inventorySlot.item.itemName} x{inventorySlot.count}";
    }

    private void Update()
    {
        if (target != null)
        {
            playerDist = Vector2.Distance(target.position, transform.position);

            if (Input.GetKeyDown(KeyCode.F) && playerDist <= pickupDist)
            {
                SoundSystem.Instance.PlaySound("TakeItem");

                Pickup();
            }
        }

        //FindToPlayer();
    }

    //private void FindToPlayer()
    //{
    //    if (target.gameObject.activeSelf)
    //    {
    //        playerDist = Vector2.Distance(target.position, transform.position);

    //        if (playerDist <= pickupDist)
    //        {
    //            transform.position = Vector2.MoveTowards(transform.position, target.position, 2f * Time.fixedDeltaTime);

    //            if (playerDist <= .5f)
    //            {
    //                Pickup();
    //            }
    //        }
    //    }
    //}

    private void Pickup()
    {
        Inventory.Instance.AddItem(inventorySlot);

        Destroy(gameObject);
    }
}
