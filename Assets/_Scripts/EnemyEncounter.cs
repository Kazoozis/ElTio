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

    private bool hasPlayedVoice = false;

    // --- Falas (iguais ao original, resumidas aqui por espaço) ---
    #region Falas
    private readonly string[] famintoPicareta = { "Não sinto mais meus braços… me empresta a tua picareta… só um golpe, um só…" };
    private readonly string[] famintoTocha = { "A escuridão me mastiga por dentro… deixa eu sentir o fogo outra vez." };
    private readonly string[] famintoComida = { "Nada preenche… mas talvez tua ração engane a dor um pouco." };
    private readonly string[] famintoOferece = { "Achei isso na lama… talvez te sirva melhor do que a mim." };
    private readonly string[] famintoRecusa = { "Então me deixa aqui, apodrecendo no breu?" };

    private readonly string[] assombradoPicareta = { "Preciso quebrar o sussurro que vem das paredes... tua picareta pode calar eles." };
    private readonly string[] assombradoTocha = { "A luz é o único rosto que ainda me olha sem ódio." };
    private readonly string[] assombradoComida = { "A fome não é no estômago... é na alma." };
    private readonly string[] assombradoAlcool = { "As vozes gritam alto. O álcool faz elas dormirem." };
    private readonly string[] assombradoCigarro = { "A fumaça sobe e forma rostos… são eles, te olhando também?" };
    private readonly string[] assombradoFolha = { "Ela me faz ver mais claro… ou mais fundo." };
    private readonly string[] assombradoOferece = { "Achei isso no breu… alguém o deixou cair antes de gritar." };
    private readonly string[] assombradoRecusa = { "Eles disseram que tu ia negar. Estão rindo agora." };

    private readonly string[] briguentoPicareta = { "Quebra essa pedra comigo, ou eu quebro tuas costelas." };
    private readonly string[] briguentoTocha = { "Tá escuro demais, porra! Me empresta essa tocha!" };
    private readonly string[] briguentoComida = { "Tô tremendo. Não é fome, é raiva." };
    private readonly string[] briguentoAlcool = { "Me dá essa garrafa. Não pergunto de onde veio." };
    private readonly string[] briguentoCigarro = { "Tu tá fumando? Passa um antes que eu arranque dos teus dedos." };
    private readonly string[] briguentoFolha = { "Essa folha dá força, não dá? Preciso pra continuar." };
    private readonly string[] briguentoOferece = { "Peguei isso dum desgraçado que tentou me enganar. Serve pra ti?" };
    private readonly string[] briguentoRecusa = { "Ah é? Vai negar? Então fica pronto pra apanhar." };
    #endregion

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

        DialogueManager.Instance.ShowDialogue(new string[] {
            $"{enemyName} aparece na sua frente...",
            dialogue,
            "Pressione 1 para ACEITAR ou 2 para RECUSAR."
        });

        if (!hasPlayedVoice && AudioManager.Instance != null)
        {
            hasPlayedVoice = true;
            AudioManager.Instance.PlayVoiceFor4Seconds();
        }
    }

    private string GenerateDialogue()
    {
        string[] pool = famintoOferece;

        if (requestedItem == "picareta" || requestedItem == "tocha" || requestedItem == "comida")
        {
            switch (enemyType)
            {
                case EnemyType.Faminto:
                    pool = requestedItem == "picareta" ? famintoPicareta :
                           requestedItem == "tocha" ? famintoTocha :
                           famintoComida;
                    break;
                case EnemyType.Assombrado:
                    pool = requestedItem == "picareta" ? assombradoPicareta :
                           requestedItem == "tocha" ? assombradoTocha :
                           assombradoComida;
                    break;
                case EnemyType.Briguento:
                    pool = requestedItem == "picareta" ? briguentoPicareta :
                           requestedItem == "tocha" ? briguentoTocha :
                           briguentoComida;
                    break;
            }
        }
        else
        {
            switch (enemyType)
            {
                case EnemyType.Assombrado:
                    pool = requestedItem == "álcool" ? assombradoAlcool :
                           requestedItem == "cigarro" ? assombradoCigarro :
                           assombradoFolha;
                    break;
                case EnemyType.Briguento:
                    pool = requestedItem == "álcool" ? briguentoAlcool :
                           requestedItem == "cigarro" ? briguentoCigarro :
                           briguentoFolha;
                    break;
            }
        }

        string chosen = pool[Random.Range(0, pool.Length)];
        return $"{chosen} Tenho {offeringItem} para trocar.";
    }

    public void Trade(PlayerInventory playerInventory)
    {
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

    public string GetRefusalLine()
    {
        string[] pool = enemyType switch
        {
            EnemyType.Faminto => famintoRecusa,
            EnemyType.Assombrado => assombradoRecusa,
            EnemyType.Briguento => briguentoRecusa,
            _ => null
        };

        return pool != null && pool.Length > 0
            ? pool[Random.Range(0, pool.Length)]
            : "Ele te olha e se vira.";
    }
}
