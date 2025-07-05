using UnityEngine;

public class BeeLifecycle : MonoBehaviour
{
    public Sprite eggSprite;
    public Sprite larvaSprite;
    public Sprite workerSprite;

    public float eggTime = 5f;      // Seconds as egg
    public float larvaTime = 5f;    // Seconds as larva
    public float moveSpeed = 2f;
    public float wanderRadius = 2f;

    private SpriteRenderer sr;
    private Vector3 origin;
    private Vector3 targetPosition;
    private bool isWorker = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = eggSprite;
        origin = transform.position;

        StartCoroutine(GrowUp());
    }

    System.Collections.IEnumerator GrowUp()
    {
        yield return new WaitForSeconds(eggTime);
        sr.sprite = larvaSprite;

        yield return new WaitForSeconds(larvaTime);
        sr.sprite = workerSprite;
        isWorker = true;

        PickNewTarget();
    }

    void Update()
    {
        if (!isWorker) return;

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = origin + new Vector3(randomOffset.x, randomOffset.y, 0);
    }
}
