using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AssetBundles
{
    /// <summary>
    /// 构建AssetBundleBuildSetting这个asset文件的选项
    /// </summary>
    public class BundleBuilderEditor : EditorWindow
    {
        //当前平台文件夹
        static string _NowPlatformName =
#if UNITY_STANDALONE_WIN
    "Win";
#elif UNITY_ANDROID
    "Android";
#elif UNITY_IOS
    "iOS";
#else
    "Android";
#endif

        [HideInInspector]
        public static BundleBuilder builder = new BundleBuilder();

        static BundleBuilderEditor window;
        Vector2 _ScrollPosition;
        string UncompressPath;       //非压缩文件存放路径
        int _NeedRemoveIndex = -1;  //当前需要删除的数据的index

        [MenuItem("Tools/打包/资源打包界面", false, 1)]    //在工具栏创建按钮  
        static void Init()
        {
            window = (BundleBuilderEditor)EditorWindow.GetWindow(typeof(BundleBuilderEditor), false, "资源打包");   //定义一个窗口对象  
            window.minSize = new Vector2(500, 300);
        }
        void OnEnable()
        {
            UncompressPath = Application.streamingAssetsPath + "/" + _NowPlatformName + "/";        //非压缩场景存放目录
        }

        public void OnGUI()
        {
            _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition);
            GUILayout.BeginVertical();
            //添加头
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 18;
            headerStyle.normal.textColor = Color.white;
            //headerStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("AssetBundle设置工具(资源)", headerStyle, GUILayout.Height(25));
            EditorGUILayout.Space();

            //设置需要打包的类型
            SetBuildSourceType();
            EditorGUILayout.Space();

            //设置要打包的资源文件夹（用来生成assetname）
            SetBuildSourcePath();
            EditorGUILayout.Space();

            //生成luaassetname
            SetBuildLuaPath();
            EditorGUILayout.Space();

            //设置打包平台
            SetBuildTarget();
            EditorGUILayout.Space();

            //打包选项
            SetBundleOptions();
            EditorGUILayout.Space();

            //开始打包
            DoBuild();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

        }
        /// <summary>
        /// 设置要生成名称的资源的类型
        /// </summary>
        void SetBuildSourceType()
        {
            EditorGUILayout.LabelField(new GUIContent("设置要设置名称的资源类型:", "如\".prefab .shader\"等，不同类型用空格分开"), EditorStyles.boldLabel);
            builder.BundleObjType = GUILayout.TextField(builder.BundleObjType, GUILayout.MinWidth(250));
        }
        /// <summary>
        /// 设置哪个文件夹下的Prefab要生成assetName
        /// </summary>
        void SetBuildSourcePath()
        {
            EditorGUILayout.LabelField(new GUIContent("设置资源 AssetBundleName:", "对此文件夹下的所有Prefab，设置AssetBundleName\n注意：在加入新的Prefab时，生成一次即可！不用每次打包就生成"), EditorStyles.boldLabel);
            if (builder.BuildSourcePaths.Count == 0)    //生成一个空的path以供选择
            {
                builder.BuildSourcePaths.Add("");
            }
            for (int i = 0; i < builder.BuildSourcePaths.Count; i++)
            {
                GUILayout.BeginHorizontal();
                string _tempPath = builder.BuildSourcePaths[i];
                _tempPath = GUILayout.TextField(_tempPath, GUILayout.MinWidth(250));
                if (GUILayout.Button("...", GUILayout.Width(22)))
                {
                    string projectFolder = BundleBuilder.GetDataPath();
                    _tempPath = EditorUtility.OpenFolderPanel("选择要设置名字的文件夹", projectFolder, "");
                    if (_tempPath.Length != 0)
                    {
                        _tempPath = BundleBuilder.GetRelativePath(_tempPath);     //去除当前路径中的绝对路径
                        builder.BuildSourcePaths[i] = _tempPath;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(25)))
                {
                    //添加一个要设置asset名称的文件夹
                    builder.BuildSourcePaths.Add("");
                }
                GUILayout.Space(10);
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    _NeedRemoveIndex = i;
                }
                GUILayout.EndHorizontal();
            }
            //删除“-”的数据
            if (_NeedRemoveIndex != -1)
            {
                builder.BuildSourcePaths.RemoveAt(_NeedRemoveIndex);
                _NeedRemoveIndex = -1;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("生成资源 AssetBundleName", GUILayout.Width(250)))
            {
                if (builder.DoSetBundleObjTypes())
                {
                    if (builder.BuildSourcePaths.Contains(""))
                    {
                        EditorUtility.DisplayDialog("提示", "存在一个空的文件夹选项，请放入对应路径，或点击“-”删除其", "确定");
                    }
                    else
                    {
                        builder.CreateAssetName();      //开始修改名字
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "要生成asset名称的类型填写错误，注意要有\".\"，如\".prefab\"。且除了空格\" \"外，不能有其他特殊字符", "确定");
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 设置哪个文件夹下的lua文件夹，要生成asset name
        /// </summary>
        void SetBuildLuaPath()
        {
            EditorGUILayout.LabelField(new GUIContent("打包Lua:", "将所有的lua都打入一个asset包"), EditorStyles.boldLabel);
            if (builder.LuaBuildSourcePaths.Count == 0)    //生成一个空的path以供选择
            {
                builder.LuaBuildSourcePaths.Add("");
            }
            for (int i = 0; i < builder.LuaBuildSourcePaths.Count; i++)
            {
                GUILayout.BeginHorizontal();
                string _tempPath = builder.LuaBuildSourcePaths[i];
                _tempPath = GUILayout.TextField(_tempPath, GUILayout.MinWidth(250));
                if (GUILayout.Button("...", GUILayout.Width(22)))
                {
                    string projectFolder = BundleBuilder.GetDataPath();
                    _tempPath = EditorUtility.OpenFolderPanel("选择要打包的Lua文件夹", projectFolder, "");
                    if (_tempPath.Length != 0)
                    {
                        _tempPath = BundleBuilder.GetRelativePath(_tempPath);     //去除当前路径中的绝对路径
                        builder.LuaBuildSourcePaths[i] = _tempPath;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(25)))
                {
                    //添加一个要设置asset名称的文件夹
                    builder.LuaBuildSourcePaths.Add("");
                }
                GUILayout.Space(10);
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    _NeedRemoveIndex = i;
                }
                GUILayout.EndHorizontal();
            }
            //删除“-”的数据
            if (_NeedRemoveIndex != -1)
            {
                builder.LuaBuildSourcePaths.RemoveAt(_NeedRemoveIndex);
                _NeedRemoveIndex = -1;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("生成Lua AssetBundleName，并直接打包", GUILayout.Width(250)))
            {
                //Debug.Log("生成Lua AssetBundleName");
                if (builder.LuaBuildSourcePaths.Contains(""))
                {
                    EditorUtility.DisplayDialog("提示", "存在一个空的文件夹选项，请放入对应路径，或点击“-”删除其", "确定");
                }
                else
                {
                    //开始生成所有Lua文件的assetName
                    builder.CreateLuaAssetName();
                    //刷新资源
                    AssetDatabase.Refresh();
                    //开始打包
                    builder.Build(UncompressPath, true);
                }

                //生成ab包

            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 设置打包平台类型
        /// </summary>
        void SetBuildTarget()
        {
            EditorGUILayout.LabelField("打包平台类型:", EditorStyles.boldLabel);
            builder.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(builder.buildTarget);
        }

        /// <summary>
        /// 显示各种打包选项
        /// </summary>
        void SetBundleOptions()
        {
            EditorGUILayout.LabelField("打包选项:", EditorStyles.boldLabel);
            string[] names = Enum.GetNames(typeof(BuildAssetBundleOptions));
            for (int i = 0; i < names.Length; i++)
            {
                if (string.IsNullOrEmpty(names[i]) || names[i] == "None")
                    continue;

                BuildAssetBundleOptions key = (BuildAssetBundleOptions)Enum.Parse(typeof(BuildAssetBundleOptions), names[i]);
                if (builder.EnabledOptions.ContainsKey(key))
                {
                    GUIContent toggleContent = new GUIContent(" " + names[i], GetTooltip(key));
                    bool _tempBool = builder.EnabledOptions[key];
                    builder.EnabledOptions[key] = EditorGUILayout.ToggleLeft(toggleContent, _tempBool);
                }
            }

            //判断更新树及不包含树两个选项
#if !(UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
            bool uncompressedAssetBundle;
            if (builder.EnabledOptions.TryGetValue(BuildAssetBundleOptions.UncompressedAssetBundle, out uncompressedAssetBundle))
            {
                if (uncompressedAssetBundle)
                {
                    bool chunkBasedCompression;
                    if (builder.EnabledOptions.TryGetValue(BuildAssetBundleOptions.ChunkBasedCompression, out chunkBasedCompression))
                    {
                        if (chunkBasedCompression)
                        {
                            if (EditorUtility.DisplayDialog("提示", "压缩和不压缩两个选项不能同时存在", "确定"))
                            {
                                builder.EnabledOptions[BuildAssetBundleOptions.UncompressedAssetBundle] = false;
                            }
                        }
                    }
                }
            }
#endif
#if UNITY_5
            bool disableWriteTypeTree;
            if (builder.EnabledOptions.TryGetValue(BuildAssetBundleOptions.DisableWriteTypeTree, out disableWriteTypeTree))
            {
                if (disableWriteTypeTree)
                {
                    bool ignoreTypeTreeChanges;
                    if (builder.EnabledOptions.TryGetValue(BuildAssetBundleOptions.IgnoreTypeTreeChanges, out ignoreTypeTreeChanges))
                    {
                        if (ignoreTypeTreeChanges)
                        {
                            if (EditorUtility.DisplayDialog("提示", "更新树及不包含树两个选项不能同时存在", "确定"))
                            {
                                builder.EnabledOptions[BuildAssetBundleOptions.IgnoreTypeTreeChanges] = false;
                            }
                        }
                    }
                }
            }
#endif
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("重置，使用默认方式打包", GUILayout.Width(150)))
            {
                UseDefaultSetting();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 设置备注
        /// </summary>
        string GetTooltip(BuildAssetBundleOptions option)
        {
            if (option == BuildAssetBundleOptions.UncompressedAssetBundle)
                return "创建不压缩的AssetBundle";
            if (option == BuildAssetBundleOptions.DeterministicAssetBundle)
                return "确保每次打包不生成一次hash，而是增量更新时才生成一个新的hash";
            if (option == BuildAssetBundleOptions.DisableWriteTypeTree)
                return "在AssetBundle中不包含类型信息";
            if (option == BuildAssetBundleOptions.ForceRebuildAssetBundle)
                return "强制重打所有AssetBundle文件";
            if (option == BuildAssetBundleOptions.IgnoreTypeTreeChanges)
                return "判断AssetBundle更新时，是否忽略TypeTree的变化";
            if (option == BuildAssetBundleOptions.AppendHashToAssetBundleName)
                return "将Hash值添加在AssetBundle文件名之后";
#if !(UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // for the version of Unity over 5.3x
            if (option == BuildAssetBundleOptions.ChunkBasedCompression)
                return "用于使用LZ4格式进行压缩";
#endif

            return string.Empty;
        }

        /// <summary>
        /// 重置所有选项
        /// </summary>
        private void UseDefaultSetting()
        {
            foreach (BuildAssetBundleOptions key in builder.EnabledOptions.Keys)
            {
                builder.EnabledOptions[key] = false;
            }
        }
        /// <summary>
        /// 开始打包
        /// </summary>
        void DoBuild()
        {
            EditorGUILayout.LabelField("开始打包:", EditorStyles.boldLabel);
            if (GUILayout.Button("打包"))
            {
                if (string.IsNullOrEmpty(UncompressPath))
                {
                    EditorUtility.DisplayDialog("提示", "请选择打包要导出的文件夹", "确定");
                }
                else
                {
                    //EditorApplication.delayCall += builder.Build;
                    builder.Build(UncompressPath);
                }
            }
        }
    }
}