using Assets.Submodules.YanevyukLibrary.Preload;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReturner : MonoBehaviour
{
    private List<IPreloadElement> elements = new List<IPreloadElement>();
    public int sceneToLoad;
    private void Start()
    {
        foreach (var item in gameObject.GetComponents<IPreloadElement>())
        {
            elements.Add(item);
        }
        StartCoroutine(waiter());
    }

    IEnumerator waiter(){
        // TODO: Doesn't seem to be working.
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
