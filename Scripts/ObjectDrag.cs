using UnityEngine;

//一个物体拖拽的脚本
public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset; // 鼠标按下时物体和鼠标之间的偏移量

    // 每帧更新建筑物的位置
    private void Update()
    {
        //偏移量自己定
        Vector3 pos = BuildingSystem.GetMouseWorldPosition() + offset;
        transform.position = BuildingSystem.current.SnapCoordinateToGrid(pos);
    }

    // // 当鼠标按下时记录偏移量
    // private void OnMouseDown()
    // {
    //     offset = transform.position - BuildingSystem.GetMouseWorldPosition();
    // }

    // // 当鼠标拖动时移动物体并对齐到网格上
    // private void OnMouseDrag()
    // {
    //     Vector3 pos = BuildingSystem.GetMouseWorldPosition() + offset;
    //     transform.position = BuildingSystem.current.SnapCoordinateToGrid(pos);
    // }
}
