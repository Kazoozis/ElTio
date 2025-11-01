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
        Debug.Log("Você inicia com: " + string.Join(", ", items));
    }

    public bool HasItem(string item)
    {
        return items.Contains(item);
    }

    public void AddItem(string item)
    {
        items.Add(item);
        Debug.Log("Agora você tem: " + string.Join(", ", items));
    }

    public void RemoveItem(string item)
    {
        items.Remove(item);
        Debug.Log("Restam: " + string.Join(", ", items));
    }

    public void CheckOfferings()
    {
        string[] requiredOfferings = { "cigarro", "álcool", "folha de coca" };
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
            Debug.Log("O Diabo sorri. Você trouxe as oferendas certas. Fim de jogo.");
        else
            Debug.Log("O Diabo ruge. Faltam oferendas. Você está perdido para sempre...");
    }
}
