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

    [Header("Rotation Settings")]
    [SerializeField]
    private float rotationAmount = 90f;
    [SerializeField]
    private float rotationDuration = 1f;

    [SerializeField]
    private float cooldownTime = 2f;
    private float cooldownTimer = 0f;

    private int rotationStep = 0;
    private float startingRotation;

    private bool isRotating = false;
    private float rotationStart;
    private float rotationTarget;
    private float rotationTimer;

    private PressurePlate currentPressurePlate;

    [SerializeField]
    private Camera playerCamera;

    private AudioSource audioSource;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

        startingRotation = transform.localEulerAngles.y;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CastLaser(transform.position, -transform.right);

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isRotating)
        {
            rotationTimer += Time.deltaTime;

            float t = rotationTimer / rotationDuration;
            t = Mathf.Clamp01(t);
            t = Mathf.SmoothStep(0f, 1f, t);

            float currentRotation = Mathf.Lerp(rotationStart, rotationTarget, t);

            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, currentRotation, transform.localEulerAngles.z);

            if (t >= 1f)
            {
                transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, rotationTarget, transform.localEulerAngles.z);

                isRotating = false;
                cooldownTimer = cooldownTime;
            }

            return;
        }

        if (!IsCursorOverThisLaser())
        {
            return;
        }

        if (cooldownTimer > 0f)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            audioSource.Play();
            Rotate(1);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            audioSource.Play();
            Rotate(-1);
        }
    }

    void Rotate(int direction)
    {
        rotationStart = startingRotation + (rotationStep * rotationAmount);

        rotationStep += direction;

        rotationTarget = startingRotation + (rotationStep * rotationAmount);

        rotationTimer = 0f;
        isRotating = true;
    }

    bool IsCursorOverThisLaser()
    {
        Camera cam = playerCamera;

        if(cam == null)
        {
            return false;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            return hit.transform == transform;
        }

        return false;
    }

    void CastLaser(Vector3 position, Vector3 direction)
    {
        ClearLaserSegments();

        PressurePlate previousPressurePlate = currentPressurePlate;
        currentPressurePlate = null;

        Color currentColor = laserColor;

        for(int i=0; i<maxBounces; i++)
        {
            Ray ray = new Ray(position, direction);


            if(Physics.Raycast(ray, out RaycastHit hit, laserDistance))
            {

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

                if(hit.collider.CompareTag("PressurePlate"))
                {
                    PressurePlate plate = hit.collider.GetComponent<PressurePlate>();

                    if(plate != null)
                    {
                        currentPressurePlate = plate;
                        plate.ReceiveLaser(currentColor);
                    }

                    break;
                }

                break;
            }
            else
            {
                Vector3 endPoint = position + direction * laserDistance;

                CreateLaserSegment(position, endPoint, currentColor);

                break;
            }
        }

        if(previousPressurePlate != null && previousPressurePlate != currentPressurePlate)
        {
            previousPressurePlate.LaserStopped();
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
