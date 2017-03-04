using UnityEngine;
using System.Collections;

public class CentralCoordinator : MonoBehaviour
{
    public AlgaeSpawner algaeSpawner;
    public BoidSpawner boidSpawner;
    public ChunkSpawner chunkSpawner;
	public ChlorineRegister chlorineRegister;

    void Start()
    {
        spawnEverything();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
            spawnEverything();
    }

    public void spawnEverything()
    {
		chlorineRegister.purge();
        algaeSpawner.spawnAlgae();
        boidSpawner.spawnBoids();
        chunkSpawner.spawnChunks();
    }

    public int getTotalClouds(){
        return chlorineRegister.totalDeployed;
    }
}
