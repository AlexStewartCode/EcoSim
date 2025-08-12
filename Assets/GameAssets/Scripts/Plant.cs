using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Plant : MonoBehaviour
{
    private float health; 
    public float growthRate;
    public float nutrientsConsumed;
    public float maxSize; 
    public float size; 
    
    public float reproductionChance;

    public bool isAlive;
    public bool isReproducing;

    public GameObject visualPrefab;
    public GameObject visual;

    public float availNutrients; 

    public void SetVisual(GameObject visualPrefab)
    {
        this.visualPrefab = visualPrefab;
    }

    public void SetValues(float growthRate, float maxSize, float reproductionChance)
    {
        health = 10; 
        this.growthRate = growthRate;
        this.maxSize = maxSize;
        this.reproductionChance = reproductionChance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        visual = Instantiate(visualPrefab, transform);
        nutrientsConsumed = 0; 
        isAlive = true;
        size = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Step(float dTime)
    {
        Terrain terrain = Core.instance.ground;
        float xPosition = transform.position.x / terrain.terrainData.size.x;
        float zPosition = transform.position.z / terrain.terrainData.size.z;

        NutrientController nutrientCont = Core.instance.nutrientCont;
        if (size >= maxSize)
        {
            TryReproduce(dTime, nutrientCont, xPosition, zPosition);
        }
        else
        {
            if(size > maxSize * 0.67f && Random.value > 0.5f)
            {
                TryReproduce(dTime, nutrientCont, xPosition, zPosition);
            }
            else
            {
                Grow(dTime, nutrientCont, xPosition, zPosition);
            }
        }

        if (health <= 0)
        {
            isAlive = false;
            nutrientCont.AddNutrients(xPosition, zPosition, nutrientsConsumed * 0.9f, size);
        }
    }

    private void TryReproduce(float dTime, NutrientController nutrientCont, float xPosition, float zPosition)
    {
        if(reproductionChance >= 1)
        {
            ReproduceSuccess(dTime, nutrientCont, xPosition, zPosition);
        }
        else
        {
            float stepChance = reproductionChance * dTime; 

            if(Random.value < stepChance)
            {
                ReproduceSuccess(dTime, nutrientCont, xPosition, zPosition);
            }
        }
    }

    private void ReproduceSuccess(float dTime, NutrientController nutrientCont, float xPosition, float zPosition)
    {
        float stepGrowthRate = maxSize - Mathf.Pow(size, 2);
        stepGrowthRate *= growthRate;
        stepGrowthRate *= dTime;

        float stepNutrientCost = (stepGrowthRate / 2) + 0.5f;
        float nutrientDeficit = nutrientCont.PlantNutrientUse(xPosition, zPosition, size, stepNutrientCost);
        
        // Take damage if there aren't enough nutrients to reproduce  
        if(nutrientDeficit > 0)
        {    
            health -= nutrientDeficit;
        }
        isReproducing = true;
    }

    private void Grow(float dTime, NutrientController nutrientCont, float xPosition, float zPosition)
    {
        float stepGrowthRate = maxSize - Mathf.Pow(size, 2);
        stepGrowthRate *= growthRate;
        stepGrowthRate *= dTime;

        float stepNutrientCost = (stepGrowthRate / 2) + 0.5f;
        float nutrientDeficit = nutrientCont.PlantNutrientUse(xPosition, zPosition, size, stepNutrientCost);
        if (nutrientDeficit > 0)
        {
            health -= nutrientDeficit * dTime;
            nutrientsConsumed += stepNutrientCost - nutrientDeficit;
        }
        else
        {
            nutrientsConsumed += stepNutrientCost;
            size += stepGrowthRate;
            GrowVisual(stepGrowthRate);
        }
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
        Handles.Label(transform.position, $"Health: {health}", big);
    }

    void GrowVisual(float growthAmount)
    {
        visual.transform.localScale += visual.transform.localScale * growthAmount;
    }
}
