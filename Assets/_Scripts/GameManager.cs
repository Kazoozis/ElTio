using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs de Inimigos (Faminto, Assombrado, Briguento)")]
    public GameObject[] enemyPrefabs;

    [Header("Chance Base de Ataque (%)")]
    public float baseHostilityChance = 15f;

    [Header("Faminto — modificadores (%)")]
    public float famintoHasFoodBonus = 5f;

    [Header("Assombrado — modificadores (%)")]
    public float assombradoNoTorchBonus = 5f;
    public float assombradoHasTorchPenalty = 5f;

    [Header("Briguento — modificadores (%)")]
    public float briguentoNoPickaxeBonus = 5f;
    public float briguentoHasPickaxePenalty = 5f;

    private List<(GameObject prefab, string type)> encounters = new();
    private int currentEncounter = 0;
    private bool isEncounterActive = false;
    private EnemyEncounter activeEnemy;
    private PlayerInventory playerInventory;

    private readonly Vector3 enemySpawnPosition = new(1.47f, 0.9999999f, 3.755702f);

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("❌ Nenhum PlayerInventory encontrado na cena! Adicione o script ao jogador.");
            enabled = false;
            return;
        }

        GenerateEncounters();
        Debug.Log("Aperte ESPAÇO para avançar na mina...");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isEncounterActive)
        {
            if (currentEncounter < encounters.Count)
            {
                SpawnEnemy();
            }
            else
            {
                Debug.Log("Você chegou ao altar do Diabo...");
                playerInventory.CheckOfferings();
            }
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

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("❌ Nenhum prefab de inimigo atribuído no GameManager!");
            return;
        }

        var (prefab, tradeType) = encounters[currentEncounter];
        if (prefab == null)
        {
            Debug.LogError($"❌ Prefab de inimigo na posição {currentEncounter} é nulo!");
            return;
        }

        GameObject enemyObj = Instantiate(prefab, enemySpawnPosition, Quaternion.identity);
        activeEnemy = enemyObj.GetComponent<EnemyEncounter>();

        if (activeEnemy == null)
        {
            Debug.LogError("❌ Prefab de inimigo não contém o script EnemyEncounter!");
            Destroy(enemyObj);
            return;
        }

        activeEnemy.ConfigureEncounter(tradeType);
        activeEnemy.StartEncounter();

        isEncounterActive = true;
        currentEncounter++;
    }

    void EndEncounter()
    {
        isEncounterActive = false;
        Debug.Log("Aperte ESPAÇO para continuar explorando...");
    }

    void GenerateEncounters()
    {
        encounters.Clear();
        string[] types = { "item>oferenda", "oferenda>item", "oferenda>oferenda" };

        for (int i = 0; i < 6; i++)
        {
            string chosenType = types[Random.Range(0, types.Length)];
            GameObject chosenEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            encounters.Add((chosenEnemy, chosenType));
        }

        Debug.Log($"Foram gerados {encounters.Count} encontros aleatórios.");
    }

    void HandleRefusal(EnemyEncounter enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("⚠️ Nenhum inimigo ativo para recusar.");
            return;
        }

        float hostility = baseHostilityChance;

        bool temPicareta = playerInventory.HasItem("picareta");
        bool temTocha = playerInventory.HasItem("tocha");
        bool temComida = playerInventory.HasItem("comida");

        switch (enemy.enemyType)
        {
            case EnemyEncounter.EnemyType.Faminto:
                if (temComida)
                    hostility += famintoHasFoodBonus;
                break;

            case EnemyEncounter.EnemyType.Assombrado:
                hostility += temTocha ? -assombradoHasTorchPenalty : assombradoNoTorchBonus;
                break;

            case EnemyEncounter.EnemyType.Briguento:
                hostility += temPicareta ? -briguentoHasPickaxePenalty : briguentoNoPickaxeBonus;
                break;
        }

        hostility = Mathf.Clamp(hostility, 0f, 100f);
        float roll = Random.Range(0f, 100f);

        Debug.Log($"Chance de hostilidade ({enemy.enemyType}): {hostility}% | Rolagem: {roll:F2}%");

        if (roll <= hostility)
        {
            Debug.Log($"{enemy.enemyType} se enfurece e te ataca! 💀 Jogador morreu.");
            Debug.Log("🔁 Reiniciando cenário...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.Log($"{enemy.enemyType} apenas te encara e te deixa ir...");
        }

        Destroy(enemy.gameObject);
    }
}
