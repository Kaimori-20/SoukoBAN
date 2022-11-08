using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearScene : MonoBehaviour
{
    [SerializeField] private string NextScene; // ���̃V�[��

    private void Update()
    {
        // Enter�L�[�ŃX�e�[�W�ɃV�[���J�ڂ���
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeScene();
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(NextScene);
    }

}
