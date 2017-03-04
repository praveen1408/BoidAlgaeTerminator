using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class AlgaeRegister : MonoBehaviour
{
    public List<AlgaeController> algaeList;
    public Text algaeCount;

    private CentralCoordinator central;
    private bool allClear = false;

    void Start()
    {
        central = transform.parent.gameObject.GetComponent<CentralCoordinator>();
    }

    public void Update()
    {
        if (algaeList.Count == 0)
        {
            algaeCount.text = "All clear " + Time.realtimeSinceStartup;
            string log = Time.realtimeSinceStartup+","+central.getTotalClouds()+"\r\n";
            Debug.Log(log);
            File.AppendAllText("log.txt", log);
            if (allClear)
                central.spawnEverything();
            allClear = true;
        }
        else
        {
            algaeCount.text = "Remaining algae " + algaeList.Count;
        }
    }

    public List<AlgaeController> getAllAlgae()
    {
        return algaeList;
    }

    public void unregister(AlgaeController algae)
    {
        algaeList.Remove(algae);
    }

    public void register(AlgaeController algae)
    {
        algaeList.Add(algae);
    }

    public void purge()
    {
        AlgaeController[] a = algaeList.ToArray();
        for (int i = 0; i < a.Length; i++)
            Destroy(a[i].gameObject);
        algaeList = new List<AlgaeController>();
        allClear = false;
    }

}
