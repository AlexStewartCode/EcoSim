using UnityEngine;

public class _IAnimal : MonoBehaviour
{
    [SerializeField] GameObject model;

    // Inherent values
    public int foodChainPlacement;
    public int gender;
    public float speed;
    public float sightDist;

    public float weightHunger;
    public float weightBreed; 

    // Current values for need of each 
    public float health;
    public float breed; 

    Collider sightSphere;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    public void Eat()
    {

    }

    public void Breed()
    {

    }
}
