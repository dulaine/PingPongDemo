

using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _intance;

    public static ResourceManager Instance
    {
        get { return _intance; }
    }

    public GameObject ResourcePoolRoot;
    public GameObject TouchPointGameObject;
    public List<GameObject> UnusedTouchPoints = new List<GameObject>( );
    public List<GameObject> UsedTouchPoints = new List<GameObject>();

    public GameObject TableObj;
    public GameObject FloorObj;
    public GameObject BallObj;

    public GameObject BillboardObj;
    public GameObject StadiumObj;
    void Awake()
    {
        _intance = this;
    }

    public GameObject GetTable()
    {
        return TableObj;
    }

    public GameObject GetFloor()
    {
        return FloorObj;
    }

    public GameObject GetBall()
    {
        return BallObj;
    }

    public GameObject GetBillboard()
    {
        return BillboardObj;
    }

    public GameObject GetStadium()
    {
        return StadiumObj;
    }



    #region TouchPoints

    public GameObject GetTouchPointGameObject()
    {
        GameObject obj;
        if (UnusedTouchPoints.Count > 0)
        {
            obj = UnusedTouchPoints[UnusedTouchPoints.Count - 1];
            UnusedTouchPoints.Remove(obj);
        }
        else
        {
            obj = Instantiate(TouchPointGameObject);
            obj.transform.SetParent(ResourcePoolRoot.transform);
        }

        UsedTouchPoints.Add(obj);
        return obj;
    }

    public void HideTouchPoints()
    {
        for (int i = 0; i < UsedTouchPoints.Count; i++)
        {
            UsedTouchPoints[i].transform.localPosition = new Vector3(100000f, 0f, 0f);
            UnusedTouchPoints.Add(UsedTouchPoints[i]);
        }
        UsedTouchPoints.Clear();
    }

    #endregion
}
