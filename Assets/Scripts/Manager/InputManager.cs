

using System;
using UnityEngine;

enum MoveDirection
{
    Up,
    Down,
    Left,
    Right,
    back,
    forward,
}

public class InputManager :MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance
    {
        get {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }


    void Update()
    {
        AdjustCameraUpdate();
        UpdateChooseBillboard();
        AdjustTableUpdate();
        FreeModeCameraUpdate();
    }

    public const float m_MouseMoveSpeed = 1000f;
    public const float m_CameraMoveSpeed = 10f;
    #region AdjustCamera

    private void AdjustCameraUpdate()
    {
        if (FunctionManager.Instance.CurState == FunctionState.ConfigCamera)
        {
            //获取控制的方向， 上下左右，   
            float KeyVertical = Input.GetAxis("Vertical");
            float KeyHorizontal = Input.GetAxis("Horizontal");

            if (KeyVertical < 0f)
            {
                MoveCamera(MoveDirection.Down);
            }
            else if (KeyVertical > 0f)
            {
                MoveCamera(MoveDirection.Up);
            }
            if (KeyHorizontal > 0f)
            {
                MoveCamera(MoveDirection.Right);
            }
            else if (KeyHorizontal < 0f)
            {
                MoveCamera(MoveDirection.Left);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                MoveCamera(MoveDirection.forward);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                MoveCamera(MoveDirection.back);
            }

            if (Input.GetMouseButton(1))
            {
                float m_MouseSpeed = 1f;

                Debug.Log("roat: " + Input.GetAxis("Mouse X") + " " + Input.GetAxis("Mouse Y"));

                //right 
                float mouseX = Input.GetAxis("Mouse X") * m_MouseSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * m_MouseSpeed;
                // 设置照相机和Player的旋转角度，X,Y值需要更具情况变化位置
                //camera.transform.localRotation = camera.transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
                //transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
                RotateCamera(mouseX, mouseY);
            }

        }
    }

    private void MoveCamera(MoveDirection dir)
    {
        GameObject camera = CameraManager.Instance.GetCamera();
        if(camera != null)
        {
            float speed = m_CameraMoveSpeed;
            //根据dir设置位置
            switch (dir)
            {
                case MoveDirection.Up:
                    camera.gameObject.transform.localPosition += camera.gameObject.transform.up * speed;//Vector3.up * speed;
                    break;
                case MoveDirection.Down:
                    camera.gameObject.transform.localPosition += -camera.gameObject.transform.up * speed;//Vector3.down * speed;
                    break;
                case MoveDirection.Left:
                    camera.gameObject.transform.localPosition += -camera.gameObject.transform.right * speed;
                    break;
                case MoveDirection.Right:
                    camera.gameObject.transform.localPosition += camera.gameObject.transform.right * speed;
                    break;
                case MoveDirection.back:
                    camera.gameObject.transform.localPosition += -camera.gameObject.transform.forward * speed;
                    break;
                case MoveDirection.forward:
                    camera.gameObject.transform.localPosition += camera.gameObject.transform.forward * speed;
                    break;
            }
        }
    }

    private void RotateCamera(float mouseX, float mouseY)
    {
        GameObject camera = CameraManager.Instance.GetCamera();
        if (camera != null)
        {
            //camera.gameObject.transform.rotation *= Quaternion.Euler(mouseY, -mouseX, 0);
            Vector3 rotation = camera.gameObject.transform.localEulerAngles;
            rotation.x -= mouseY;
            rotation.y += mouseX;
            camera.gameObject.transform.localEulerAngles = rotation;
        }
          
    }

    #endregion

    #region ChooseBillard

    void UpdateChooseBillboard()
    {
        if (FunctionManager.Instance.CurState == FunctionState.ChooseBillbard)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = CameraManager.Instance.GetCamera().GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Billboard")))
                {
                    //ColorPickerManager.Instance.PickColor(hit.transform.GetComponent<Renderer>());
                    OpenFileManager.Instance.ChooseFileFor(hit.transform.GetComponent<Renderer>());
                    FunctionManager.Instance.EndChooseBillard();
                }
            }

        }
    }

    #endregion

    #region AdjustTable

    private void AdjustTableUpdate()
    {
        if (FunctionManager.Instance.CurState == FunctionState.ConfigTable)
        {
            //获取控制的方向， 上下左右，   
            float KeyVertical = Input.GetAxis("Vertical");
            float KeyHorizontal = Input.GetAxis("Horizontal");

            if (KeyVertical < 0f)
            {
                MoveTable(MoveDirection.Down);
            }
            else if (KeyVertical > 0f)
            {
                MoveTable(MoveDirection.Up);
            }
            if (KeyHorizontal > 0f)
            {
                MoveTable(MoveDirection.Right);
            }
            else if (KeyHorizontal < 0f)
            {
                MoveTable(MoveDirection.Left);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                MoveTable(MoveDirection.forward);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                MoveTable(MoveDirection.back);
            }


            if (Input.GetMouseButton(1))
            {
                float m_MouseSpeed = 1f;

                Debug.Log("roat: " + Input.GetAxis("Mouse X") + " " + Input.GetAxis("Mouse Y"));

                //right 
                float mouseX = Input.GetAxis("Mouse X") * m_MouseSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * m_MouseSpeed;
                RotateTable(mouseX, mouseY);
            }

            if (Input.GetMouseButton(2))
            {
                float m_MouseSpeed = 1f;

                Debug.Log("roat: " + Input.GetAxis("Mouse X") + " " + Input.GetAxis("Mouse Y"));

                //right 
                float mouseX = Input.GetAxis("Mouse X") * m_MouseSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * m_MouseSpeed;
                RotateTableAroundCenter(mouseX, mouseY);
            }

        }
    }

    private void MoveTable(MoveDirection dir)
    {
        GameObject table = ResourceManager.Instance.GetTable();
        if (table != null)
        {
            float speed = 0.01f;//m_CameraMoveSpeed;
            //根据dir设置位置
            switch (dir)
            {
                case MoveDirection.Up:
                    table.gameObject.transform.localPosition += Vector3.up * speed;
                    break;
                case MoveDirection.Down:
                    table.gameObject.transform.localPosition += Vector3.down * speed;
                    break;
                case MoveDirection.Left:
                    table.gameObject.transform.localPosition += Vector3.left * speed;
                    break;
                case MoveDirection.Right:
                    table.gameObject.transform.localPosition += Vector3.right * speed;
                    break;
                case MoveDirection.back:
                    table.gameObject.transform.localPosition += Vector3.back * speed;
                    break;
                case MoveDirection.forward:
                    table.gameObject.transform.localPosition += Vector3.forward * speed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dir", dir, null);
            }
        }
    }

    private void RotateTable(float mouseX, float mouseY)
    {
        GameObject table = ResourceManager.Instance.GetTable();
        if (table != null)
        {
            //camera.gameObject.transform.rotation *= Quaternion.Euler(mouseY, -mouseX, 0);

            Vector3 rotation = table.gameObject.transform.localEulerAngles;
            //rotation.x += mouseY;
            //rotation.y += mouseX;
            //table.gameObject.transform.localEulerAngles = rotation;

            //
            //
            if (Mathf.Abs(mouseX) >  Mathf.Abs(mouseY))
            {
                table.gameObject.transform.RotateAround(table.transform.localPosition, table.transform.forward, mouseX);
                //table.gameObject.transform.RotateAround(table.transform.localPosition, Vector3.up, -mouseX);
            }
            else
            {
                table.gameObject.transform.RotateAround(table.transform.localPosition, table.transform.right, mouseY);
                //table.gameObject.transform.RotateAround(table.transform.localPosition, Vector3.left, -mouseY);
            }
            
            Debug.Log("final rotation :"  + mouseY + " " + mouseX +  " " + rotation + " euler:" + table.gameObject.transform.localEulerAngles);
        }
    }

    private void RotateTableAroundCenter(float mouseX, float mouseY)
    {
        GameObject table = ResourceManager.Instance.GetTable();
        if (table != null)
        {

            Vector3 rotation = table.gameObject.transform.localEulerAngles;

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                table.gameObject.transform.RotateAround(table.transform.localPosition, table.transform.up, mouseX);
            }

            Debug.Log("final rotation Ceneter:" + mouseY + " " + mouseX + " " + rotation + " euler:" + table.gameObject.transform.localEulerAngles);
        }
    }

    #endregion

    #region FreeMode

    private void FreeModeCameraUpdate()
    {
        if (FunctionManager.Instance.CurState == FunctionState.FreeCamera)
        {
            //获取控制的方向， 上下左右，   
            float KeyVertical = Input.GetAxis("Vertical");
            float KeyHorizontal = Input.GetAxis("Horizontal");

            if (KeyVertical < 0f)
            {
                MoveFreemodeCamera(MoveDirection.Down);
            }
            else if (KeyVertical > 0f)
            {
                MoveFreemodeCamera(MoveDirection.Up);
            }
            if (KeyHorizontal > 0f)
            {
                MoveFreemodeCamera(MoveDirection.Right);
            }
            else if (KeyHorizontal < 0f)
            {
                MoveFreemodeCamera(MoveDirection.Left);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                MoveFreemodeCamera(MoveDirection.forward);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                MoveFreemodeCamera(MoveDirection.back);
            }

            if (Input.GetMouseButton(1))
            {
                float m_MouseSpeed = 1f;

                //Debug.Log("roat: " + Input.GetAxis("Mouse X") + " " + Input.GetAxis("Mouse Y"));

                //right 
                float mouseX = Input.GetAxis("Mouse X") * m_MouseSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * m_MouseSpeed;
                RotateFreemodeCamera(mouseX, mouseY);
            }


        }
    }

    private void MoveFreemodeCamera(MoveDirection dir)
    {
        GameObject camera = CameraManager.Instance.GetCamera();
        if (camera != null)
        {
            float speed = m_CameraMoveSpeed;
            //根据dir设置位置
            switch (dir)
            {
                case MoveDirection.Up:
                    camera.gameObject.transform.localPosition += camera.gameObject.transform.up * speed;//Vector3.up * speed;
                    break;
                case MoveDirection.Down:
                    camera.gameObject.transform.localPosition += -camera.gameObject.transform.up * speed;//Vector3.down * speed;
                    break;
                case MoveDirection.Left:
                    camera.gameObject.transform.localPosition += -camera.gameObject.transform.right * speed;//  Vector3.left * speed;
                    break;
                case MoveDirection.Right:
                    camera.gameObject.transform.localPosition += camera.gameObject.transform.right * speed;//Vector3.right * speed;
                    break;
                case MoveDirection.back:
                    camera.gameObject.transform.localPosition += -camera.gameObject.transform.forward * speed;
                    break;
                case MoveDirection.forward:
                    camera.gameObject.transform.localPosition += camera.gameObject.transform.forward * speed;
                    break;
            }
        }
    }

    private void RotateFreemodeCamera(float mouseX, float mouseY)
    {
        GameObject camera = CameraManager.Instance.GetCamera();
        if (camera != null)
        {
            //camera.gameObject.transform.rotation *= Quaternion.Euler(mouseY, -mouseX, 0);
            Vector3 rotation = camera.gameObject.transform.localEulerAngles;
            rotation.x -= mouseY;
            rotation.y += mouseX;
            camera.gameObject.transform.localEulerAngles = rotation;
        }
    }

    #endregion

}
