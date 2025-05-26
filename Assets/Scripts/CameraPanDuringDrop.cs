using UnityEngine;

public class CameraPanDuringDrop : MonoBehaviour
{
    [Header("Pan Settings")]
    public Vector3 moveDirection = new Vector3(0, 0, -1); // direção do movimento (diagonal sobre o tabuleiro)
    public float moveDistance = 2f;                      // distância total
    public float moveDuration = 3f;                      // tempo total de animação
    public bool startMoving = false;

    private Vector3 initialPosition;
 
    [Header("Handheld Motion")]
    public float positionAmplitude = 0.05f;
    public float positionFrequency = 0.5f;
    public float rotationAmplitude = 0.2f;
    public float rotationFrequency = 0.5f;

    [Header("Timing")]
    public float delayBeforePan = 0.5f;

    private Vector3 panStartPos;
    private Quaternion panStartRot;
    private Vector3 panTargetPos;
    private bool isPanning = false;
    private float elapsed = 0f;

    private Vector3 handheldOriginPos;
    private Quaternion handheldOriginRot;
    private bool handheldActive = false;

    void Start()
    {
        panStartPos = transform.localPosition;
        panStartRot = transform.localRotation;
        panTargetPos = panStartPos + moveDirection.normalized * moveDistance;

        // Start panning with delay
        Invoke(nameof(StartPan), delayBeforePan);
    }

    void StartPan()
    {
        isPanning = true;
    }

    void Update()
    {
        if (isPanning)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            t = Mathf.SmoothStep(0, 1, t);
            transform.localPosition = Vector3.Lerp(panStartPos, panTargetPos, t);

            if (t >= 1f)
            {
                isPanning = false;
                handheldOriginPos = transform.localPosition;
                handheldOriginRot = transform.localRotation;
                handheldActive = true;
            }
        }
        else if (handheldActive)
        {
            ApplyHandheldMotion();
        }
    }

    void ApplyHandheldMotion()
    {
        float time = Time.time;

        float posOffsetX = Mathf.PerlinNoise(time * positionFrequency, 0.0f) - 0.5f;
        float posOffsetY = Mathf.PerlinNoise(0.0f, time * positionFrequency) - 0.5f;
        float posOffsetZ = Mathf.PerlinNoise(time * positionFrequency, time * 0.5f) - 0.5f;

        float rotOffsetX = Mathf.PerlinNoise(time * rotationFrequency, 1.0f) - 0.5f;
        float rotOffsetY = Mathf.PerlinNoise(1.0f, time * rotationFrequency) - 0.5f;

        Vector3 positionOffset = new Vector3(posOffsetX, posOffsetY, posOffsetZ) * positionAmplitude;
        Vector3 rotationOffset = new Vector3(rotOffsetX, rotOffsetY, 0) * rotationAmplitude;

        transform.localPosition = handheldOriginPos + positionOffset;
        transform.localRotation = Quaternion.Euler(rotationOffset) * handheldOriginRot;
    }

    public void TriggerPan()
    {
        elapsed = 0f;
        initialPosition = transform.position;
        startMoving = true;
    }
}