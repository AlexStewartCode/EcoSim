using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    [SerializeField] GameObject plantPrefab;
    [SerializeField] List<GameObject> plantModels;
    List<GameObject> plants;
    List<GameObject> plantQueue; 
    float xBound;
    float zBound;
    Terrain terrain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plants = new List<GameObject>();
        plantQueue = new List<GameObject>();
        terrain = Core.instance.ground;
        xBound = terrain.GetComponent<Terrain>().terrainData.size.x;
        zBound = terrain.GetComponent<Terrain>().terrainData.size.z;

        for (int i = 0; i < 100; i++)
        {
            GameObject newPlant = Instantiate(plantPrefab);
            newPlant.GetComponent<Plant>().SetVisual(plantModels[Random.Range(0, plantModels.Count - 1)]);
            newPlant.GetComponent<Plant>().SetValues(Random.Range(0.1f, 0.2f), Random.Range(1.5f, 5), Random.Range(0.25f, 0.5f));
            float x = Random.Range(0, xBound);
            float z = Random.Range(0, zBound);
            float y = terrain.SampleHeight(new Vector3(x, 0, z));
            Vector3 normal = terrain.terrainData.GetInterpolatedNormal(x / xBound, z / zBound);
            newPlant.transform.LookAt(normal);
            newPlant.transform.Rotate(new Vector3(90, 0, 0));
            Vector3 newPlantPos = new Vector3(x, y, z);
            newPlant.transform.position = newPlantPos;
            plants.Add(newPlant);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float reproAvg = 0; 
        for(int i = plants.Count - 1; i >= 0;i --)
        {
            GameObject currentPlant = plants[i];
            Plant plantI = currentPlant.GetComponent<Plant>();
            reproAvg += plantI.maxSize;
            if (!plantI.isAlive)
            {
                currentPlant.SetActive(false);
                plants.Remove(currentPlant);
                Destroy(currentPlant);
            }
            else
            {
                if(plantI.isReproducing)
                {
                    if(plants.Count < 1000)
                    {
                        // Gather data and alter slightly 
                        float childGrowthRate = plantI.growthRate + Random.Range(-0.1f, 0.1f);
                        float childMaxSize = plantI.maxSize + Random.Range(-0.1f, 0.1f);
                        float childReproductionChance = plantI.reproductionChance + Random.Range(-0.01f, 0.01f);

                        if (childReproductionChance > 1)
                        {
                            childReproductionChance = 1;
                        }

                        GameObject childVisualPrefab = plantI.visualPrefab;
                        GameObject child = Instantiate(plantPrefab);
                        child.GetComponent<Plant>().SetVisual(childVisualPrefab);
                        child.GetComponent<Plant>().SetValues(childGrowthRate, childMaxSize, childReproductionChance);
                        float x = Random.Range(0, xBound);
                        float z = Random.Range(0, zBound);
                        float y = terrain.SampleHeight(new Vector3(x, 0, z));
                        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(x / xBound, z / zBound);
                        child.transform.LookAt(normal);
                        child.transform.Rotate(new Vector3(90, 0, 0));
                        Vector3 newPlantPos = new Vector3(x, y, z);
                        child.transform.position = newPlantPos;
                        plantQueue.Add(child);
                    }
                    plantI.isReproducing = false;
                }
            }
        }
        Debug.Log($"Max Size Avg  = {reproAvg / plants.Count}");
        if(plantQueue.Count > 0)
        {
            plants.AddRange(plantQueue);
            plantQueue.Clear();
        }
    }

    public void Step(float dTime)
    {
        foreach(GameObject plant in plants)
        {
            plant.GetComponent<Plant>().Step(dTime);
        }
    }
}
