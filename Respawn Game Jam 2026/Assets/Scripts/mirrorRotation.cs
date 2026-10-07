using UnityEngine;

public class mirrorRotation : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private Camera playerCamera;

    [Header("Rotation Settings")]
    [SerializeField]
    private float rotationAmount = 90f;

    [SerializeField]
    private float rotationDuration = 1f;

    [SerializeField]
    private float cooldownTime = 2f;

    private bool isRotating = false;
    private float cooldownTimer = 0f;

    private float rotationStart;
    private float rotationTarget;
    private float rotationTimer;

    // Update is called once per frame
    void Update()
    {
        if(cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if(isRotating)
        {
            rotationTimer += Time.deltaTime;

            float t = rotationTimer / rotationDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            float currentRotation = Mathf.LerpAngle(rotationStart, rotationTarget, t);

            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentRotation, transform.eulerAngles.z);

            if(rotationTimer >= rotationDuration)
            {
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotationTarget, transform.eulerAngles.z);

                isRotating = false;
                cooldownTimer = cooldownTime;
            }

            return;

        }

        if(!IsCursorOverThisMirror())
        {
            return;
        }

        if (cooldownTimer > 0f)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            RotateRight();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            RotateLeft();
        }
        
    }

    bool IsCursorOverThisMirror()
    {
        if(playerCamera == null)
        {
            return false;
        }

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            return hit.transform == transform;
        }

        return false;
    }

    void RotateRight()
    {
        isRotating = true;
        rotationTimer = 0f;

        rotationStart = transform.eulerAngles.y;
        rotationTarget = rotationStart + rotationAmount;
    }

    void RotateLeft()
    {
        isRotating = true;
        rotationTimer = 0f;

        rotationStart = transform.eulerAngles.y;
        rotationTarget = rotationStart - rotationAmount;
    }

    
}
