using UnityEngine;

public class NPCPathMovement : MonoBehaviour
{
    [Header("MoveGroup")]
    public Transform npc;
    public float moveSpeed = 3f;
    private float _originalSpeed;

    [Header("AreaSize")]
    public Transform areaCenter;
    public Vector2 areaSize = new Vector2(10, 10);

    [Header("RotateSet")]
    public Transform rotateTarget;
    public float rotateSpeed = 20f;

    [Header("StopArea")]
    public Transform stopZoneCenter;
    public float stopZoneRadius = 5f;
    //size set//
    private Vector3 _moveDir;
    private Vector3 _areaMin;
    private Vector3 _areaMax;

    void Start()
    {
        if (areaCenter == null || rotateTarget == null)
        {
            Debug.LogError("please set areaCenter and rotateTarget", this);
            enabled = false;
            return;
        }
        _originalSpeed = moveSpeed;
        RandomizeDir();
        UpdateAreaBoundary();
    }

    void Update()
    {
        UpdateAreaBoundary();
        CheckStopZone();
        if (moveSpeed > 0)
        {
            RotateAroundTargetYAxis();
            MoveInArea();
            CheckBoundaryAndTurn();
        }
    }

    //set area bound//

    void UpdateAreaBoundary()
    {
        _areaMin = new Vector3(
            areaCenter.position.x - areaSize.x / 2,
            areaCenter.position.y,
            areaCenter.position.z - areaSize.y / 2
        );
        _areaMax = new Vector3(
            areaCenter.position.x + areaSize.x / 2,
            areaCenter.position.y,
            areaCenter.position.z + areaSize.y / 2
        );
    }

    /// Check Stop Area//
    void CheckStopZone()
    {
        if (stopZoneCenter == null) return;
        float distance = Vector3.Distance(
            new Vector3(npc.position.x, 0, npc.position.z),
            new Vector3(stopZoneCenter.position.x, 0, stopZoneCenter.position.z)
        );

        moveSpeed = distance <= stopZoneRadius ? 0 : _originalSpeed;
    }

    //the object rotate around its own Y-axis//
    void RotateAroundTargetYAxis()
    {
        Vector3 dirToMove = _moveDir;
        dirToMove.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dirToMove);
        targetRot = Quaternion.Euler(
            0,
            targetRot.eulerAngles.y,
            0
        );
        npc.rotation = Quaternion.Lerp(npc.rotation, targetRot, Time.deltaTime * rotateSpeed);
    }

    //move in area//
    void MoveInArea()
    {
        npc.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
    }

    //move in a random direction//
    void RandomizeDir()
    {
        _moveDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    //check the boundary and turn//
    void CheckBoundaryAndTurn()
    {
        Vector3 pos = npc.position;
        bool isHitBoundary = false;

        if (pos.x <= _areaMin.x || pos.x >= _areaMax.x)
        {
            _moveDir.x = -_moveDir.x;
            isHitBoundary = true;
            pos.x = Mathf.Clamp(pos.x, _areaMin.x + 0.1f, _areaMax.x - 0.1f);
        }

        if (pos.z <= _areaMin.z || pos.z >= _areaMax.z)
        {
            _moveDir.z = -_moveDir.z;
            isHitBoundary = true;
            pos.z = Mathf.Clamp(pos.z, _areaMin.z + 0.1f, _areaMax.z - 0.1f);
        }

        if (isHitBoundary)
        {
            npc.position = pos;
            _moveDir = _moveDir.normalized;
        }
    }

    //draw guide line(can show in scene)//

    void OnDrawGizmos()
    {
        if (areaCenter == null) return;

        //draw move area//
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter.position, new Vector3(areaSize.x, 0.1f, areaSize.y));

        //draw rotate object//
        if (rotateTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(rotateTarget.position, 0.3f);
        }

        //draw stop area//
        if (stopZoneCenter != null)
        {
            Gizmos.color = moveSpeed == 0 ? Color.red : Color.yellow;//red is stop,yellow is ok//
            Gizmos.DrawWireSphere(stopZoneCenter.position, stopZoneRadius);
        }
    }
}