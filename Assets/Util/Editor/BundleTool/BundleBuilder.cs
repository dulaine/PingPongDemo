using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssetBundles
{
    /// <summary>
    /// CollectDependencies，DeterministicAssetBundle，CompleteAssets 默认包含
    /// </summary>
    public class BundleBuilder
    {
        //public BuildTarget buildTarget = BuildTarget.Android;
        BuildTarget mBuildTarget =
#if UNITY_ANDROID
                BuildTarget.Android;
#elif UNITY_STANDALONE
                BuildTarget.StandaloneWindows;
#else
                BuildTarget.iOS;
#endif
        public BuildTarget buildTarget
        {
            get
            {
                return mBuildTarget;
            }
            set
            {
                mBuildTarget = value;
            }
        }
        //哪些后缀引用，要单独打包
        static readonly string _BundleObjTypeString = ".prefab .shader";
        [HideInInspector]
        public string BundleObjType = _BundleObjTypeString;
        List<string> _BundleObjTypes;
        //保存当前设置的打包选项
        [Serializable]
        public class OptionDictionary : SerializableDictionary<BuildAssetBundleOptions, bool> { }
        [HideInInspector]
        public OptionDictionary EnabledOptions;
        //要设置资源 asset名字的文件夹
        public List<string> BuildSourcePaths = new List<string>();
        //要设置Lua asset名字的文件夹
        public List<string> LuaBuildSourcePaths = new List<string>() { "Assets/XLua/Lua" };
        //保存导出路径
        [HideInInspector]
        public string BuildOutputPath = "Assets/StreamingAssets";

        //临时保存lua导出的名称，以便以后只导出lua文件
        public Dictionary<string, List<string>> assetNames = new Dictionary<string, List<string>>();
        string mLuaABName = "hotfix.unity3d";

        public BundleBuilder()
        {
            if (EnabledOptions == null)
                EnabledOptions = new OptionDictionary();
            foreach (string name in Enum.GetNames(typeof(BuildAssetBundleOptions)))
            {
                if (string.IsNullOrEmpty(name) || name == "None")
                    continue;
                BuildAssetBundleOptions key = (BuildAssetBundleOptions)Enum.Parse(typeof(BuildAssetBundleOptions), name);
                if (!EnabledOptions.ContainsKey(key))
                {
                    bool val = false;
#if UNITY_5
                    //加入常用选项
                    if (key != BuildAssetBundleOptions.None &&
                       (key == BuildAssetBundleOptions.UncompressedAssetBundle ||
                        key == BuildAssetBundleOptions.DeterministicAssetBundle ||
                        key == BuildAssetBundleOptions.DisableWriteTypeTree ||
                        key == BuildAssetBundleOptions.ForceRebuildAssetBundle ||
                        key == BuildAssetBundleOptions.IgnoreTypeTreeChanges ||
                        key == BuildAssetBundleOptions.AppendHashToAssetBundleName
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) //加入5.3之后支持的压缩选项
                        || key == BuildAssetBundleOptions.ChunkBasedCompression))
#endif
                    {
                        EnabledOptions.Add(key, val);
                    }
#else
                    if (key == BuildAssetBundleOptions.ChunkBasedCompression)
                    {
                        val = true;
                    }
                    EnabledOptions.Add(key, val);
#endif
                }
            }
        }

        /// <summary>
        /// 开始打包
        /// </summary>
        public void Build(string outPath, bool isLua = false)
        {
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }
            //合并打包选项
            var options = BuildAssetBundleOptions.None;
            foreach (KeyValuePair<BuildAssetBundleOptions, bool> enabled in EnabledOptions)
            {
                if (enabled.Key == BuildAssetBundleOptions.None)
                    continue;
                if (enabled.Value)
                    options |= enabled.Key;
            }
            //开始打包
            if (isLua)
            {
                AssetBundleBuild[] build = new AssetBundleBuild[assetNames.Count];
                int tempI = 0;
                foreach (var key in assetNames.Keys)
                {
                    AssetBundleBuild _b = new AssetBundleBuild();
                    _b.assetBundleName = key;
                    _b.assetNames = assetNames[key].ToArray();
                    build[tempI] = _b;
                    tempI++;
                }
                BuildPipeline.BuildAssetBundles(outPath, build, options, this.buildTarget);
            }
            else
            {
                BuildPipeline.BuildAssetBundles(outPath, options, this.buildTarget);
            }
        }

        /// <summary>
        /// 创建资源 AssetBundleName
        /// </summary>
        public void CreateAssetName()
        {
            for (int i = 0; i < BuildSourcePaths.Count; i++)
            {
                int _index = BuildSourcePaths[i].LastIndexOf('/');
                string _removePath = BuildSourcePaths[i].Substring(0, _index);
                FindPrefab(GetProjectPath() + "/" + BuildSourcePaths[i], _removePath, this);
            }
        }

        /// <summary>
        /// 创建Lua的 AssetBundleName
        /// </summary>
        public void CreateLuaAssetName()
        {
            assetNames.Clear();
            for (int i = 0; i < LuaBuildSourcePaths.Count; i++)
            {
                var dir = LuaBuildSourcePaths[i];
                var toDir = dir + "_Backup";
                //复制Lua文件夹
                IOTool.CopyDir(dir, toDir);
                //修改后缀
                IOTool.ChangeExtension(toDir, ".lua", ".bytes");
                FindLuaSimple(toDir);
            }
        }

        /// <summary>
        /// 创建Lua AssetBundleName
        /// </summary>
        public void CreateLuaAssetName(string tempPath, string removePath)
        {
            FindLua(tempPath, removePath);
        }

        //---------------工具方法------------------
        #region 工具方法
        /// <summary>
        /// 开始分割类型
        /// </summary>
        public bool DoSetBundleObjTypes()
        {
            _BundleObjTypes = BundleObjType.Split(' ').ToList();
            _BundleObjTypes.RemoveAll(IsEmpty);
            for (int i = 0; i < _BundleObjTypes.Count; i++)
            {
                if ((_BundleObjTypes[i]).Substring(0, 1) != ".")
                {
                    return false;
                }
            }
            return true;
        }
        bool IsEmpty(String s)
        {
            if (s == "") return true;
            return false;
        }
        /// <summary>
        /// 获取asset路径
        /// </summary>
        public static string GetDataPath()
        {
            return Application.dataPath + "/";
        }

        /// <summary>
        /// 获取当前绝对路径
        /// </summary>
        public static string GetProjectPath()
        {
            string projectPath = Directory.GetCurrentDirectory();
            projectPath = projectPath.Replace('\\', '/');
            return projectPath;
        }

        /// <summary>
        /// 返回此路径的相对路径
        /// </summary>
        public static string GetRelativePath(string path)
        {
            return path.Replace(GetProjectPath() + "/", "");
        }
        #endregion

        //---------------内部调用------------------
        #region 内部调用
        /// <summary>
        /// 查找所有prefab，修改其名字
        /// </summary>
        void FindPrefab(string source, string removePath, BundleBuilder bundle)
        {
            DirectoryInfo _folder = new DirectoryInfo(source);
            FileSystemInfo[] _files = _folder.GetFileSystemInfos();
            int length = _files.Length;
            for (int i = 0; i < length; i++)
            {
                if (_files[i] is DirectoryInfo)
                {
                    FindPrefab(_files[i].FullName, removePath, bundle);
                }
                else
                {
                    if (_files[i].Name.EndsWith(".prefab"))     //只找prefab即可
                    {
                        string _tempS = _files[i].FullName;
                        _tempS = _tempS.Replace('\\', '/');
                        _tempS = GetRelativePath(_tempS);
                        Debug.Log("prefab路径：" + _tempS);
                        //Debug.Log("路径：" + _files[i].FullName);
                        //Debug.Log("路径：->" + AssetDatabase.AssetPathToGUID(_tempS));
                        //获取关联资源
                        string[] _tempList = AssetDatabase.GetDependencies(_tempS);
                        for (int j = 0; j < _tempList.Length; j++)
                        {
                            string _fullPath = _tempList[j];
                            if (_tempS == _fullPath)
                            {
                                //Debug.Log("路径：-->" + _tempList[j]);
                                //Debug.Log("路径：-->-->" + AssetDatabase.AssetPathToGUID(_tempList[j]));

                                //将自己的assetname以路径方式命名
                                AssetImporter assetImporter = AssetImporter.GetAtPath(_tempS);
                                string _name = _tempS.Replace(removePath + "/", "");
                                Debug.Log("路径：-->" + _name);
                                assetImporter.assetBundleName = _name.Replace(".prefab", ".unity3d");   //将包后缀改为.unity3d
                            }
                            else
                            {
                                if (CheckNeedCreateName(_fullPath))
                                {
                                    Debug.Log("路径：-->-->" + _fullPath);
                                    //将关联资源assetname以id方式命名
                                    string _tempID = AssetDatabase.AssetPathToGUID(_fullPath);
                                    AssetImporter assetImporter = AssetImporter.GetAtPath(_fullPath);
                                    //进行asset名称组合
                                    int _index = _fullPath.LastIndexOf('.') + 1;
                                    string _suffix = "depend" + _fullPath.Substring(_index, _fullPath.Length - _index) + "/" + _tempID;
                                    assetImporter.assetBundleName = _suffix;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 将所有的Lua脚本改为统一的asset name
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_replacePath">要去掉的路径</param>
        public void FindLuaSimple(string _path)
        {
            DirectoryInfo _folder = new DirectoryInfo(_path);
            FileSystemInfo[] _files = _folder.GetFileSystemInfos();
            int length = _files.Length;
            for (int i = 0; i < length; i++)
            {
                if (_files[i] is DirectoryInfo)
                {
                    FindLuaSimple(_files[i].FullName);
                }
                //else if (_files[i].Name.EndsWith(".lua"))
                else if (_files[i].Name.EndsWith(".bytes"))
                {
                    Debug.Log("路径：-->" + _files[i].FullName);
                    var temps = _files[i].FullName.Replace('\\', '/');
                    temps = GetRelativePath(temps);
                    //AssetImporter assetImporter = AssetImporter.GetAtPath(temps);
                    //assetImporter.assetBundleName = "hotfix.unity3d";
                    if (assetNames.ContainsKey(mLuaABName))
                    {
                        assetNames[mLuaABName].Add(temps);
                    }
                    else
                    {
                        assetNames[mLuaABName] = new List<string>();
                        assetNames[mLuaABName].Add(temps);
                    }
                }
            }
        }
        /// <summary>
        /// 查找所有的Lua脚本，并修改asset name
        /// 按路径打Lua assetName的方式
        /// </summary>
        public void FindLua(string source, string removePath)
        {
            DirectoryInfo _folder = new DirectoryInfo(source);
            FileSystemInfo[] _files = _folder.GetFileSystemInfos();
            int length = _files.Length;
            for (int i = 0; i < length; i++)
            {
                if (_files[i] is DirectoryInfo)
                {
                    FindLua(_files[i].FullName, removePath);
                }
                else
                {
                    if (_files[i].Name.EndsWith(".lua.bytes"))
                    {
                        string _tempS = _files[i].FullName;
                        _tempS = _tempS.Replace('\\', '/');
                        var _tempS1 = GetRelativePath(_tempS);
                        string _name = _tempS.Replace(removePath + "/", "");    //去掉需要去掉的前置路径
                        int _tempIndex = _name.LastIndexOf('/');
                        _name = _name.Substring(0, _tempIndex);     //去掉最后的名字，将asset Name 只保留为当前所在文件夹的name（ulua加载要求这种格式）
                        _name = _name.Replace('/', '_');
                        _name = ("lua/" + _name + ".unity3d").ToLower();        //以便将所有的lua文件，放在同一个文件夹下
                        //Debug.Log("lua路径：" + _tempS + " " + removePath + " " + _name);

                        //不设置AssetName的方式(使用AssetName会导致全部打包)
                        if (assetNames.ContainsKey(_name))
                        {
                            assetNames[_name].Add(_tempS1);
                        }
                        else
                        {
                            assetNames[_name] = new List<string>();
                            assetNames[_name].Add(_tempS1);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 检测引用文件的类型
        /// </summary>
        bool CheckNeedCreateName(string path)
        {
            for (int i = 0; i < _BundleObjTypes.Count; i++)
            {
                if (path.EndsWith(_BundleObjTypes[i]))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}