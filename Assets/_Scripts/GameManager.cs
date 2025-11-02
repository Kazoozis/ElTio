using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs dos Inimigos")]
    public GameObject famintoPrefab;
    public GameObject assombradoPrefab;
    public GameObject briguentoPrefab;

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

    // posição e rotação exatas solicitadas
    private readonly Vector3 enemySpawnPosition = new Vector3(1.321f, 0.099f, 3.75f);
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
            playerInventory.CheckOfferings();
    }

    void SpawnEnemy()
    {
        var (prefab, type, tradeType) = encounters[currentEncounter];

        // instancia inimigo na posição e rotação exatas
        GameObject obj = Instantiate(prefab, enemySpawnPosition, enemySpawnRotation);

        activeEnemy = obj.GetComponent<EnemyEncounter>();
        activeEnemy.enemyType = type;
        activeEnemy.ConfigureEncounter(tradeType);
        activeEnemy.StartEncounter();

        // Se for o último encontro → risada do Diabo
        if (currentEncounter == encounters.Count - 1 && AudioManager.Instance != null)
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

        // ✅ Gera uma sequência com os 3 tipos de inimigo
        GameObject[] prefabs = { famintoPrefab, assombradoPrefab, briguentoPrefab };
        EnemyEncounter.EnemyType[] types = {
            EnemyEncounter.EnemyType.Faminto,
            EnemyEncounter.EnemyType.Assombrado,
            EnemyEncounter.EnemyType.Briguento
        };

        for (int i = 0; i < 6; i++)
        {
            int index = i % 3; // alterna entre Faminto, Assombrado e Briguento
            encounters.Add((prefabs[index], types[index], tradeTypes[Random.Range(0, tradeTypes.Length)]));
        }
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
}
