using PlatformServices;
using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        PlatformUserStats.SetAchievement("cameraControlled");
    }

    private void LateUpdate()
    {
        transform.position = PlayerController.Instance.transform.position + offset;
    }
}