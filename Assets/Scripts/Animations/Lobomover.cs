using UnityEngine;

public class Lobomover : MonoBehaviour
{
    public float speed = 2f;
    public float delay = 8f;

    private float timer = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > delay)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }
}
