using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChlorineRegister : MonoBehaviour
{
    public float startScale = 1;
    public float endScale = 10;
    public float baseKillingPower = 10;
    public float lifeTime = 10;
    public float updateDelay = .2f;
    public Text cloudCount;

    public List<ChlorineController> clouds;

    public int totalDeployed = 0;

    public void Update()
    {
        cloudCount.text = "Total clouds " + totalDeployed;
    }

    public List<ChlorineController> getClouds()
    {
        return clouds;
    }

    public void unregister(ChlorineController cloud)
    {
        clouds.Remove(cloud);
    }

    public void register(ChlorineController cloud)
    {
        totalDeployed++;
        clouds.Add(cloud);
    }

    public void purge()
    {
        totalDeployed = 0;
        ChlorineController[] c = clouds.ToArray();
        for (int i = 0; i < c.Length; i++)
            Destroy(c[i].gameObject);
        clouds = new List<ChlorineController>();
    }
}
