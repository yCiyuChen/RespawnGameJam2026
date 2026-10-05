using System.Collections.Generic;
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

    [SerializeField]
    private Color laserColor = Color.red;
    private List<LineRenderer> laserSegments = new List<LineRenderer>();

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
        ClearLaserSegments();

        Color currentColor = laserColor;

        for(int i=0; i<maxBounces; i++)
        {
            Ray ray = new Ray(position, direction);


            if(Physics.Raycast(ray, out RaycastHit hit, laserDistance))
            {
                Debug.Log("Laser hit: " + hit.collider.gameObject.name);

                CreateLaserSegment(position, hit.point, currentColor);

                if(hit.collider.CompareTag("Color"))
                {
                    ColorBoard colorBoard = hit.collider.GetComponent<ColorBoard>();
                    if(colorBoard != null)
                    {
                        currentColor = colorBoard.laserColor;
                    }

                    position = hit.point + direction * rayOffset;

                    continue;
                }

                if(hit.collider.CompareTag("Mirror"))
                {
                    direction = Vector3.Reflect(direction, hit.normal);

                    position = hit.point + direction * rayOffset;

                    continue;
                }

                return;
            }
            else
            {
                Vector3 endPoint = position + direction * laserDistance;

                CreateLaserSegment(position, endPoint, currentColor);

                return;
            }
        }
    }

    void CreateLaserSegment(Vector3 start, Vector3 end, Color color)
    {
        GameObject segmentObject = new GameObject("Laser Segment");

        segmentObject.transform.SetParent(transform);

        LineRenderer segment = segmentObject.AddComponent<LineRenderer>();

        LineRenderer original = GetComponent<LineRenderer>();

        if(original != null)
        {
            segment.material = original.material;
            segment.startWidth = original.startWidth;
            segment.endWidth = original.endWidth;
            segment.alignment = original.alignment;
            segment.textureMode = original.textureMode;
        }

        segment.positionCount = 2;

        segment.SetPosition(0, start);
        segment.SetPosition(1, end);

        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
            );

        segment.colorGradient = gradient;

        laserSegments.Add(segment);

    }

    void ClearLaserSegments()
    {
        foreach(LineRenderer segment in laserSegments)
        {
            if(segment != null)
            {
                Destroy(segment.gameObject);
            }
        }

        laserSegments.Clear();
    }
}
