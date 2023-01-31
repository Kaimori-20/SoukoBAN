using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearScene : MonoBehaviour
{
    [SerializeField] private string _nextScene = ""; // 次のシーン

    private void Update()
    {
        // Enterキーでステージにシーン遷移する
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeScene();
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(_nextScene);
    }

}
