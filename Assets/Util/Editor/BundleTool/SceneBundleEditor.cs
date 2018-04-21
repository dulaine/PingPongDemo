using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 场景打包
/// 所有在asset下的文件，均是未进行压缩的文件，压缩的存放在中间文件中
/// </summary>
public class SceneBundleEditor : EditorWindow
{
    //当前平台文件夹
    static string _NowPlatformName =
#if UNITY_EDITOR
    "Android";
#elif UNITY_ANDROID
    "Android";
#elif UNITY_IOS
    "iPhone";
#else
    "Android";
#endif

    //编辑器用数据
    public string[] FileFilters = new string[] { "Scene", "unity" };    //能选择的文件类型
    public static List<string> Scenes = new List<string>();            //当前要导出的场景
    public static BuildTarget BuildTarget = BuildTarget.Android;

    static SceneBundleEditor window;
    Vector2 _ScrollPosition;
    string UncompressPath;       //非压缩文件存放路径
    int _NeedRemoveIndex = -1;  //当前需要删除的数据的index

    //----------编辑器脚本------------
    #region 编辑器脚本
    [MenuItem("Tools/打包/场景打包界面", false, 2)]
    static void Init()
    {
        window = (SceneBundleEditor)EditorWindow.GetWindow(typeof(SceneBundleEditor), false, "场景打包");   //定义一个窗口对象
        window.minSize = new Vector2(500, 300);
    }
    void OnEnable()
    {
        UncompressPath = Application.streamingAssetsPath + "/" + _NowPlatformName + "/Scenes/";         //非压缩场景存放目录
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
        EditorGUILayout.LabelField("场景打包", headerStyle, GUILayout.Height(25));
        EditorGUILayout.Space();

        //设置要打包的场景
        SetScenePath();
        EditorGUILayout.Space();

        //设置打包平台
        SetBuildTarget();
        EditorGUILayout.Space();

        //开始打包场景
        if (DoParser())     //如果窗口关闭了，就不用向下执行了，否则报错
        {
            return;
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
    /// <summary>
    /// 设置要生成脚本的excel
    /// </summary>
    void SetScenePath()
    {
        EditorGUILayout.LabelField(new GUIContent("选择要打包的场景:"), EditorStyles.boldLabel);
        if (Scenes.Count == 0)   //生成一个空的path以供选择
        {
            Scenes.Add("");
        }
        //显示所有选项
        for (int i = 0; i < Scenes.Count; i++)
        {
            var _data = Scenes[i];
            //设置路径
            GUILayout.BeginHorizontal();
            string _tempPath = Scenes[i];
            _tempPath = GUILayout.TextField(_tempPath, GUILayout.MinWidth(250));
            if (GUILayout.Button("...", GUILayout.Width(22)))
            {
                string projectFolder = Application.dataPath;
                _tempPath = EditorUtility.OpenFilePanelWithFilters("选择要生成脚本的Excel文件", projectFolder, FileFilters);
                if (_tempPath.Length != 0)
                {
                    Scenes[i] = _tempPath.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            //“+”、“-”
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.MaxWidth(25)))
            {
                Scenes.Add("");  //添加一个空列
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
            Scenes.RemoveAt(_NeedRemoveIndex);
            _NeedRemoveIndex = -1;
        }
    }

    /// <summary>
    /// 设置打包平台类型
    /// </summary>
    void SetBuildTarget()
    {
        EditorGUILayout.LabelField("打包平台类型:", EditorStyles.boldLabel);
        BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(BuildTarget);
    }

    /// <summary>
    /// 开始解析xml
    /// </summary>
    bool DoParser()
    {
        //判断是否有文件夹，没有就创建，否则直接写入会报错
        if (!Directory.Exists(UncompressPath))
        {
            Directory.CreateDirectory(UncompressPath);
        }
        EditorGUILayout.LabelField("开始打包场景:", EditorStyles.boldLabel);
        if (GUILayout.Button("打包场景"))
        {
            window.Close();
            for (int i = 0; i < Scenes.Count; i++)
            {
                string _scenePath = Scenes[i].Replace(GetProjectPath(), "");        //截止到Assets目录，以供场景加载
                string _name = Path.GetFileName(_scenePath);
                string _outPath = UncompressPath + _name;
                string _outPathDir = Path.GetDirectoryName(_outPath);
                if (Directory.Exists(_outPathDir) == false)
                {
                    Directory.CreateDirectory(_outPathDir);
                }
                string[] tmpLevels = { _scenePath };
                BuildPipeline.BuildPlayer(tmpLevels, _outPath, BuildTarget, BuildOptions.BuildAdditionalStreamedScenes);
            }
            return true;
        }
        return false;
    }
    #endregion

    #region 工具方法
    string GetProjectPath()
    {
        string _projectPath = Directory.GetCurrentDirectory();
        return _projectPath.Replace('\\', '/') + "/";
    }
    #endregion
}
