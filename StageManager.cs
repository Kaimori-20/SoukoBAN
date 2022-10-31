using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] TextAsset stageFile;

    // �z����̐����̒�`
    enum TILE_TIPE
    {
        WALL,        // ��(0)
        GROUND,      // ��(1)
        BLOCK_POINT, // �ړI�n(2)
        BLOCK,       // ��(3)
        PLAYER,      // �v���C���[(4)
    }
    // enum�Œ�`����
    TILE_TIPE[,] tilrTable;



    private void Start()
    {
        LoadTileData(); // �^�C���̏����Ăяo��
    }

    // �^�C���̏����Ăяo��
    void LoadTileData()
    {
        // �^�C���̏�����s���Ƃɕ���
        string[] lines = stageFile.text.Split(new[] { '\n','\r' },
            System.StringSplitOptions.RemoveEmptyEntries);

        // �z��̏�����s���\������
        foreach(string line in lines)
        {
            // �z��̏���','���Ƃɕ�������
            string[] values = line.Split(new[] { ',' });

            // 
            foreach(string value in values)
            {
                Debug.Log(value);
            }
        }
    }


}
