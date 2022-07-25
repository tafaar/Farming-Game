using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGiverMenu : MonoBehaviour
{
    [SerializeField] GameObject giverButton;
    [SerializeField] Player player;

    void Start()
    {
        foreach(Item item in ItemManager.instance.items)
        {
            GameObject newButton = Instantiate(giverButton, gameObject.transform);
            newButton.GetComponent<DebugGiver>().item = Instantiate(item);
            newButton.GetComponent<DebugGiver>().player = player;
        }
    }
}
