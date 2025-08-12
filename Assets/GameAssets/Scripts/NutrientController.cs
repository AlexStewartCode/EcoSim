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
    [SerializeField, Range(0f, 1f)] float recoveryRate; 
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

    public void Step(float dTime)
    {
        for (int r = 0; r < sizeX; r++)
        {
            for (int c = 0; c < sizeY; c++)
            {
                if (nutrientMap[r,c] < 1)
                {
                    nutrientMap[r, c] += (recoveryRate/60.0f * dTime);
                    if(nutrientMap[r, c] > 1)
                    {
                        nutrientMap[r, c] = 1;
                    }
                }
            }
        }
    }

    public float PlantNutrientUse(float x, float z, float radius, float nutrientsNeeded)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locZ = Mathf.FloorToInt(z * sizeY);

        int flooredRadius = Mathf.FloorToInt(radius);
        float totalNutrients = nutrientMap[locX, locZ];

        float[,] nutrientsAvailable;

        nutrientsAvailable = new float[flooredRadius * 2 + 1, flooredRadius * 2 + 1];

        // Calculate total amount of nutrients available to the plant, at a 1/ (2 * steps from origin) ratio. 
        for (int r = -flooredRadius; r <= flooredRadius; r++)
        {
            int currentLocX = locX + r; 

            for(int c = -flooredRadius; c <= flooredRadius; c++)
            {
                int currentLocZ = locZ + c;
                // 26 , 255
                if (currentLocX < 0 || currentLocX >= nutrientMap.GetLength(0) || currentLocZ < 0 || currentLocZ >= nutrientMap.GetLength(1))
                {
                    nutrientsAvailable[flooredRadius + r, flooredRadius + c] = 0;
                }
                else
                {
                    if (r == 0 && c == 0)
                    {
                        nutrientsAvailable[flooredRadius, flooredRadius] = nutrientMap[currentLocX, currentLocZ];
                    }
                    else
                    {
                        nutrientsAvailable[flooredRadius + r, flooredRadius + c] = nutrientMap[currentLocX, currentLocZ] / ((Mathf.Abs(r) + Mathf.Abs(c)) * 2);
                        totalNutrients += nutrientMap[currentLocX, currentLocZ] / ((Mathf.Abs(r) + Mathf.Abs(c)) * 2);
                    }
                }
            }
        }

        // If not enough nutrients are available to the plant to sustain it, return false. 
        // Can be changed to returning a float value indicating the deficit in nutrients with the plant taking that much in health damage 
        if (totalNutrients < nutrientsNeeded)
        {
            for (int r = -flooredRadius; r <= flooredRadius; r++)
            {
                int currentLocX = locX + r;

                for (int c = -flooredRadius; c <= flooredRadius; c++)
                {
                    int currentLocY = locZ + c;
                    if (currentLocX >= 0 && currentLocX < nutrientMap.GetLength(0) && currentLocY >= 0 && currentLocY < nutrientMap.GetLength(1))
                    {
                        nutrientMap[currentLocX, currentLocY] -= (nutrientsAvailable[flooredRadius + r, flooredRadius + c]);
                    }
                }
            }
            return nutrientsNeeded - totalNutrients;
        }
        for (int r = -flooredRadius; r <= flooredRadius; r++)
        {
            int currentLocX = locX + r;

            for (int c = -flooredRadius; c <= flooredRadius; c++)
            {
                int currentLocY = locZ + c;
                if (currentLocX >= 0 && currentLocX < nutrientMap.GetLength(0) && currentLocY >= 0 && currentLocY < nutrientMap.GetLength(1))
                {
                    nutrientMap[currentLocX, currentLocY] -= (nutrientsAvailable[flooredRadius + r, flooredRadius + c] / totalNutrients) * nutrientsNeeded;
                }
            }
        }
        return 0; 
    }

    public float GetAvailNutrients(float x, float y, float radius)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locY = Mathf.FloorToInt(y * sizeY);

        int flooredRadius = Mathf.FloorToInt(radius);
        float totalNutrients = nutrientMap[locX, locY];

        // Calculate total amount of nutrients available to the plant, at a 1/ (2 * steps from origin) ratio. 
        for (int r = -flooredRadius; r <= flooredRadius; r++)
        {
            int currentLocX = locX + r;

            for (int c = -flooredRadius; c <= flooredRadius; c++)
            {
                int currentLocY = locY + c;
                if (currentLocX < 0 || currentLocX >= nutrientMap.GetLength(0) || currentLocY < 0 || currentLocY >= nutrientMap.GetLength(1))
                {

                }
                else
                {
                    if (r == 0 && c == 0)
                    {
                        
                    }
                    else
                    {
                        totalNutrients += nutrientMap[currentLocX, currentLocY] / ((Mathf.Abs(r) + Mathf.Abs(c)) * 2);
                    }
                }
            }
        }
        return totalNutrients;
    }

    public void AddNutrients(float x, float y, float added, float radius)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locY = Mathf.FloorToInt(y * sizeY);

        int flooredRadius = Mathf.FloorToInt(radius);
        float totalDistro = 0; 

        for (int r = -flooredRadius; r <= flooredRadius; r++)
        {
            int currentLocX = locX + r;

            for (int c = -flooredRadius; c <= flooredRadius; c++)
            {
                int currentLocY = locY + c;
                if (currentLocX >= 0 && currentLocX < nutrientMap.GetLength(0) && currentLocY >= 0 && currentLocY < nutrientMap.GetLength(1))
                {
                    if (r == 0 && c == 0)
                    {
                        totalDistro += 1;
                    }
                    else
                    {
                        totalDistro += 1.0f / ((Mathf.Abs(r) + Mathf.Abs(c)) * 2);
                    }
                }
            }
        }

        for (int r = -flooredRadius; r <= flooredRadius; r++)
        {
            int currentLocX = locX + r;

            for (int c = -flooredRadius; c <= flooredRadius; c++)
            {
                int currentLocY = locY + c;
                if (currentLocX >= 0 && currentLocX < nutrientMap.GetLength(0) && currentLocY >= 0 && currentLocY < nutrientMap.GetLength(1))
                {
                    if(r == 0 && c == 0)
                    {
                        nutrientMap[currentLocX, currentLocY] += (added / totalDistro);
                    }
                    else
                    {
                        nutrientMap[currentLocX, currentLocY] += (added / totalDistro) / ((Mathf.Abs(r) + Mathf.Abs(c)) * 2);
                    }

                    if(nutrientMap[currentLocX, currentLocY] > 1)
                    {
                        nutrientMap[currentLocX, currentLocY] = 1;
                    }
                }
            }
        }
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
