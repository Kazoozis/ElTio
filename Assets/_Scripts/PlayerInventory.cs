﻿using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private List<string> items = new();

    private InventoryUI inventoryUI; // 🆕 referência automática à UI

    void Start()
    {
        // 🆕 Encontrar a UI automaticamente na cena
        inventoryUI = FindObjectOfType<InventoryUI>();

        string[] possibleItems = { "picareta", "tocha", "comida", "cigarro", "álcool", "folha de coca" };

        List<string> randomized = new(possibleItems);
        for (int i = 0; i < randomized.Count; i++)
        {
            int randomIndex = Random.Range(i, randomized.Count);
            (randomized[i], randomized[randomIndex]) = (randomized[randomIndex], randomized[i]);
        }

        for (int i = 0; i < 3; i++)
            items.Add(randomized[i]);

        Debug.Log("Você inicia com: " + string.Join(", ", items));

        // 🆕 Atualiza a UI inicial
        inventoryUI?.UpdateUI();
    }

    public bool HasItem(string item) => items.Contains(item);

    public void AddItem(string item)
    {
        items.Add(item);
        inventoryUI?.UpdateUI(); // 🆕 atualiza UI ao adicionar
    }

    public void RemoveItem(string item)
    {
        items.Remove(item);
        inventoryUI?.UpdateUI(); // 🆕 atualiza UI ao remover
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

        Debug.Log(hasAll
            ? "🔥 O Diabo sorri. Você trouxe as oferendas certas. Fim de jogo."
            : "😈 O Diabo ruge. Faltam oferendas. Você está perdido para sempre...");
    }

    public void ShowInventory()
    {
        Debug.Log("📦 Inventário atual: " + string.Join(", ", items));
    }

    public void ShowUpdatedInventory(string tradedFrom, string tradedTo)
    {
        Debug.Log($"Você trocou sua {tradedFrom} por {tradedTo}.");
        Debug.Log("Agora você tem: " + string.Join(", ", items));
    }

    public List<string> GetItems() => new(items);
}
