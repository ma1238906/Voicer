using UnityEngine;

public class AstronautRotation : MonoBehaviour
{

    //是否被拖拽
    private bool onDrag = false;
    //旋转速度
    public float speed = 6f;
    //阻尼速度
    private float zSpeed;
    //鼠标沿水平方向拖拽的增量
    private float X;
    //鼠标沿竖直方向拖拽的增量
    //private float Y;
    //鼠标移动的距离
    private float mXY;

    //接受鼠标按下的事件
    void OnMouseDown()
    {
        X = 0f;
    }

    //鼠标拖拽时的操作
    void OnMouseDrag()
    {
        onDrag = true;
        X = -Input.GetAxis("Mouse X");
        mXY = Mathf.Sqrt(X * X);
        if (mXY == 0f)
        {
            mXY = 1f;
        }
    }

    //获取阻尼速度 
    float RiSpeed()
    {
        if (onDrag)
        {
            zSpeed = speed;
        }
        else
        {
            zSpeed = 0;
        }
        return zSpeed;
    }

    void LateUpdate()
    {
        transform.Rotate(new Vector3(0, X, 0) * RiSpeed(), Space.World);
        if (!Input.GetMouseButtonDown(0))
        {
            onDrag = false;
        }
    }
}
