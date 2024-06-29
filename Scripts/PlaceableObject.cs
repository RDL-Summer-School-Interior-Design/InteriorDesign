using UnityEngine;

//可放置对象脚本
public class PlaceableObject : MonoBehaviour
{
    // 是否已经放置
    public bool Placed { get; private set; }

    // 物体占据的格子数
    public Vector3Int Size { get; private set; }

    // 物体碰撞器的四个顶点（本地坐标系）
    private Vector3[] Vertices;

    private void Start()
    {
        // 获取物体碰撞器的四个顶点
        GetColliderVertexPositionsLocal();
        // 计算物体占据的格子数
        CalculateSizeInCells();
    }

    // 获取物体碰撞器的四个顶点
    private void GetColliderVertexPositionsLocal()
    {
        BoxCollider b = gameObject.GetComponent<BoxCollider>();
        Vertices = new Vector3[4];
        Vertices[0] = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;
        Vertices[1] = b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;
        Vertices[2] = b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
        Vertices[3] = b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
    }

    // 计算物体占据的格子数
    private void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[Vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            // 将物体顶点从本地坐标系转换到世界坐标系
            Vector3 worldPos = transform.TransformPoint(Vertices[i]);
            // 将世界坐标系中的位置转换成格子坐标系中的位置
            vertices[i] = BuildingSystem.current.gridLayout.WorldToCell(worldPos);
        }
        // 计算物体占据的格子数
        Size = new Vector3Int(
            Mathf.Abs(vertices[0].x - vertices[1].x),
            Mathf.Abs(vertices[0].y - vertices[3].y),
            1
        );
    }

    // 获取物体的起始位置（左下角的格子位置）
    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }

    // 放置物体
    public virtual void Place()
    {
        // 删除物体拖拽组件
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        Destroy(drag);

        // 标记物体已经放置
        Placed = true;

        // TODO：触发放置事件
    }

    //旋转
    public void Rotate()
    {
        transform.Rotate(eulers: new Vector3(0, 90, 0)); // 绕 Y 轴顺时针旋转 90 度

        // 交换长宽并限制高度为 1
        Size = new Vector3Int(Size.y, Size.x, 1);

        // 旋转顶点数组
        Vector3[] vertices = new Vector3[Vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vertices[(i + 1) % Vertices.Length]; // 将顶点数组顺时针旋转
        }
        Vertices = vertices; // 更新顶点数组
    }
}
