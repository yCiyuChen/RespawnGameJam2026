using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private float openHeight = 3f;

    [SerializeField]
    private float openSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen = false;

    private int activatedPlates = 0;

    [SerializeField]
    private int requiredPlates = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPosition = transform.position;

        openPosition = closedPosition + Vector3.up * openHeight;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition;

        if(isOpen)
        {
            targetPosition = openPosition;
        }
        else
        {
            targetPosition = closedPosition;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
    }

    public void PlateActivated()
    {
        activatedPlates++;

        UpdateDoorState();
    }

    public void PlateDeactivated()
    {
        activatedPlates--;

        activatedPlates = Mathf.Max(activatedPlates, 0);

        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        if(activatedPlates >= requiredPlates)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
    }

}
