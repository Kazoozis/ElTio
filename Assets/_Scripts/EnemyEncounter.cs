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

    private readonly string[] famintoAlcool = {
        "O corpo dói… preciso queimar por dentro pra esquecer.",
        "Uma garrafa, só uma… pra me aquecer, pra me calar.",
        "O frio entrou nos ossos. Me dá o álcool antes que o breu me leve."
    };

    private readonly string[] famintoCigarro = {
        "O gosto da fumaça… talvez me faça lembrar do ar limpo lá de fora.",
        "Uma tragada, só isso. Pra fingir que ainda tô vivo.",
        "O cheiro da mina me enjoa… me dá o fumo."
    };

    private readonly string[] famintoFolha = {
        "Ela engana o corpo, né? Engana a dor também?",
        "Os outros mastigavam e riam… quero sentir isso de novo.",
        "A cabeça dói, o peito queima. Me dá uma folha."
    };

    private readonly string[] famintoOfereceAlcool = {
        "Toma isso. Achei no altar… mas ele não me quis.",
        "Dizem que o Diabo troca melhor que os homens. Prova que não.",
        "Se ele não me aceitou, talvez aceite a ti."
    };

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
                do offeringItem = offerings[Random.Range(0, offerings.Length)];
                while (offeringItem == requestedItem);
                break;
        }

        enemyName = enemyType.ToString();
    }

    public void StartEncounter()
{
    if (playerInventory == null)
        playerInventory = FindObjectOfType<PlayerInventory>();

    string dialogue = GenerateDialogue();

    // inicia a fala **uma única vez**
    AudioManager.Instance.StartVoice();

    DialogueManager.Instance.ShowDialogue(new string[] {
        $"{enemyName} aparece na sua frente...",
        dialogue,
        $"Ele quer trocar **{requestedItem}** por **{offeringItem}**.",
        "Pressione **1** para ACEITAR ou **2** para RECUSAR."
    });
}

public void EndEncounter()
{
    // garante que a fala pare
    AudioManager.Instance.StopVoice();
}


    private string GenerateDialogue()
    {
        if (enemyType == EnemyType.Faminto)
        {
            string[] pool = requestedItem switch
            {
                "álcool" => famintoAlcool,
                "cigarro" => famintoCigarro,
                "folha de coca" => famintoFolha,
                _ => famintoOfereceAlcool
            };

            return $"{pool[Random.Range(0, pool.Length)]} Tenho {offeringItem} para trocar.";
        }

        return $"Ei, minerador... troco minha {offeringItem} pela sua {requestedItem}.";
    }

    public void Trade(PlayerInventory playerInventory)
    {
        if (playerInventory.HasItem(requestedItem))
        {
            playerInventory.RemoveItem(requestedItem);
            playerInventory.AddItem(offeringItem);
            playerInventory.ShowUpdatedInventory(requestedItem, offeringItem);
        }
        else Debug.Log($"Você não tem {requestedItem} para trocar!");

        // 🔇 para voz ao encerrar o encontro
        FindObjectOfType<TunnelMovement>().StopMinerVoice();

        Destroy(gameObject);
    }
}
