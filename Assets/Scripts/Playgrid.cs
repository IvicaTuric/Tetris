using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playgrid : MonoBehaviour
{
    public static Playgrid instance;

    public int gridSizeX, gridSizeY, gridSizeZ;
    public static int resizeScale = 10;
    public static int moveScale = resizeScale / 5;

    float prevTime;
    float spawnTime = 4f;

    [Header("Blocks")]
    public GameObject[] blockList;
    [Header("Playfield visuals")]
    public GameObject bottomPlane;
    public GameObject front, back, left, right;
    [Header("Materials")]
    public Material[] materialList;

    public Transform[,,] theGrid;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        theGrid = new Transform[gridSizeX, gridSizeY, gridSizeZ];
    }

    void Update()
    {
        // Spawn every 5s
        if (Time.time - prevTime > spawnTime)
        {
            prevTime = Time.time;
            Playgrid.instance.SpawnNewBlock();
        }
    }

    public Vector3 RoundPosition(Vector3 vec)
    {
        return new Vector3(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }

    public Vector3 RoundRotation(Vector3 vec)
    {
        return new Vector3(Mathf.Round(vec.x / 90) * 90, Mathf.Round(vec.y / 90) * 90, Mathf.Round(vec.z / 90) * 90);
    }


    public bool CheckInsideGrid(Vector3 vec)
    {
        return ((int)vec.x >= 0 && (int)vec.x < gridSizeX &&
                (int)vec.y >= 0 && (int)vec.y < gridSizeY &&
                (int)vec.z >= 0 && (int)vec.z < gridSizeZ);
    }

    public void UpdateGrid(PlayBlock block)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (theGrid[x, y, z] != null)
                    {
                        if (theGrid[x, y, z].parent == block.transform)
                        {
                            theGrid[x, y, z] = null;
                        }
                    }
                }
            }
        }

        foreach (Transform child in block.transform)
        {
            Vector3 pos = RoundPosition(child.position);
            if (pos.y < gridSizeY)
            {
                theGrid[(int)pos.x, (int)pos.y, (int)pos.z] = child;
            }
        }

    string row = "";
    for (int x = 0; x < gridSizeX; x++)
    {
        for (int z = 0; z < gridSizeZ; z++)
        {
            if (theGrid[x, 0, z] != null) row += "x";
            else row += "_";
        }
        row += "\n";
    }
    Debug.Log(row);

    }

    public Transform GetTransformOnGridPos(Vector3 pos)
    {
        if (pos.y > gridSizeY - 1)
        {
            return null;
        }
        else
        {
            return theGrid[(int)pos.x, (int)pos.y, (int)pos.z];
        }
    }

    public void SpawnNewBlock()
    {
        Vector3 spawnPoint = new Vector3(2, 15, 2);
        int randomPoint = Random.Range(0, blockList.Length);

        //Spwan
        GameObject newBlock = Instantiate(blockList[randomPoint], spawnPoint, Quaternion.identity) as GameObject;
        int randomMaterial = Random.Range(0, materialList.Length);
        foreach (Transform child in newBlock.transform)
        {
            child.GetComponent<Renderer>().material = materialList[randomMaterial];
        }
    }

    void OnDrawGizmos()
    {

        if (bottomPlane != null)
        {
            //Resize
            Vector3 scaler = new Vector3((float)gridSizeX / resizeScale, 1, (float)gridSizeZ / resizeScale);
            bottomPlane.transform.localScale = scaler;

            //Reposition
            bottomPlane.transform.position = new Vector3(transform.position.x + (float)gridSizeX / moveScale,
                                                        transform.position.y,
                                                        transform.position.z + (float)gridSizeZ / moveScale);

            // Retile
            bottomPlane.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeZ);
        }

        if (right != null)
        {
            //Resize
            Vector3 scaler = new Vector3((float)gridSizeX / resizeScale, 1, (float)gridSizeY / resizeScale);
            right.transform.localScale = scaler;

            //Reposition
            right.transform.position = new Vector3(transform.position.x + (float)gridSizeX / moveScale,
                                                        transform.position.y + (float)gridSizeY / moveScale,
                                                        transform.position.z + (float)gridSizeZ / ((float)moveScale / 2));

            // Retile
            right.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeY);
        }

        if (left != null)
        {
            //Resize
            Vector3 scaler = new Vector3((float)gridSizeX / resizeScale, 1, (float)gridSizeY / resizeScale);
            left.transform.localScale = scaler;

            //Reposition
            left.transform.position = new Vector3(transform.position.x + (float)gridSizeX / moveScale,
                                                        transform.position.y + (float)gridSizeY / moveScale,
                                                        transform.position.z);

            // Retile shared from right
        }

        if (front != null)
        {
            //Resize
            Vector3 scaler = new Vector3((float)gridSizeZ / resizeScale, 1, (float)gridSizeY / resizeScale);
            front.transform.localScale = scaler;

            //Reposition
            front.transform.position = new Vector3(transform.position.x + (float)gridSizeX / ((float)moveScale / 2),
                                                        transform.position.y + (float)gridSizeY / moveScale,
                                                        transform.position.z + (float)gridSizeZ / moveScale);

            front.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeZ, gridSizeY);
        }

        if (back != null)
        {
            //Resize
            Vector3 scaler = new Vector3((float)gridSizeZ / resizeScale, 1, (float)gridSizeY / resizeScale);
            back.transform.localScale = scaler;

            //Reposition
            back.transform.position = new Vector3(transform.position.x,
                                                        transform.position.y + (float)gridSizeY / moveScale,
                                                        transform.position.z + (float)gridSizeZ / moveScale);

        }
    }
}
