using UnityEngine;

public class FpsLimit : MonoBehaviour
{
    [SerializeField, Range(0, 240)] private int _fps;
    private void Awake()
    {
        Application.targetFrameRate = _fps;
        QualitySettings.vSyncCount = 0;
    }

    private void OnValidate()
    {
        Application.targetFrameRate = _fps;
    }
}