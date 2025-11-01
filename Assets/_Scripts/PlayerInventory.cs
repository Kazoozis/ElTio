using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private List<string> items = new List<string>();

    void Start()
    {
        string[] possibleItems = { "picareta", "tocha", "comida", "cigarro", "álcool", "folha de coca" };

        // Embaralha e pega 3 itens aleatórios
        List<string> randomized = new List<string>(possibleItems);
        for (int i = 0; i < randomized.Count; i++)
        {
            int randomIndex = Random.Range(i, randomized.Count);
            (randomized[i], randomized[randomIndex]) = (randomized[randomIndex], randomized[i]);
        }

        items.Add(randomized[0]);
        items.Add(randomized[1]);
        items.Add(randomized[2]);

        Debug.Log("Você inicia com: " + string.Join(", ", items));
    }

    public bool HasItem(string item)
    {
        return items.Contains(item);
    }

    public void AddItem(string item)
    {
        items.Add(item);
    }

    public void RemoveItem(string item)
    {
        items.Remove(item);
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

    // ✅ Método público para exibir o inventário
    public void ShowInventory()
    {
        Debug.Log("📦 Inventário atual: " + string.Join(", ", items));
    }

    // ✅ Novo método para exibir inventário após troca
    public void ShowUpdatedInventory(string tradedFrom, string tradedTo)
    {
        Debug.Log($"Você trocou sua {tradedFrom} por {tradedTo}.");
        Debug.Log("Agora você tem: " + string.Join(", ", items));
    }
}
