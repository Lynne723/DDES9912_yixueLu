using UnityEngine;

public class MusicStart : MonoBehaviour
{
    Animator anim;
    public bool isSlap = false;          //slap status(adapt animation controller)//
    public GameObject objectA;           //object//
    public AudioClip bgm;                //set music//
    public Vector3 areaCenterPos;        //area center//
    public Vector3 areaSize = new Vector3(10, 10, 10); //area size//
    private AudioSource audio;

    void Start()
    {
        audio = gameObject.AddComponent<AudioSource>();
        audio.clip = bgm;
        audio.loop = true;
        audio.volume = 1;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (objectA == null || bgm == null) return;

        //set area size//
        Vector3 min = areaCenterPos - areaSize / 2;
        Vector3 max = areaCenterPos + areaSize / 2;
        Vector3 aPos = objectA.transform.position;

        bool inArea = aPos.x > min.x && aPos.x < max.x &&
                      aPos.y > min.y && aPos.y < max.y &&
                      aPos.z > min.z && aPos.z < max.z;

        if (inArea != isSlap)
        {
            isSlap = inArea;
            if (isSlap) audio.Play();
            else audio.Stop();
        }
        UpdateAnim();
    }

    //draw guide line
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaCenterPos, areaSize);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(areaCenterPos, 0.3f);
    }

    void UpdateAnim()
    {
        anim.SetBool("isSlap", isSlap);
    }
}