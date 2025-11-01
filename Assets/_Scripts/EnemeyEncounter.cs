using UnityEngine;

public class EnemyEncounter : MonoBehaviour
{
    public string enemyName;
    public string offeringItem;  // "cigarro", "álcool" ou "folha de coca"
    public string requestedEquipment; // "tocha", "picareta" ou "comida"

    public void StartEncounter()
    {
        Debug.Log(enemyName + ": Ei, minerador... troco minha " + offeringItem + " pela sua " + requestedEquipment + ".");
        Debug.Log("Digite 1 para TROCAR ou 2 para RECUSAR.");
    }

    public void Trade(PlayerInventory playerInventory)
    {
        if (playerInventory.HasItem(requestedEquipment))
        {
            playerInventory.RemoveItem(requestedEquipment);
            playerInventory.AddItem(offeringItem);
            Debug.Log("Você trocou sua " + requestedEquipment + " por " + offeringItem + ".");
        }
        else
        {
            Debug.Log("Você não tem " + requestedEquipment + " para trocar!");
        }

        Destroy(gameObject);
    }
}
