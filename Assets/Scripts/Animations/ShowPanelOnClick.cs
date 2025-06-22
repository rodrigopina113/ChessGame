using UnityEngine;

public class ShowPanelOnClick : MonoBehaviour
{
    [SerializeField]
    private GameObject panel; // drag your panel here in the Inspector

    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf); // invert the current state
    }
}
