using UnityEngine;

public class TunnelMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 2f;
    public Vector3[] stopPositions;
    public bool isMoving = false;

    private int currentIndex = 0;
    private Vector3 targetPosition;

    private void Start()
    {
        if (stopPositions.Length > 0)
            transform.position = stopPositions[0];

        isMoving = false;

        AudioManager.Instance.stepsSource.loop = true;
    }

    private void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            AudioManager.Instance.StopSteps();
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

        AudioManager.Instance.StartSteps();

        if (Random.value < 0.25f)
            AudioManager.Instance.PlayWhisper();
    }

    // 🔹 Controle da fala
    public void StartMinerVoice()
    {
        AudioManager.Instance.StartVoice();
    }

    public void StopMinerVoice()
    {
        AudioManager.Instance.StopVoice();
    }

    // 🔹 El Tío
    public void PlayDevilIntro()
    {
        AudioManager.Instance.PlayDevilLaugh();
    }
}
