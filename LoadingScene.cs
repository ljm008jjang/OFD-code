using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PathCreation;

public class LoadingScene : MonoBehaviour
{

    public Image progressBar;
    public static int mapIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneProcess());
        Debug.Log("실행");
    }

    public static void LoadScene(int currentMapIndex)
    {
        mapIndex = currentMapIndex;
        SceneManager.LoadSceneAsync("Map" + (mapIndex + 1));
        Debug.Log(mapIndex);
    }

    IEnumerator LoadSceneProcess()
    {
        Debug.Log("실행");
        AsyncOperation op = SceneManager.LoadSceneAsync("Map"+(mapIndex+1));
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            
            yield return null;
            
            if(op.progress < 0.9f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
 
        }
    }
    
}
