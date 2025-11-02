using UnityEngine;

public class TunnelMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Vector3[] stopPositions;
    public bool isMoving = false;

    private int currentIndex = 0;
    private Vector3 targetPosition;

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
            FindObjectOfType<GameManager>().OnTunnelArrived();
        }
    }

    public void MoveToNextPosition()
    {
        if (currentIndex >= stopPositions.Length - 1 || isMoving) return;

        currentIndex++;
        targetPosition = stopPositions[currentIndex];
        isMoving = true;
    }
}
