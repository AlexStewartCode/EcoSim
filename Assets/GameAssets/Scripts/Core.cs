using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] public NutrientController nutrientCont;
    [SerializeField] public PlantController plantCont;
    [SerializeField] public Terrain ground;
    [SerializeField, Range(0, 10)] public float speedOfStep;

    float timeSoFar;

    public static Core instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeSoFar = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        timeSoFar += Time.deltaTime; 
        if(timeSoFar >= speedOfStep)
        {
            plantCont.Step(timeSoFar);
            nutrientCont.Step(timeSoFar);
            timeSoFar = 0;
        }
    }
}
