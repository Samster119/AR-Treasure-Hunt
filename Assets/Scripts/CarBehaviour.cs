using System.Collections;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip failSound;

    public GameObject CrossHair;
    public float Speed = 1.2f;
    public int shipHealth = 3;
    public float CollectRadius = 0.4f;
    private AudioSource audioSource;
    private UIHud HUD;
    private UIScoreboard scoreBoard;
    private float lastHitTime = -10f;
    private const float HitCooldown = 1.5f;

    void Start()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        audioSource = GetComponent<AudioSource>();
        HUD = FindObjectOfType<UIHud>();
        scoreBoard = FindObjectOfType<UIScoreboard>();
        HUD.UpdateHP(shipHealth);
    }

    private void Update()
    {
        if (CrossHair == null) return;

        var trackingPosition = CrossHair.transform.position;
        if (Vector3.Distance(trackingPosition, transform.position) >= 0.2)
        {
            var lookRotation = Quaternion.LookRotation(trackingPosition - transform.position);
            lookRotation.x = 0f;
            lookRotation.z = 0f;
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            transform.position = Vector3.MoveTowards(transform.position, trackingPosition, Speed * Time.deltaTime);
        }

        // Distance-based hit detection: guaranteed to catch overlaps
        // even when physics triggers miss (thin colliders, fast frames).
        CheckProximityHits();
    }

    // Floor-based AR game: judge hits by horizontal (XZ) distance only,
    // so tracking drift / height differences can't make the ship slide through.
    private float HorizontalDistanceTo(Vector3 otherPos)
    {
        var a = transform.position;
        a.y = 0f;
        var b = otherPos;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void CheckProximityHits()
    {
        if (!HUD.isTimerRunning) return;

        foreach (var pkg in FindObjectsOfType<PackageBehaviour>())
        {
            if (pkg != null && HorizontalDistanceTo(pkg.transform.position) <= CollectRadius)
            {
                ApplyHit(pkg, null);
            }
        }

        foreach (var enemy in FindObjectsOfType<EnnemyBehaviour>())
        {
            if (enemy != null && HorizontalDistanceTo(enemy.transform.position) <= CollectRadius)
            {
                ApplyHit(null, enemy);
            }
        }
    }

    private void ApplyHit(PackageBehaviour Package, EnnemyBehaviour Ennemy)
    {
        if (!HUD.isTimerRunning) return;

        if (Package != null && !Package.IsBlocked())
        {
            Destroy(Package.gameObject);
            audioSource.PlayOneShot(successSound);
            HUD.IncreaseScore();
        }
        else if (Ennemy != null && Time.time - lastHitTime >= HitCooldown)
        {
            lastHitTime = Time.time;
            shipHealth--;
            Ennemy.TakeDamage(1);
            HUD.UpdateHP(shipHealth);

            if (shipHealth <= 0)
            {
                audioSource.PlayOneShot(failSound);
                HUD.HideHUD();
                scoreBoard.showGameOver();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyHit(other.GetComponent<PackageBehaviour>(), other.GetComponent<EnnemyBehaviour>());
    }
}
