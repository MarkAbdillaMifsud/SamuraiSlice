using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SamuraiSlice
{
    public class SplashController : MonoBehaviour
    {
        private const float DelayTime = 2.0f;

        private void Start()
        {
            StartCoroutine(DelaySceneLoad());
        }

        private IEnumerator DelaySceneLoad()
        {
            yield return new WaitForSeconds(DelayTime);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
