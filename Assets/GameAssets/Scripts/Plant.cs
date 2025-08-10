using UnityEditor;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float healthMax;
    public float healthCurrent;
    public float nutrientNeed;
    public float growthRate;
    public float nutrientsConsumed;
    public float nutrientsRequiredStep;
    public float size; 
    
    public float reproductionChance;
    public float reproductionDist;
    public bool isReproducing; 
    public float childXPos;
    public float childYPos; 

    public bool isAlive;

    public GameObject visualPrefab;
    public GameObject visual;

    public float availNutrients; 

    public void SetVisual(GameObject visualPrefab)
    {
        this.visualPrefab = visualPrefab;
    }

    public void SetValues(float healthMax, float nutrientNeed, float growthRate)
    {
        this.healthMax = healthMax;
        this.nutrientNeed = nutrientNeed;
        this.growthRate = growthRate;
        reproductionChance = 1;
        reproductionDist = Random.Range(1f, 500f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        visual = Instantiate(visualPrefab, transform);
        healthCurrent = healthMax;
        nutrientsConsumed = 0; 
        isAlive = true;
        isReproducing = false;
        size = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Step(float dTime)
    {
        Terrain terrain = Core.instance.ground;
        NutrientController nutrientCont = Core.instance.nutrientCont;
        float xBound = terrain.terrainData.size.x;
        float yBound = terrain.terrainData.size.z;
        float x = transform.position.x / xBound;
        float y = transform.position.z / yBound;

        //availNutrients = nutrientCont.GetAvailNutrients(x, y, size);

        nutrientsRequiredStep = Mathf.Pow(size, 2f) * nutrientNeed * dTime;

        float nutrientDeficit = nutrientCont.PlantNutrientUse(x, y, size, nutrientsRequiredStep);

        if (nutrientDeficit > 0)
        {
            healthCurrent -= nutrientDeficit;
            nutrientsConsumed += (nutrientsRequiredStep - nutrientDeficit);
        }
        else
        {
            float adjustedGrowth = (growthRate * 1.5f) / size; 
            size += adjustedGrowth * dTime;
            if(visual != null)
            {
                visual.transform.localScale = new Vector3(size, size, size);
            }
            nutrientsConsumed += nutrientsRequiredStep;
        }

        if (healthCurrent <= 0)
        {
            nutrientCont.AddNutrients(x, y, (nutrientsConsumed * 0.9f), size);
            isAlive = false;
        }

        float reproRoll = Random.Range(0.0f, 1f);
        if(reproRoll > 1 - (reproductionChance * dTime))
        {
            float direction;
            float xOffset;
            float yOffset;
            int counter = 0; 
            do
            {
                direction = Random.Range(0f, 360f);
                xOffset = (reproductionDist + Random.Range(-0.1f * reproductionDist, 0.1f * reproductionDist)) * Mathf.Cos(direction * Mathf.Deg2Rad);
                yOffset = (reproductionDist + Random.Range(-0.1f * reproductionDist, 0.1f * reproductionDist)) * Mathf.Sin(direction * Mathf.Deg2Rad);

                childXPos = transform.position.x + xOffset;
                childYPos = transform.position.y + yOffset;
                counter++; 
                if(counter >= 50)
                {
                    return;
                }
            } while (childXPos > xBound || childXPos < 0 || childYPos > yBound || childYPos < 0);
            isReproducing = true;
        }
    }

    public void childMade()
    {
        childXPos = 0;
        childYPos = 0;
        isReproducing = false;
    }

    private void OnDrawGizmos()
    {
        Vector3 camPos = SceneView.currentDrawingSceneView.camera.transform.position;
        if(Vector3.Distance(camPos, this.transform.position) > 15 * visual.transform.localScale.x)
        {
            return;
        }
        GUIStyle big = new GUIStyle();
        big.fontSize = 50;
        big.normal.textColor = Color.purple;
        Handles.Label(transform.position, $"Health: {healthCurrent}\nSize: {size}\nnRequired: {nutrientsRequiredStep}", big);
    }
}
