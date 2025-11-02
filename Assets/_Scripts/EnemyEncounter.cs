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

    // 🎧 Controle para impedir repetição do áudio
    private bool hasPlayedVoice = false;

    // ----------------------
    // FAMINTO — falas extras
    // ----------------------
    private readonly string[] famintoPicareta = {
        "Não sinto mais meus braços… me empresta a tua picareta… só um golpe, um só…",
        "A rocha me chamou… preciso abrir o caminho, mesmo que custe sangue.",
        "As mãos doem, mas eu ainda posso cavar… me dá tua ferramenta."
    };
    private readonly string[] famintoTocha = {
        "A escuridão me mastiga por dentro… deixa eu sentir o fogo outra vez.",
        "A luz... preciso ver se ainda tenho rosto.",
        "A caverna fala comigo, mas não vejo mais de onde…"
    };
    private readonly string[] famintoComida = {
        "Nada preenche… mas talvez tua ração engane a dor um pouco.",
        "O gosto do ferro e da terra já não basta... compartilha tua comida.",
        "Prometo não te seguir… se me deixar só um pedaço."
    };
    private readonly string[] famintoOferece = {
        "Achei isso na lama… talvez te sirva melhor do que a mim.",
        "O Diabo gosta de quem divide, não é?",
        "Peguei isso de um companheiro... ele não vai mais precisar."
    };
    private readonly string[] famintoRecusa = {
        "Então me deixa aqui, apodrecendo no breu?",
        "Tu também vai secar… ninguém volta inteiro dessa mina.",
        "Tua alma vai gritar como a minha."
    };

    // -------------------------
    // ASSOMBRADO — falas novas
    // -------------------------
    private readonly string[] assombradoPicareta = {
        "Preciso quebrar o sussurro que vem das paredes... tua picareta pode calar eles.",
        "O som ecoa, mas não volta... me deixa tentar escavar minha saída.",
        "Há algo preso aqui comigo... quero libertá-lo."
    };
    private readonly string[] assombradoTocha = {
        "A luz é o único rosto que ainda me olha sem ódio.",
        "Ela sussurra atrás de mim. Me dá tua tocha, por favor.",
        "Não quero ver, mas também não quero mais o escuro."
    };
    private readonly string[] assombradoComida = {
        "A fome não é no estômago... é na alma.",
        "Se eu comer, talvez as vozes parem por um instante.",
        "Comida... ou silêncio. Não sei o que peço mais."
    };
    private readonly string[] assombradoAlcool = {
        "As vozes gritam alto. O álcool faz elas dormirem.",
        "Não bebo há dias, mas o eco pede… diz que é pra mim.",
        "O breu arde nos olhos. Só o fogo da bebida apaga."
    };
    private readonly string[] assombradoCigarro = {
        "A fumaça sobe e forma rostos… são eles, te olhando também?",
        "Acende um pra mim… talvez o cheiro espante o que me segue.",
        "Os mortos fumam junto, sabias? Só eles me entendem."
    };
    private readonly string[] assombradoFolha = {
        "Ela me faz ver mais claro… ou mais fundo.",
        "As paredes sussurram em que língua? Talvez a folha saiba.",
        "Preciso mastigar o silêncio antes que ele me engula."
    };
    private readonly string[] assombradoOferece = {
        "Achei isso no breu… alguém o deixou cair antes de gritar.",
        "É um presente, dizem as vozes. Elas querem que tu aceite.",
        "A luz piscou quando peguei isso… acho que é um sinal."
    };
    private readonly string[] assombradoRecusa = {
        "Eles disseram que tu ia negar. Estão rindo agora.",
        "Negou... agora eles vêm.",
        "Tua sombra se mexeu sozinha... viu?"
    };

    // ------------------------
    // BRIGUENTO — falas novas
    // ------------------------
    private readonly string[] briguentoPicareta = {
        "Quebra essa pedra comigo, ou eu quebro tuas costelas.",
        "Tua picareta é boa… deixa eu usar antes que eu tome à força.",
        "Vamos, me empresta. Eu sei fazer render."
    };
    private readonly string[] briguentoTocha = {
        "Tá escuro demais, porra! Me empresta essa tocha antes que eu enfie a mão na tua bolsa.",
        "Não confio nessa escuridão. Me dá logo a luz!",
        "Sem fogo, sem saída. Tu quer morrer aqui?"
    };
    private readonly string[] briguentoComida = {
        "Tô tremendo. Não é fome, é raiva. Me dá um pedaço antes que eu morda tua mão.",
        "A gente dividia tudo lá fora… lembra?",
        "Não te custa nada, covarde."
    };
    private readonly string[] briguentoAlcool = {
        "Me dá essa garrafa. Não pergunto de onde veio.",
        "Bebo pra esquecer o barulho… e o sangue.",
        "Sem ela, minha cabeça vai explodir. Me dá logo!"
    };
    private readonly string[] briguentoCigarro = {
        "Tu tá fumando? Passa um antes que eu arranque dos teus dedos.",
        "Um trago e eu fico calmo, juro.",
        "O ar aqui é podre. Quero sentir algo queimando."
    };
    private readonly string[] briguentoFolha = {
        "Essa folha dá força, não dá? Preciso pra continuar.",
        "Mastigar é o que ainda me mantém de pé.",
        "Não te custa nada, vai. Eu pago com o que sobrou."
    };
    private readonly string[] briguentoOferece = {
        "Peguei isso dum desgraçado que tentou me enganar. Serve pra ti?",
        "É só porcaria, mas talvez o Diabo aceite.",
        "Toma. Pra te lembrar que ainda tem quem manda aqui."
    };
    private readonly string[] briguentoRecusa = {
        "Ah é? Vai negar? Então fica pronto pra apanhar.",
        "Tu acha que é melhor que eu?",
        "Sai do meu caminho ou te quebro."
    };

    // ------------------------
    // CONFIGURAÇÃO DO ENCONTRO
    // ------------------------
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

    // ------------------------
    // INÍCIO DO ENCONTRO
    // ------------------------
    public void StartEncounter()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        string dialogue = GenerateDialogue();

        DialogueManager.Instance.ShowDialogue(new string[] {
            $"{enemyName} aparece na sua frente...",
            dialogue,
            $"Ele quer trocar **{requestedItem}** por **{offeringItem}**.",
            "Pressione **1** para ACEITAR ou **2** para RECUSAR."
        });

        // 🎧 Garante que o som da fala só toque uma vez
        if (!hasPlayedVoice && AudioManager.Instance != null)
        {
            hasPlayedVoice = true;
            AudioManager.Instance.PlayVoiceFor4Seconds();
        }
    }

    // -----------------------------------
    // GERA A FALA DE ACORDO COM O INIMIGO/ITEM
    // -----------------------------------
    private string GenerateDialogue()
    {
        string[] pool = null;

        if (requestedItem == "picareta" || requestedItem == "tocha" || requestedItem == "comida")
        {
            switch (enemyType)
            {
                case EnemyType.Faminto:
                    pool = requestedItem switch
                    {
                        "picareta" => famintoPicareta,
                        "tocha" => famintoTocha,
                        "comida" => famintoComida,
                        _ => famintoOferece
                    };
                    break;

                case EnemyType.Assombrado:
                    pool = requestedItem switch
                    {
                        "picareta" => assombradoPicareta,
                        "tocha" => assombradoTocha,
                        "comida" => assombradoComida,
                        _ => assombradoOferece
                    };
                    break;

                case EnemyType.Briguento:
                    pool = requestedItem switch
                    {
                        "picareta" => briguentoPicareta,
                        "tocha" => briguentoTocha,
                        "comida" => briguentoComida,
                        _ => briguentoOferece
                    };
                    break;
            }
        }
        else
        {
            switch (enemyType)
            {
                case EnemyType.Faminto:
                    pool = famintoComida;
                    break;
                case EnemyType.Assombrado:
                    pool = requestedItem switch
                    {
                        "álcool" => assombradoAlcool,
                        "cigarro" => assombradoCigarro,
                        "folha de coca" => assombradoFolha,
                        _ => assombradoOferece
                    };
                    break;
                case EnemyType.Briguento:
                    pool = requestedItem switch
                    {
                        "álcool" => briguentoAlcool,
                        "cigarro" => briguentoCigarro,
                        "folha de coca" => briguentoFolha,
                        _ => briguentoOferece
                    };
                    break;
            }
        }

        string chosen = (pool != null && pool.Length > 0)
            ? pool[Random.Range(0, pool.Length)]
            : $"Ei, minerador... troco minha {offeringItem} pela sua {requestedItem}.";

        return $"{chosen} Tenho {offeringItem} para trocar.";
    }

    // ------------------------
    // TROCA DE ITENS
    // ------------------------
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

    // ------------------------
    // FALA DE RECUSA
    // ------------------------
    public string GetRefusalLine()
    {
        string[] pool = null;
        switch (enemyType)
        {
            case EnemyType.Faminto: pool = famintoRecusa; break;
            case EnemyType.Assombrado: pool = assombradoRecusa; break;
            case EnemyType.Briguento: pool = briguentoRecusa; break;
        }

        return pool != null && pool.Length > 0 ? pool[Random.Range(0, pool.Length)] : "Ele te olha e se vira.";
    }
}
