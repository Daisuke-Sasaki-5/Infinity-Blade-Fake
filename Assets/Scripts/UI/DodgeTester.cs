using UnityEngine;

public class DodgeTester : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private MobileInputVisualizer input;

    public void DodgeLeft()
    {
        player.DodgeLeft();
        input.SetState("Dodge Left");
        Debug.Log("left");
    }

    public void DodgeRight()
    {
        player.DodgeRight();
        input.SetState("Dodge Right");
        Debug.Log("right");
    }
}
