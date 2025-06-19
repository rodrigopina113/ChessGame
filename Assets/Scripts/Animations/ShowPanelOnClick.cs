using UnityEngine;

public class ShowPanelOnClick : MonoBehaviour
{
    public GameObject panelToShow;

    // This method can be linked to the button's OnClick event in the inspector
    public void TogglePanel()
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(!panelToShow.activeSelf);
        }
    }
}
