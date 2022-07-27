using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Playgrid : MonoBehaviour
{
    public static Playgrid instance;

    public int gridSizeX, gridSizeY, gridSizeZ;
    public static int resizeScale = 10;
    public static int moveScale = resizeScale / 5;
    public static int lastRandomPoint = 0;
    public static int lastRandomColor = 0;
    public static int numberOfBlocks = 0;
    public static int numberOfMaterials = 0;
    public static int score = 0;
    public static AudioSource audioSource;
    public static float prevTime;
    public static float spawnTime = 4f;
    public static bool gameOver = false;

    [Header("Blocks")]
    public GameObject[] blockList;
    [Header("Playfield visuals")]
    public GameObject bottomPlane;
    public GameObject front, back, left, right;
    [Header("Materials")]
    public Material[] materialList;

    [Header("Text fields")]
    public GameObject scoreText;
    public GameObject gameOverText;

    [Header("Audio")]
    public AudioClip failure;
    public AudioClip success;

    public Transform[,,] theGrid;

    public static List<GameObject> allBlocks = new List<GameObject>();
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        theGrid = new Transform[gridSizeX, gridSizeY, gridSizeZ];
        numberOfBlocks = blockList.Length;
        numberOfMaterials = materialList.Length;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Time.time - prevTime > spawnTime && !gameOver)
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

        //PrintGrid();

    }

    public void PrintGrid()
    {
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
        int randomPoint = Random.Range(0, numberOfBlocks);

        // No 2 random in a row
        if (lastRandomPoint == randomPoint) randomPoint = (randomPoint + 1) % numberOfBlocks;
        lastRandomPoint = randomPoint;

        //Spwan
        GameObject newBlock = Instantiate(blockList[randomPoint], spawnPoint, Quaternion.identity) as GameObject;
        int randomMaterial = Random.Range(0, numberOfMaterials);
        // No 2 random in a row
        if (lastRandomColor == randomMaterial) randomMaterial = (randomMaterial + 1) % numberOfMaterials;
        lastRandomColor = randomMaterial;
        foreach (Transform child in newBlock.transform)
        {
            child.GetComponent<Renderer>().material = materialList[randomMaterial];
        }
        allBlocks.Add(newBlock);
    }

    static public void RestartGrid()
    {
        foreach (GameObject block in allBlocks)
        {
            Destroy(block);
        }
    }

    public void blockFailureSound()
    {
        audioSource.PlayOneShot(this.failure);
    }

    public void blockSuccessSound()
    {
        audioSource.PlayOneShot(this.success);
    }

    public void CheckLayer()
    {
        int scoreBonus = 0;
        for (int y = gridSizeY - 1; y >= 0; y--)
        {
            //Check full layer
            if (CheckFullLayer(y))
            {
                //Delete blocks
                DeleteLayer(y);
                // Move all down by 1
                MoveAllLayerDown(y);
                blockSuccessSound();
                // Add score ++, extra if more than 1 row at once
                addScore(100 + scoreBonus);
                scoreBonus += 25;
                switch (score)
                {
                    case > 1200:
                        spawnTime = 2.0f;
                        PlayBlock.fallTime = 0.4f;
                        break;
                    case > 900:
                        spawnTime = 2.5f;
                        PlayBlock.fallTime = 0.5f;
                        break;
                    case > 600:
                        spawnTime = 3.0f;
                        PlayBlock.fallTime = 0.6f;
                        break;
                    case > 300:
                        spawnTime = 3.5f;
                        PlayBlock.fallTime = 0.7f;
                        break;
                }
            }
        }
    }

    public void addScore(int newScore)
    {
        score += newScore;
        scoreText.GetComponent<TextMeshPro>().text = "Score: " + score;
    }

    bool CheckFullLayer(int y)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                if (theGrid[x, y, z] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void DeleteLayer(int y)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Destroy(theGrid[x, y, z].gameObject);
                theGrid[x, y, z] = null;
            }
        }
    }

    void MoveAllLayerDown(int y)
    {
        for (int i = y; i < gridSizeY; i++)
        {
            MoveOneLayerDown(i);
        }
    }

    void MoveOneLayerDown(int y)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                if (theGrid[x, y, z] != null)
                {
                    theGrid[x, y - 1, z] = theGrid[x, y, z];
                    theGrid[x, y, z] = null;
                    theGrid[x, y - 1, z].position += Vector3.down;
                }
            }
        }
    }

    public void GameOver()
    {
        gameOver=true;
        //Game over zvuk
        gameOverText.transform.GetComponent<Renderer>().enabled=true;
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
