using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public class SceneControl : MonoBehaviour
    {
        public void ReloadScene(String sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ReloadScene()
        {
            Scene scene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(scene.name);
        }
    }
}