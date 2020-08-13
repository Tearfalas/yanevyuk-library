using Assets.Submodules.YanevyukLibrary.Preload;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReturner : MonoBehaviour
{
    private List<PreloadElement> elements = new List<PreloadElement>();
    public int sceneToLoad;
    private void Start()
    {
        foreach (var item in gameObject.GetComponents<PreloadElement>())
        {
            elements.Add(item);
        }
        StartCoroutine(waiter());
    }

    IEnumerator waiter(){
        while (true)
        {
            yield return null;
            foreach (var elt in elements)
            {
                if (!elt.isComplete())
                {
                    continue;
                }
            }
            break;
        }
        SceneManager.LoadScene(sceneToLoad);
    }

}
