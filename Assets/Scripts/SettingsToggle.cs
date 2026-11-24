using UnityEngine;
using UnityEngine.UI;

public class SettingsToggle : MonoBehaviour
{
    public GameObject settingsPanel;      
    public Button settingsButton;          
    public GraphicRaycaster raycaster;     

    void Start()
    {
        settingsPanel.SetActive(false);    // hidden by default
    }

    public void ToggleSettings()
    {
        bool isOpen = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isOpen);

        // Pause when open, resume when closed
        Time.timeScale = isOpen ? 1f : 0f;
    }

    void Update()
    {
        // Close when clicking outside
        if (settingsPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            // Check if click was outside the panel
            RectTransform panelRect = settingsPanel.GetComponent<RectTransform>();
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition))
            {
                settingsPanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
}