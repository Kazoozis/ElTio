using UnityEngine;

public class EnemyEncounter : MonoBehaviour
{
    public enum EnemyType { Faminto, Assombrado, Briguento }
    public EnemyType enemyType;

    public string enemyName;
    private string offeringItem;
    private string requestedItem;

    private readonly string[] items = { "picareta", "tocha", "comida" };
    private readonly string[] offerings = { "cigarro", "álcool", "folha de coca" };
    private string tradeType;
    private PlayerInventory playerInventory;

    public void ConfigureEncounter(string type)
    {
        tradeType = type;

        switch (tradeType)
        {
            case "item>oferenda":
                requestedItem = items[Random.Range(0, items.Length)];
                offeringItem = offerings[Random.Range(0, offerings.Length)];
                break;

            case "oferenda>item":
                requestedItem = offerings[Random.Range(0, offerings.Length)];
                offeringItem = items[Random.Range(0, items.Length)];
                break;

            case "oferenda>oferenda":
                requestedItem = offerings[Random.Range(0, offerings.Length)];
                do
                {
                    offeringItem = offerings[Random.Range(0, offerings.Length)];
                } while (offeringItem == requestedItem);
                break;
        }

        enemyName = enemyType.ToString();
    }

    public void StartEncounter()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogError("❌ PlayerInventory não encontrado na cena!");
            return;
        }

        Debug.Log($"{enemyName}: Ei, minerador... troco minha {offeringItem} pela sua {requestedItem}.");
        Debug.Log("Digite 1 para TROCAR ou 2 para RECUSAR.");
        playerInventory.ShowInventory();
    }

    public void Trade(PlayerInventory playerInventory)
    {
        if (playerInventory == null)
        {
            Debug.LogError("❌ PlayerInventory é nulo durante a troca!");
            return;
        }

        if (playerInventory.HasItem(requestedItem))
        {
            playerInventory.RemoveItem(requestedItem);
            playerInventory.AddItem(offeringItem);
            playerInventory.ShowUpdatedInventory(requestedItem, offeringItem);
        }
        else
        {
            Debug.Log($"Você não tem {requestedItem} para trocar!");
        }

        Destroy(gameObject);
    }
}
