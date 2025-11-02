using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float baseHostilityChance = 15f;
    public float famintoHasFoodBonus = 5f;
    public float assombradoNoTorchBonus = 5f;
    public float assombradoHasTorchPenalty = 5f;
    public float briguentoNoPickaxeBonus = 5f;
    public float briguentoHasPickaxePenalty = 5f;

    private List<(GameObject prefab, string type)> encounters = new();
    private int currentEncounter = 0;

    private bool isEncounterActive = false;
    private bool isDialogueActive = false;
    private bool waitingForTunnel = false;

    private EnemyEncounter activeEnemy;
    private PlayerInventory playerInventory;
    private TunnelMovement tunnelMovement;

    private readonly Vector3 enemySpawnPosition = new Vector3(1.47f, 1f, 3.75f);

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        tunnelMovement = FindObjectOfType<TunnelMovement>();

        GenerateEncounters();
        Debug.Log("Aperte ESPAÇO para avançar...");
    }

    public void SetDialogueState(bool state)
    {
        isDialogueActive = state;
    }

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
        var (prefab, type) = encounters[currentEncounter];
        GameObject obj = Instantiate(prefab, enemySpawnPosition, Quaternion.identity);
        activeEnemy = obj.GetComponent<EnemyEncounter>();

        activeEnemy.ConfigureEncounter(type);
        activeEnemy.StartEncounter();

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
        string[] t = { "item>oferenda", "oferenda>item", "oferenda>oferenda" };

        for (int i = 0; i < 6; i++)
            encounters.Add((enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], t[Random.Range(0, t.Length)]));
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
                if (hasFood)
                    hostility += famintoHasFoodBonus;
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