using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private List<string> items = new List<string>();

    void Start()
    {
        items.Add("picareta");
        items.Add("tocha");
        items.Add("comida");
        Debug.Log("Voc� inicia com: " + string.Join(", ", items));
    }

    public bool HasItem(string item)
    {
        return items.Contains(item);
    }

    public List<string> GetItems()
{
    return new List<string>(items);
}


    public void AddItem(string item)
    {
        items.Add(item);
        Debug.Log("Agora voc� tem: " + string.Join(", ", items));
        FindObjectOfType<InventoryUI>()?.UpdateUI();
    }

    public void RemoveItem(string item)
    {
        items.Remove(item);
        Debug.Log("Restam: " + string.Join(", ", items));
        FindObjectOfType<InventoryUI>()?.UpdateUI();
    }

    public void CheckOfferings()
    {
        string[] requiredOfferings = { "cigarro", "�lcool", "folha de coca" };
        bool hasAll = true;

        foreach (string offering in requiredOfferings)
        {
            if (!items.Contains(offering))
            {
                hasAll = false;
                break;
            }
        }

        if (hasAll)
            Debug.Log("O Diabo sorri. Voc� trouxe as oferendas certas. Fim de jogo.");
        else
            Debug.Log("O Diabo ruge. Faltam oferendas. Voc� est� perdido para sempre...");
    }
}
