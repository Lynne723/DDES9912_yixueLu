using UnityEngine;

public class BallID : MonoBehaviour
{
    public int id;

    void Start()
    {
        gameObject.name = id.ToString() + "ºÅ";
    }
}
