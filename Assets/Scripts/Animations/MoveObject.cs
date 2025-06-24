using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float duration = 15f;

    private float timer;

    void Start()
    {
        if (pointA != null)
        {
            transform.position = pointA.position;
        }
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        timer += Time.deltaTime;

        float t = timer / duration;
        t = Mathf.Clamp01(t);

        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);

        if (timer >= duration)
        {
            timer = 0f;
            transform.position = pointA.position;
        }
    }
}
