using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField]
    private Color requiredColor = Color.red;

    [Header("Door")]
    [SerializeField]
    private Door door;

    private bool isActivated = false;

    private AudioSource audioSource;
    private int audioCount = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = Mathf.Clamp01(0.7f);
    }

    public void ReceiveLaser(Color laserColor)
    {
        if(ColorsMatch(laserColor, requiredColor))
        {
            Activated();
        }
        else
        {
            Deactivated();
        }
    }

    private void Activated()
    {
        if(isActivated)
        {
            return;
        }

        if(audioCount == 1)
        {
            audioSource.mute = true;
        }

        isActivated = true;
        audioSource.Play();
        audioCount++;

        if (door != null)
        {
            door.PlateActivated();
        }
    }

    public void LaserStopped()
    {
        Deactivated();
    }

    private void Deactivated()
    {
        if(!isActivated)
        {
            return;
        }

        isActivated = false;

        if(door != null)
        {
            door.PlateDeactivated();
        }
    }

    bool ColorsMatch(Color color1, Color color2)
    {
        float tolerance = 0.05f;

        return Mathf.Abs(color1.r - color2.r) < tolerance
            && Mathf.Abs(color1.g - color2.g) < tolerance
            && (color1.b - color2.b) < tolerance;
    }
    
}
