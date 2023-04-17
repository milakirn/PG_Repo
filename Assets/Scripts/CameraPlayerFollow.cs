using PlatformServices;
using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        PlatformUserStats.SetAchievement("cameraControlled");
    }

    private void LateUpdate()
    {
        transform.position = PlayerController.Instance.transform.position + offset;
    }
}