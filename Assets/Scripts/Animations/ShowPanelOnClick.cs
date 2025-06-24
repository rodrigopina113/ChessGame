using UnityEngine;
using UnityEngine.UI;

public class ShowPanelOnClick : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private Button[] buttonsToDisable;

    private bool isPausedByThisPanel = false;

    public void TogglePanel()
    {
        bool newState = !panel.activeSelf;
        panel.SetActive(newState);

        foreach (Button btn in buttonsToDisable)
            btn.interactable = !newState;

        if (newState)
        {
            Time.timeScale = 0f;
            isPausedByThisPanel = true;
        }
        else if (isPausedByThisPanel)
        {
            Time.timeScale = 1f;
            isPausedByThisPanel = false;
        }
    }
}
