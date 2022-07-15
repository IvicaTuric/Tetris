using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBlock : MonoBehaviour
{
    public static int resizeScale = 1;
    float prevTime;
    float fallTime = 1f;
    bool grabbed = false;
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
        if (Time.time - prevTime > fallTime && !grabbed)
        {
            transform.position += Vector3.down;
            prevTime = Time.time;
            try
            {
                if (!CheckValidFall())
                {
                    transform.position += Vector3.up;
                    MoveToValidPos();
                    GetComponent<Rigidbody>().detectCollisions = false;
                    EndMove();
                }
            }
            catch
            {
                Destroy(gameObject);
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

        //Stops fall if encounters another block
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

    bool CheckValidMove()
    {
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

            // Cube is at lowest level or there is a cube from different block below
            if (pos.y != 0)
            {
                Vector3 posBelow = new Vector3(pos.x, pos.y - 1, pos.z);
                Transform t2 = Playgrid.instance.GetTransformOnGridPos(posBelow);
                if (t2 != null && t2.parent != transform)
                {
                    return true;
                }
            }
            else return true;
        }
        return false;
    }

    public void ReleaseBlock()
    {
        grabbed = false;
        MoveToValidPos();

        EndMove();


    }

    public void GrabBlock()
    {
        grabbed = true;
        GetComponent<Rigidbody>().detectCollisions = false;
    }

    public void MoveToValidPos()
    {
        Vector3 rot = Playgrid.instance.RoundRotation(transform.eulerAngles);
        transform.eulerAngles = rot;
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            child.position = pos;
        }
        enabled = false;
    }

    public void EndMove()
    {
        if (!CheckValidMove())
        {
            //Destroy block because out of grid or inside other block
            Destroy(gameObject);
            //Dodaj zvuk / animaciju?
        }
        else
        {
            Playgrid.instance.UpdateGrid(this);
            //
        }
    }

}
