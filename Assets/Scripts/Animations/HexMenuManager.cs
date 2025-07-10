using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexMenuManager : MonoBehaviour
{
    [Header("Piece Prefabs (Order: Campanha, 1v1, Inventário, Créditos, Definições, Sair)")]
    public GameObject[] piecePrefabs = new GameObject[6];
    [Header("Unselected Piece Prefabs (Order: Campanha, 1v1, Inventário, Créditos, Definições, Sair)")]
    public GameObject[] unselectedPiecePrefabs = new GameObject[6];
    [Header("Piece Labels")]
    public string[] pieceLabels = new string[6] { "Campanha", "1v1", "Inventário", "Créditos", "Definições", "Sair" };
    [Header("Label Display (TextMeshPro)")]
    public TextMeshProUGUI labelDisplay;
    [Header("Radius of Hexagon")]
    public float radius = 0.2f;
    [Header("Rotation Speed (deg/sec)")]
    public float rotationSpeed = 300f;
    [Header("Piece Scales")]
    public Vector3 selectedScale = Vector3.one * 1.2f;
    public Vector3 unselectedScale = Vector3.one;
    [Header("Menu Actions")]
    public VerifyInternet verifyInternet;
    public SceneChanger sceneChanger;
    public GameObject settingsPanel;
    public ExitButton exitButton;
    [Header("Objects to Hide When Settings Open")]
    public GameObject mainCanvasToHide;
    public GameObject objectToHide;

    private List<GameObject> pieceParents = new List<GameObject>();
    private List<GameObject> selectedChildren = new List<GameObject>();
    private List<GameObject> unselectedChildren = new List<GameObject>();
    private int frontIndex = 0;
    private bool isRotating = false;
    private float targetAngle = 0f;
    private float currentAngle = 0f;

    void Start()
    {
        SpawnPieces();
        UpdateLabel();
    }

    void SpawnPieces()
    {
        float angleStep = 360f / piecePrefabs.Length;
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            Vector3 pos = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * radius;
            // Create parent
            GameObject parent = new GameObject($"MenuPieceParent_{pieceLabels[i]}");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = pos;
            parent.transform.localRotation = Quaternion.identity;
            // Instantiate selected child
            GameObject selected = Instantiate(piecePrefabs[i], parent.transform);
            selected.name = "SelectedVisual";
            // Instantiate unselected child
            GameObject unselected = Instantiate(unselectedPiecePrefabs[i], parent.transform);
            unselected.name = "UnselectedVisual";
            // Set active state
            selected.SetActive(i == frontIndex);
            unselected.SetActive(i != frontIndex);
            // Track
            pieceParents.Add(parent);
            selectedChildren.Add(selected);
            unselectedChildren.Add(unselected);
        }
        transform.rotation = Quaternion.identity;
        currentAngle = 0f;
        targetAngle = 0f;
    }

    void Update()
    {
        if (isRotating)
        {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
            if (Mathf.Approximately(currentAngle, targetAngle))
            {
                isRotating = false;
                UpdateLabel();
            }
        }
        else
        {
            // Raycast for mouse click
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    // Find which menu slot this child belongs to
                    Transform t = hit.collider.transform;
                    int idx = -1;
                    for (int i = 0; i < pieceParents.Count; i++)
                    {
                        if (t.IsChildOf(pieceParents[i].transform))
                        {
                            idx = i;
                            break;
                        }
                    }
                    if (idx != -1)
                    {
                        if (idx == frontIndex)
                        {
                            SelectFrontPiece();
                        }
                        else
                        {
                            RotateToPiece(idx);
                        }
                    }
                }
            }
            // Keyboard input (left/right)
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                RotateToPiece((frontIndex + 5) % 6);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                RotateToPiece((frontIndex + 1) % 6);
            if (Input.GetKeyDown(KeyCode.Return))
                SelectFrontPiece();
        }
    }

    void RotateToPiece(int idx)
    {
        int diff = (idx - frontIndex + 6) % 6;
        float angleStep = 360f / 6f;
        targetAngle += -angleStep * diff;
        frontIndex = idx;
        isRotating = true;
    }

    void UpdateLabel()
    {
        if (labelDisplay)
            labelDisplay.text = pieceLabels[frontIndex];
        // Update active states and scales
        for (int i = 0; i < pieceParents.Count; i++)
        {
            bool isSelected = (i == frontIndex);
            selectedChildren[i].SetActive(isSelected);
            unselectedChildren[i].SetActive(!isSelected);
            selectedChildren[i].transform.localScale = selectedScale;
            unselectedChildren[i].transform.localScale = unselectedScale;
        }
    }

    void SelectFrontPiece()
    {
        switch (frontIndex)
        {
            case 0: // Campanha
                if (verifyInternet != null)
                    verifyInternet.OnCheckButtonClicked();
                else
                    Debug.LogWarning("VerifyInternet reference not set in HexMenuManager");
                break;
            case 1: // 1v1
                if (sceneChanger != null)
                    sceneChanger.LoadScene("LocalMultiplayerMenu");
                else
                    Debug.LogWarning("SceneChanger reference not set in HexMenuManager");
                break;
            case 2: // Inventário
                if (sceneChanger != null)
                    sceneChanger.LoadScene("SkinSelector");
                else
                    Debug.LogWarning("SceneChanger reference not set in HexMenuManager");
                break;
            case 3: // Créditos
                if (sceneChanger != null)
                    sceneChanger.LoadScene("Creditos");
                else
                    Debug.LogWarning("SceneChanger reference not set in HexMenuManager");
                break;
            case 4: // Definições
                if (settingsPanel != null)
                    settingsPanel.SetActive(true);
                if (mainCanvasToHide != null)
                    mainCanvasToHide.SetActive(false);
                if (objectToHide != null)
                    objectToHide.SetActive(false);
                break;
            case 5: // Sair
                if (exitButton != null)
                    exitButton.ExitGame();
                else
                    Debug.LogWarning("ExitButton reference not set in HexMenuManager");
                break;
        }
    }

    public void OnSettingsClosed()
    {
        if (mainCanvasToHide != null)
            mainCanvasToHide.SetActive(true);
        if (objectToHide != null)
            objectToHide.SetActive(true);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
} 