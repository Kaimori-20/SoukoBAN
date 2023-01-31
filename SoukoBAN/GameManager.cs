using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager _stage; // ���̃V�[���̃X�e�[�W
    [SerializeField] private string _nextScene = ""; // ���̃V�[��
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
        _stage.LoadTileData(); // �^�C���̏����Ăяo��
        _stage.CreateStage(); // �X�e�[�W���Ăяo��
        _player = _stage._player;�@// stage�̃X�N���v�g����v���C���[���Ăяo��
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

        // �X�e�[�W���N���A�������ǂ����𒲂ׂ�
        CheckAllClear();
    }

    // �X�e�[�W���N���A��������ɂȂ�����
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

    // �X�e�[�W���N���A�������̏���
    void Clear()
    {
        // _isClear�����Ƃɖ߂�
        _isClear = false;

        // ���̃V�[���֑J�ڂ���
        SceneManager.LoadScene(_nextScene);
    }

    // �X�y�[�X�L�[�Ń��g���C
    void Retry()
    {
        // ������X�e�[�W�̌��̏�Ԃ��Ăяo��
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    // �v���C���[�ƃu���b�N�̈ړ�
    void MoveTo(DIRECTION direction)
    {
        // tile��̃v���C���[��position���擾����
        Vector2Int currentPlayerPositionOnTile =
            _stage._moveObjPositionOnTile[_player.gameObject];
        // ���̃v���C���[�̊e�����̃|�W�V�������擾
        Vector2Int nextPlayerPositionOnTile =
            GetNextrPositionOnTile(currentPlayerPositionOnTile, direction);

        // ���ɐi�݂����ꏊWALL���ǂ���
        if(_stage.IsWall(nextPlayerPositionOnTile))
        {
            return; // �������~�߂����ŏI��������
        }
        // ���ɐi�݂����ꏊ��BLOCK���ǂ���
        if (_stage.IsBlock(nextPlayerPositionOnTile))
        {
            // �v���C���[�̓�悪�u���b�N���������܂��ʒu
            Vector2Int nextBlockPositionOnTile = 
                GetNextrPositionOnTile(nextPlayerPositionOnTile, direction);
            
            // block�̈ړ��悪WALL��BLOCK��������
            if(_stage.IsBlock(nextBlockPositionOnTile) || _stage.IsWall(nextBlockPositionOnTile))
            {
                return; // �ړ������Ȃ�
            }

            // �u���b�N�̃A�b�v�f�[�g�A�����ꂽ�u���b�N�����̈ʒu�ɒu��������
            _stage.UpdateBlockPosition(nextPlayerPositionOnTile,nextBlockPositionOnTile);
        }

        //�X�e�[�W�̃A�b�v�f�[�g�A�v���C���[�����̈ʒu�ɒu��������
        _stage.UpdateTileTableForPlayer(currentPlayerPositionOnTile, nextPlayerPositionOnTile);

        // nextPlayerPosition�Ƀv���C���[���ړ�������
        _player.Move(_stage.GetScreenPositionFromTileTable(nextPlayerPositionOnTile));

        // �^�C�������X�V������
        _stage._moveObjPositionOnTile[_player.gameObject] = nextPlayerPositionOnTile;
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
