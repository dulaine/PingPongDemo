using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

using Excel;
using System.Data;

/// <summary>
/// Excel 解析成 脚本
/// </summary>
public class ExcelParserScript : EditorWindow
{
    /// <summary>
    /// 要导出的excel数据
    /// </summary>
    public class ExcelOutputData
    {
        public string ExcelPath = "";   //要生成的excel的路径
        public string SheetNames = "";  //此excel要生成哪个页签，不填为第一个，多个用空格隔开
        public bool IsNeedCS = true;    //是否需要生成C#脚本
        public bool IsNeedLua = true;   //是否需要生成Lua脚本
    }

    //若此列数据的标识为下面的标识，则不进行数据导出
    const string NOC = "c";
    const string NOS = "s";
    const string NOCS = "cs";

    //编辑器用数据
    public static List<ExcelOutputData> Excels = new List<ExcelOutputData>();  //当前已有的导出选项
    public static string CSharpOutputPath = "";     //C#脚本导出目录
    public static string LuaOutputPath = "";        //Lua脚本导出目录
    public string[] FileFilters = new string[] { "Excel", "xlsx" };  //能选择的文件类型

    //excel数据
    List<string> DataNames = new List<string>();      //变量名称
    List<string> DataType = new List<string>();       //类型
    List<string> DataLength = new List<string>();     //长度（仅仅string需要）
    List<string> Remarks = new List<string>();        //列名，当注释用
    //名称
    string _TableRecordName;
    string _TableName;
    //Lua脚本所需临时字符串
    string _TitleImportStr = "--用表格名替换所有mytablename\nimport \"ByteBuffer\"\nlocal TableBase = require('Table/TableBase')\n";
    string _TableRecordStr;
    string _TableRecordCtorStr;
    string _TableRecordLoadStr;
    string _TableLoadStr;
    //C#脚本所需临时字符串
    string _CSUsingStr = "using UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\nusing System.IO;\nusing System;\n";
    string _CSDefine;
    string _CSLoad;

    static ExcelParserScript window;
    Vector2 _ScrollPosition;
    int _NeedRemoveIndex = -1;  //当前需要删除的数据的index
    bool _IsSucceed = true;     //是否生成成功
    string _ReturnVariable;     //要返回的变量

    //----------编辑器脚本------------
    #region 编辑器脚本
    [MenuItem("Tools/Excel生成C#\\Lua脚本", false, 202)]
    static void Init()
    {
        if (string.IsNullOrEmpty(CSharpOutputPath))
        {
            CSharpOutputPath = Application.dataPath + "/UtilMiddle/ExcelOutputScript/CSharp";   //C#脚本导出目录
        }
        if (string.IsNullOrEmpty(LuaOutputPath))
        {
            LuaOutputPath = Application.dataPath + "/UtilMiddle/ExcelOutputScript/Lua";     //Lua脚本导出目录
        }
        window = (ExcelParserScript)EditorWindow.GetWindow(typeof(ExcelParserScript), false, "Excel生成脚本");   //定义一个窗口对象
        window.minSize = new Vector2(500, 300);
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
        EditorGUILayout.LabelField("Excel生成脚本", headerStyle, GUILayout.Height(25));
        EditorGUILayout.Space();

        //设置要生成脚本的excel
        SetExcelPath();
        EditorGUILayout.Space();

        //导出路径选择
        SetOutputPath();
        EditorGUILayout.Space();

        //开始生成脚本
        DoParser();
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
    /// <summary>
    /// 设置要生成脚本的excel
    /// </summary>
    void SetExcelPath()
    {
        EditorGUILayout.LabelField(new GUIContent("选择要生成脚本的Excel文件:"), EditorStyles.boldLabel);
        if (Excels.Count == 0)   //生成一个空的path以供选择
        {
            Excels.Add(new ExcelOutputData());
        }
        //显示所有选项
        for (int i = 0; i < Excels.Count; i++)
        {
            var _data = Excels[i];
            //设置路径
            GUILayout.BeginHorizontal();
            string _tempPath = _data.ExcelPath;
            _tempPath = GUILayout.TextField(_tempPath, GUILayout.MinWidth(250));
            if (GUILayout.Button("...", GUILayout.Width(22)))
            {
                string projectFolder = Application.dataPath;
                _tempPath = EditorUtility.OpenFilePanelWithFilters("选择要生成脚本的Excel文件", projectFolder, FileFilters);
                if (_tempPath.Length != 0)
                {
                    _data.ExcelPath = _tempPath.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            //设置sheet页签名称
            GUILayout.BeginHorizontal();
            GUILayout.Label("设置要导出的Sheet页名称(不填默认第一页，多页用英文逗号“,”隔开)：");
            _data.SheetNames = GUILayout.TextField(_data.SheetNames, GUILayout.MinWidth(100));
            GUILayout.EndHorizontal();
            //导出lua、导出C#、“+”、“-”
            GUILayout.BeginHorizontal();
            _data.IsNeedCS = GUILayout.Toggle(_data.IsNeedCS, "导出C#");
            _data.IsNeedLua = GUILayout.Toggle(_data.IsNeedLua, "导出Lua");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.MinWidth(25)))
            {
                Excels.Add(new ExcelOutputData());  //添加一个空列
            }
            GUILayout.Space(10);
            if (GUILayout.Button("-", GUILayout.MinWidth(25)))
            {
                _NeedRemoveIndex = i;
            }
            GUILayout.EndHorizontal();
        }
        //删除“-”的数据
        if (_NeedRemoveIndex != -1)
        {
            Excels.RemoveAt(_NeedRemoveIndex);
            _NeedRemoveIndex = -1;
        }
    }
    /// <summary>
    /// 设置导出路径
    /// </summary>
    void SetOutputPath()
    {
        EditorGUILayout.LabelField("导出路径:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        CSharpOutputPath = GUILayout.TextField(CSharpOutputPath, GUILayout.MinWidth(230));
        if (GUILayout.Button("C#脚本生成位置"))
        {
            string path = "";
            path = EditorUtility.OpenFolderPanel("选择C#导出路径", Application.dataPath, "");
            if (path.Length != 0)
            {
                CSharpOutputPath = path;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        LuaOutputPath = GUILayout.TextField(LuaOutputPath, GUILayout.MinWidth(230));
        if (GUILayout.Button("Lua脚本生成位置"))
        {
            string path = "";
            path = EditorUtility.OpenFolderPanel("选择Lua导出路径", Application.dataPath, "");
            if (path.Length != 0)
            {
                LuaOutputPath = path;
            }
        }
        GUILayout.EndHorizontal();
    }
    /// <summary>
    /// 开始解析xml
    /// </summary>
    void DoParser()
    {
        EditorGUILayout.LabelField("开始生成脚本:", EditorStyles.boldLabel);
        if (GUILayout.Button("生成C#\\Lua代码"))
        {
            _IsSucceed = true;
            for (int i = 0; i < Excels.Count; i++)
            {
                DoParserExcelData(Excels[i]);
            }
            //刷新
            if (_IsSucceed)
            {
                AssetDatabase.Refresh();
                if (EditorUtility.DisplayDialog("提示", "脚本生成成功！", "确定"))
                {
                    window.Close();
                }
            }
        }
    }
    #endregion

    void DoParserTable(DataTable table)
    {
        DataNames.Clear();
        DataType.Clear();
        DataLength.Clear();
        Remarks.Clear();
        int columns = table.Columns.Count;
        for (int i = 0; i < columns; i++)   //循环所有列
        {
            var _s = table.Rows[1][i].ToString().Trim().ToLower();
            if (_s == NOCS || _s == NOC)    //如果第2行填写c或cs，则不打次列
            {
                continue;
            }
            //表class的数据load完毕后，要返回的变量，通常是第一列的id
            if (DataNames.Count == 0)
            {
                _ReturnVariable = table.Rows[2][i].ToString().Trim();
            }
            //每行对应的参数不同，第2行预留
            DataNames.Add(table.Rows[2][i].ToString().Trim());      //第3行为属性名称
            DataType.Add(table.Rows[3][i].ToString().Trim());       //第4行为类型
            DataLength.Add(table.Rows[4][i].ToString().Trim());     //第5行为长度，仅对string有用（方便与byte[]互相转换）
            Remarks.Add(table.Rows[0][i].ToString().Trim());        //第1行为中文名，做注释使用
        }
    }
    /// <summary>
    /// 读取Excel
    /// </summary>
    void DoParserExcelData(ExcelOutputData data)
    {
        FileStream stream = File.Open(data.ExcelPath, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        //解析表名
        var _tableName = data.ExcelPath.Substring(data.ExcelPath.LastIndexOf("/") + 1);
        _tableName = _tableName.Substring(0, _tableName.LastIndexOf("."));
        SetTableName(_tableName);
        //解析表内容
        if (string.IsNullOrEmpty(data.SheetNames.Trim()))
        {
            //sheetName没填默认第一个页
            var _table = result.Tables[0];
            //解析头
            DoParserTable(_table);
            //开始导出对应文件
            DoOutputFile(data);
            return;
        }
        else
        {
            var _names = data.SheetNames.Trim().Split(',');
            for (int i = 0; i < _names.Length; i++)
            {
                var _name = _names[i].Trim();
                for (int j = 0; j < result.Tables.Count; j++)
                {
                    if (result.Tables[j].TableName == _name)
                    {
                        //解析头
                        DoParserTable(result.Tables[j]);
                        //开始导出对应文件
                        DoOutputFile(data);
                        return;
                    }
                }
            }
        }
        EditorUtility.DisplayDialog("提示", "Sheet页名称错误：" + data.SheetNames, "确定");
    }
    /// <summary>
    /// 开始导出对应文件
    /// </summary>
    void DoOutputFile(ExcelOutputData data)
    {
        if (data.IsNeedCS)
        {
            if (string.IsNullOrEmpty(CSharpOutputPath))
            {
                EditorUtility.DisplayDialog("提示", "当前C#脚本导出路径不存在", "确定");
                return;
            }
            else
            {
                if (!Directory.Exists(CSharpOutputPath))
                {
                    Directory.CreateDirectory(CSharpOutputPath);
                }
                string _code = GetCSCode();
                IOTool.CreateFileString(CSharpOutputPath + "/" + _TableName + ".cs", _code);
            }
        }
        if (data.IsNeedLua)
        {
            if (string.IsNullOrEmpty(LuaOutputPath))
            {
                EditorUtility.DisplayDialog("提示", "当前Lua脚本导出路径不存在", "确定");
                return;
            }
            else
            {
                if (!Directory.Exists(LuaOutputPath))
                {
                    Directory.CreateDirectory(LuaOutputPath);
                }
                string _code = GetLuaCode();
                IOTool.CreateFileString(LuaOutputPath + "/" + _TableName + ".lua", _code);
            }
        }
        ClearData();    //一组数据导出后，清除临时数据
    }
    /// <summary>
    /// 清除临时数据
    /// </summary>
    void ClearData()
    {
        _TitleImportStr = "--用表格名替换所有mytablename\nimport \"ByteBuffer\"\n";
        _TableRecordStr = "";
        _TableRecordCtorStr = "";
        _TableRecordLoadStr = "";
        _TableLoadStr = "";

        _TableRecordName = "";
        _TableName = "";

        _CSLoad = "";
        _CSDefine = "";
    }

    /// <summary>
    /// 设置此表表明
    /// </summary>
    void SetTableName(string name)
    {
        _TableRecordName = name + "Record";
        _TableName = name + "Table";
    }
    //----------------获取lua解析数据-------------------
    #region 获取lua解析数据
    string GetLuaCode()
    {
        GetTableRecordStr();
        GetTableRecordCtorStr();
        GetTableRecordLoadStr();
        GetTableLoadStr();

        string code = _TitleImportStr + _TableRecordStr + _TableRecordCtorStr + _TableRecordLoadStr + _TableLoadStr;
        return code;
    }
    void GetTableRecordStr()
    {
        _TableRecordStr = "\n--------------------表格数据类----------------------\n";
        _TableRecordStr += _TableRecordName + " = class(\"" + _TableRecordName + "\");\n\n";
        int Columncount = DataNames.Count;
        for (int i = 0; i < Columncount; i++)
        {
            _TableRecordStr += _TableRecordName + "." + DataNames[i] + " = nil;\n";
        }
    }
    void GetTableRecordCtorStr()
    {
        _TableRecordCtorStr = "\n---------------------构造&加载-----------------------\n";
        _TableRecordCtorStr += "function " + _TableRecordName + ":ctor()\n";
        _TableRecordCtorStr += "\n";
        _TableRecordCtorStr += "end\n";
    }
    void GetTableRecordLoadStr()
    {
        _TableRecordLoadStr = "\nfunction " + _TableRecordName + ":Load(byteBuffer)\n";
        int Columncount = DataNames.Count;
        for (int i = 0; i < Columncount; i++)
        {
            string readmethod = "";
            switch (DataType[i].ToString())
            {
                case "uint":
                    readmethod = "ReadUInt()";
                    break;
                case "string":
                    readmethod = "ReadString(" + DataLength[i] + ")";
                    break;
                case "int":
                    readmethod = "ReadInt()";
                    break;
                case "byte":
                    readmethod = "ReadByte()";
                    break;
                case "ushort":
                    readmethod = "ReadUShort()";
                    break;
                case "short":
                    readmethod = "ReadShort()";
                    break;
                case "float":
                    readmethod = "ReadFloat()";
                    break;
                default:
                    continue;
            }
            _TableRecordLoadStr += "\tself." + DataNames[i] + " = byteBuffer:" + readmethod + ";\n";
        }
        _TableRecordLoadStr += "\n\treturn self." + _ReturnVariable + ";\n";
        _TableRecordLoadStr += "end\n";
    }
    void GetTableLoadStr()
    {
        _TableLoadStr = "\n----------------------表格类----------------------\n";
        _TableLoadStr += _TableName + " = class(\"" + _TableName + "\", TableBase);\n";
        _TableLoadStr += _TableName + ".__index = " + _TableName + ";\n";
        _TableLoadStr += "\nfunction " + _TableName + ":ctor()\n";
        _TableLoadStr += "\t" + _TableName + ".super.ctor(self);\n";
        _TableLoadStr += "end\n";
        _TableLoadStr += "\nfunction " + _TableName + ":Load(bytes, tableType)\n";
        _TableLoadStr += "\t" + "self.super" + ":ReleaseData();\n";
        _TableLoadStr += "\t" + "self.super" + ".mTableType = tableType;\n";
        _TableLoadStr += "\tlocal byteBuffer = ByteBuffer.New(bytes);\n";
        _TableLoadStr += "\n\tlocal countTemp = byteBuffer:ReadUInt();\n";
        _TableLoadStr += "\tfor i = 0,countTemp - 1 do\n";
        _TableLoadStr += "\t\tlocal record = " + _TableRecordName + ".new(" + _TableRecordName + ")\n";
        _TableLoadStr += "\t\tlocal Id = record:Load(byteBuffer);\n";
        _TableLoadStr += "\t\t" + "self" + ".super:OnLoadRecord(Id,record,\"" + _TableName + "\")\n";
        _TableLoadStr += "\tend\n";
        _TableLoadStr += "\n\tbyteBuffer:Close();\n";
        _TableLoadStr += "end\n";
        _TableLoadStr += "return " + _TableName;
    }
    #endregion

    //----------------解析C#脚本------------------
    #region 解析C#脚本
    void GetCSDefineAndLoad()
    {
        _CSDefine = "";
        _CSLoad = "";
        int Columncount = DataNames.Count;
        for (int i = 0; i < Columncount; i++)
        {
            string mdatatype = "";
            string readmethod = "";
            string defaltdata = "";
            switch (DataType[i])//判断变量类型
            {
                case "uint":
                    mdatatype = "uint";
                    readmethod = "byteBuffer.ReadUInt()";
                    defaltdata = "0";
                    break;
                case "string":
                    mdatatype = "string";
                    readmethod = "byteBuffer.ReadString(" + DataLength[i] + ")";
                    defaltdata = "string.Empty";
                    break;
                case "int":
                    mdatatype = "int";
                    readmethod = "byteBuffer.ReadInt()";
                    defaltdata = "0";
                    break;
                case "byte":
                    mdatatype = "byte";
                    readmethod = "byteBuffer.ReadByte()";
                    defaltdata = "0";
                    break;
                case "ushort":
                    mdatatype = "ushort";
                    readmethod = "byteBuffer.ReadUShort()";
                    defaltdata = "0";
                    break;
                case "short":
                    mdatatype = "short";
                    readmethod = "byteBuffer.ReadShort()";
                    defaltdata = "0";
                    break;
                case "float":
                    mdatatype = "float";
                    readmethod = "byteBuffer.ReadFloat()";
                    defaltdata = "0";
                    break;
                default:
                    continue;
            }

            _CSDefine += "\tpublic " + mdatatype + " " + DataNames[i] + " = " + defaltdata + ";\t//" + Remarks[i] + "\n";
            _CSLoad += "\t\t" + DataNames[i] + " = " + readmethod + ";\n";
        }
    }

    string GetCSCode()
    {
        GetCSDefineAndLoad();
        string code = _CSUsingStr;
        code += "public class " + _TableName + " : ITableLoad\n{\n";
        code += _CSDefine + "\n\tpublic uint Load(ByteBuffer byteBuffer)\n\t{\n";
        code += _CSLoad + "\n\t\treturn " + _ReturnVariable + ";\n\t}\n}";
        return code;
    }
    #endregion
}
