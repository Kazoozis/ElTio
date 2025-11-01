using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // inimigo1, inimigo2, inimigo3
    private int currentEncounter = 0;
    private bool isEncounterActive = false;
    private EnemyEncounter activeEnemy;
    private PlayerInventory playerInventory;

    // posição fixa para spawn do inimigo
    private Vector3 enemySpawnPosition = new Vector3(1.47f, 0.9999999f, 3.755702f);

    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        Debug.Log("Aperte ESPAÇO para avançar na mina...");
    }

    void Update()
    {
        // Avançar na mina (pressionar espaço)
        if (Input.GetKeyDown(KeyCode.Space) && !isEncounterActive)
        {
            if (currentEncounter < enemyPrefabs.Length)
            {
                SpawnEnemy();
            }
            else
            {
                Debug.Log("Você chegou ao altar do Diabo...");
                playerInventory.CheckOfferings();
            }
        }

        // Resposta do jogador (1 = trocar, 2 = recusar)
        if (isEncounterActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                activeEnemy.Trade(playerInventory);
                EndEncounter();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Você recusou a troca.");
                EndEncounter();
            }
        }
    }

    void SpawnEnemy()
    {
        // Instancia o inimigo na posição fixa e sem rotação
        GameObject enemyObj = Instantiate(enemyPrefabs[currentEncounter], enemySpawnPosition, Quaternion.identity);

        activeEnemy = enemyObj.GetComponent<EnemyEncounter>();
        activeEnemy.StartEncounter();

        isEncounterActive = true;
        currentEncounter++;
    }

    void EndEncounter()
    {
        isEncounterActive = false;
        Debug.Log("Aperte ESPAÇO para continuar explorando...");
    }
}
