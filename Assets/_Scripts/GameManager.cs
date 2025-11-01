using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs de Inimigos (Faminto, Assombrado, Briguento)")]
    public GameObject[] enemyPrefabs;

    [Header("Chance Base de Ataque (%)")]
    [Tooltip("Chance inicial de qualquer inimigo atacar ao recusar a troca.")]
    public float baseHostilityChance = 15f;

    [Header("Faminto — modificadores (%)")]
    [Tooltip("+X% se jogador TEM comida")]
    public float famintoHasFoodBonus = 5f;

    [Header("Assombrado — modificadores (%)")]
    [Tooltip("+X% se jogador NÃO TEM tocha")]
    public float assombradoNoTorchBonus = 5f;
    [Tooltip("-X% se jogador TEM tocha")]
    public float assombradoHasTorchPenalty = 5f;

    [Header("Briguento — modificadores (%)")]
    [Tooltip("+X% se jogador NÃO TEM picareta")]
    public float briguentoNoPickaxeBonus = 5f;
    [Tooltip("-X% se jogador TEM picareta")]
    public float briguentoHasPickaxePenalty = 5f;

    private List<(GameObject prefab, string type)> encounters = new List<(GameObject, string)>();
    private int currentEncounter = 0;
    private bool isEncounterActive = false;
    private EnemyEncounter activeEnemy;
    private PlayerInventory playerInventory;

    private Vector3 enemySpawnPosition = new Vector3(1.47f, 0.9999999f, 3.755702f);

    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
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
        var (prefab, tradeType) = encounters[currentEncounter];
        GameObject enemyObj = Instantiate(prefab, enemySpawnPosition, Quaternion.identity);

        activeEnemy = enemyObj.GetComponent<EnemyEncounter>();
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

        for (int i = 0; i < 6; i++)
        {
            string[] types = { "item>oferenda", "oferenda>item", "oferenda>oferenda" };
            string chosenType = types[Random.Range(0, types.Length)];
            GameObject chosenEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            encounters.Add((chosenEnemy, chosenType));
        }

        Debug.Log("Foram gerados " + encounters.Count + " encontros aleatórios.");
    }

    // ⚔️ Cálculo de hostilidade baseado no tipo e inventário
    void HandleRefusal(EnemyEncounter enemy)
    {
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
                if (temTocha)
                    hostility -= assombradoHasTorchPenalty;
                else
                    hostility += assombradoNoTorchBonus;
                break;

            case EnemyEncounter.EnemyType.Briguento:
                if (temPicareta)
                    hostility -= briguentoHasPickaxePenalty;
                else
                    hostility += briguentoNoPickaxeBonus;
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
