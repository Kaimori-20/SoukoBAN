using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] TextAsset stageFile; // ステージのテキスト
    [SerializeField] GameObject[] prefabs; // プレハブ化したゲームオブジェクトの格納

    // 配列内の数字の定義
    enum TILE_TYPE
    {
        WALL,        // 壁(0)
        GROUND,      // 床(1)
        BLOCK_POINT, // 目的地(2)
        BLOCK,       // 箱(3)
        PLAYER,      // プレイヤー(4)
        BLOCK_ON_POINT,  // BLOCKが乗ったターゲット
        PLAYER_ON_POINT, // プレイヤーが乗ったターゲット
    }
    // enumで定義した
    TILE_TYPE[,] tileTable;
    float tileSize; 
    Vector2 centerPosition; // 画面中央に生成するための変数
    public PlayerManager player; // プレイヤーのスクリプトを取得
    int blockCount; // ブロックの数

    // GameObjectを検索キーワードにし位置情報を取得する
    public Dictionary<GameObject, Vector2Int> 
        moveObjPositionOnTile = new Dictionary<GameObject, Vector2Int>();

    // タイルの情報を呼び出す
    public void LoadTileData()
    {
        // タイルの情報を一行ごとに分割
        string[] lines = stageFile.text.Split(new[] { '\n','\r' },
            System.StringSplitOptions.RemoveEmptyEntries);

        //ステージの長さを取得
        int columns = lines[0].Split(new[] { ',' }).Length; // 列
        int rows = lines.Length; // 行
        // colu,sとrowsからそれぞれ列と行を取得
        tileTable = new TILE_TYPE[columns, rows];

        for (int y = 0; y < rows; y++)
        {
            string[] values = lines[y].Split(new[] { ',' });
            for (int x = 0; x < columns; x++)
            {
                // tileTableをint型に直し、使いやすくする
                tileTable[x, y] = (TILE_TYPE)int.Parse(values[x]);
                Debug.Log(tileTable[x, y]);
            }
        }
    }

    // tileTableを使ってタイルを生成する
    public void CreateStage()
    {
        tileSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;

        // センターポジションの設定
        // X座標のセンターポジションを列の個数の半分の位置に設定する
        centerPosition.x = (tileTable.GetLength(0) / 2) * tileSize;
        // Y座標のセンターポジションを行の個数の半分の位置に設定する
        centerPosition.y = (tileTable.GetLength(1) / 2) * tileSize;
        
        for (int y = 0; y < tileTable.GetLength(1); y++)
        {
            for (int x = 0; x < tileTable.GetLength(0); x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                // groundを敷き詰める
                GameObject ground = Instantiate(prefabs[(int)TILE_TYPE.GROUND]);
                ground.transform.position = GetScreenPositionFromTileTable(position);

                // x,y座標のTileの種類を取得
                TILE_TYPE tileType = tileTable[x, y];
                // オブジェクトを生成し、生成したオブジェクトの位置を修正する
                GameObject obj = Instantiate(prefabs[(int)tileType]);
                obj.transform.position = GetScreenPositionFromTileTable(position);

                // TILETYPEがplayerの場合
                if(tileType == TILE_TYPE.PLAYER)
                {
                    // PlayerManagerを取得
                    player = obj.GetComponent<PlayerManager>();
                    // 
                    moveObjPositionOnTile.Add(obj, position);
                }
                // TILE_TYPEがblockの場合
                if(tileType == TILE_TYPE.BLOCK)
                {
                    // ブロックの数をカウントする
                    blockCount++;

                    // 
                    moveObjPositionOnTile.Add(obj, position);
                }

                
            }
        }
    }

    public Vector2 GetScreenPositionFromTileTable(Vector2Int position)
    {
        return new Vector2(position.x * tileSize - centerPosition.x, 
            -(position.y * tileSize - centerPosition.y));
    }

    // 進みたい方向が壁だったら
    public bool IsWall(Vector2Int position)
    {
        // 次に動きたい方向が壁だったら
        if(tileTable[position.x,position.y] == TILE_TYPE.WALL)
        {
            return true;// boolをtrueにする
        }
        return false;// boolをfalseにする
    }

    // 進みたい方向がブロックだったら
    public bool IsBlock(Vector2Int position)
    {
        // 次に動きたい方向が壁だったら
        if (tileTable[position.x, position.y] == TILE_TYPE.BLOCK　||
            tileTable[position.x, position.y] == TILE_TYPE.BLOCK_ON_POINT)
        {
            return true;// boolをtrueにする
        }
        return false;// boolをfalseにする
    }

    // ぶつかったBlockを取得する関数
    GameObject GetBlockObjectAt(Vector2Int position)
    {
        // Dictionaryの性質を利用
        // pairにはキー(Obj)とvalue(位置)が入っている
        foreach(KeyValuePair<GameObject,Vector2Int>pair in moveObjPositionOnTile)
        {
            // 位置情報が同じなら
            if (pair.Value == position)
            {
                // 目の前にあるオブジェクトを返す
                return pair.Key;
            }
        }
        return null;
    }
    // Blockを移動させる
    public void UpdateBlockPosition(Vector2Int currentBlockPosition, Vector2Int nextBlockPosition)
    {
        // positionから動かしたいブロックを取得
        GameObject block = GetBlockObjectAt(currentBlockPosition);
        // Blockを新しい位置に置き換える
        block.transform.position = GetScreenPositionFromTileTable(nextBlockPosition);
        // 
        moveObjPositionOnTile[block] = nextBlockPosition;

        // tileTableの更新
        // blockを置いた場所が
        if(tileTable[nextBlockPosition.x, nextBlockPosition.y] == TILE_TYPE.BLOCK_POINT)
        {
            // BLOCK_POINTならBLOCK_ON_POINTにする
            tileTable[nextBlockPosition.x, nextBlockPosition.y] = TILE_TYPE.BLOCK_ON_POINT;
        }
        else
        {
            // 次にブロックが置かれる場所をBlockとする
            tileTable[nextBlockPosition.x, nextBlockPosition.y] = TILE_TYPE.BLOCK;
        }

        

    }

    public void UpdateTileTableForPlayer(Vector2Int currentPosition, Vector2Int nextPosition)
    {
        // tileTableの更新
        // プレイヤーがブロックの上に乗ったら
        if(tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.BLOCK_POINT ||
            tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.BLOCK_ON_POINT)
        {
            // 次にPlayerが置かれる場所をPLAYER_ON_POINTとする
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.PLAYER_ON_POINT;
        }
        else
        {
            // 次にPlayerが置かれる場所をPLAYERとする
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.PLAYER;
        }

        // プレイヤーが移動した際、プレイヤーがPLAYER_ON_POINT上にいたら
        if(tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.PLAYER_ON_POINT)
        {
            // BLOCK_POINTに戻す
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.BLOCK_POINT;
        }
        else
        {
            // GROUNDに戻す
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.GROUND;
        }
    }

    // blockの数とBLOCK_ON_POINTの数が一致する
    public bool IsAllClear()
    {
        // 
        int clearCount = 0;

        // 
        for (int y = 0; y < tileTable.GetLength(1); y++)
        {
            // 
            for (int x = 0; x < tileTable.GetLength(0); x++)
            {
                if(tileTable[x,y] == TILE_TYPE.BLOCK_ON_POINT)
                {
                    clearCount++;
                }
            }
        }

        // 
        if(blockCount == clearCount)
        {
            return true;
        }
        return false;
    }
}
