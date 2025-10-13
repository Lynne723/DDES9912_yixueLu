using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LotteryMachine : MonoBehaviour
{
    public float rotationSpeed = 200f;
    public float shakeDuration = 5f;
    public int ballsToSelect = 3;
    public GameObject ballPrefab;
    public int ballCount = 10;
    public float spawnRadius = 0.8f;
    public Transform[] targetPositions;
    public float moveSpeed = 5f;
    private List<GameObject> smallBalls = new List<GameObject>();
    private bool isRotating = false;
    private float timer = 0f;
    private Quaternion originalRotation;
    private List<GameObject> selectedBalls = new List<GameObject>();
    private bool isMovingBalls = false;

    void Start()
    {
        originalRotation = transform.rotation;
        for (int i = 0; i < ballCount; i++)
        {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
            ball.tag = "SmallBall";
            BallID ballID = ball.GetComponent<BallID>();
            if (ballID != null)
            {
                ballID.id = i + 1;
            }
            smallBalls.Add(ball);
        }
        if (targetPositions.Length < ballsToSelect)
        {
            Debug.LogWarning("Target positions array is too short! Please assign enough positions.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartRotation();
        }
        if (isRotating)
        {
            RotateMachine();
        }
        if (isMovingBalls)
        {
            MoveSelectedBalls();
        }
    }

    void StartRotation()
    {
        if (!isRotating && !isMovingBalls)
        {
            isRotating = true;
            timer = shakeDuration;
            selectedBalls.Clear();
        }
    }

    void RotateMachine()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            StopRotation();
            SelectBalls();
            return;
        }
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }

    void StopRotation()
    {
        isRotating = false;
    }

    void SelectBalls()
    {
        int selectCount = Mathf.Min(ballsToSelect, smallBalls.Count);
        selectedBalls = smallBalls.OrderBy(x => Random.value).Take(selectCount).ToList();
        for (int i = 0; i < selectedBalls.Count; i++)
        {
            GameObject ball = selectedBalls[i];
            BallID ballID = ball.GetComponent<BallID>();
            Debug.Log($"Ñ¡³öÇò£º {i + 1}: ID {ballID.id}");
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        isMovingBalls = true;
    }

    void MoveSelectedBalls()
    {
        bool allBallsInPosition = true;
        for (int i = 0; i < selectedBalls.Count; i++)
        {
            GameObject ball = selectedBalls[i];
            if (i >= targetPositions.Length) break;
            Vector3 targetPos = targetPositions[i].position;
            Vector3 currentPos = ball.transform.position;
            ball.transform.position = Vector3.Lerp(currentPos, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(currentPos, targetPos) > 0.01f)
            {
                allBallsInPosition = false;
            }
        }
        if (allBallsInPosition)
        {
            isMovingBalls = false;
        }
    }
}