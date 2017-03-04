using UnityEngine;
using System.Collections;

public class AlgaeSpawner : MonoBehaviour
{
    public float pondSize = 100;
    public int numberOfClusters = 10;
    public int averageAlgaePerCluster = 5;
    public float clusterSize = 5;
    private Vector3 offset;
    public GameObject prefab;

    private AlgaeRegister register;

    public void spawnAlgae()
    {        
        float pondRadius = pondSize / 2;
        register = GetComponent<AlgaeRegister>();
        register.purge();
        offset = new Vector3(pondRadius,pondRadius,pondRadius);
        pondRadius -= 10;
        for (int i = 0; i < numberOfClusters; i++)
        {
            Vector3 clusterPostion = Random.insideUnitSphere * pondRadius;
            for (int j = 0; j < averageAlgaePerCluster; j++)
            {
                Vector3 algaePosition = Random.insideUnitSphere * clusterSize;
                GameObject algae = (GameObject)Instantiate(prefab, clusterPostion + algaePosition + offset, Quaternion.identity);
                algae.transform.parent = transform;
            }
        }
    }
}
