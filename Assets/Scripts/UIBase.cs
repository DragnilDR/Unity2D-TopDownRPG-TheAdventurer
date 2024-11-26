using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public abstract class UIBase : MonoBehaviour
{
    public abstract void GetItemInfo();

    public void DestroyUIElements(Transform content)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public virtual void CreateUIElement<T>(Transform content, GameObject uiElement, List<T> itemList)
    {
        if (itemList != null)
        {
            foreach (var item in itemList)
            {
                GameObject itemUIElement = Instantiate(uiElement, content);

                itemUIElement.GetComponent<Button>().onClick.AddListener(GetItemInfo);
            }
        }
    }

    protected int GetClickedButton()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        int index = -1;

        if (clickedButton != null)
        {
            if (clickedButton.TryGetComponent<Button>(out var button))
            {
                Button[] buttons = GetComponentsInChildren<Button>();

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i] == button)
                    {
                        index = i;
                        break;
                    }
                }
            }
        }
        return index;
    }
}
