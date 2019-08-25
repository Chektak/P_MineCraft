using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Block
{
    public Type type;
    public bool visible;
    public GameObject obj;

    public enum Type { Snow, Grass, Dirt, Gold, Diamond}
    public Block(Type t, bool v, GameObject blockObj)
    {
        type = t;
        visible = v;
        obj = blockObj;
    }
}
public class MapGenerator : MonoBehaviour
{
    [Header("블록")]
    public GameObject b_DirtPrefab;
    public GameObject b_GrassPrefab;
    public GameObject b_SandPrefab;
    public GameObject b_GoldPrefab;
    public GameObject b_DiaPrefab;
    public GameObject b_SnowPrefab;

    [Header("생성할 맵과 관련한 정보")]
    public static int width_x = 50;
    public static int width_z = 50;
    public static int height = 125;

    [Header("x,z축 완만함값 (파장에서의 주기)")]
    public int waveLength; //완만함값 (파장에서의 주기)
    [Header("y축 완만함값 (파장에서의 진폭)")]
    public float amplitude; //최대 높이값 (파장에서의 진폭)
    [Header("땅 전체를 얼마나 높일지")]
    public int groundHeightOffset = 0; //땅의 
    private int seed = 0;

    public Block[,,] worldBlocks = new Block[width_x, height, width_z];
    void Start()
    {
        StartCoroutine(InitGame());
    }
    

    IEnumerator InitGame() {
        //맵만들기
        yield return StartCoroutine(MapInit()); 

        //구름만들기

        //동굴만들기

    }
   
    IEnumerator MapInit() {
        seed = (int)Random.Range(0, 0);

        for (int x = 0; x < width_x; x++)
        {
            for (int z = 0; z < width_z; z++)
            {
               
                Vector3 pos = new Vector3(x, 0, z);

                float xCoord = (pos.x + seed) / waveLength;
                float zCoord = (pos.z + seed) / waveLength;
                pos.y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * amplitude + groundHeightOffset);
                
                CreateBlock(pos, true);
                while (pos.y > 0)
                {
                    //지면 아래의 보이지 않는 블럭들을 배열에 넣는다.
                    //실제로 객체는 만들어지지 않는다.
                    pos.y--;
                    CreateBlock(pos, false);
                }
            }
        }
        yield return null;
    }

    void CreateBlock(Vector3 blockpos, bool visual) {
        
        if (blockpos.y > 22 + groundHeightOffset)//눈블럭
        {
            if (visual)
            {
                GameObject blockObj = (GameObject)Instantiate(b_SnowPrefab, blockpos, Quaternion.identity);
                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Snow, visual, blockObj);
            }
            else
            {
                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Snow, visual, null);
            }
        }
        else if (blockpos.y > 15 + groundHeightOffset)//잔디블럭
        {
            if (visual)
            {
                GameObject blockObj = (GameObject)Instantiate(b_GrassPrefab, blockpos, Quaternion.identity);
                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Grass, visual, blockObj);
            }
            else
            {
                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Grass, visual, null);
            }
        }
        else if (blockpos.y > 0 + groundHeightOffset)
        {
            if (visual)//흙블럭
            {
                GameObject blockObj = (GameObject)Instantiate(b_DirtPrefab, blockpos, Quaternion.identity);
                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Dirt, visual, blockObj);
            }
            else
            {
                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Dirt, visual, null);
            }
        }

        // 0~7층에는 자원이 랜덤으로 스폰되게 한다
        if (blockpos.y > 0 && blockpos.y < 7 && Random.Range(0, 100) < 3)
        {
            
            if (visual)
            {
                GameObject blockObj = (GameObject)Instantiate(b_GoldPrefab, blockpos, Quaternion.identity);
                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Gold, visual, blockObj);
            }
            else
            {
                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Gold, visual, null);
            }
        }

        if (blockpos.y == 0)//기반암 생성
        {
            GameObject blockObj = (GameObject)Instantiate(b_DiaPrefab, blockpos, Quaternion.identity);
            worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Diamond, true, blockObj);
        }

        
    }
}
