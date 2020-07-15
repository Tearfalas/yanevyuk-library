using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReturner : MonoBehaviour
{
    public int frameWait;
    public int sceneToLoad;
    private void Start()
    {
        StartCoroutine(waiter());
    }

    IEnumerator waiter(){
        for(int i = 0; i< frameWait;i++){
            yield return null;
        }
        EventManager.Initialize();
        print(SceneManager.GetSceneByBuildIndex(sceneToLoad).name);
        SceneManager.LoadScene(sceneToLoad);
    }

}
