using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SceneName] public string toSceneName;
    [SceneName] public string backSceneName;
    // Start is called before the first frame update
    public void Forward()
    {
        SceneManager.LoadScene(toSceneName);
    }

    public void Backward()
    {
        SceneManager.LoadScene(backSceneName);
    }
}
