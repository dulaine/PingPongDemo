

using UnityEngine;

public class MultiDisplayManager : MonoBehaviour
{
    private static MultiDisplayManager _instance;

    public static MultiDisplayManager Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        
    }

    public void Open()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
    }

    public void CLose()
    {
        //Camera2.gameObject.SetActive(false);
    }
}
