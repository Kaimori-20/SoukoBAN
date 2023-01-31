using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] TextAsset _stageFile; // �X�e�[�W�̃e�L�X�g
    [SerializeField] GameObject[] _prefabs; // �v���n�u�������Q�[���I�u�W�F�N�g�̊i�[

    // �z����̐����̒�`
    enum TILE_TYPE
    {
        WALL,        // ��(0)
        GROUND,      // ��(1)
        BLOCK_POINT, // �ړI�n(2)
        BLOCK,       // ��(3)
        PLAYER,      // �v���C���[(4)
        BLOCK_ON_POINT,  // BLOCK��������^�[�Q�b�g(5)
        PLAYER_ON_POINT, // �v���C���[��������^�[�Q�b�g(6)
    }
    // enum�Œ�`����
    TILE_TYPE[,] _tileTable;
    float _tileSize; 
    Vector2 _centerPosition; // ��ʒ����ɐ������邽�߂̕ϐ�
    public PlayerManager _player; // �v���C���[�̃X�N���v�g���擾
    int _blockCount; // �u���b�N�̐�

    // GameObject�������L�[���[�h�ɂ��ʒu�����擾����
    public Dictionary<GameObject, Vector2Int> 
        _moveObjPositionOnTile = new Dictionary<GameObject, Vector2Int>();

    // �^�C���̏����Ăяo��
    public void LoadTileData()
    {
        // �^�C���̏�����s���Ƃɕ���
        string[] lines = _stageFile.text.Split(new[] { '\n','\r' },
            System.StringSplitOptions.RemoveEmptyEntries);

        //�X�e�[�W�̒������擾
        int columns = lines[0].Split(new[] { ',' }).Length; // ��
        int rows = lines.Length; // �s
        // colu,s��rows���炻�ꂼ���ƍs���擾
        _tileTable = new TILE_TYPE[columns, rows];

        for (int y = 0; y < rows; y++)
        {
            string[] values = lines[y].Split(new[] { ',' });
            for (int x = 0; x < columns; x++)
            {
                // _tileTable��int�^�ɒ����A�g���₷������
                _tileTable[x, y] = (TILE_TYPE)int.Parse(values[x]);
                Debug.Log(_tileTable[x, y]);
            }
        }
    }

    // _tileTable���g���ă^�C���𐶐�����
    public void CreateStage()
    {
        _tileSize = _prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;

        // �Z���^�[�|�W�V�����̐ݒ�
        // X���W�̃Z���^�[�|�W�V�������̌��̔����̈ʒu�ɐݒ肷��
        _centerPosition.x = _tileTable.GetLength(0) / 2 * _tileSize;
        // Y���W�̃Z���^�[�|�W�V�������s�̌��̔����̈ʒu�ɐݒ肷��
        _centerPosition.y = _tileTable.GetLength(1) / 2 * _tileSize;
        
        for (int y = 0; y < _tileTable.GetLength(1); y++)
        {
            for (int x = 0; x < _tileTable.GetLength(0); x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                // ground��~���l�߂�
                GameObject ground = Instantiate(_prefabs[(int)TILE_TYPE.GROUND]);
                ground.transform.position = GetScreenPositionFromTileTable(position);

                // x,y���W��Tile�̎�ނ��擾
                TILE_TYPE tileType = _tileTable[x, y];
                // �I�u�W�F�N�g�𐶐����A���������I�u�W�F�N�g�̈ʒu���C������
                GameObject obj = Instantiate(_prefabs[(int)tileType]);
                obj.transform.position = GetScreenPositionFromTileTable(position);

                // TILETYPE��player�̏ꍇ
                if(tileType == TILE_TYPE.PLAYER || tileType == TILE_TYPE.PLAYER_ON_POINT)
                {
                    // PlayerManager���擾
                    _player = obj.GetComponent<PlayerManager>();
                    // �v���C���[�̈ʒu��o�^
                    _moveObjPositionOnTile.Add(obj, position);
                }
                // TILE_TYPE��block��������BLOCK_ON_TYLE�̏ꍇ
                if (tileType == TILE_TYPE.BLOCK || tileType == TILE_TYPE.BLOCK_ON_POINT)
                {
                    // �u���b�N�̐����J�E���g����
                    _blockCount++;

                    // �u���b�N�̈ʒu��o�^
                    _moveObjPositionOnTile.Add(obj, position);
                }
            }
        }
    }

    // �X�e�[�W�̒��S�n���v�Z����
    public Vector2 GetScreenPositionFromTileTable(Vector2Int position)
    {
        return new Vector2((position.x * _tileSize) - _centerPosition.x, 
            -((position.y * _tileSize) - _centerPosition.y));
    }

    // �i�݂����������ǂ�������
    public bool IsWall(Vector2Int position)
    {
        // ���ɓ��������������ǂ�������
        if(_tileTable[position.x,position.y] == TILE_TYPE.WALL)
        {
            return true;// �ǂ�����t���O��true�ɂ���
        }
        return false;// �ǂ�����t���O��false�ɂ���
    }

    // �i�݂����������u���b�N��������
    public bool IsBlock(Vector2Int position)
    {
        // ���ɓ��������������u���b�N��������
        if (_tileTable[position.x, position.y] == TILE_TYPE.BLOCK�@||
            _tileTable[position.x, position.y] == TILE_TYPE.BLOCK_ON_POINT)
        {
            return true;// �u���b�N������t���O��true�ɂ���
        }
        return false;// �u���b�N������t���O��false�ɂ���
    }

    // �Ԃ�����Block���擾����֐�
    GameObject GetBlockObjectAt(Vector2Int position)
    {
        // Dictionary�̐����𗘗p
        // pair�ɂ̓L�[(Obj)��value(�ʒu)�������Ă���
        foreach(KeyValuePair<GameObject,Vector2Int>pair in _moveObjPositionOnTile)
        {
            // �ʒu��񂪓����Ȃ�
            if (pair.Value == position)
            {
                // �ڂ̑O�ɂ���I�u�W�F�N�g��Ԃ�
                return pair.Key;
            }
        }
        return null;
    }
    // Block���ړ�������
    public void UpdateBlockPosition(Vector2Int currentBlockPosition, Vector2Int nextBlockPosition)
    {
        // position���瓮���������u���b�N���擾
        GameObject block = GetBlockObjectAt(currentBlockPosition);
        // Block��V�����ʒu�ɒu��������
        block.transform.position = GetScreenPositionFromTileTable(nextBlockPosition);
        // 
        _moveObjPositionOnTile[block] = nextBlockPosition;

        // tileTable�̍X�V
        // block��u�����ꏊ��
        if(_tileTable[nextBlockPosition.x, nextBlockPosition.y] == TILE_TYPE.BLOCK_POINT)
        {
            // BLOCK_POINT�Ȃ�BLOCK_ON_POINT�ɂ���
            _tileTable[nextBlockPosition.x, nextBlockPosition.y] = TILE_TYPE.BLOCK_ON_POINT;
        }
        else
        {
            // ���Ƀu���b�N���u�����ꏊ��Block�Ƃ���
            _tileTable[nextBlockPosition.x, nextBlockPosition.y] = TILE_TYPE.BLOCK;
        }

        

    }

    public void UpdateTileTableForPlayer(Vector2Int currentPosition, Vector2Int nextPosition)
    {
        // tileTable�̍X�V
        // �v���C���[���u���b�N�̏�ɏ������
        if(_tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.BLOCK_POINT ||
            _tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.BLOCK_ON_POINT)
        {
            // ����Player���u�����ꏊ��PLAYER_ON_POINT�Ƃ���
            _tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.PLAYER_ON_POINT;
        }
        else
        {
            // ����Player���u�����ꏊ��PLAYER�Ƃ���
            _tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.PLAYER;
        }

        // �v���C���[���ړ������ہA�v���C���[��PLAYER_ON_POINT��ɂ�����
        if(_tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.PLAYER_ON_POINT)
        {
            // BLOCK_POINT�ɖ߂�
            _tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.BLOCK_POINT;
        }
        else
        {
            // GROUND�ɖ߂�
            _tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.GROUND;
        }
    }

    // block�̐���BLOCK_ON_POINT�̐�����v����
    public bool IsAllClear()
    {
        // clearCount�̏�����
        int clearCount = 0;

        // BLOCK_ON_POINT��y��tileTable�̍ŏI�n�_�܂ŒT��
        for (int y = 0; y < _tileTable.GetLength(1); y++)
        {
            // BLOCK_ON_POINT��x��tileTable�̍ŏI�n�_�܂ŒT��
            for (int x = 0; x < _tileTable.GetLength(0); x++)
            {
                // Block��bliockPoint�̏�ɏ������
                if (_tileTable[x,y] == TILE_TYPE.BLOCK_ON_POINT)
                {
                    // clearCount�����߂�
                    clearCount++;
                }
            }
        }

        // clearCount��blockCount�Ɠ����ɂȂ�����IsAllClear��true�ɂ���
        if(_blockCount == clearCount)
        {
            return true;
        }
        return false;
    }
}
