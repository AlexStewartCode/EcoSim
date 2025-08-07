using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] public NutrientController nutrientCont;
    [SerializeField] public PlantController plantCont;
    [SerializeField] public Terrain ground;

    public static Core instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
