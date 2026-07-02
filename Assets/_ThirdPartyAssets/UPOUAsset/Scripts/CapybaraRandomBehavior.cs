using UnityEngine;

public class CapybaraRandomBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 0.2f;
    private Animator animator;
    private float timer;
    private float timeBetweenActions;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetNextActionTime();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenActions)
        {
            PerformRandomAction();
            SetNextActionTime();
        }

        if (animator.GetBool("walk"))
        {
            MoveForward();
        }

    }

    void SetNextActionTime()
    {
        timer = 0f;
        timeBetweenActions = Random.Range(2f, 5f); // choose random delay between actions
    }

    void PerformRandomAction()
    {
        int action = Random.Range(0, 4); // 0 = idle, 1 = sit, 2 = stand, 3 = walk

        animator.SetBool("idle", false);
        animator.SetBool("sit", false);
        animator.SetBool("stand", false);
        animator.SetBool("walk", false);

        switch (action)
        {
            case 0:
                animator.SetBool("idle", true);
                break;
            case 1:
                animator.SetBool("sit", true);
                break;
            case 2:
                animator.SetBool("stand", true);
                break;
            case 3:
                animator.SetBool("walk", true);
                break;
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

}
