using UnityEngine;
using UnityEngine.Tilemaps;

//一个建筑系统的脚本
public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] private Tilemap mainTilemap; // 地图的Tilemap组件
    [SerializeField] private TileBase whiteTile; // 白色方块的TileBase

    [Space, Header("预制体")]
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;

    private PlaceableObject objectToPlace; // 当前要放置的对象

    private void Awake()
    {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>(); // 获取网格组件
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            InitializeWithObject(prefab1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InitializeWithObject(prefab2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            InitializeWithObject(prefab3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            InitializeWithObject(prefab4);
        }

        //放置测试
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanBePlaced(objectToPlace)) // 检查物体是否可以放置
            {
                objectToPlace.Place(); // 放置物体
                Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition()); // 将世界坐标转换为格子坐标
                TakeArea(start, objectToPlace.Size); // 将物体所占据的区域填充为白色瓦片
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(objectToPlace.gameObject); // 按下 Esc 键，销毁物体
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            objectToPlace.Rotate();
        }
    }

    //获取一个区域内的瓦片信息数组
    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;
        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos); // 获取指定位置上的瓦片
            counter++;
        }
        return array;
    }

    //检查物体是否可以放置在指定位置
    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(placeableObject.GetStartPosition()); // 将世界坐标转换为格子坐标
        area.size = placeableObject.Size; // 获取物体所占据的格子大小
        TileBase[] baseArray = GetTilesBlock(area, mainTilemap); // 获取该区域内的瓦片数组
        foreach (var b in baseArray)
        {
            if (b == whiteTile) // 如果有白色瓦片，表示物体无法放置
            {
                return false;
            }
        }
        return true; // 没有白色瓦片，可以放置物体
    }

    //在指定区域填充为白色瓦片
    public void TakeArea(Vector3Int start, Vector3Int size)
    {
        mainTilemap.BoxFill(start, whiteTile, start.x, start.y, start.x + size.x, start.y + size.y); // 将指定区域填充为白色瓦片
    }



    // 工具方法：将鼠标位置转换为世界坐标系下的位置
    public static Vector3 GetMouseWorldPosition()
    {
        // 从相机发出一条射线，将鼠标位置转换为世界坐标系下的位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 如果射线碰撞到物体，则返回碰撞点的世界坐标
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return raycastHit.point;
        }
        else // 否则返回零向量
        {
            return Vector3.zero;
        }
    }


    // 将坐标对齐到网格上
    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position); // 将世界坐标转换为网格单元坐标
        position = grid.GetCellCenterWorld(cellPos); // 获取网格单元中心点的世界坐标
        return position;
    }

    //初始化放置物体
    public void InitializeWithObject(GameObject prefab)
    {
        // 将物体初始位置设为网格对齐的原点
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);

        // 在初始位置实例化物体
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        // 获取PlaceableObject组件并添加ObjectDrag组件
        objectToPlace = obj.GetComponent<PlaceableObject>(); // 获取可放置物体组件
        obj.AddComponent<ObjectDrag>(); // 添加拖拽组件
    }

}
