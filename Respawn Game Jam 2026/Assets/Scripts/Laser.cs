using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField]
    private Transform startPoint;

    //laser bounce variables
    [SerializeField]
    int maxBounces = 20;
    [SerializeField]
    private bool reflectOnlyMirror;
    [SerializeField]
    private float laserDistance = 300f;
    [SerializeField]
    private float rayOffset = 0.01f;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CastLaser(transform.position, -transform.right);
    }

    void CastLaser(Vector3 position, Vector3 direction)
    {
        Vector3[] points = new Vector3[maxBounces + 1];

        points[0] = startPoint.position;

        int pointCount = 1;

        for(int i=0; i<maxBounces; i++)
        {
            Ray ray = new Ray(position, direction);


            if(Physics.Raycast(ray, out RaycastHit hit, laserDistance))
            {
                Debug.Log("Laser hit: " + hit.collider.gameObject.name);

                points[pointCount] = hit.point;
                pointCount++;

                if(!hit.collider.CompareTag("Mirror"))
                {
                    break;
                }

                direction = Vector3.Reflect(direction, hit.normal);

                position = hit.point + direction * rayOffset;
            }
            else
            {
                points[pointCount] = position + direction * laserDistance;
                pointCount++;

                break;
            }
        }

        lr.positionCount = pointCount;

        for(int i=0; i<pointCount; i++)
        {
            lr.SetPosition(i, points[i]);
        }
    }
}
