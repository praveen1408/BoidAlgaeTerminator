using UnityEngine;
using System.Collections;

public class ChunkSpawner : MonoBehaviour
{
    public float edgeLength = 100;
    public int divisions = 10;
    public float triggerDistance = -1;
    public GameObject prefab;
    public Material exploredMaterial;

    private bool[] isChunkExplored;
    private GameObject[] cubes;
    private float separation;
    private bool allExplored;
    private bool firstTime = true;

    public void spawnChunks()
    {
        if (!firstTime)
            destroyAllCubes();
        firstTime = false;
        separation = edgeLength / divisions;
        triggerDistance = separation;
        isChunkExplored = new bool[divisions * divisions * divisions];
        cubes = new GameObject[divisions * divisions * divisions];

        for (int i = 0; i < divisions; i++)
        {
            for (int j = 0; j < divisions; j++)
            {
                for (int k = 0; k < divisions; k++)
                {
                    spawnCubeAt(new Vector3(i * separation, j * separation, k * separation));
                }
            }
        }
        allExplored = false;
    }

    public Vector3 getClosestChunk(Vector3 position)
    {
        Vector3 target = new Vector3(-1, 0, 0);
        float minDistance = float.PositiveInfinity;
        if (allExplored) return target;
        allExplored = true;
        for (int i = 0; i < divisions * divisions * divisions; i++)
        {
            allExplored &= isChunkExplored[i];
            if (isChunkExplored[i]) continue;
            float distance = Vector3.Distance(cubes[i].transform.position, position);
            if (distance < minDistance)
            {
                target = cubes[i].transform.position;
                minDistance = distance;
            }
        }
        //Debug.Log("Target: "+target);
        return target;
    }

    public void setChunkExplored(Vector3 chunk)
    {
        int index = getIndexFromVector3(chunk);
        if (isChunkExplored[index]) return;
        isChunkExplored[index] = true;

        if (cubes[index] == null)
            spawnCubeAt(chunk);

        //Debug.Log("Explored " + chunk);
        cubes[index].GetComponent<Renderer>().material = exploredMaterial;
    }

    public bool isExplored(Vector3 chunk)
    {
        return isChunkExplored[getIndexFromVector3(chunk)];
    }

    private void spawnCubeAt(Vector3 target)
    {
        GameObject obj = (GameObject)Instantiate(prefab, target, Quaternion.identity);
        cubes[getIndexFromVector3(target)] = obj;
        obj.transform.localScale = new Vector3(separation, separation, separation);
        obj.transform.parent = transform;
        obj.name = "Chunk " + target;
    }

    private int getIndexFromVector3(Vector3 vec)
    {
        int x = (int)Mathf.Clamp(Mathf.Floor(vec.x / separation), 0, divisions);
        int y = (int)Mathf.Clamp(Mathf.Floor(vec.y / separation), 0, divisions);
        int z = (int)Mathf.Clamp(Mathf.Floor(vec.z / separation), 0, divisions);
        return x * divisions * divisions + y * divisions + z;
    }

    private void destroyAllCubes()
    {
        for (int i = 0; i < divisions * divisions * divisions; i++)
        {
            Destroy(cubes[i]);
            isChunkExplored[i] = false;
        }
    }

}
