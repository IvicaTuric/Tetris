using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static int numberOfMaterials = 0;
    public static int lastRandomColor = 0;
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
        
    }

    public void startButton(){
        SceneManager.LoadScene("GameScene");
    }

    public void exitButton(){
        Debug.Log("EXITTTT");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void goToMenuButton(){
        //Save score to high score
        SceneManager.LoadScene("MainMenu");
    }

    public void restartButton(){
        //Save score to high score
        Debug.Log("Restarting grid");
        Playgrid.RestartGrid();
        Playgrid.score = 0;
        Playgrid.instance.addScore(0);
        Playgrid.spawnTime = 4.0f;
        PlayBlock.fallTime = 0.8f;
    }
}
