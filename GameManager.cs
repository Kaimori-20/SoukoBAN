using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stage; // このシーンのステージ
    [SerializeField] private string NextScene; // 次のシーン
    PlayerManager player;
    bool isClear;

    enum DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    private void Start()
    {
        stage.LoadTileData(); // タイルの情報を呼び出す
        stage.CreateStage(); // ステージを呼び出す
        player = stage.player;
    }

    // ユーザーの入力を受けて更新する
    private void Update()
    {
        // 上矢印キーを押したときの処理
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 上方向に行く
            MoveTo(DIRECTION.UP);
        }
        // 下矢印キーを押したときの処理
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 下方向に行く
            MoveTo(DIRECTION.DOWN);
        }
        // 左矢印キーを押したときの処理
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 左方向に行く
            MoveTo(DIRECTION.LEFT);
        }
        // 右矢印キーを押したときの処理
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 右方向に行く
            MoveTo(DIRECTION.RIGHT);
        }

        // スペースキーでリトライする
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Retry();
        }

        CheckAllClear();
    }

    // 
    void CheckAllClear()
    {
        if(isClear)
        {
            return;
        }
        if(stage.IsAllClear())
        {
            isClear = true;
            Invoke("Clear",1f);
        }
    }

    // 
    void Clear()
    {
        isClear = false;
        SceneManager.LoadScene(NextScene);
    }

    // スペースキーでリトライ
    void Retry()
    {
        // 今いるステージの元の状態を呼び出す
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    void MoveTo(DIRECTION direction)
    {
        // tile上のプレイヤーのpositionを取得する
        Vector2Int currentPlayerPositionOnTile =
            stage.moveObjPositionOnTile[player.gameObject];
        // 次のプレイヤーの各方向のポジションを取得
        Vector2Int nextPlayerPositionOnTile =
            GetNextrPositionOnTile(currentPlayerPositionOnTile, direction);

        // 次に進みたい場所WALLかどうか
        if(stage.IsWall(nextPlayerPositionOnTile))
        {
            return; // 処理を止めここで終了させる
        }
        // 次に進みたい場所がBLOCKかどうか
        if (stage.IsBlock(nextPlayerPositionOnTile))
        {
            // 
            Vector2Int nextBlockPositionOnTile = 
                GetNextrPositionOnTile(nextPlayerPositionOnTile, direction);
            
            // blockの移動先がWALLかBLOCKだったら
            if(stage.IsBlock(nextBlockPositionOnTile) || stage.IsWall(nextBlockPositionOnTile))
            {
                return; // 移動させない
            }

            // 
            stage.UpdateBlockPosition(nextPlayerPositionOnTile,nextBlockPositionOnTile);
        }

        //
        stage.UpdateTileTableForPlayer(currentPlayerPositionOnTile, nextPlayerPositionOnTile);

        // 
        player.Move(stage.GetScreenPositionFromTileTable(nextPlayerPositionOnTile));

        //
        stage.moveObjPositionOnTile[player.gameObject] = nextPlayerPositionOnTile;
    }

    Vector2Int GetNextrPositionOnTile(Vector2Int currentPosition,DIRECTION direction)
    {
        switch(direction)
        {
            // 与えられたものが上方向だったら
            case DIRECTION.UP:
                return currentPosition + Vector2Int.down;// downを返す

            // 与えられたものが下方向だったら
            case DIRECTION.DOWN:
                return currentPosition + Vector2Int.up;// upを返す

            // 与えられたものが左方向だったら
            case DIRECTION.LEFT:
                return currentPosition + Vector2Int.left;// leftを返す

            // 与えられたものが右方向だったら
            case DIRECTION.RIGHT:
                return currentPosition + Vector2Int.right;// rightを返す
        }
        // 何も入力されなかった場合、そのままの位置を返す
        return currentPosition;

    }
}
