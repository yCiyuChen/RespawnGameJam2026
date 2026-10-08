using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    [SerializeField]
    private float openHeight = 100f;

    [SerializeField]
    private float openSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private AudioSource audioSource;
    private int audioCount = 0;

    [SerializeField]
    private Door door;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPosition = transform.position;

        openPosition = closedPosition + Vector3.up * openHeight;

        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition;

        if (door != null && door.isOpen)
        {
            targetPosition = openPosition;
        }
        else
        {
            targetPosition = closedPosition;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
    }

    private void UpdateDoorState()
    {
        if (door.isOpen)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (audioCount == 1)
        {
            audioSource.mute = true;
        }
        audioSource.Play();
        audioCount++;
    }

}
