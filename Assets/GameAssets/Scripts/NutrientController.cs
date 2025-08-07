using UnityEngine;

public class NutrientController : MonoBehaviour
{
    private float[,] nutrientMap;
    [SerializeField, Range(10, 100)] int sizeX;
    [SerializeField, Range(10, 100)] int sizeY;

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

    public float GetNutrients(float x, float y)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locY = Mathf.FloorToInt(y * sizeY);

        return nutrientMap[locX, locY];
    }

    public void UseNutrients(float x, float y, float used)
    {
        int locX = Mathf.FloorToInt(x * sizeX);
        int locY = Mathf.FloorToInt(y * sizeY);

        nutrientMap[locX, locY] -= used;
    }
}
