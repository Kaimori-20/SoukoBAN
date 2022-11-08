using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] TextAsset stageFile; // �X�e�[�W�̃e�L�X�g
    [SerializeField] GameObject[] prefabs; // �v���n�u�������Q�[���I�u�W�F�N�g�̊i�[

    // �z����̐����̒�`
    enum TILE_TYPE
    {
        WALL,        // ��(0)
        GROUND,      // ��(1)
        BLOCK_POINT, // �ړI�n(2)
        BLOCK,       // ��(3)
        PLAYER,      // �v���C���[(4)
        BLOCK_ON_POINT,  // BLOCK��������^�[�Q�b�g
        PLAYER_ON_POINT, // �v���C���[��������^�[�Q�b�g
    }
    // enum�Œ�`����
    TILE_TYPE[,] tileTable;
    float tileSize; 
    Vector2 centerPosition; // ��ʒ����ɐ������邽�߂̕ϐ�
    public PlayerManager player; // �v���C���[�̃X�N���v�g���擾
    int blockCount; // �u���b�N�̐�

    // GameObject�������L�[���[�h�ɂ��ʒu�����擾����
    public Dictionary<GameObject, Vector2Int> 
        moveObjPositionOnTile = new Dictionary<GameObject, Vector2Int>();

    // �^�C���̏����Ăяo��
    public void LoadTileData()
    {
        // �^�C���̏�����s���Ƃɕ���
        string[] lines = stageFile.text.Split(new[] { '\n','\r' },
            System.StringSplitOptions.RemoveEmptyEntries);

        //�X�e�[�W�̒������擾
        int columns = lines[0].Split(new[] { ',' }).Length; // ��
        int rows = lines.Length; // �s
        // colu,s��rows���炻�ꂼ���ƍs���擾
        tileTable = new TILE_TYPE[columns, rows];

        for (int y = 0; y < rows; y++)
        {
            string[] values = lines[y].Split(new[] { ',' });
            for (int x = 0; x < columns; x++)
            {
                // tileTable��int�^�ɒ����A�g���₷������
                tileTable[x, y] = (TILE_TYPE)int.Parse(values[x]);
                Debug.Log(tileTable[x, y]);
            }
        }
    }

    // tileTable���g���ă^�C���𐶐�����
    public void CreateStage()
    {
        tileSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;

        // �Z���^�[�|�W�V�����̐ݒ�
        // X���W�̃Z���^�[�|�W�V�������̌��̔����̈ʒu�ɐݒ肷��
        centerPosition.x = (tileTable.GetLength(0) / 2) * tileSize;
        // Y���W�̃Z���^�[�|�W�V�������s�̌��̔����̈ʒu�ɐݒ肷��
        centerPosition.y = (tileTable.GetLength(1) / 2) * tileSize;
        
        for (int y = 0; y < tileTable.GetLength(1); y++)
        {
            for (int x = 0; x < tileTable.GetLength(0); x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                // ground��~���l�߂�
                GameObject ground = Instantiate(prefabs[(int)TILE_TYPE.GROUND]);
                ground.transform.position = GetScreenPositionFromTileTable(position);

                // x,y���W��Tile�̎�ނ��擾
                TILE_TYPE tileType = tileTable[x, y];
                // �I�u�W�F�N�g�𐶐����A���������I�u�W�F�N�g�̈ʒu���C������
                GameObject obj = Instantiate(prefabs[(int)tileType]);
                obj.transform.position = GetScreenPositionFromTileTable(position);

                // TILETYPE��player�̏ꍇ
                if(tileType == TILE_TYPE.PLAYER)
                {
                    // PlayerManager���擾
                    player = obj.GetComponent<PlayerManager>();
                    // 
                    moveObjPositionOnTile.Add(obj, position);
                }
                // TILE_TYPE��block�̏ꍇ
                if(tileType == TILE_TYPE.BLOCK)
                {
                    // �u���b�N�̐����J�E���g����
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

    // �i�݂����������ǂ�������
    public bool IsWall(Vector2Int position)
    {
        // ���ɓ��������������ǂ�������
        if(tileTable[position.x,position.y] == TILE_TYPE.WALL)
        {
            return true;// bool��true�ɂ���
        }
        return false;// bool��false�ɂ���
    }

    // �i�݂����������u���b�N��������
    public bool IsBlock(Vector2Int position)
    {
        // ���ɓ��������������ǂ�������
        if (tileTable[position.x, position.y] == TILE_TYPE.BLOCK�@||
            tileTable[position.x, position.y] == TILE_TYPE.BLOCK_ON_POINT)
        {
            return true;// bool��true�ɂ���
        }
        return false;// bool��false�ɂ���
    }

    // �Ԃ�����Block���擾����֐�
    GameObject GetBlockObjectAt(Vector2Int position)
    {
        // Dictionary�̐����𗘗p
        // pair�ɂ̓L�[(Obj)��value(�ʒu)�������Ă���
        foreach(KeyValuePair<GameObject,Vector2Int>pair in moveObjPositionOnTile)
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
        moveObjPositionOnTile[block] = nextBlockPosition;

        // tileTable�̍X�V
        // block��u�����ꏊ��
        if(tileTable[nextBlockPosition.x, nextBlockPosition.y] == TILE_TYPE.BLOCK_POINT)
        {
            // BLOCK_POINT�Ȃ�BLOCK_ON_POINT�ɂ���
            tileTable[nextBlockPosition.x, nextBlockPosition.y] = TILE_TYPE.BLOCK_ON_POINT;
        }
        else
        {
            // ���Ƀu���b�N���u�����ꏊ��Block�Ƃ���
            tileTable[nextBlockPosition.x, nextBlockPosition.y] = TILE_TYPE.BLOCK;
        }

        

    }

    public void UpdateTileTableForPlayer(Vector2Int currentPosition, Vector2Int nextPosition)
    {
        // tileTable�̍X�V
        // �v���C���[���u���b�N�̏�ɏ������
        if(tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.BLOCK_POINT ||
            tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.BLOCK_ON_POINT)
        {
            // ����Player���u�����ꏊ��PLAYER_ON_POINT�Ƃ���
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.PLAYER_ON_POINT;
        }
        else
        {
            // ����Player���u�����ꏊ��PLAYER�Ƃ���
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.PLAYER;
        }

        // �v���C���[���ړ������ہA�v���C���[��PLAYER_ON_POINT��ɂ�����
        if(tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.PLAYER_ON_POINT)
        {
            // BLOCK_POINT�ɖ߂�
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.BLOCK_POINT;
        }
        else
        {
            // GROUND�ɖ߂�
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.GROUND;
        }
    }

    // block�̐���BLOCK_ON_POINT�̐�����v����
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
