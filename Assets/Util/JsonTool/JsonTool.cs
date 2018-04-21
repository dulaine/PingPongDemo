using UnityEngine;

public class JsonTool : MonoBehaviour
{
    /// <summary>
    /// 根据结构体解析json
    /// </summary>
    public static T RecodeJson<T>(string _json)
    {
        return JsonUtility.FromJson<T>(_json);
    }

}
