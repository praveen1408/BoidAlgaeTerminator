using UnityEngine;
using System.Collections.Generic;

public class BoidController : MonoBehaviour
{
    public int chlorine;
    public float power;
    public GameObject chlorinePrefab;
    public bool headingHome = false;
    public int id = 0;

    public Vector3 boidVelocity;
    public Vector3 explorationVelocity;
    public Vector3 algaeVelocity;
    public Vector3 chlorineVelocity;
    public Vector3 heading;

    private BoidRegister registry;
    private AlgaeRegister algaeCluster;
    private ChlorineRegister chlorineClouds;
    private Rigidbody rb;
    private HashSet<BoidController> boids;
    private HashSet<AlgaeController> cluster;
    private HashSet<ChlorineController> clouds;
    public Vector3 closestChunk;
    private bool isInExplorationMode = true;

    void Start()
    {
        registry = transform.parent.gameObject.GetComponent<BoidRegister>();
        registry.register(this);
        rb = GetComponent<Rigidbody>();
        chlorine = registry.maxChlorine;
        power = registry.maxPower;
        algaeCluster = registry.algaeCluster;
        chlorineClouds = registry.chlorineClouds;
        boids = new HashSet<BoidController>();
        cluster = new HashSet<AlgaeController>();
        clouds = new HashSet<ChlorineController>();
        closestChunk = Vector3.zero;
    }

    void FixedUpdate()
    {
        // Stagger calculation by some frames
        if (id % registry.skipEvery != registry.offset % registry.skipEvery) return;

        // Battery decays over time
        //power -= registry.powerDecayRate;   

        if (isLowOnPowerOrChlorine())
            headingHome = true;

        heading = Vector3.zero;
        if (headingHome)
            heading = (registry.home.position - transform.position).normalized;

        // Update velocity
        boidVelocity = getVelocityDueToBoids();
        heading += boidVelocity;
        if (isInExplorationMode)
        {
            explorationVelocity = getExplorationVelocity();
            heading += explorationVelocity;
        }
        else
        {
            algaeVelocity = getVelocityDueToAlgae();
            if (algaeVelocity.magnitude == 0)
            {
                isInExplorationMode = true;
            }
            else
            {
                heading += algaeVelocity;
            }
        }
        chlorineVelocity = getVelocityDueToChlorine();
        heading += chlorineVelocity;
        //+ getNoiseVelocity()        

        // Update rotation
        Quaternion target = Quaternion.LookRotation(heading);
        rb.rotation = Quaternion.Lerp(rb.rotation, target, registry.rotationSpeed);

        rb.velocity += transform.forward;//heading.normalized * registry.skipEvery;

        // Clamp velocity
        if (rb.velocity.magnitude > registry.maxSpeed)
            rb.velocity = rb.velocity.normalized * registry.maxSpeed;
    }

    private Vector3 getExplorationVelocity()
    {
        if (closestChunk == Vector3.zero || registry.chunkSpawner.isExplored(closestChunk))
            closestChunk = registry.chunkSpawner.getClosestChunk(transform.position);

        if (closestChunk.x == -1)
        {
            headingHome = true; // Head home if error
        }

        if (headingHome) return Vector3.zero;

        Vector3 direction = (closestChunk - transform.position);
        if (direction.magnitude < registry.chunkSpawner.triggerDistance)
        {
            registry.chunkSpawner.setChunkExplored(closestChunk);
            closestChunk = registry.chunkSpawner.getClosestChunk(transform.position);
            isInExplorationMode = false;
        }

        return direction.normalized;
    }

    private Vector3 getVelocityDueToBoids()
    {
        Vector3 v = Vector3.zero;
        int n = boids.Count;
        if (n == 1) return Vector3.zero;

        // Moment towards center of the flock
        Vector3 center = Vector3.zero;
        foreach (BoidController boid in boids)
        {
            if (boid.Equals(this)) continue;
            if (boid.headingHome) continue;
            center += boid.transform.position;
        }
        center.x /= n - 1; center.y /= n - 1; center.z /= n - 1;
        Vector3 delta = center - transform.position;
        v += delta * registry.centripetalForce;

        // Moment away from each other
        Vector3 separation = Vector3.zero;
        foreach (BoidController boid in boids)
        {
            if (boid.Equals(this)) continue;
            Vector3 line = transform.position - boid.transform.position;
            float distance = line.magnitude;
            if (distance < registry.minDistance)
            {
                separation -= line * Mathf.Exp(-(distance * distance));
            }
        }
        v -= separation * registry.centrifugalForce;

        // Match velocity with the flock
        Vector3 flockVelocity = Vector3.zero;
        foreach (BoidController boid in boids)
        {
            if (boid.Equals(this)) continue;
            if (boid.headingHome) continue;
            Vector3 line = transform.position - boid.transform.position;
            float distance = line.magnitude;
            Rigidbody other = boid.gameObject.GetComponent<Rigidbody>();
            flockVelocity += other.velocity * Mathf.Exp(-(distance * distance));
        }
        flockVelocity.x /= n - 1; flockVelocity.y /= n - 1; flockVelocity.z /= n - 1;
        v += Vector3.Lerp(rb.velocity, flockVelocity, 0.5f) * registry.velocityMatching;
        //Debug.Log("Flock: "+flockVelocity);		

        return v.normalized;
    }

    private Vector3 getVelocityDueToAlgae()
    {
        if (headingHome) return Vector3.zero;

        cluster.RemoveWhere(x => x == null);
        if (cluster.Count == 0) return Vector3.zero;

        AlgaeController nearest = null;
        float minDistance = float.PositiveInfinity;
        foreach (AlgaeController algae in cluster)
        {
            float mag = (algae.transform.position - transform.position).magnitude;
            if (mag < minDistance)
            {
                minDistance = mag;
                nearest = algae;
            }
        }
        Vector3 delta = Vector3.zero;
        delta = nearest.transform.position - transform.position;

        return delta.normalized;
    }

    private Vector3 getVelocityDueToChlorine()
    {
        if (headingHome) return Vector3.zero;
        clouds.RemoveWhere(x => x == null);
        int n = clouds.Count;
        if (n == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (ChlorineController cloud in clouds)
        {
            float distance = Vector3.Distance(cloud.transform.position, transform.position);
            center -= cloud.transform.position * Mathf.Exp(-(distance * distance));
        }
        center.x /= n; center.y /= n; center.z /= n;
        Vector3 delta = transform.position - center;

        return delta.normalized;
    }

    private Vector3 getNoiseVelocity()
    {
        float x = Random.Range(-registry.randomRange, registry.randomRange);
        float y = Random.Range(-registry.randomRange, registry.randomRange);
        float z = Random.Range(-registry.randomRange, registry.randomRange);
        return new Vector3(x, y, z);
    }

    void OnTriggerStay(Collider other)
    {
        Vector3 toTarget = (other.transform.position - transform.position).normalized;
        if (other.CompareTag("TAG_BOID"))
        {
            // 180 degrees field of view
            if (Vector3.Dot(toTarget, transform.forward) > 0 || registry.proximitySensors)
                boids.Add(other.gameObject.GetComponent<BoidController>());
            else
                boids.Remove(other.gameObject.GetComponent<BoidController>());
        }
        else if (other.CompareTag("TAG_ALGAE"))
        {
            if (Vector3.Dot(toTarget, transform.forward) > 0 || registry.proximitySensors)
                cluster.Add(other.gameObject.GetComponent<AlgaeController>());

            if (Vector3.Distance(transform.position, other.transform.position) < registry.chlorineRange)
                if (!isChlorineAlreadyDeployed())
                    deployChlorine();
        }
        else if (other.CompareTag("TAG_CHLORINE"))
        {
            if (Vector3.Dot(toTarget, transform.forward) > 0 || registry.proximitySensors)
                clouds.Add(other.gameObject.GetComponent<ChlorineController>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TAG_BOID"))
        {
            boids.Remove(other.gameObject.GetComponent<BoidController>());
        }
    }

    private bool isChlorineAlreadyDeployed()
    {
        foreach (ChlorineController cloud in clouds)
        {
            if (cloud == null) continue;
            if (Vector3.Distance(transform.position, cloud.transform.position) < registry.chlorineRange)
                return true;
        }
        return false;
    }

    private void deployChlorine()
    {
        if (chlorine > 0)
        {
            GameObject temp = Instantiate(chlorinePrefab, transform.position, Quaternion.identity) as GameObject;
            temp.transform.parent = chlorineClouds.transform;
            chlorine--;
        }
    }

    private bool isLowOnPowerOrChlorine()
    {
        return chlorine == 0;// || power <= registry.maxPower/2;
                             //return false;
    }
}
