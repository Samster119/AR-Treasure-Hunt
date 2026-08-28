using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PackageSpawner : MonoBehaviour
{
    public DrivingSurfaceManager DrivingSurfaceManager;
    public GameObject PackagePrefab;
    public GameObject EnnemyPrefab;

    private readonly List<PackageBehaviour> packages = new List<PackageBehaviour>();
    private readonly List<Transform> enemies = new List<Transform>();

    private const int MaxPackages = 3;
    private const float KrakenSpawnChance = 0.3f;
    private const float MinDistanceFromEnemy = 0.5f;
    [SerializeField] private float heightOffset = 0.15f;

    // Uniform random point inside the FULL triangle (all 3 vertices)
    public static Vector3 RandomInTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float u = Random.Range(0f, 1f);
        float v = Random.Range(0f, 1f);
        if (u + v > 1)
        {
            u = 1 - u;
            v = 1 - v;
        }
        return (v1 * u) + (v2 * v) + (v3 * (1 - u - v));
    }

    public Vector3 FindRandomLocation(ARPlane plane)
    {
        var meshComponent = plane.GetComponent<ARPlaneMeshVisualizer>();
        if (meshComponent == null || meshComponent.mesh == null)
        {
            return plane.transform.position;
        }

        var mesh = meshComponent.mesh;
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;

        if (triangles.Length < 3 || vertices.Length < 3)
        {
            return plane.transform.position;
        }

        int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;

        var randomInTriangle = RandomInTriangle(
            vertices[triangles[triangleIndex]],
            vertices[triangles[triangleIndex + 1]],
            vertices[triangles[triangleIndex + 2]]);

        var randomPoint = plane.transform.TransformPoint(randomInTriangle);
        // Y comes straight from the mesh triangle -> exactly on the VISIBLE surface,
        // then lifted slightly so the box never sinks into the floor.
        randomPoint.y += heightOffset;

        return randomPoint;
    }

    private Vector3 FindSafeLocation(ARPlane plane)
    {
        var location = FindRandomLocation(plane);
        for (int i = 0; i < 10; i++)
        {
            bool tooClose = false;
            foreach (var e in enemies)
            {
                if (e != null && Vector3.Distance(location, e.position) < MinDistanceFromEnemy)
                {
                    tooClose = true;
                    break;
                }
            }
            if (!tooClose) break;
            location = FindRandomLocation(plane);
        }
        return location;
    }

    // Kraken spot: another ON-MESH point near the chest
    private Vector3 FindEnemySpotNear(ARPlane plane, Vector3 chestPos)
    {
        var p = FindRandomLocation(plane);
        for (int i = 0; i < 12; i++)
        {
            p = FindRandomLocation(plane);
            float d = Vector3.Distance(p, chestPos);
            if (d >= 0.4f && d <= 2.0f)
            {
                return p;
            }
        }
        return p; // last sample is still guaranteed on-mesh
    }

    public void SpawnPackage(ARPlane plane)
    {
        var meshComponent = plane.GetComponent<ARPlaneMeshVisualizer>();
        if (meshComponent == null || meshComponent.mesh == null || meshComponent.mesh.vertexCount < 3)
        {
            return;
        }

        // Wait until the platform is big enough before spawning anything on it
        var size = meshComponent.mesh.bounds.size;
        if (Mathf.Max(size.x, size.z) < 1.0f)
        {
            return;
        }

        var randomLocation = FindSafeLocation(plane);

        var packageClone = GameObject.Instantiate(PackagePrefab);
        packageClone.transform.position = randomLocation;

        var package = packageClone.GetComponent<PackageBehaviour>();
        packages.Add(package);

        Vector3 position = package.gameObject.transform.position;
        position.y = 0f;
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.y = 0f;
        Vector3 direction = cameraPosition - position;
        Vector3 targetRotationEuler = Quaternion.LookRotation(forward: direction).eulerAngles;

        Vector3 scaledEuler = Vector3.Scale(a: targetRotationEuler, b: package.gameObject.transform.up.normalized);
        Quaternion targetRotation = Quaternion.Euler(euler: scaledEuler);
        package.gameObject.transform.rotation = package.gameObject.transform.rotation * targetRotation;

        if (Random.value <= KrakenSpawnChance)
        {
            Debug.Log("Release the Kraken!");
            var enemyClone = GameObject.Instantiate(EnnemyPrefab);
            var ennemy = enemyClone.GetComponent<EnnemyBehaviour>();
            ennemy.SetGuardedPackage(package);
            package.Block();

            enemyClone.transform.position = FindEnemySpotNear(plane, randomLocation);
            enemies.Add(enemyClone.transform);
        }
    }

    private void Update()
    {
        var lockedPlane = DrivingSurfaceManager.LockedPlane;
        if (lockedPlane == null)
        {
            return;
        }

        packages.RemoveAll(p => p == null);
        enemies.RemoveAll(e => e == null);

        if (packages.Count < MaxPackages)
        {
            SpawnPackage(lockedPlane);
        }
    }
}
