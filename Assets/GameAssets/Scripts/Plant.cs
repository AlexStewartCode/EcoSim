using UnityEngine;
using System.Collections.Generic;

public class Plant : MonoBehaviour
{
    private float healthMax;
    private float healthCurrent;
    private float nutrientNeed;
    private float growthRate;
    private float nutrientsConsumed;
    public bool isAlive;

    private GameObject visualPrefab;
    public GameObject visual;

    public void SetVisual(GameObject visualPrefab)
    {
        this.visualPrefab = visualPrefab;
    }

    public void SetValues(float healthMax, float nutrientNeed, float growthRate)
    {
        this.healthMax = healthMax;
        this.nutrientNeed = nutrientNeed;
        this.growthRate = growthRate;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        visual = Instantiate(visualPrefab, transform);
        healthCurrent = healthMax;
        nutrientsConsumed = 0; 
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        Terrain terrain = Core.instance.ground;
        NutrientController nutrientCont = Core.instance.nutrientCont; 

        float x = transform.position.x / terrain.terrainData.size.x;
        float y = transform.position.z / terrain.terrainData.size.z;

        float nutrientsAvailable = nutrientCont.GetNutrients(x, y);
        float nutrientsNeededFrame = (nutrientNeed * Time.deltaTime);
        if (nutrientsNeededFrame > nutrientsAvailable)
        {
            healthCurrent -= Time.deltaTime * 0.1f * healthMax;
        }
        else
        {
            visual.transform.localScale += visual.transform.localScale * (growthRate * Time.deltaTime);
            nutrientCont.UseNutrients(x, y, nutrientsNeededFrame);
            nutrientsConsumed += nutrientsNeededFrame;
        }

        if (healthCurrent <= 0)
        {
            nutrientCont.AddNutrients(x, y, (nutrientsConsumed * 0.9f));
            isAlive = false;
        }
    }
}
