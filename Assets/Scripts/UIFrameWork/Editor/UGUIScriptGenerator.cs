//#define USE_FOR_EBO
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;

using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;

#if USE_FOR_EBO
using UnityEngine.EboUI.UI;
#else
using UnityEngine.UI;
#endif

public class UGUIScriptGenerator
{
    List<string> prefabs;//所有Resource下面的Prefab

    // game object 有组件名和用户命名 
    List<string> compName;
    List<string> objName;

    private string curPrefabPathRelativeToResource = "";
    // const
    // 文件起始命名空间
    const string s_Title = @"
/*
    file desc: Auto Generation by [UGUIScriptGenerator] 
*/
";
    const string s_NS_UnityEngine = "using UnityEngine;";
#if USE_FOR_EBO
    const string s_NS_UnityEngineUI = "using UnityEngine.EboUI.UI;";
    const string s_NS_UnityEngineEboUIEvent = "using UnityEngine.EboUI.EventSystems;";
#else
    const string s_NS_UnityEngineUI = "using UnityEngine.UI;";
    const string s_NS_UnityEngineEboUIEvent = "using UnityEngine.EventSystems;";
#endif
    const string s_NS_SystemIO = "using System.IO;";
    const string s_NS_System = "using System;";
    const string s_NS_Generic = "using System.Collections.Generic;";
    const string s_NS_PureMVC = "using PureMVC.Interfaces;";

    //version44.2 
    private const string s_GameNameSpace = "namespace " + UIGlobalDefine.UINameSPace;//version44.2
    private const string s_GameNameSpaceBeginBracket = "{";
    private const string s_GameNameSpaceEndBracket = "}";
    //version44.2 

    const string s_Public = "public ";
    const string s_Protected_Override = "protected override ";
    const string s_Protected = "protected ";
    const string s_Override = "override ";
    const string s_Class = "class ";
    const string s_Void = "void ";
    const string s_InheritUIViewClass = " : UIViewBase";
    const string s_InheritUIControlClass = " : UIControllerBase";
    const string s_InheritMonoClass = " : MonoBehaviour";
    const string s_4Space = "    ";
    const string s_8Space = "        ";
    public const string OutputDirName = "PrefabScriptOutput";

    const string s_WindowID_Dot = "WindowID.";
    public UGUIScriptGenerator()
    {
        prefabs = new List<string>();
        compName = new List<string>();
        objName = new List<string>();
    }

    /// <summary>
    /// 搜索游戏目下某个名为Resources文件夹中 UIPrefabPath["UIPrefab"] 的所有prefab文件
    /// </summary>
    void SearchPrefabs()
    {
        prefabs.Clear();

        StringBuilder sb = new StringBuilder(128);
        sb.Append(Directory.GetCurrentDirectory()).Append(Path.DirectorySeparatorChar).Append("Assets").Append(
            Path.DirectorySeparatorChar).Append("Resources").Append(Path.DirectorySeparatorChar).Append(UIGlobalDefine.UIPrefabPath);

        DirectoryInfo root = new DirectoryInfo(sb.ToString());
        var files = root.GetFiles("*.prefab", SearchOption.AllDirectories);

        string path;
        string keyWords = "Resources/";

        foreach (var file in files)
        {
            path = file.FullName.Replace('\\', '/');

            if (file.Extension.Length > 0)
                path = path.Replace(file.Extension, "");

            path = path.Substring(path.LastIndexOf(keyWords) + keyWords.Length);  // 截取Resources/之后的路径

            prefabs.Add(path);
        }
    }

    /// <summary>
    /// prefab文件转代码
    /// </summary>
    public void PrefabToScript()
    {
        SearchPrefabs();

        foreach (var prefab in prefabs)
        {
            ProcPrefabFile(prefab);
        }

        // popup
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            string outputDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + OutputDirName;
            System.Diagnostics.Process.Start("explorer.exe", outputDir);
        }
    }

    enum OutputPathType { OutputPathDirectory, OutputPathUIViewScript, OutputPathUIControlScript, OutputPathUIViewProxyScript, OutputPathWindowsIDDefineScripte }

    string GetOutputPath(OutputPathType type, string prefabName)
    {
        string dir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + OutputDirName;
        Directory.CreateDirectory(dir);
        string path = dir;

        if (OutputPathType.OutputPathUIViewScript == type)
        {
            path = dir + Path.DirectorySeparatorChar + prefabName + UIGlobalDefine.ViewScriptSuffix + ".cs";
        }
        else if (OutputPathType.OutputPathUIControlScript == type)
        {
            path = dir + Path.DirectorySeparatorChar + prefabName + UIGlobalDefine.ControlScriptSuffix + ".cs";
        }
        else if (OutputPathType.OutputPathUIViewProxyScript == type)
        {
            path = dir + Path.DirectorySeparatorChar + prefabName + UIGlobalDefine.ProxyViewScriptSuffix + ".cs";
        }
        else if (OutputPathType.OutputPathWindowsIDDefineScripte == type)
        {
            path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Assets"+ Path.DirectorySeparatorChar + UIGlobalDefine.UIWindowsIDDefineScriptPath+ ".cs";
        }

        return path;
    }

    /// <summary>
    /// 从prefab文件生成cs脚本
    /// </summary>
    /// <param name="prefab">路径</param>
    public void ProcPrefabFile(string prefab)
    {
        Debug.Log("ProcPrefabFile : " + prefab);

        compName.Clear();
        objName.Clear();

        curPrefabPathRelativeToResource = prefab;
        GameObject go = GameObject.Instantiate(Resources.Load(prefab)) as GameObject;

        if (go)
        {
            CheckObjectChildren(go);
        }

        // write to file
        string fileName = go.name.Replace("(Clone)", "");

        MakeScript(fileName);

        GameObject.DestroyImmediate(go);
    }

    //遍历找到所有Component,只找名字包含UI_的
    void CheckObjectChildren(GameObject child)
    {
        foreach (Transform sub in child.transform)
        {
            CheckComponent(sub.gameObject);
            CheckObjectChildren(sub.gameObject);
        }
    }

    bool TryParseComponent<T>(GameObject obj) where T : Component
    {
        T type = obj.GetComponent<T>();

        //if (type != null && type.name.Trim() != "UI")
        if (type != null && type.name.Trim().Contains("UI_"))
        {
            this.compName.Add(typeof(T).Name);
            this.objName.Add(type.name);
            return true;
        }

        return false;
    }

    void CheckComponent(GameObject obj)
    {
        do
        {
            if (TryParseComponent<InputField>(obj)) break;
            if (TryParseComponent<Slider>(obj)) break;
            if (TryParseComponent<Scrollbar>(obj)) break;
            if (TryParseComponent<CanvasGroup>(obj)) break;
            if (TryParseComponent<ScrollRect>(obj)) break;
            if (TryParseComponent<GridLayoutGroup>(obj)) break;
            if (TryParseComponent<Toggle>(obj)) break;
            if (TryParseComponent<Button>(obj)) break;
            if (TryParseComponent<Text>(obj)) break;
            if (TryParseComponent<Image>(obj)) break;
            if (TryParseComponent<RawImage>(obj)) break;
        } while (false);
    }

#region COMMON
    void COMMON_MakeScriptOnDestroy(StreamWriter stream)
    {
        stream.WriteLine(s_4Space + s_Public + s_Void + "OnDestroy()");
        stream.WriteLine(s_4Space + '{');
        stream.WriteLine();
        stream.WriteLine(s_4Space + '}');
    }

    void COMMON_MakeScriptTitleNamespace(StreamWriter stream, params string[] ns)
    {
        stream.WriteLine(s_Title);
        stream.WriteLine();

        for (int i = 0; i < ns.Length; i++)
            stream.WriteLine(ns[i]);

        if (ns.Length > 0)
            stream.WriteLine();
    }
#endregion

#region UI View Maker
    // ----public COMPONENT fieldName UI成员字段
    void UIView_MakeScriptField(StreamWriter stream, string className)
    {
        string lineCode;

        for (int i = 0; i < compName.Count; i++)
        {
            lineCode = s_4Space + s_Public + compName[i] + ' ' + objName[i] + ';';
            stream.WriteLine(lineCode);
        }

        stream.WriteLine();
    }

    //protected override void SetWindowId()
    //{
    //    ID = WindowID.MyUI;
    //}
    void UIView_MakScriptFunc_SetWindowId(StreamWriter stream, string className)
    {
        stream.WriteLine(s_4Space + s_Protected_Override + s_Void + "SetWindowId()");
        stream.WriteLine(s_4Space + '{');

        stream.WriteLine(s_8Space + "ID = " + s_WindowID_Dot + className + ";");

        stream.WriteLine(s_4Space + '}');
        stream.WriteLine();
    }

    //public override void InitWindowConfigData()
    //{
    //}
    void UIView_MakScriptFunc_InitWindowConfigData(StreamWriter stream)
    {
        stream.WriteLine(s_4Space + s_Protected_Override + s_Void + "InitWindowConfigData()");
        stream.WriteLine(s_4Space + '{');

        //暂时没有内容

        stream.WriteLine(s_4Space + '}');
        stream.WriteLine();
    }

    // ----  public override void InitWindowOnAwake() {
    // --------comName = GetComponentByName<T>("compName");
    void UIView_MakeScriptFunc_InitWindowOnAwake(StreamWriter stream)
    {
        stream.WriteLine(s_4Space + s_Protected_Override+ s_Void + "InitWindowOnAwake()");
        stream.WriteLine(s_4Space + '{');

        if (objName.Count > 0)
        {
            stream.WriteLine(s_4Space + "if (" + objName[0] +" != null) return;");
        }
        
        Dictionary<string, string> compCacheDic = new Dictionary<string, string>();

        // makes cache dic
        for (int i = 0; i < compName.Count; i++)
        {
            compCacheDic[compName[i]] = "";
        }

        //List<Button> buttonCache = GameUIUtility.GetComponentCache<Button>(this.gameObject);
        // get cache keys
        List<string> compCacheKeys = new List<string>(compCacheDic.Keys);

        // set cache key-value
        for (int i = 0; i < compCacheKeys.Count; i++)
        {
            compCacheDic[compCacheKeys[i]] = compCacheKeys[i].ToLower() + "Cache";
            stream.WriteLine(string.Format("{0}List<{1}> {2} = GameUIUtility.GetComponentCache<{1}>(this.gameObject);", s_8Space, compCacheKeys[i], compCacheDic[compCacheKeys[i]]));
        }

        stream.WriteLine();

        //MyButton = GameUIUtility.GetComponentFromCache(ref buttonCache, "MyButton");
        //if (MyButton == null) Debug.LogError("MyButton获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        for (int i = 0; i < compName.Count; i++)
        {
            stream.WriteLine(string.Format("{0}{1} = GameUIUtility.GetComponentFromCache(ref {2}, \"{1}\");", s_8Space, objName[i], compCacheDic[compName[i]]));
            stream.WriteLine(string.Format("{0}if ({1} == null) Debug.LogError(\"{1}获取为空, 请检查是不是没用Ebo.UI的脚本!!!\");", s_8Space, objName[i]));
        }

        stream.WriteLine(s_4Space + '}');
        stream.WriteLine();
    }


    //public override void InitController()
    //{
    //    Controller = new MyUIControl(this);
    //}
    void UIView_MakScriptFunc_InitController(StreamWriter stream, string className)
    {
        stream.WriteLine(s_4Space + s_Protected_Override + s_Void + "InitController()");
        stream.WriteLine(s_4Space + '{');

        stream.WriteLine(s_8Space + "Controller = new "+ className + UIGlobalDefine.ControlScriptSuffix +"(this);");

        stream.WriteLine(s_4Space + '}');
        stream.WriteLine();
    }


#endregion

#region UI Control Maker

    //public MyUIControl(UIViewBase viewBase) : base(viewBase)
    //{
    //    m_mediatorName = "MyUIControl";
    //}
    void UIControl_MakeScript_Constructor(StreamWriter stream, string className)
    {
        stream.WriteLine(s_4Space + s_Public + className + "(UIViewBase viewBase) : base(viewBase)");
        stream.WriteLine(s_4Space + "{");
        stream.WriteLine(s_8Space + "m_mediatorName = " + "\""+ className +"\";");
        stream.WriteLine(s_4Space + "}");
    }

    //protected MyUIView View
    //{
    //    get { return (MyUIView)baseView; }
    //}
    void UIControl_MakeScript_ViewProperty(StreamWriter stream, string prefabName)
    {
        string ViewScriptName = prefabName + UIGlobalDefine.ViewScriptSuffix;
        stream.WriteLine(s_4Space + s_Protected + ViewScriptName + " View");
        stream.WriteLine(s_4Space + "{");
        stream.WriteLine(s_8Space + "get { return ("+ ViewScriptName + ")baseView; }");
        stream.WriteLine(s_4Space + "}");
    }

    void UIControl_MakeScript(StreamWriter stream)
    {
        stream.WriteLine(@"
#region UI初始化,打开,关闭,销毁

    //第一次创建时候触发,只执行一次, 在Open前执行
    public override void OnInit()
    {
        base.OnInit();
        
        UIConfigData.forceClearNavigation = false;                            //以后需要导航的时候使用 清除导航信息
        UIConfigData.navigationMode = UIWindowNavigationMode.IgnoreNavigation;//以后需要导航的时候使用 是否记录导航信息
        UIConfigData.PreWindowId = WindowID.WindowID_Invaild;                 //以后需要导航的时候使用 默认的前置UI

        UIConfigData.showMode = UIWindowShowMode.HideOtherWindow; //默认打开ui前,会关闭其他显示的UI,
        UIConfigData.colliderMode = UIWindowColliderMode.Normal;  //默认添加一个背景遮罩
        UIConfigData.windowType = UIWindowType.Normal;            //默认,切换场景销毁;如果是DontDestory类型, 选择DontDestory,切换场景不销毁UI
        UIConfigData.depth = 0;                                   //默认深度为0

        AddUIEvent();
    }

    protected override void OnUIOpened()
    {
        base.OnUIOpened();

        //todo 具体的UI打开处理 在播放UI进场动画之前
    }

    protected override void OnUIHided()
    {
        base.OnUIHided();

        //todo 具体的UI关闭处理/退场动画结束回调

    }

    protected override void BeforeDestroyWindow()
    {
        base.BeforeDestroyWindow();
        RemoveUIEvent();
    }

    public override void DestroyWindow()
    {
        base.DestroyWindow();
    }
#endregion

#region 如果有入场和退场动画在这里添加
    public override void EnteringAnimation(Action onComplete)
    {
        //todo 具体的Dotween动画, 必须在结束回调onComplete, 否则OnEnteringAnimationEnd函数不会被调用
        if (onComplete != null)
        {
            onComplete();
        }
    }

    public override void ExitingAnimation(Action onComplete)
    {
        //todo 具体的Dotween动画, 必须在结束回调onComplete, 否则OnUIHided()函数不会被调用
        if (onComplete != null)
        {
            onComplete();
        }
    }
#endregion


    public override void UpdateUI()
    {
        base.UpdateUI();
    }

#region 外部事件处理
    public override IEnumerable<string> ListNotificationInterests
    {
        get
        {
            return mCmdList;
        }
    }
    List<string> mCmdList
    {
        get
        {
            return new List<string>()
            {
            };
        }
    }

    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
        }
    }
#endregion
");
    }

    //UIGUI事件的监听处理
    void UIControl_MakeScriptActions(StreamWriter stream)
    {
        List<string> tempOnClick = new List<string>() ;
        List<string> tempObjName = new List<string>();
        GetAllButtonClickAndActions(ref tempOnClick, ref tempObjName);

        stream.WriteLine(s_4Space + "#region UGUI事件监听处理");
        // AddActions
        stream.WriteLine(s_4Space + s_Void + "AddUIEvent()");
        stream.WriteLine(s_4Space + '{');
        for (int i = 0; i < tempObjName.Count; i++)
            stream.WriteLine(string.Format("{0}View.{1}.onClick.AddListener({2});", s_8Space, tempObjName[i], tempOnClick[i]));

        //Add MVC Event
        stream.WriteLine();
        stream.WriteLine(s_8Space + "Facade.RegisterMediator(this);");
        stream.WriteLine(s_4Space + '}');

        // RemoveActions
        stream.WriteLine();
        stream.WriteLine(s_4Space + s_Void + "RemoveUIEvent()");
        stream.WriteLine(s_4Space + '{');
        stream.WriteLine(s_8Space + "//注意消息的监听的删除, 检测View是否为空"); //version49  
        for (int i = 0; i < tempObjName.Count; i++)
            stream.WriteLine(string.Format("if(View != null) {0}View.{1}.onClick.RemoveListener({2});", s_8Space, tempObjName[i], tempOnClick[i]));//version49  

        //remove MVC Event
        stream.WriteLine();
        stream.WriteLine(s_8Space + "Facade.RemoveMediator(MediatorName);");
        stream.WriteLine(s_4Space + '}');

        // make callback
        for (int i = 0; i < tempObjName.Count; i++)
        {
            stream.WriteLine();
            stream.WriteLine(s_4Space + s_Void + tempOnClick[i] + "()");
            stream.WriteLine(s_4Space + '{');
            stream.WriteLine();
            stream.WriteLine(s_4Space + '}');
        }

        stream.WriteLine(s_4Space + "#endregion");
    }

#endregion

#region UI View Proxy Maker

    void UIViewProxy_MakeScriptFiled(StreamWriter stream, string viewName)
    {
        stream.WriteLine(string.Format("{0}{1} view;", s_4Space, viewName));

        for (int i = 0; i < compName.Count; i++)
        {
            if (compName[i] == "Button")
            {
                stream.WriteLine(string.Format("{0}{1}Action {2}Action;", s_4Space, s_Public, objName[i]));
            }
        }
    }

    void GetAllButtonClickAndActions(ref List<string> onClick, ref List<string> objNames)
    {
        for (int i = 0; i < compName.Count; i++)
        {
            if (compName[i] == "Button")
            {
                if (onClick != null)
                {
                    onClick.Add(string.Format("On{0}Click", objName[i]));
                }
                if (objNames != null)
                {
                    objNames.Add(objName[i]);
                }
            }
        }
    }

    // void AddUIEvent()  +  RemoveUIEvent() + callbacks
    void UIViewProxy_MakeScriptUIEvent(StreamWriter stream)
    {
        List<string> tempOnClick = new List<string>();  // list of OnXXXClick
        List<string> tempObjName = new List<string>();
        GetAllButtonClickAndActions(ref tempOnClick, ref tempObjName);

        // AddUIEvent
        stream.WriteLine(s_4Space + s_Void + "AddUIEvent()");
        stream.WriteLine(s_4Space + '{');
        for (int i = 0; i < tempObjName.Count; i++)
            stream.WriteLine(string.Format("{0}view.{1}.onClick.AddListener({2});", s_8Space, tempObjName[i], tempOnClick[i]));
        stream.WriteLine(s_4Space + '}');

        // RemoveUIEvent
        stream.WriteLine();
        stream.WriteLine(s_4Space + s_Void + "RemoveUIEvent()");
        stream.WriteLine(s_4Space + '{');
        for (int i = 0; i < tempObjName.Count; i++)
            stream.WriteLine(string.Format("{0}view.{1}.onClick.RemoveListener({2});", s_8Space, tempObjName[i], tempOnClick[i]));
        stream.WriteLine(s_4Space + '}');

        // make callback
        for (int i = 0; i < tempObjName.Count; i++)
        {
            stream.WriteLine();
            stream.WriteLine(s_4Space + s_Void + tempOnClick[i] + "()");
            stream.WriteLine(s_4Space + '{');
            stream.WriteLine(string.Format("{0}if ({1}Action != null)", s_8Space, tempObjName[i]));
            stream.WriteLine("{0}{1}{2}Action();", s_8Space, s_4Space, tempObjName[i]);
            stream.WriteLine(s_4Space + '}');
        }
        // clear
        tempOnClick.Clear();
        tempObjName.Clear();
    }

    // ----public Constructor(VIEW_CLASS view)
    void UIViewProxy_MakeScriptConstrutor(StreamWriter stream, string viewName)
    {
        stream.WriteLine(string.Format("{0}{1}{2}{3}({2} view)", s_4Space, s_Public, viewName, UIGlobalDefine.ProxyViewScriptSuffix));
        stream.WriteLine(s_4Space + '{');
        stream.WriteLine(s_8Space + "this.view = view;");
        stream.WriteLine(s_8Space + "AddUIEvent();");
        stream.WriteLine(s_4Space + '}');
    }
    void UIViewProxy_MakeScriptOnDestroy(StreamWriter stream)
    {
        stream.WriteLine(s_4Space + s_Public + s_Void + "OnDestroy()");
        stream.WriteLine(s_4Space + '{');
        stream.WriteLine(s_8Space + "RemoveUIEvent();");
        stream.WriteLine(s_4Space + '}');
    }

#endregion


#region Script File Maker

    // UI view
    void MakeUIViewScript(string className)
    {
        StreamWriter stream = File.CreateText(GetOutputPath(OutputPathType.OutputPathUIViewScript, className));

        COMMON_MakeScriptTitleNamespace(stream, s_NS_UnityEngine, s_NS_UnityEngineUI, s_NS_Generic);

        //version44.2 
        stream.WriteLine(s_GameNameSpace);
        stream.WriteLine(s_GameNameSpaceBeginBracket);
        //version44.2 
        //类名 定义
        stream.WriteLine(s_Public + s_Class + className + UIGlobalDefine.ViewScriptSuffix + s_InheritUIViewClass);

        //类实现
        stream.WriteLine('{');
        UIView_MakeScriptField(stream, className);  // 生成字段
        stream.WriteLine();
        UIView_MakScriptFunc_SetWindowId(stream, className);  // 生成SetWindowId()
        UIView_MakScriptFunc_InitWindowConfigData(stream);// InitWindowConfigData()
        UIView_MakeScriptFunc_InitWindowOnAwake(stream);
        UIView_MakScriptFunc_InitController(stream, className);  // 生成 InitController()
        stream.WriteLine('}');
        //version44.2 
        stream.WriteLine(s_GameNameSpaceEndBracket);
        ////version44.2 
        stream.WriteLine();

        stream.Flush();
        stream.Close();
    }

    // UI control
    void MakeUIControlScript(string prefabName)
    {
        StreamWriter stream = File.CreateText(GetOutputPath(OutputPathType.OutputPathUIControlScript, prefabName));
        COMMON_MakeScriptTitleNamespace(stream, s_NS_UnityEngine, s_NS_UnityEngineUI, s_NS_SystemIO, s_NS_System, s_NS_Generic, s_NS_PureMVC, s_NS_UnityEngineEboUIEvent);

        string controllerClassName = prefabName + UIGlobalDefine.ControlScriptSuffix;
        //version44.2 
        stream.WriteLine(s_GameNameSpace);
        stream.WriteLine(s_GameNameSpaceBeginBracket);
        //version44.2 
        //类定义
        stream.WriteLine(s_Public + s_Class + controllerClassName + s_InheritUIControlClass);

        //类实现
        stream.WriteLine('{');
        UIControl_MakeScript_Constructor(stream, controllerClassName);
        stream.WriteLine();
        UIControl_MakeScript_ViewProperty(stream, prefabName);
        stream.WriteLine();
        UIControl_MakeScript(stream);
        stream.WriteLine();
        UIControl_MakeScriptActions(stream);
        stream.WriteLine();
        stream.WriteLine('}');
        //version44.2 
        stream.WriteLine(s_GameNameSpaceEndBracket);
        ////version44.2 
        stream.WriteLine();

        stream.Flush();
        stream.Close();
    }

    // UI proxy
    void MakeUIViewProxycript(string className)
    {
        StreamWriter stream = File.CreateText(GetOutputPath(OutputPathType.OutputPathUIViewProxyScript, className));
        COMMON_MakeScriptTitleNamespace(stream, s_NS_UnityEngine, s_NS_UnityEngineUI, s_NS_System);
        stream.WriteLine(s_Public + s_Class + className + UIGlobalDefine.ProxyViewScriptSuffix);

        stream.WriteLine('{');
        UIViewProxy_MakeScriptFiled(stream, className);
        stream.WriteLine();
        UIViewProxy_MakeScriptConstrutor(stream, className);
        stream.WriteLine();
        UIViewProxy_MakeScriptUIEvent(stream);
        stream.WriteLine();
        UIViewProxy_MakeScriptOnDestroy(stream);
        stream.WriteLine('}');
        stream.WriteLine();

        stream.Flush();
        stream.Close();
    }

    void MakeWindowsIDDefinecript(string prefabName)
    {
        string filePath = GetOutputPath(OutputPathType.OutputPathWindowsIDDefineScripte, prefabName);

        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //不存在就新建, 存在往后添加
        if (!File.Exists(filePath))
        {
            StreamWriter stream = File.CreateText(filePath);
            stream.WriteLine(@"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindowID
{
    WindowID_Invaild = 0,
    Count,
}

public class UIResourceDefine
{
    // Define the UIWindow prefab paths
    // all window prefab placed in Resources folder
    // or assetbundle path
    public static Dictionary<int, string> windowPrefabPath = new Dictionary<int, string>()
    {
        {(int) WindowID.Count, """"},
    };

    // Main folder
    public static string UIPrefabPath = UIGlobalDefine.UIPrefabPath;
}"
);

            stream.Flush();
            stream.Close();
        }

        AppendToExistingWindowIDFile(prefabName, filePath);
    }

    void AppendToExistingWindowIDFile(string prefabName, string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        StringBuilder newContentBuilder = new StringBuilder();

        int IDStartLineIndex = 0;
        int IDEndLineIndex = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Contains(s_4Space + "WindowID_Invaild = 0,"))
            {
                IDStartLineIndex = i;
            }
            else if (line.Contains(s_4Space + "Count,"))
            {
                //这里添加新的ID
                IDEndLineIndex = i;
                // MyUI = 1,
                newContentBuilder.Append(s_4Space).Append(prefabName).Append(" = ").Append(IDEndLineIndex - IDStartLineIndex).Append(",")
                    .AppendLine();
            }
            else if (line.Contains(s_4Space + "{(int) WindowID.Count, \"\"},"))
            {
                //这里添加 prefab 地址 {(int) WindowID.MyUI, "MyUI"},
                int index = curPrefabPathRelativeToResource.IndexOf(UIGlobalDefine.UIPrefabPath);
                string path = curPrefabPathRelativeToResource.Substring(index + UIGlobalDefine.UIPrefabPath.Length);
                newContentBuilder.Append(s_8Space + "{(int) WindowID.").Append(prefabName).Append(", \"").Append(path).Append("\"},")
                    .AppendLine();
            }
            else
            {
                if (IDStartLineIndex > 0 && IDEndLineIndex < 1)
                {
                    int index = line.IndexOf("=");
                    if (index < 0)
                    {
                        Debug.LogError("文件内容:" + line +"不符合 Name = 数字 的规范, 不是不手动修改了");
                        EditorUtility.DisplayDialog("Warning", "文件内容:" + line + "不符合 Name = 数字 的规范, 不是不手动修改了", "OK");
                    }
                    else
                    {
                        //比较当前prefab是不是已经在文件中存在
                        string id = line.Substring(0, index).Trim();
                        if (id == prefabName)
                        {
                            Debug.LogError("在WindowsIDDefine中已经存在" + prefabName + " 先在WindowIDDefine中删除相关信息, 后再执行. 未完成自动替换功能");
                            return;
                        }
                    }

                }

            }

            newContentBuilder.Append(line).AppendLine();
        }

        File.WriteAllText(filePath, newContentBuilder.ToString());
    }


#endregion

    void MakeScript(string prefabName)
    {
        MakeUIViewScript(prefabName);
        MakeUIControlScript(prefabName);
        //MakeUIViewProxycript(className); 暂时不用
        MakeWindowsIDDefinecript(prefabName);
    }
}

////////////////////// menu
public class Prefab2ScriptMenu : Editor
{
    private static string GetPathRelativeToResourceByGUID(string guid)
    {
        string keyWords = "Resources/";
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!path.Contains(UIGlobalDefine.UIPrefabPath))
        {
            Debug.LogError("请把prefab放在Resource路径下面:" + UIGlobalDefine.UIPrefabPath);
            EditorUtility.DisplayDialog("Warning", "请把prefab放在Resource路径下面:" + UIGlobalDefine.UIPrefabPath, "OK");
            return null;
        }
        FileInfo info = new FileInfo(Application.dataPath + path.Substring(path.IndexOf(keyWords)));
        if (info != null)
        {
            if (info.Extension == ".prefab")
            {
                if (info.Extension.Length > 0)
                    path = path.Replace(info.Extension, "");
                path = path.Substring(path.LastIndexOf(keyWords) + keyWords.Length); // 截取Resources/之后的路径
                return path;
            }
            else
            {
                Debug.LogWarning("选择的不是prefab:" + path);
            }
        }
        return null;
    }

    [MenuItem("Tools/UGUI/Generate ALL UIScript")]
    static void Prefab2ScriptMethod()
    {
        UGUIScriptGenerator temp = new UGUIScriptGenerator();
        temp.PrefabToScript();
    }

    [MenuItem("Tools/UGUI/给选中的prefab生成 UI脚本")]
    static void Prefab2ScriptMethod2()
    {
        string[] assetGUIDs = Selection.assetGUIDs;
        UGUIScriptGenerator temp = new UGUIScriptGenerator();
        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            string path = GetPathRelativeToResourceByGUID(assetGUIDs[i]);
            if(path != null)temp.ProcPrefabFile(path);
        }

        // popup
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            string outputDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + UGUIScriptGenerator.OutputDirName;
            System.Diagnostics.Process.Start("explorer.exe", outputDir);
        }

    }

    [MenuItem("Tools/UGUI/给选中的prefab添加view脚本")]
    static void AddScript()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorUtility.DisplayDialog("info", "稍后再尝试.... Unity正在编译", "OK");
            return;
        }

        string[] assetGUIDs = Selection.assetGUIDs;
        UGUIScriptGenerator temp = new UGUIScriptGenerator();
        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            string path = GetPathRelativeToResourceByGUID(assetGUIDs[i]);
            int index = path.LastIndexOf("/");
            string name = path.Substring(path.LastIndexOf("/") + 1);

            bool find = false;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = assembly.GetType(string.Format("{0}.{1}", UIGlobalDefine.UINameSPace, name + UIGlobalDefine.ViewScriptSuffix) );//version44.2
                if (t != null)
                {
                    //添加对应的Script脚本 到prefab上
                    UnityEngine.Object obj = Resources.Load(path);
                    GameObject go = GameObject.Instantiate(obj) as GameObject;

                    UIViewBase test = go.GetComponent(t) as UIViewBase;
                    if (test == null)
                    {
                        test = go.AddComponent(t) as UIViewBase;
                        test.Awake();
                        //EditorUtility.SetDirty(go);
                        PrefabUtility.ReplacePrefab(go, obj);
                    }

                    GameObject.DestroyImmediate(go);
                    obj = null;
                    go = null;
                    find = true;
                    break;
                }
            }

            if(!find)Debug.LogWarning("没找到prefab对应的脚本:" + path);
        }

        Resources.UnloadUnusedAssets();

    }

}

