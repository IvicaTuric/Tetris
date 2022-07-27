using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTitle : MonoBehaviour
{
    
    public static int numberOfMaterials = 0;
    public static int lastRandomColor = 0;
    float prevTime;
    public static int numFalls = 0;
    public static float fallTime = 0.8f;
    [Header("Materials")]
    public Material[] materialList;

    // Start is called before the first frame update
    void Start()
    {
        numberOfMaterials = materialList.Length;
        foreach (Transform child in transform)
        {
            int randomMaterial = Random.Range(0, numberOfMaterials);
            // No 2 random in a row
            if (lastRandomColor == randomMaterial) randomMaterial = (randomMaterial + 1) % numberOfMaterials;
            lastRandomColor = randomMaterial;
            foreach (Transform cube in child){
                cube.GetComponent<Renderer>().material = materialList[randomMaterial];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Fall every 1s
        if (Time.time - prevTime > fallTime && numFalls<45)
        {
            transform.position += Vector3.down;
            prevTime = Time.time;
            numFalls++;
        }
    }
}
