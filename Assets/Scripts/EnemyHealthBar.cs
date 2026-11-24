using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    public Image fillImage;          
    public Transform enemyTransform; 

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public bool faceCamera = true;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // Always face the camera (so it's readable)
        if (faceCamera && mainCam != null)
            transform.rotation = mainCam.transform.rotation;
    }

    public void SetHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}