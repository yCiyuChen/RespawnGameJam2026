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

        isActivated = true;

        if(door != null)
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
