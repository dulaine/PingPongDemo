using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 场景打包
/// 所有在asset下的文件，均是未进行压缩的文件，压缩的存放在中间文件中
/// </summary>
public class CompressBundleEditor : EditorWindow
{
    //编辑器用数据
    public static List<string> Dirs = new List<string>();       //要进行压缩的路径
    public static string OutputPath = "";       //压缩文件导出路径

    static CompressBundleEditor window;
    Vector2 _ScrollPosition;
    int _NeedRemoveIndex = -1;  //当前需要删除的数据的index

    //----------编辑器脚本------------
    #region 编辑器脚本
    [MenuItem("Tools/打包/包压缩", false, 3)]
    static void Init()
    {
        window = (CompressBundleEditor)EditorWindow.GetWindow(typeof(CompressBundleEditor), false, "包压缩");     //定义一个窗口对象
        window.minSize = new Vector2(500, 300);
    }
    void OnEnable()
    {
        if (string.IsNullOrEmpty(OutputPath))
        {
            OutputPath = Application.dataPath + "/UtilMiddle/CompressBundle";       //压缩过的场景包文件的导出目录
        }
    }
    void OnGUI()
    {
        _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition);
        GUILayout.BeginVertical();
        //添加头
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 18;
        headerStyle.normal.textColor = Color.white;
        //headerStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("包压缩", headerStyle, GUILayout.Height(25));
        EditorGUILayout.Space();

        //设置要压缩的文件夹
        SetDirsPath();
        EditorGUILayout.Space();

        //设置要压缩包的存储路径
        SetOutputPath();
        EditorGUILayout.Space();

        //开始压缩
        DoCumpress();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
    /// <summary>
    /// 设置要压缩的文件夹
    /// </summary>
    void SetDirsPath()
    {
        EditorGUILayout.LabelField(new GUIContent("选择要压缩的文件夹:"), EditorStyles.boldLabel);
        if (Dirs.Count == 0)   //生成一个空的path以供选择
        {
            Dirs.Add("");
        }
        //显示所有选项
        for (int i = 0; i < Dirs.Count; i++)
        {
            var _data = Dirs[i];
            //设置路径
            GUILayout.BeginHorizontal();
            string _tempPath = Dirs[i];
            _tempPath = GUILayout.TextField(_tempPath, GUILayout.MinWidth(250));
            if (GUILayout.Button("...", GUILayout.Width(22)))
            {
                string projectFolder = Application.dataPath;
                _tempPath = EditorUtility.OpenFolderPanel("选择要压缩的文件夹", projectFolder, "");
                if (_tempPath.Length != 0)
                {
                    Dirs[i] = _tempPath.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            //“+”、“-”
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.MaxWidth(25)))
            {
                Dirs.Add("");  //添加一个空列
            }
            GUILayout.Space(10);
            if (GUILayout.Button("-", GUILayout.MaxWidth(25)))
            {
                _NeedRemoveIndex = i;
            }
            GUILayout.EndHorizontal();
        }
        //删除“-”的数据
        if (_NeedRemoveIndex != -1)
        {
            Dirs.RemoveAt(_NeedRemoveIndex);
            _NeedRemoveIndex = -1;
        }
    }

    /// <summary>
    /// 设置导出路径
    /// </summary>
    void SetOutputPath()
    {
        EditorGUILayout.LabelField("压缩文件导出路径:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        OutputPath = GUILayout.TextField(OutputPath, GUILayout.MinWidth(230));
        if (GUILayout.Button("压缩数据生成位置"))
        {
            string path = "";
            path = EditorUtility.OpenFolderPanel("选择压缩场景包的导出路径", Application.dataPath, "");
            if (path.Length != 0)
            {
                OutputPath = path;
            }
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 开始压缩打出的场景包
    /// </summary>
    void DoCumpress()
    {
        //判断是否有文件夹，没有就创建，否则直接写入会报错
        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }
        EditorGUILayout.LabelField("开始压缩打包资源:", EditorStyles.boldLabel);
        if (GUILayout.Button("压缩"))
        {
            //将所有打包出来的场景，进行压缩
            for (int i = 0; i < Dirs.Count; i++)
            {
                DirectoryInfo _dir = new DirectoryInfo(Dirs[i]);
                if (_dir.Exists)
                {
                    var _files = _dir.GetFiles("*.unity3d");
                    for (int j = 0; j < _files.Length; j++)
                    {
                        var _fileName = _files[j].FullName.Replace("\\", "/");
                        string _name = _fileName.Replace(Application.streamingAssetsPath, "");
                        string _fullPath = OutputPath + _name;
                        Debug.Log("压缩文件存放路径:" + _fullPath);
                        string _tempDir = Path.GetDirectoryName(_fullPath);
                        if (!Directory.Exists(_tempDir))
                        {
                            Directory.CreateDirectory(_tempDir);
                        }
                        //将文件进行压缩
                        ZipTool.Zip(ZipTool.ZipType.CompressBySevenZip, null, _fileName, _fullPath);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "导入文件路径不存在，请导出场景文件", "确定");
                    return;
                }
            }
        }
    }
    #endregion
}
