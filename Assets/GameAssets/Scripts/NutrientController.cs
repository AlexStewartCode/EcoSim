using System;
using UnityEditor.TextCore.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class NutrientController : MonoBehaviour
{
    private float[,] nutrientMap;
    [SerializeField, Range(10, 256)] int sizeX;
    [SerializeField, Range(10, 256)] int sizeY;
    [SerializeField] MeshRenderer meshRenderer; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nutrientMap = new float[sizeX, sizeY];
        for(int r = 0; r < sizeX; r ++)
        {
            for(int c = 0; c < sizeY; c++)
            {
                nutrientMap[r, c] = 1; 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool PlantNutrientUse(float x, float y, float radius, float nutrientsNeeded)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locY = Mathf.FloorToInt(y * sizeY);

        int flooredRadius = Mathf.FloorToInt(radius);
        float totalNutrients = nutrientMap[locX, locY];

        float[,] nutrientsAvailable = new float[flooredRadius * 2 + 1, flooredRadius * 2 + 1];

        for (int r = -flooredRadius; r <= flooredRadius; r++)
        {
            int currentLocX = locX + r; 

            for(int c = -flooredRadius; c <= flooredRadius; c++)
            {
                int currentLocY = locY + c; 

                if(r == 0 && c == 0)
                {
                    nutrientsAvailable[flooredRadius, flooredRadius] = nutrientMap[currentLocX, currentLocY];
                }
                else
                {
                    nutrientsAvailable[flooredRadius + r, flooredRadius + c] = nutrientMap[currentLocX, currentLocY] / ((Mathf.Abs(r) + Mathf.Abs(c)) * 2);
                    totalNutrients += nutrientMap[currentLocX, currentLocY] / ((Mathf.Abs(r) + Mathf.Abs(c)) * 2);
                }
            }
        }

        if (totalNutrients < nutrientsNeeded)
        {
            return false; 
        }

        float[,] amountToUse = new float[flooredRadius * 2 + 1, flooredRadius * 2 + 1];

        for(int r = 0; r < nutrientsAvailable.GetLength(0); r++)
        {
            for (int c = 0; c < nutrientsAvailable.GetLength(1); c++)
            {
                amountToUse[r, c] = (nutrientsAvailable[r, c] / totalNutrients) * nutrientsNeeded;
            }
        }

        string output2 = "NUTRIENTS TO USE\n";
        for (int r = 0; r < amountToUse.GetLength(0); r++)
        {
            for (int c = 0; c < amountToUse.GetLength(1); c++)
            {
                output2 += amountToUse[r, c] + " ";
            }
            output2 += "\n";
        }
        Debug.Log(output2);

        return true; 
    }

    public float GetNutrients(float x, float y, float radius)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locY = Mathf.FloorToInt(y * sizeY);

        int ceilRad = Mathf.CeilToInt(radius);
        float totalNutrients = 0; 

        for(int r = -ceilRad; r < ceilRad; r++)
        {
            for (int c = -ceilRad; c < ceilRad; c++)
            {
                if((locX + ceilRad > 0 && locX + ceilRad < sizeX) && (locY + ceilRad > 0 && locY + ceilRad < sizeY))
                {
                    int fraction = Mathf.Abs(r) + Mathf.Abs(c);
                    if(fraction == 0)
                    {
                        totalNutrients += nutrientMap[locX + ceilRad, locY + ceilRad];
                    }
                    else
                    {
                        totalNutrients += nutrientMap[locX + ceilRad, locY + ceilRad] / (fraction * 2);
                    }
                }
            }
        }

        return totalNutrients;
    }

    public void UseNutrients(float x, float y, float radius, float used)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locY = Mathf.FloorToInt(y * sizeY);

        nutrientMap[locX, locY] -= used;
    }

    public void AddNutrients(float x, float y, float added)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locY = Mathf.FloorToInt(y * sizeY);

        nutrientMap[locX, locY] += added;
    }

    private void Render()
    {
        Texture2D texture = new Texture2D(sizeX, sizeY);
        for (int r = 0; r < sizeX; r++)
        {
            for (int c = 0; c < sizeY; c++)
            {
                float colorValue = nutrientMap[r, c];
                texture.SetPixel(r, c, new Color(colorValue, colorValue, colorValue, 0.5f));
            }
        }

        texture.Apply();
        meshRenderer.material.mainTexture = texture;
        meshRenderer.gameObject.SetActive(true);
    }

    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(NutrientController))]
    
    public class NutrientControllerEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Sample Button"))
            {
                (target as NutrientController).Render();
            }
        }
    }
    #endif
}
