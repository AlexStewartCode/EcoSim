using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    [SerializeField] GameObject plantPrefab;
    [SerializeField] List<GameObject> plantModels;
    List<GameObject> plants;
    float xBound;
    float zBound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plants = new List<GameObject>();
        Terrain terrain = Core.instance.ground;
        xBound = terrain.GetComponent<Terrain>().terrainData.size.x;
        zBound = terrain.GetComponent<Terrain>().terrainData.size.z;

        for (int i = 0; i < 1; i++)
        {
            GameObject newPlant = Instantiate(plantPrefab);
            newPlant.GetComponent<Plant>().SetVisual(plantModels[Random.Range(0, plantModels.Count - 1)]);
            newPlant.GetComponent<Plant>().SetValues(Random.Range(10f, 100f), Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.25f));
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
        for(int i = 0; i < plants.Count; i ++)
        {
            GameObject currentPlant = plants[i];
            Plant plantI = currentPlant.GetComponent<Plant>();
            if(!plantI.isAlive)
            {
                currentPlant.SetActive(false);
                plants.Remove(currentPlant);
                Destroy(currentPlant);
                i--;
            }
        }
    }
}
