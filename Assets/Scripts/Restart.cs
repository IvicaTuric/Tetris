using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void RestartAll()
    {
        Debug.Log("Restarting grid");
        Playgrid.RestartGrid();
        Playgrid.score = 0;
        Playgrid.instance.addScore(0);
        Playgrid.spawnTime = 4.0f;
        PlayBlock.fallTime = 0.8f;
    }
}
