using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class BallMachine : MonoBehaviour
{
    public TextMeshPro resultText;
    public GameObject[] colorSelectModels;
    public GameObject[] colorDisplayModels;
    public GameObject startButtonModel;
    public GameObject rotateButtonModel;
    public GameObject stopButtonModel;

    public GameObject ballPrefab;
    public Transform container;
    public Transform drawPosition;

    private List<GameObject> balls = new List<GameObject>();
    private List<Color> selectedColors = new List<Color>();
    private List<Color> drawnColors = new List<Color>();
    private Rigidbody containerRigidbody;

    private Color[] availableColors = new Color[]
    {
        Color.red, Color.blue, Color.green, Color.yellow, Color.magenta,
        Color.cyan, Color.white, Color.black, new Color(1f, 0.5f, 0f), Color.gray
    };

    private int[] colorIndices = new int[3];

    private bool canStart = true;
    private bool canRotate = false;
    private bool canStop = false;

    private RotatingObjectController rotatingObjectController;

    void Start()
    {
        containerRigidbody = container.GetComponent<Rigidbody>();
        if (containerRigidbody == null)
        {
            containerRigidbody = container.gameObject.AddComponent<Rigidbody>();
            containerRigidbody.useGravity = false;
            containerRigidbody.isKinematic = true;
        }

        rotatingObjectController = GetComponent<RotatingObjectController>();

        for (int i = 0; i < colorSelectModels.Length; i++)
        {
            colorIndices[i] = i % availableColors.Length;
            UpdateColorDisplay(i);
        }

        resultText.text = "0";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                for (int i = 0; i < colorSelectModels.Length; i++)
                {
                    if (hit.collider.gameObject == colorSelectModels[i])
                    {
                        ChangeColor(i);
                        return;
                    }
                }

                if (hit.collider.gameObject == startButtonModel && canStart)
                {
                    StartMachine();
                }
                else if (hit.collider.gameObject == rotateButtonModel && canRotate)
                {
                    RotateContainer();
                }
                else if (hit.collider.gameObject == stopButtonModel && canStop)
                {
                    StopMachine();
                }
            }
        }
    }

    void UpdateColorDisplay(int index)
    {
        colorDisplayModels[index].GetComponent<Renderer>().material.color = availableColors[colorIndices[index]];
    }

    void ChangeColor(int index)
    {
        colorIndices[index] = (colorIndices[index] + 1) % availableColors.Length;
        UpdateColorDisplay(index);
    }

    void StartMachine()
    {
        foreach (var ball in balls)
        {
            Destroy(ball);
        }
        balls.Clear();
        drawnColors.Clear();
        resultText.text = "0";

        for (int i = 0; i < 10; i++)
        {
            GameObject ball = Instantiate(ballPrefab, container);
            ball.GetComponent<Renderer>().material.color = availableColors[i];
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = ball.AddComponent<Rigidbody>();
                rb.useGravity = true;
            }
            balls.Add(ball);
            ball.transform.localPosition = Vector3.zero;
        }

        canStart = false;
        canRotate = true;
        canStop = false;
    }

    void RotateContainer()
    {
        selectedColors.Clear();
        for (int i = 0; i < colorIndices.Length; i++)
        {
            selectedColors.Add(availableColors[colorIndices[i]]);
        }

        containerRigidbody.angularVelocity = new Vector3(0, 5f, 0);
        if (rotatingObjectController != null)
        {
            rotatingObjectController.StartRotating();
        }

        StartCoroutine(DrawBalls());
    }

    IEnumerator DrawBalls()
    {
        drawnColors.Clear();
        List<GameObject> remainingBalls = new List<GameObject>(balls);

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 3; i++)
        {
            if (remainingBalls.Count == 0) break;

            int randomIndex = Random.Range(0, remainingBalls.Count);
            GameObject drawnBall = remainingBalls[randomIndex];

            drawnBall.transform.SetParent(null);
            drawnBall.transform.position = drawPosition.position;

            Rigidbody rb = drawnBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Color col = drawnBall.GetComponent<Renderer>().material.color;
            drawnColors.Add(col);

            remainingBalls.RemoveAt(randomIndex);

            yield return new WaitForSeconds(2f);
        }

        int matchCount = 0;
        foreach (var drawnColor in drawnColors)
        {
            if (selectedColors.Contains(drawnColor))
            {
                matchCount++;
            }
        }

        resultText.text = matchCount.ToString();

      //  canRotate = false;
        canStop = true;

    }

    void StopMachine()
    {
        containerRigidbody.angularVelocity = Vector3.zero;
        if (rotatingObjectController != null)
        {
            rotatingObjectController.StopRotating();
        }

        foreach (var ball in balls)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        canStart = true;
        canRotate = false;
        canStop = false;
    }
}