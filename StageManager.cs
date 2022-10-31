using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] TextAsset stageFile;

    // 配列内の数字の定義
    enum TILE_TIPE
    {
        WALL,        // 壁(0)
        GROUND,      // 床(1)
        BLOCK_POINT, // 目的地(2)
        BLOCK,       // 箱(3)
        PLAYER,      // プレイヤー(4)
    }
    // enumで定義した
    TILE_TIPE[,] tilrTable;



    private void Start()
    {
        LoadTileData(); // タイルの情報を呼び出す
    }

    // タイルの情報を呼び出す
    void LoadTileData()
    {
        // タイルの情報を一行ごとに分割
        string[] lines = stageFile.text.Split(new[] { '\n','\r' },
            System.StringSplitOptions.RemoveEmptyEntries);

        // 配列の情報を一行ずつ表示する
        foreach(string line in lines)
        {
            // 配列の情報を','ごとに分割する
            string[] values = line.Split(new[] { ',' });

            // 
            foreach(string value in values)
            {
                Debug.Log(value);
            }
        }
    }


}
