using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoidRegister : MonoBehaviour
{
    public float centripetalForce = 1.0f;
    public float centrifugalForce = 1.0f;
    public float velocityMatching = 1.0f;
    public float minDistance = 5.0f;
    public float maxSpeed = 3.0f;
    public float rotationSpeed = 2.0f;
    public float randomRange = 1.0f;
    public int maxChlorine = 1;
    public float maxPower = 100.0f;
    public float powerDecayRate = .01f;
    public float chlorineRange = 5;
    public bool proximitySensors = false;
    public int skipEvery = 1;
    public Text elapsedTime;
    public Transform home;
    public ChunkSpawner chunkSpawner;
    public AlgaeRegister algaeCluster;
    public ChlorineRegister chlorineClouds;

    public List<BoidController> boids;
    public int offset = 0;

    void Start()
    {

    }

    public void FixedUpdate()
    {
        offset += 1;
        elapsedTime.text = "Elapsed Steps " + offset;
    }

    public List<BoidController> getBoids()
    {
        return boids;
    }

    public void unregister(BoidController boid)
    {
        boids.Remove(boid);
    }

    public void register(BoidController boid)
    {
        boids.Add(boid);
    }

    public void purge()
    {
        BoidController[] b = boids.ToArray();
        for (int i = 0; i < b.Length; i++)
            Destroy(b[i].gameObject);        
        boids = new List<BoidController>();
    }

}
