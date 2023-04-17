using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlatformServices;

public class SelectSkin : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Transform skinPrefab;
    private void OnMouseDown()
    {
        playerData.SelectedSkin = skinPrefab;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PlatformUserStats.SetAchievement($"skinSelected_{skinPrefab.name}");
    }
}
