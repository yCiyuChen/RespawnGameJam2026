using System.Collections.Generic;
using UnityEngine;

// Scene prototype: a board converts red to purple by an explicit game rule.
// Separate renderers retain the color of each segment before/after the board.
[RequireComponent(typeof(LineRenderer))]
public class Level3Laser : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private int maxBounces = 20;
    [SerializeField] private bool reflectOnlyMirror = true;
    [SerializeField] private float laserDistance = 30f;
    [SerializeField] private float rayOffset = 0.01f;
    [SerializeField] private Collider colorBoard;
    [SerializeField] private Color initialColor = new Color(1f, 0.1f, 0.05f);
    [SerializeField] private Color filteredColor = new Color(0.7f, 0.15f, 1f);

    private LineRenderer template;
    private readonly List<LineRenderer> segments = new List<LineRenderer>();
    private int usedSegments;

    private void Awake()
    {
        template = GetComponent<LineRenderer>();
        template.enabled = false;
    }

    private void Update()
    {
        usedSegments = 0;
        Vector3 origin = startPoint != null ? startPoint.position : transform.position;
        Vector3 direction = -transform.right;
        Color color = initialColor;
        int limit = Mathf.Clamp(maxBounces, 1, 100);
        float offset = Mathf.Max(rayOffset, 0.001f);

        for (int i = 0; i < limit; i++)
        {
            if (!Physics.Raycast(origin, direction, out RaycastHit hit,
                                 laserDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                DrawSegment(origin, origin + direction * laserDistance, color);
                break;
            }

            DrawSegment(origin, hit.point, color);
            if (colorBoard != null && hit.collider == colorBoard)
            {
                // Find the far surface using a reverse ray starting outside its bounds.
                // Continue after it so the next cast cannot get stuck inside the board.
                float span = colorBoard.bounds.size.magnitude + 1f;
                Vector3 beyond = hit.point + direction * span;
                if (!colorBoard.Raycast(new Ray(beyond, -direction), out RaycastHit exit, span + offset))
                    break;
                color = filteredColor;
                DrawSegment(hit.point, exit.point, color);
                origin = exit.point + direction * offset;
            }
            else if (!reflectOnlyMirror || hit.collider.CompareTag("Mirror"))
            {
                direction = Vector3.Reflect(direction, hit.normal).normalized;
                origin = hit.point + direction * offset;
            }
            else
            {
                break;
            }
        }

        for (int i = usedSegments; i < segments.Count; i++)
            segments[i].enabled = false;
    }

    private void DrawSegment(Vector3 from, Vector3 to, Color color)
    {
        if (usedSegments == segments.Count)
        {
            GameObject child = new GameObject("BeamSegment_" + usedSegments);
            child.transform.SetParent(transform, false);
            LineRenderer line = child.AddComponent<LineRenderer>();
            line.sharedMaterial = template.sharedMaterial;
            line.useWorldSpace = true;
            line.widthMultiplier = 0.06f;
            line.numCapVertices = 4;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            segments.Add(line);
        }
        LineRenderer segment = segments[usedSegments++];
        segment.enabled = true;
        segment.positionCount = 2;
        segment.startColor = color;
        segment.endColor = color;
        segment.SetPosition(0, from);
        segment.SetPosition(1, to);
    }
}
