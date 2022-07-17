using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBlock : MonoBehaviour
{
    public static int resizeScale = 1;
    float prevTime;
    float fallTime = 0.8f;
    bool grabbed = false;
    void Start()
    {
        MoveToValidPos();
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
            if (!CheckValidFall())
            {
                transform.position += Vector3.up;
                GetComponent<Rigidbody>().detectCollisions = false;
                EndMove();
            }
        }

    }

    bool CheckValidFall()
    {

        //Stops fall if encounters another block
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            if( pos.y==-1) return false;
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
        // Check inside grid
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            if (!Playgrid.instance.CheckInsideGrid(pos))
            {
                return false;
            };
        }

        
        // Cube position in grid not empty and taken by cube of different block
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
            Transform t = Playgrid.instance.GetTransformOnGridPos(pos);

            if (t != null && t.parent != transform)
            {
                return false;
            }
        }
        
        // Cube is at lowest level or there is a cube from different block below
        foreach (Transform child in transform)
        {
            Vector3 pos = Playgrid.instance.RoundPosition(child.position);
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

    public void GrabBlock()
    {
        grabbed = true;
        GetComponent<Rigidbody>().detectCollisions = false;
    }

    public void ReleaseBlock()
    {
        MoveToValidPos();
        EndMove();
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
    }

    public void EndMove()
    {
        enabled = false;
        if (!CheckValidMove())
        {
            //Destroy block because out of grid or inside other block
            DestroyBlock();
            //Dodaj zvuk / animaciju?
        }
        else
        {
            Playgrid.instance.UpdateGrid(this);
            //
            Playgrid.instance.CheckLayer();
        }
    }

    public void DestroyBlock(){
        foreach (Transform child in transform){
            child.GetComponent<Renderer>().material.EnableKeyword("__EMISSION");
            StartCoroutine(Blink(child));
        }
        Playgrid.blockFailureSound();
        Destroy(gameObject, 0.8f);
    }

    IEnumerator Blink(Transform obj)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        objRenderer.enabled = true;
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 3; i++)
        {
            objRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            objRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
