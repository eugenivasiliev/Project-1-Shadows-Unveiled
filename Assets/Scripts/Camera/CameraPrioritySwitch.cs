using Cinemachine;
using UnityEngine;

public class CameraPrioritySwitch : MonoBehaviour
{
    public CinemachineVirtualCamera origin;
    public CinemachineVirtualCamera final;

    private void OnTriggerEnter2D(Collider2D other)
    {
        final.Priority = 1;
        origin.Priority = 0;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        origin.Priority = 1;
        final.Priority = 0;
    }
}
