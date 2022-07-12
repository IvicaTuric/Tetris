using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBlock : MonoBehaviour
{
    public static int resizeScale = 1;
    float prevTime;
    float fallTime = 1f;
    void Start()
    {

    }

    void OnDrawGizmos()
    {
        //Resize
        Vector3 scaler = new Vector3((float)1 / resizeScale, (float)1 / resizeScale, (float)1 / resizeScale);
        this.transform.localScale = scaler;
    }

    void Update()
    {
        // Fall every 1s
        if (Time.time - prevTime > fallTime)
        {
            transform.position += Vector3.down;
            prevTime = Time.time;

            if (!CheckValidFall())
            {
                transform.position += Vector3.up;
                GetComponent<Rigidbody>().detectCollisions = false;
                enabled = false;
            }
            else if (CheckValidMove())
            {
                Playgrid.instance.UpdateGrid(this);
                //Update
            }

        }

    }

    bool CheckValidFall()
    {
        //Check if above 6lvl for each cube in block
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            if (!Playgrid.instance.CheckFallLevel(pos))
            {
                return false;
            };
        }
        
        //Stops fall if encounters anouter block
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            Transform t = Playgrid.instance.GetTransformOnGridPos(pos);
            // Cube position in grid not empty and taken by cube of different block
            if (t != null && t.parent != transform)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }

    bool CheckValidMove(){
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            if (!Playgrid.instance.CheckInsideGrid(pos))
            {
                return false;
            };
        }

        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            Transform t = Playgrid.instance.GetTransformOnGridPos(pos);
            // Cube position in grid not empty and taken by cube of different block
            if (t != null && t.parent != transform)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }

    public void ReleaseBlock()
    {
        
        Vector3 rot = Playgrid.instance.RoundRotation(transform.eulerAngles);
        transform.eulerAngles=rot;
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            child.position=pos;
        }
        GetComponent<Rigidbody>().detectCollisions = false; //Stavi na primanje?
        enabled = false;

        if(!CheckValidMove()){
            //Unisti kocku test
            Debug.Log("Destroyeeeed");
            Destroy(this);
        }
        else{
            Playgrid.instance.UpdateGrid(this);
            //
        }
    }


}
