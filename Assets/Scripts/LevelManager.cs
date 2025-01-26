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
                        StartCoroutine(OnWin());
                    }
                    break;
                }
            case 5:
            case 6:
                {
                    bool cabbageAlive = false;
                    foreach (var item in Actor.actors)
                    {
                        if(item.Type == Noun.Cabbage)
                        {
                            cabbageAlive = true;
                        }
                    }

                    if(!cabbageAlive)
                    {
                        // win
                        StartCoroutine(OnWin());
                    }
                    break;
                }
            case 7:
                {
                    bool onlyCabbage = true;
                    foreach (var item in Actor.actors)
                    {
                        if (item.Type != Noun.Cabbage)
                        {
                            onlyCabbage = false;
                        }
                    }

                    if (onlyCabbage)
                    {
                        // win
                        StartCoroutine(OnWin());
                    }
                    break;
                }
            case 8:
                {
                    break;
                }
        }
    }

    bool won = false;
    private IEnumerator OnWin()
    {
        if (!won)
        {
            won = true;
            textMeshProUGUI.gameObject.SetActive(true);

            yield return new WaitForSeconds(2);

            SceneManager.LoadScene(CurrentLevel + 1);
        }
    }
}
