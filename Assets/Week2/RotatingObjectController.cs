using UnityEngine;

public class RotatingObjectController : MonoBehaviour
{
    public Transform rotatingObject;
    private bool isRotating = false;

    public void StartRotating()
    {
        isRotating = true;
    }


    public void StopRotating()
    {
        isRotating = false;
    }

    void Update()
    {
        if (isRotating && rotatingObject != null)
        {
            rotatingObject.Rotate(0, 0, 90f * Time.deltaTime);
        }
    }
}