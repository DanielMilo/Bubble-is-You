using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public BarksManager barksManager;

    public int CurrentLevel;

    public TextMeshProUGUI textMeshProUGUI;

    float startTime;

    private void Awake()
    {
        startTime = Time.time;
    }

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

        if (Time.time - startTime > 3)
        {
            CheckForWin();
        }
    }

    private void CheckForWin()
    {
        switch (CurrentLevel)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                {
                    if(Actor.actors.Count == 1)
                    {
                        // win
                        _ = OnWin();
                    }
                    break;
                }
            case 5:
                {
                    break;
                }
            case 6:
                {
                    break;
                }
        }
    }

    bool won = false;
    private async Task OnWin()
    {
        if (!won)
        {
            won = true;
            textMeshProUGUI.gameObject.SetActive(true);
            await Task.Delay(2000);

            SceneManager.LoadScene(CurrentLevel + 1);
        }
    }
}
