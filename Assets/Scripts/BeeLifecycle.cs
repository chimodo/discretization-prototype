using System.Linq;
using UnityEngine;

public class BeeLifecycle : MonoBehaviour
{
    public Sprite eggSprite;
    public Sprite larvaSprite;
    public Sprite workerSprite;

    [Header("Timings")]
    public float eggTime = 5f;
    public float larvaTime = 5f;
    public float workerLifespan = 30f;
    public float warningTime = 3f; // Time before death to turn gray

    [Header("Bee Behavior")]
    public float moveSpeed = 2f;
    public float wanderRadius = 2f;
    [Range(0f, 1f)]
    public float honeyProductionChance = 0.3f;

    private SpriteRenderer sr;
    private Vector3 origin;
    private Vector3 targetPosition;
    private bool isWorker = false;

    private bool makingHoney = false;
    private Vector3Int honeyTargetCell;

    private bool isDying = false;

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

        // Start lifespan countdown and warning
        Invoke(nameof(TriggerWarning), workerLifespan - warningTime);
        Invoke(nameof(Die), workerLifespan);

        PickNewTarget();
    }

    void Update()
    {
        if (!isWorker || isDying) return;

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (makingHoney)
            {
                HiveManager.Instance.cellStates[honeyTargetCell] = CellState.Honey;
                HiveManager.Instance.UpdateTileVisual(honeyTargetCell);
                makingHoney = false;
            }

            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        if (!makingHoney && Random.value < honeyProductionChance)
        {
            Vector3Int? emptyCell = FindEmptyCell();
            if (emptyCell.HasValue)
            {
                makingHoney = true;
                honeyTargetCell = emptyCell.Value;
                targetPosition = HiveManager.Instance.hiveTilemap.CellToWorld(honeyTargetCell) + new Vector3(0.5f, 0.5f, 0f);
                return;
            }
        }

        makingHoney = false;
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = origin + new Vector3(randomOffset.x, randomOffset.y, 0);
    }

    Vector3Int? FindEmptyCell()
    {
        var emptyCells = HiveManager.Instance.cellStates
            .Where(kvp => kvp.Value == CellState.Empty)
            .Select(kvp => kvp.Key)
            .ToList();

        if (emptyCells.Count == 0)
            return null;

        int randomIndex = Random.Range(0, emptyCells.Count);
        return emptyCells[randomIndex];
    }

    void TriggerWarning()
    {
        sr.color = Color.gray;
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;

        // Stop logic and start falling
        isWorker = false;
        sr.color = Color.gray;

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0.5f;
        rb.angularVelocity = Random.Range(-180f, 180f); // spin like Insaniquarium

        // Destroy after falling off screen
        Destroy(gameObject, 4f);
    }
}
