using UnityEngine;
using System.Collections;

public class TunnelMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Vector3[] stopPositions;
    public bool isMoving = false;

    private int currentIndex = 0;
    private Vector3 targetPosition;
    private Coroutine whisperRoutine;

    void Start()
    {
        if (stopPositions.Length > 0)
            transform.position = stopPositions[0];
        isMoving = false;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;

            // 🎧 Para os passos e o sussurro quando parar
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopSteps();

            if (whisperRoutine != null)
            {
                StopCoroutine(whisperRoutine);
                whisperRoutine = null;
            }

            FindObjectOfType<GameManager>().OnTunnelArrived();
        }
    }

    public void MoveToNextPosition()
    {
        if (currentIndex >= stopPositions.Length - 1 || isMoving) return;

        currentIndex++;
        targetPosition = stopPositions[currentIndex];
        isMoving = true;

        // 🎧 Inicia passos e sussurros aleatórios
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartSteps();
            if (whisperRoutine != null)
                StopCoroutine(whisperRoutine);
            whisperRoutine = StartCoroutine(RandomWhisperLoop());
        }
    }

    private IEnumerator RandomWhisperLoop()
    {
        while (isMoving)
        {
            yield return new WaitForSeconds(Random.Range(3f, 8f));
            if (AudioManager.Instance != null && isMoving)
                AudioManager.Instance.PlayWhisper();
        }
    }
}
