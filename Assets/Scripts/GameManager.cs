using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{

    static GameManager instanceObject;

    public static GameManager instance()
    {
        if (instanceObject == null)
        {
            GameObject gameObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("GameManager"));
            DontDestroyOnLoad(gameObject);
            instanceObject = gameObject.GetComponent<GameManager>();
        }
        return instanceObject;
    }

    [Header("Level Settings")]
    public string titleSceneName = "Title";
    public string gameOverSceneName = "GameOver";
    public List<string> levels = new List<string>();


    [Header("Stats")]
    public int memoCompletedCount = 0;
    public int memoDeclinedCount = 0;
    public int itemRottenCount = 0;
    public int maxPackedItemCount = 0;

    public void LoadTitle()
    {
        LoadScene(titleSceneName);
    }

    public void LoadFirstLevel()
    {
        LoadScene(levels[0]);
    }

    public void LoadNextLevel()
    {
        string active = SceneManager.GetActiveScene().name;
        int index = levels.FindIndex(name => { return name == active; });
        if (index == -1)
        {
            Debug.LogError("Unexpected not finding level '{0}' in the levels list.");
            LoadGameOver();
        }
        else
        {
            if (index >= levels.Count - 1)
            {
                LoadGameOver();
            }
            else
            {
                LoadScene(levels[index + 1]);
            }
        }
    }

    public void LoadGameOver()
    {
        LoadScene(gameOverSceneName);
    }

    void LoadScene(string name)
    {
        string active = SceneManager.GetActiveScene().name;
        if (active != name)
        {
            Debug.LogFormat("Scene transition [{0}] -> [{1}]", active, name);
            SceneManager.LoadScene(name);
        }
    }

    public void ResetStats() {
        memoCompletedCount = 0;
        memoDeclinedCount = 0;
        itemRottenCount = 0;
        maxPackedItemCount = 0;
    }

    public void IncreaseMemoCompleted()
    {
        memoCompletedCount++;
    }

    public void IncreaseMemoDeclined()
    {
        memoDeclinedCount++;
    }
    public void IncreaseItemRotten()
    {
        itemRottenCount++;
    }
    public void UpdateMaxPackedItem(int count)
    {
        if (count > maxPackedItemCount)
        {
            maxPackedItemCount = count;
        }
    }
}
