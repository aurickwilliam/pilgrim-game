using UnityEngine;
using UnityEngine.UI;

public class ControlsToggle : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;
    private Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Toggle);

        // Start hidden
        controlsPanel.SetActive(false);
    }

    void Toggle()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
    }
}