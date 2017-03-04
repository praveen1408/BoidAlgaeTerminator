using UnityEngine;
using System.Collections;

public class BoidSpawner : MonoBehaviour
{
    public int x = 25, y = 3;
    public float separation = 3;
    public GameObject prefab;

    private BoidRegister register;

    public void spawnBoids()
    {
        register = GetComponent<BoidRegister>();
        register.purge();
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                Vector3 position = new Vector3(i * separation, 0, j * separation);
                GameObject boid = (GameObject)Instantiate(prefab, position, Quaternion.identity);
                int index = (i * y + j);
                boid.name = "Boid " + index;
                boid.GetComponent<BoidController>().id = index;
                boid.GetComponent<BoidController>().headingHome = false;
                boid.transform.parent = transform;
            }
        }
    }

}
