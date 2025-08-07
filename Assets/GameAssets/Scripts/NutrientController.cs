using UnityEditor.TextCore.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class NutrientController : MonoBehaviour
{
    private float[,] nutrientMap;
    [SerializeField, Range(10, 100)] int sizeX;
    [SerializeField, Range(10, 100)] int sizeY;
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
                texture.SetPixel(r, c, new Color(colorValue, colorValue, colorValue));
            }
        }

        texture.Apply();
        meshRenderer.material.mainTexture = texture;
    }

    private void ConsoleDump()
    {
        string toPrint = "\n";
        for (int r = 0; r < sizeX; r++)
        {
            for (int c = 0; c < sizeY; c++)
            {
                toPrint += nutrientMap[r, c] + " ";
            }
            toPrint += "\n";
        }
        Debug.Log(toPrint);
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

            if(GUILayout.Button("Dump it"))
            {
                (target as NutrientController).ConsoleDump();
            }
        }
    }
    #endif
}
