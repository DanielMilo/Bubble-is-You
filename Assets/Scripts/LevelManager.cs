using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public BarksManager barksManager;

    public void loadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    private void Update()
    {
        if(BarksManager.Instance ==  null && barksManager != null)
        {
            barksManager.Load();
        }

        if(Input.GetKeyDown(KeyCode.R)) 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
