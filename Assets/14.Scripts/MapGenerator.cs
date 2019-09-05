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
    public static  Block.Type PosYWithBlockType(int y) {
        int groundHeightOffset = GameManager.Instance.mapGenerator.groundHeightOffset;
        if (y > 22 + groundHeightOffset)//눈블럭
        {
            return Block.Type.Snow;
        }
        else if (y > 15 + groundHeightOffset)//잔디블럭
        {
            return Block.Type.Grass;
        }
        else if (y > 0 + groundHeightOffset)
        {
            return Block.Type.Dirt;
        }

        // 0~7층에는 자원이 랜덤으로 스폰되게 한다
        if (y > 0 && y < 7 && Random.Range(0, 100) < 3)
        {
            return Block.Type.Gold;
        }

        if (y == 0)//기반암 생성
        {
            return Block.Type.Diamond;
        }

        return 0;
    }
}
public class MapGenerator : MonoBehaviour
{
    public Block.Type typeViewer;
    [Header("블록의 타입 번호에 따라 배열에 프리팹을 부여")]
    public GameObject[] blockPrefabs;
    //public GameObject b_DirtPrefab;
    //public GameObject b_GrassPrefab;
    //public GameObject b_SandPrefab;
    //public GameObject b_GoldPrefab;
    //public GameObject b_DiaPrefab;
    //public GameObject b_SnowPrefab;

    [Header("생성할 맵과 관련한 정보")]
    public static int width_x = 50;
    public static int width_z = 50;
    public static int height = 50;

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

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;

            //화면의 정가운데 위치에서 ray변수를 만든다.
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);

            if (Physics.Raycast(ray, out hit,6f)) {
                Vector3 blockPos = hit.transform.position;

                //맨 아래 블록은 소멸되지 않게 한다.
                if (blockPos.y <= 0) return;

                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
                Destroy(hit.collider.gameObject);

                //자기 자신을 뺀 이웃들을 인스턴스화한다
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        for (int z = -1; z <= 1; z++) {

                            if (blockPos.x + x < 0 || blockPos.x + x >= width_x) continue;
                            if (blockPos.y + y < 0 || blockPos.y + y >= height) continue;
                            if (blockPos.z + z < 0 || blockPos.z + z >= width_z) continue;

                            Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                            DrawBlock(neighbour);
                        }
                    }
                }
            }
        }
        if (Input.mouseScrollDelta.y > 0)
            UIManager.Instance.ItemBar_SelectRightSlot();
        else if (Input.mouseScrollDelta.y < 0)
            UIManager.Instance.ItemBar_SelectLeftSlot();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            UIManager.Instance.ItemBar_SelectNumSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            UIManager.Instance.ItemBar_SelectNumSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            UIManager.Instance.ItemBar_SelectNumSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            UIManager.Instance.ItemBar_SelectNumSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            UIManager.Instance.ItemBar_SelectNumSlot(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            UIManager.Instance.ItemBar_SelectNumSlot(6);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            UIManager.Instance.ItemBar_SelectNumSlot(7);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            UIManager.Instance.ItemBar_SelectNumSlot(8);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            UIManager.Instance.ItemBar_SelectNumSlot(9);
    }

    void DrawBlock(Vector3 blockPos) {
        Block tempBlock = worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];
        if (tempBlock == null) return;

        if (!tempBlock.visible) {
            GameObject newBlock = null;
            tempBlock.visible = true;
            newBlock = Instantiate(blockPrefabs[(int)tempBlock.type], blockPos, Quaternion.identity);

            if (newBlock != null)
                tempBlock.obj = newBlock;
        }
    }
    IEnumerator InitGame() {
        //맵만들기
        yield return StartCoroutine(MapInit());

        //구름만들기
        yield return StartCoroutine(CreateCloud(120, 50, 80));
        //동굴만들기
        yield return StartCoroutine(CreateMines(2, 200));
        Debug.Log("동굴만들기");
    }

    IEnumerator CreateCloud(int Num, int unitCloudSize, int Cloudheight) {
        for (int i = 0; i < Num; i++) {
            //월드 맵 넓이로 랜덤하게..
            int xPos = Random.Range(0, width_x + 200);
            int zPos = Random.Range(0, width_z + 200);

            //각 구름의 사이즈
            for (int j = 0; j < unitCloudSize; j++) {
                Vector3 blockPos = new Vector3(xPos, Cloudheight, zPos);
                Instantiate(blockPrefabs[(int)Block.Type.Snow], blockPos, Quaternion.identity);
                xPos += Random.Range(-1, 2);
                zPos += Random.Range(-1, 2);
            }
        }
        yield return null;
    }
    IEnumerator CreateMines(int Num, int UnitMineSize) {
        int HoleSize = 2;//구멍의 크기

        //광산의 갯수
        for (int i = 0; i < Num; i++) {
            //월드맵안에서의 랜덤한 위치를 정한다.
            int xPos = Random.Range(HoleSize, width_x - HoleSize);
            int yPos = Random.Range(HoleSize, 15);//홀사이즈부터 15층까지(15층은 낮은지면이다)
            int zPos = Random.Range(HoleSize, width_z - HoleSize);

            //각 광산의 사이즈 // 0-1
            for (int j = 0; j < UnitMineSize; j++) {
                for (int x = -HoleSize; x <= HoleSize; x++) {
                    for (int y = -HoleSize; y <= HoleSize; y++) {
                        for (int z = -HoleSize; z <= HoleSize; z++) {
                            Vector3 blockPos = new Vector3(xPos + x, yPos + y, zPos + z);

                            if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] != null) {
                                if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Diamond ||
                                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Gold)
                                {
                                    //현재 타입이 골드이거나 다이아면 뚫지 않는다.
                                    continue;
                                }
                                else
                                {
                                    Destroy(worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].obj);
                                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]=null;
                                }

                            }

                            
                        }//UnitMapSize for 루프
                        

                    }
                }
                while (true)
                {
                    xPos += Random.Range(-1, 2); //-1~1사이의 랜덤값 반환
                    if (xPos < HoleSize || xPos >= width_x - HoleSize) continue;
                    else break;
                }
                while (true)
                {
                    zPos += Random.Range(-1, 2);
                    if (zPos < HoleSize || zPos >= width_z - HoleSize) continue;
                    else break;
                }
                while (true)
                {
                    yPos += Random.Range(-1, 2);
                    if (yPos < HoleSize || yPos >= height - HoleSize) continue;
                    else break;
                }
            }
            for (int z = 1; z < width_z - 1; z++)
            {
                for (int x = 1; x < width_x - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        if (worldBlocks[x, y, z] == null)
                        {
                            for (int x1 = -1; x1 <= 1; x1++)
                            {
                                for (int y1 = -1; y1 <= 1; y1++)
                                {
                                    for (int z1 = -1; z1 <= 1; z1++)
                                    {
                                        if (!(x1 == 0 && y1 == 0 && z1 == 0))
                                        {
                                            Vector3 neighbour = new Vector3(x + x1, y + y1, z + z1);
                                            
                                            DrawBlock(neighbour);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        yield return null;
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
        Block.Type blockType = Block.PosYWithBlockType((int)blockpos.y);
        if (visual)
        {
            GameObject blockObj = (GameObject)Instantiate(blockPrefabs[(int)blockType], blockpos, Quaternion.identity);
            worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(blockType, visual, blockObj);
        }
        else
        {
            worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(blockType, visual, null);
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class Block
//{
//    public Type type;
//    public bool visible;
//    public GameObject obj;

//    public enum Type { Snow, Grass, Dirt, Gold, Diamond}
//    public Block(Type t, bool v, GameObject blockObj)
//    {
//        type = t;
//        visible = v;
//        obj = blockObj;
//    }
//}
//public class MapGenerator : MonoBehaviour
//{
//    [Header("블록")]
//    public GameObject b_DirtPrefab;
//    public GameObject b_GrassPrefab;
//    public GameObject b_SandPrefab;
//    public GameObject b_GoldPrefab;
//    public GameObject b_DiaPrefab;
//    public GameObject b_SnowPrefab;

//    [Header("생성할 맵과 관련한 정보")]
//    public static int width_x = 50;
//    public static int width_z = 50;
//    public static int height = 50;

//    [Header("x,z축 완만함값 (파장에서의 주기)")]
//    public int waveLength; //완만함값 (파장에서의 주기)
//    [Header("y축 완만함값 (파장에서의 진폭)")]
//    public float amplitude; //최대 높이값 (파장에서의 진폭)
//    [Header("땅 전체를 얼마나 높일지")]
//    public int groundHeightOffset = 0; //땅의 
//    private int seed = 0;

//    public Block[,,] worldBlocks = new Block[width_x, height, width_z];
//    void Start()
//    {
//        StartCoroutine(InitGame());
//    }

//    void Update() {
//        if (Input.GetMouseButtonDown(1)) {
//            RaycastHit hit;

//            //화면의 정가운데 위치에서 ray변수를 만든다.
//            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

//            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);

//            if (Physics.Raycast(ray, out hit, 1000.0f)) {
//                Vector3 blockPos = hit.transform.position;

//                //맨 아래 블록은 소멸되지 않게 한다.
//                if (blockPos.y <= 0) return;

//                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
//                Destroy(hit.collider.gameObject);

//                //자기 자신을 뺀 이웃들을 인스턴스화한다
//                for (int x = -1; x <= 1; x++) {
//                    for (int y = -1; y <= 1; y++) {
//                        for (int z = -1; z <= 1; z++) {

//                            if (blockPos.x + x < 0 || blockPos.x + x >= width_x) continue;
//                            if (blockPos.y + y < 0 || blockPos.y + y >= height) continue;
//                            if (blockPos.z + z < 0 || blockPos.z + z >= width_z) continue;

//                            Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
//                            DrawBlock(neighbour);
//                        }
//                    }
//                }
//            }
//        }
//    }

//void DrawBlock(Vector3 blockPos)
//{
//    if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] == null) return;

//    if (!worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].visible)
//    {
//        GameObject newBlock = null;
//        worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].visible = true;
//        if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Snow)
//            newBlock = (GameObject)Instantiate(b_SnowPrefab, blockPos, Quaternion.identity);
//        else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Grass)
//            newBlock = (GameObject)Instantiate(b_GrassPrefab, blockPos, Quaternion.identity);
//        else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Dirt)
//            newBlock = (GameObject)Instantiate(b_DirtPrefab, blockPos, Quaternion.identity);
//        else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Gold)
//            newBlock = (GameObject)Instantiate(b_GoldPrefab, blockPos, Quaternion.identity);
//        else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Diamond)
//            newBlock = (GameObject)Instantiate(b_DiaPrefab, blockPos, Quaternion.identity);
//        else//비어있는 공간
//            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].visible = false;

//        if (newBlock != null)
//            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].obj = newBlock;
//    }
//}
//    IEnumerator InitGame() {
//        //맵만들기
//        yield return StartCoroutine(MapInit());

//        //구름만들기
//        yield return StartCoroutine(CreateCloud(120, 50, 80));
//        //동굴만들기
//        yield return StartCoroutine(CreateMines(5, 400));
//        Debug.Log("동굴만들기");
//    }

//    IEnumerator CreateCloud(int Num, int unitCloudSize, int Cloudheight) {
//        for (int i = 0; i < Num; i++) {
//            //월드 맵 넓이로 랜덤하게..
//            int xPos = Random.Range(0, width_x + 200);
//            int zPos = Random.Range(0, width_z + 200);

//            //각 구름의 사이즈
//            for (int j = 0; j < unitCloudSize; j++) {
//                Vector3 blockPos = new Vector3(xPos, Cloudheight, zPos);
//                Instantiate(b_SnowPrefab, blockPos, Quaternion.identity);
//                xPos += Random.Range(-1, 2);
//                zPos += Random.Range(-1, 2);
//            }
//        }
//        yield return null;
//    }
//    IEnumerator CreateMines(int Num, int UnitMineSize) {
//        int HoleSize = 2;//구멍의 크기

//        //광산의 갯수
//        for (int i = 0; i < Num; i++) {
//            //월드맵안에서의 랜덤한 위치를 정한다.
//            int xPos = Random.Range(HoleSize, width_x - HoleSize);
//            int yPos = Random.Range(HoleSize, 15);//홀사이즈부터 15층까지(15층은 낮은지면이다)
//            int zPos = Random.Range(HoleSize, width_z - HoleSize);

//            //각 광산의 사이즈 // 0-1
//            for (int j = 0; j < UnitMineSize; j++) {
//                for (int x = -HoleSize; x <= HoleSize; x++) {
//                    for (int y = -HoleSize; y <= HoleSize; y++) {
//                        for (int z = -HoleSize; z <= HoleSize; z++) {
//                            Vector3 blockPos = new Vector3(xPos + x, yPos + y, zPos + z);

//                            if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] != null) {
//                                if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Diamond ||
//                                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == Block.Type.Gold)
//                                {
//                                    //현재 타입이 골드이거나 다이아면 뚫지 않는다.
//                                    continue;
//                                }
//                                else
//                                {
//                                    Destroy(worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].obj);
//                                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]=null;
//                                }

//                            }


//                        }//UnitMapSize for 루프


//                    }
//                }
//                while (true)
//                {
//                    xPos += Random.Range(-1, 2); //-1~1사이의 랜덤값 반환
//                    if (xPos < HoleSize || xPos >= width_x - HoleSize) continue;
//                    else break;
//                }
//                while (true)
//                {
//                    zPos += Random.Range(-1, 2);
//                    if (zPos < HoleSize || zPos >= width_z - HoleSize) continue;
//                    else break;
//                }
//                while (true)
//                {
//                    yPos += Random.Range(-1, 2);
//                    if (yPos < HoleSize || yPos >= height - HoleSize) continue;
//                    else break;
//                }
//            }
//            for (int z = 1; z < width_z - 1; z++)
//            {
//                for (int x = 1; x < width_x - 1; x++)
//                {
//                    for (int y = 1; y < height - 1; y++)
//                    {
//                        if (worldBlocks[x, y, z] == null)
//                        {
//                            for (int x1 = -1; x1 <= 1; x1++)
//                            {
//                                for (int y1 = -1; y1 <= 1; y1++)
//                                {
//                                    for (int z1 = -1; z1 <= 1; z1++)
//                                    {
//                                        if (!(x1 == 0 && y1 == 0 && z1 == 0))
//                                        {
//                                            Vector3 neighbour = new Vector3(x + x1, y + y1, z + z1);

//                                            DrawBlock(neighbour);
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        yield return null;
//    }

//    IEnumerator MapInit() {
//        seed = (int)Random.Range(0, 0);

//        for (int x = 0; x < width_x; x++)
//        {
//            for (int z = 0; z < width_z; z++)
//            {

//                Vector3 pos = new Vector3(x, 0, z);

//                float xCoord = (pos.x + seed) / waveLength;
//                float zCoord = (pos.z + seed) / waveLength;
//                pos.y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * amplitude + groundHeightOffset);

//                CreateBlock(pos, true);
//                while (pos.y > 0)
//                {
//                    //지면 아래의 보이지 않는 블럭들을 배열에 넣는다.
//                    //실제로 객체는 만들어지지 않는다.
//                    pos.y--;
//                    CreateBlock(pos, false);
//                }
//            }
//        }
//        yield return null;
//    }

//    void CreateBlock(Vector3 blockpos, bool visual) {

//        if (blockpos.y > 22 + groundHeightOffset)//눈블럭
//        {
//            if (visual)
//            {
//                GameObject blockObj = (GameObject)Instantiate(b_SnowPrefab, blockpos, Quaternion.identity);
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Snow, visual, blockObj);
//            }
//            else
//            {
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Snow, visual, null);
//            }
//        }
//        else if (blockpos.y > 15 + groundHeightOffset)//잔디블럭
//        {
//            if (visual)
//            {
//                GameObject blockObj = (GameObject)Instantiate(b_GrassPrefab, blockpos, Quaternion.identity);
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Grass, visual, blockObj);
//            }
//            else
//            {
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Grass, visual, null);
//            }
//        }
//        else if (blockpos.y > 0 + groundHeightOffset)
//        {
//            if (visual)//흙블럭
//            {
//                GameObject blockObj = (GameObject)Instantiate(b_DirtPrefab, blockpos, Quaternion.identity);
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Dirt, visual, blockObj);
//            }
//            else
//            {
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Dirt, visual, null);
//            }
//        }

//        // 0~7층에는 자원이 랜덤으로 스폰되게 한다
//        if (blockpos.y > 0 && blockpos.y < 7 && Random.Range(0, 100) < 3)
//        {
//            //if (worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z].obj != null) {
//            //    Destroy(worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z].obj);
//            //}
//            //GameObject blockObj = (GameObject)Instantiate(b_GoldPrefab, blockpos, Quaternion.identity);
//            //worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Gold, true, null);
//            if (visual)
//            {
//                GameObject blockObj = (GameObject)Instantiate(b_GoldPrefab, blockpos, Quaternion.identity);
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Gold, visual, blockObj);
//            }
//            else
//            {
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Gold, visual, null);
//            }
//        }

//        if (blockpos.y == 0)//기반암 생성
//        {
//            if (visual)
//            {
//                GameObject blockObj = (GameObject)Instantiate(b_DiaPrefab, blockpos, Quaternion.identity);
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Diamond, visual, blockObj);
//            }
//            else
//            {
//                worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Diamond, visual, null);
//            }
//            //GameObject blockObj = (GameObject)Instantiate(b_DiaPrefab, blockpos, Quaternion.identity);
//            //worldBlocks[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(Block.Type.Diamond, true, blockObj);
//        }


//    }
//}
