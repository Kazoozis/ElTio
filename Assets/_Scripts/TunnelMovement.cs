using UnityEngine;
using System.Collections;

public class TunnelMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float moveSpeed = 2f; // velocidade do movimento
    public Vector3[] stopPositions; // posições exatas para cada encontro
    public bool isMoving = false;

    private int currentIndex = 0;
    private Vector3 targetPosition;

    void Start()
    {
        // Define o primeiro alvo como a posição inicial
        if (stopPositions.Length > 0)
            targetPosition = stopPositions[0];
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                Debug.Log($"🛑 Túnel chegou ao ponto {currentIndex + 1}/{stopPositions.Length}");
            }
        }
    }

    // 🔄 Chamado pelo GameManager a cada novo encontro
    public void MoveToNextPosition()
    {
        if (isMoving || currentIndex >= stopPositions.Length - 1)
            return;

        currentIndex++;
        targetPosition = stopPositions[currentIndex];
        isMoving = true;

        Debug.Log($"🚇 Movendo túnel para o ponto {currentIndex + 1}: {targetPosition}");
    }
}
