using UnityEngine;

public class TunnelMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 2f;
    public Vector3[] stopPositions;
    public bool isMoving = false;

    private int currentIndex = 0;
    private Vector3 targetPosition;

    [Header("Som de Passos")]
    public float stepInterval = 0.5f; // tempo entre passos
    private float stepTimer;

    private void Start()
    {
        if (stopPositions.Length > 0)
            transform.position = stopPositions[0];

        isMoving = false;
        stepTimer = stepInterval;
    }

    private void Update()
    {
        if (!isMoving) return;

        // movimenta o cenário (dando impressão de que o jogador anda)
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // toca passos enquanto se move
        HandleFootsteps();

        // checa chegada no ponto de parada
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            FindObjectOfType<GameManager>().OnTunnelArrived();
        }
    }

    public void MoveToNextPosition()
    {
        if (currentIndex >= stopPositions.Length - 1 || isMoving) return;

        currentIndex++;
        targetPosition = stopPositions[currentIndex];
        isMoving = true;

        // chance de tocar sussurros no meio do caminho
        if (Random.value < 0.25f)
        {
            AudioManager.Instance.PlayWhisper();
        }
    }

    private void HandleFootsteps()
    {
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f)
        {
            AudioManager.Instance.PlayStep();
            stepTimer = stepInterval;
        }
    }

    // chamada quando encontra um minerador (fala estilo Animal Crossing)
    public void PlayMinerVoice()
    {
        AudioManager.Instance.PlayVoice();
    }

    // chamada quando chega no El Tío (risada marcante)
    public void PlayDevilIntro()
    {
        AudioManager.Instance.PlayDevilLaugh();
    }
}
