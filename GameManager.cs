using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stage; // ���̃V�[���̃X�e�[�W
    [SerializeField] private string NextScene; // ���̃V�[��
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
        stage.LoadTileData(); // �^�C���̏����Ăяo��
        stage.CreateStage(); // �X�e�[�W���Ăяo��
        player = stage.player;
    }

    // ���[�U�[�̓��͂��󂯂čX�V����
    private void Update()
    {
        // ����L�[���������Ƃ��̏���
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            // ������ɍs��
            MoveTo(DIRECTION.UP);
        }
        // �����L�[���������Ƃ��̏���
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // �������ɍs��
            MoveTo(DIRECTION.DOWN);
        }
        // �����L�[���������Ƃ��̏���
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // �������ɍs��
            MoveTo(DIRECTION.LEFT);
        }
        // �E���L�[���������Ƃ��̏���
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // �E�����ɍs��
            MoveTo(DIRECTION.RIGHT);
        }

        // �X�y�[�X�L�[�Ń��g���C����
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

    // �X�y�[�X�L�[�Ń��g���C
    void Retry()
    {
        // ������X�e�[�W�̌��̏�Ԃ��Ăяo��
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    void MoveTo(DIRECTION direction)
    {
        // tile��̃v���C���[��position���擾����
        Vector2Int currentPlayerPositionOnTile =
            stage.moveObjPositionOnTile[player.gameObject];
        // ���̃v���C���[�̊e�����̃|�W�V�������擾
        Vector2Int nextPlayerPositionOnTile =
            GetNextrPositionOnTile(currentPlayerPositionOnTile, direction);

        // ���ɐi�݂����ꏊWALL���ǂ���
        if(stage.IsWall(nextPlayerPositionOnTile))
        {
            return; // �������~�߂����ŏI��������
        }
        // ���ɐi�݂����ꏊ��BLOCK���ǂ���
        if (stage.IsBlock(nextPlayerPositionOnTile))
        {
            // 
            Vector2Int nextBlockPositionOnTile = 
                GetNextrPositionOnTile(nextPlayerPositionOnTile, direction);
            
            // block�̈ړ��悪WALL��BLOCK��������
            if(stage.IsBlock(nextBlockPositionOnTile) || stage.IsWall(nextBlockPositionOnTile))
            {
                return; // �ړ������Ȃ�
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
            // �^����ꂽ���̂��������������
            case DIRECTION.UP:
                return currentPosition + Vector2Int.down;// down��Ԃ�

            // �^����ꂽ���̂���������������
            case DIRECTION.DOWN:
                return currentPosition + Vector2Int.up;// up��Ԃ�

            // �^����ꂽ���̂���������������
            case DIRECTION.LEFT:
                return currentPosition + Vector2Int.left;// left��Ԃ�

            // �^����ꂽ���̂��E������������
            case DIRECTION.RIGHT:
                return currentPosition + Vector2Int.right;// right��Ԃ�
        }
        // �������͂���Ȃ������ꍇ�A���̂܂܂̈ʒu��Ԃ�
        return currentPosition;

    }
}
