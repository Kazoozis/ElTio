using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs dos Inimigos")]
    public GameObject famintoPrefab;
    public GameObject assombradoPrefab;
    public GameObject briguentoPrefab;

    [Header("Prefab do Último Encontro (Diabo)")]
    public GameObject diaboPrefab;

    [Header("Configurações de Hostilidade")]
    public float baseHostilityChance = 15f;
    public float famintoHasFoodBonus = 5f;
    public float assombradoNoTorchBonus = 5f;
    public float assombradoHasTorchPenalty = 5f;
    public float briguentoNoPickaxeBonus = 5f;
    public float briguentoHasPickaxePenalty = 5f;

    private List<(GameObject prefab, EnemyEncounter.EnemyType type, string tradeType)> encounters = new();
    private int currentEncounter = 0;

    private bool isEncounterActive = false;
    private bool isDialogueActive = false;
    private bool waitingForTunnel = false;

    private EnemyEncounter activeEnemy;
    private PlayerInventory playerInventory;
    private TunnelMovement tunnelMovement;

    // posições e rotação dos inimigos
    private readonly Vector3 enemySpawnPosition = new Vector3(1.321f, 0.099f, 3.75f);   // padrão
    private readonly Vector3 devilSpawnPosition = new Vector3(1.321f, 2.345f, 3.75f);   // 👹 Diabo
    private readonly Quaternion enemySpawnRotation = Quaternion.Euler(0f, -90f, 0f);

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        tunnelMovement = FindObjectOfType<TunnelMovement>();

        GenerateEncounters();
        Debug.Log("Aperte ESPAÇO para avançar...");
    }

    public void SetDialogueState(bool state) => isDialogueActive = state;

    void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                DialogueManager.Instance.NextLine();
            return;
        }

        if (waitingForTunnel)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && !isEncounterActive)
        {
            tunnelMovement.MoveToNextPosition();
            waitingForTunnel = true;
            return;
        }

        if (isEncounterActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                activeEnemy.Trade(playerInventory);
                EndEncounter();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                HandleRefusal(activeEnemy);
                EndEncounter();
            }
        }
    }

    public void OnTunnelArrived()
    {
        waitingForTunnel = false;

        if (currentEncounter < encounters.Count)
            SpawnEnemy();
        else
            Debug.Log("⚠️ Nenhum encontro restante.");
    }

    void SpawnEnemy()
    {
        var (prefab, type, tradeType) = encounters[currentEncounter];

        // 👹 Se for o Diabo, usa posição elevada
        Vector3 spawnPos = (tradeType == "diabo") ? devilSpawnPosition : enemySpawnPosition;

        GameObject obj = Instantiate(prefab, spawnPos, enemySpawnRotation);
        activeEnemy = obj.GetComponent<EnemyEncounter>();

        // 🔥 Encontro final com o Diabo
        if (tradeType == "diabo")
        {
            StartCoroutine(HandleDevilEncounter());
            currentEncounter++;
            return;
        }

        activeEnemy.enemyType = type;
        activeEnemy.ConfigureEncounter(tradeType);
        activeEnemy.StartEncounter();

        // Risada do Diabo no último encontro comum
        if (currentEncounter == encounters.Count - 2 && AudioManager.Instance != null)
            AudioManager.Instance.PlayDevilLaugh();

        isEncounterActive = true;
        currentEncounter++;
    }

    void EndEncounter()
    {
        isEncounterActive = false;
        waitingForTunnel = false;
        Debug.Log("Aperte ESPAÇO para continuar...");
    }

    void GenerateEncounters()
    {
        encounters.Clear();
        string[] tradeTypes = { "item>oferenda", "oferenda>item", "oferenda>oferenda" };

        // ✅ Gera 6 encontros normais alternando entre os 3 tipos de inimigo
        GameObject[] prefabs = { famintoPrefab, assombradoPrefab, briguentoPrefab };
        EnemyEncounter.EnemyType[] types = {
            EnemyEncounter.EnemyType.Faminto,
            EnemyEncounter.EnemyType.Assombrado,
            EnemyEncounter.EnemyType.Briguento
        };

        for (int i = 0; i < 6; i++)
        {
            int index = i % 3;
            encounters.Add((prefabs[index], types[index], tradeTypes[Random.Range(0, tradeTypes.Length)]));
        }

        // ✅ Adiciona o encontro final fixo com o Diabo
        encounters.Add((diaboPrefab, EnemyEncounter.EnemyType.Faminto, "diabo"));
    }

    void HandleRefusal(EnemyEncounter enemy)
    {
        float hostility = baseHostilityChance;

        bool hasPickaxe = playerInventory.HasItem("picareta");
        bool hasTorch = playerInventory.HasItem("tocha");
        bool hasFood = playerInventory.HasItem("comida");

        switch (enemy.enemyType)
        {
            case EnemyEncounter.EnemyType.Faminto:
                if (hasFood) hostility += famintoHasFoodBonus;
                break;
            case EnemyEncounter.EnemyType.Assombrado:
                hostility += hasTorch ? -assombradoHasTorchPenalty : assombradoNoTorchBonus;
                break;
            case EnemyEncounter.EnemyType.Briguento:
                hostility += hasPickaxe ? -briguentoHasPickaxePenalty : briguentoNoPickaxeBonus;
                break;
        }

        hostility = Mathf.Clamp(hostility, 0f, 100f);
        float roll = Random.Range(0f, 100f);

        Debug.Log($"🎲 {enemy.enemyType} Hostilidade: {hostility}% | Rolagem: {roll:F2}");

        if (roll <= hostility)
        {
            Debug.Log($"{enemy.enemyType} se enfurece e te ataca! 💀 Jogador morreu.");
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayDeath();

            Debug.Log("🔁 Reiniciando cenário...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.Log($"{enemy.enemyType} deixa você passar...");
        }

        Destroy(enemy.gameObject);
    }

    // 🩸 Encontro final com o Diabo
    private IEnumerator HandleDevilEncounter()
    {
        Debug.Log("😈 O Diabo surge das sombras...");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDevilLaugh();

        yield return new WaitForSeconds(2f);

        // Verifica se o jogador tem todas as oferendas
        string[] required = { "cigarro", "álcool", "folha de coca" };
        bool hasAll = true;
        foreach (string item in required)
        {
            if (!playerInventory.HasItem(item))
            {
                hasAll = false;
                break;
            }
        }

        if (hasAll)
        {
            DialogueManager.Instance.ShowDialogue(new string[]
            {
                "🔥 O Diabo sorri.",
                "Você trouxe todas as oferendas certas.",
                "Fim de jogo."
            });

            Debug.Log("🏁 Final bom atingido.");
        }
        else
        {
            DialogueManager.Instance.ShowDialogue(new string[]
            {
                "😈 O Diabo ruge.",
                "Faltam oferendas...",
                "Você se perde para sempre."
            });

            Debug.Log("☠️ Fim ruim — faltaram oferendas.");

            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
