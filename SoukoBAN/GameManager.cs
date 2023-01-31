using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager _stage; // このシーンのステージ
    [SerializeField] private string _nextScene = ""; // 次のシーン
    private PlayerManager _player;
    bool _isClear;

    enum DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    private void Start()
    {
        _stage.LoadTileData(); // タイルの情報を呼び出す
        _stage.CreateStage(); // ステージを呼び出す
        _player = _stage._player;　// stageのスクリプトからプレイヤーを呼び出す
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

        // ステージをクリアしたかどうかを調べる
        CheckAllClear();
    }

    // ステージをクリアした判定になったら
    void CheckAllClear()
    {
        if(_isClear)
        {
            return;
        }
        if(_stage.IsAllClear())
        {
            _isClear = true;
            Invoke("Clear",1f);
        }
    }

    // ステージをクリアした時の処理
    void Clear()
    {
        // _isClearをもとに戻す
        _isClear = false;

        // 次のシーンへ遷移する
        SceneManager.LoadScene(_nextScene);
    }

    // スペースキーでリトライ
    void Retry()
    {
        // 今いるステージの元の状態を呼び出す
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    // プレイヤーとブロックの移動
    void MoveTo(DIRECTION direction)
    {
        // tile上のプレイヤーのpositionを取得する
        Vector2Int currentPlayerPositionOnTile =
            _stage._moveObjPositionOnTile[_player.gameObject];
        // 次のプレイヤーの各方向のポジションを取得
        Vector2Int nextPlayerPositionOnTile =
            GetNextrPositionOnTile(currentPlayerPositionOnTile, direction);

        // 次に進みたい場所WALLかどうか
        if(_stage.IsWall(nextPlayerPositionOnTile))
        {
            return; // 処理を止めここで終了させる
        }
        // 次に進みたい場所がBLOCKかどうか
        if (_stage.IsBlock(nextPlayerPositionOnTile))
        {
            // プレイヤーの二つ先がブロックが押し込まれる位置
            Vector2Int nextBlockPositionOnTile = 
                GetNextrPositionOnTile(nextPlayerPositionOnTile, direction);
            
            // blockの移動先がWALLかBLOCKだったら
            if(_stage.IsBlock(nextBlockPositionOnTile) || _stage.IsWall(nextBlockPositionOnTile))
            {
                return; // 移動させない
            }

            // ブロックのアップデート、押されたブロックを次の位置に置き換える
            _stage.UpdateBlockPosition(nextPlayerPositionOnTile,nextBlockPositionOnTile);
        }

        //ステージのアップデート、プレイヤーを次の位置に置き換える
        _stage.UpdateTileTableForPlayer(currentPlayerPositionOnTile, nextPlayerPositionOnTile);

        // nextPlayerPositionにプレイヤーを移動させる
        _player.Move(_stage.GetScreenPositionFromTileTable(nextPlayerPositionOnTile));

        // タイル情報を更新させる
        _stage._moveObjPositionOnTile[_player.gameObject] = nextPlayerPositionOnTile;
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
