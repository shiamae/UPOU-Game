using UnityEngine;

public class CapybaraSimpleWander : MonoBehaviour
{
    [Header("Area")]
    public BoxCollider area; // Movement boundary

    [Header("Settings")]
    public float walkSpeed = 0.5f;
    public Vector2 actionDelayRange = new Vector2(2f, 5f);

    [Header("Animation Probabilities (Sum to 1)")]
    [Range(0f, 1f)] public float idleProbability = 0.3f;
    [Range(0f, 1f)] public float walkProbability = 0.5f;
    [Range(0f, 1f)] public float sitProbability = 0.1f;
    [Range(0f, 1f)] public float standProbability = 0.1f;

    private Animator animator;
    private Vector3 targetPosition;
    private float timer;
    private float actionDelay;

    private enum State { Idle, Sit, Stand, Walk }
    private State currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        NormalizeProbabilities();
        SetNextActionTime();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= actionDelay)
        {
            ChooseActionByProbability();
            SetNextActionTime();
        }

        if (currentState == State.Walk)
        {
            WalkTowardTarget();
        }
    }

    void SetNextActionTime()
    {
        timer = 0f;
        actionDelay = Random.Range(actionDelayRange.x, actionDelayRange.y);
    }

    void ChooseActionByProbability()
    {
        float r = Random.value;

        animator.SetBool("idle", false);
        animator.SetBool("sit", false);
        animator.SetBool("stand", false);
        animator.SetBool("walk", false);

        switch (currentState) // Stop motion from previous state
        {
            case State.Walk:
                targetPosition = transform.position;
                break;
        }

        if (r < idleProbability)
        {
            animator.SetBool("idle", true);
            currentState = State.Idle;
        }
        else if (r < idleProbability + walkProbability)
        {
            animator.SetBool("walk", true);
            currentState = State.Walk;
            PickRandomPointInArea();
        }
        else if (r < idleProbability + walkProbability + sitProbability)
        {
            animator.SetBool("sit", true);
            currentState = State.Sit;
        }
        else
        {
            animator.SetBool("stand", true);
            currentState = State.Stand;
        }
    }


    void WalkTowardTarget()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("walk", false);
            currentState = State.Idle;
            targetPosition = transform.position; // Clear target to prevent sliding
        }
    }


    void PickRandomPointInArea()
    {
        if (area == null) return;

        Vector3 center = area.bounds.center;
        Vector3 size = area.bounds.size;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

        targetPosition = new Vector3(x, transform.position.y, z);
        transform.LookAt(targetPosition);
    }

    void NormalizeProbabilities()
    {
        float total = idleProbability + walkProbability + sitProbability + standProbability;

        if (Mathf.Abs(total - 1f) > 0.01f)
        {
            idleProbability /= total;
            walkProbability /= total;
            sitProbability /= total;
            standProbability /= total;
            Debug.LogWarning("Capybara probabilities normalized to sum to 1.");
        }
    }
}
