using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

using Excel;
using System.Data;

/// <summary>
/// Excel 解析成 byte[]
/// </summary>
public class ExcelParserData : EditorWindow
{
    /// <summary>
    /// 导出类型
    /// </summary>
    enum EExprotType
    {
        C,
        S,
    }
    /// <summary>
    /// 要导出的excel数据
    /// </summary>
    public class ExcelOutputData
    {
        public string ExcelPath = "";   //要生成的excel的路径
        public string SheetNames = "";  //此excel要生成哪个页签，不填为第一个，多个用空格隔开
    }

    //若此列数据的标识为下面的标识，则不进行数据导出
    const string NOC = "c";
    const string NOS = "s";
    const string NOCS = "cs";

    //编辑器用数据
    public string[] FileFilters = new string[] { "Excel", "xlsx" };  //能选择的文件类型
    public static List<ExcelOutputData> Excels = new List<ExcelOutputData>();  //当前已有的导出选项
    public static string OutputPath = "";       //未压缩数据导出目录
    public static string OutputPathCompress = "";       //压缩数据导出目录
    public static string OutputPathServer = "";         //服务器数据导出目录
    public static bool IsNeedCompress = true;   //是否需要压缩数据
    public static bool IsExportClient = true;   //是否导出客户端文件
    public static bool IsExportServer = true;   //是否导出服务端文件

    const int _HEADROWNUM = 5;  //前5行不是数据，而是类型名称描述等
    ByteBuffer NowBuffer = new ByteBuffer();    //此条数据的ByteBuffer

    ////excel导脚本
    //List<string> DataNames = new List<string>();      //变量名称
    //List<string> DataType = new List<string>();       //类型
    //List<string> DataLength = new List<string>();     //长度（仅仅string需要）
    //List<string> Remarks = new List<string>();        //列名，当注释用

    //名称
    string _TableName;

    static ExcelParserData window;
    Vector2 _ScrollPosition;
    int _NeedRemoveIndex = -1;  //当前需要删除的数据的index
    bool _IsSucceed = true;     //是否生成成功

    //----------编辑器脚本------------
    #region 编辑器脚本
    [MenuItem("Tools/Excel生成Byte[]数据", false, 201)]
    static void Init()
    {
        if (string.IsNullOrEmpty(OutputPath))
        {
            OutputPath = Application.dataPath + "/UtilMiddle/ExcelOutputData/Client";       //不压缩客户端数据导出目录
        }
        if (string.IsNullOrEmpty(OutputPathCompress))
        {
            OutputPathCompress = Application.dataPath + "/UtilMiddle/ExcelOutputData/ClientCompress";     //压缩客户端数据导出目录
        }
        if (string.IsNullOrEmpty(OutputPathServer))
        {
            OutputPathServer = Application.dataPath + "/UtilMiddle/ExcelOutputData/Server";     //服务器数据导出目录
        }
        window = (ExcelParserData)EditorWindow.GetWindow(typeof(ExcelParserData), false, "Excel生成数据");  //定义一个窗口对象
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
        EditorGUILayout.LabelField("Excel生成数据", headerStyle, GUILayout.Height(25));
        EditorGUILayout.Space();

        //设置要生成数据的excel
        SetExcelPath();
        EditorGUILayout.Space();

        //导出路径选择
        SetOutputPath();
        EditorGUILayout.Space();

        //选择导出数据类型
        DoExprot();
        EditorGUILayout.Space();

        //是否压缩
        DoIsCompress();
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
        EditorGUILayout.LabelField(new GUIContent("选择要生成数据的Excel文件:"), EditorStyles.boldLabel);
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
                string projectFolder = Application.dataPath + "/Resource/Table";
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
            //“+”、“-”
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.MaxWidth(25)))
            {
                Excels.Add(new ExcelOutputData());  //添加一个空列
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
        OutputPath = GUILayout.TextField(OutputPath, GUILayout.MinWidth(230));
        if (GUILayout.Button("未压缩数据生成位置"))
        {
            string path = "";
            path = EditorUtility.OpenFolderPanel("未压缩数据生成位置", Application.dataPath, "");
            if (path.Length != 0)
            {
                OutputPath = path;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        OutputPathCompress = GUILayout.TextField(OutputPathCompress, GUILayout.MinWidth(230));
        if (GUILayout.Button("压缩数据生成位置"))
        {
            string path = "";
            path = EditorUtility.OpenFolderPanel("压缩数据生成位置", Application.dataPath, "");
            if (path.Length != 0)
            {
                OutputPathCompress = path;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        OutputPathServer = GUILayout.TextField(OutputPathServer, GUILayout.MinWidth(230));
        if (GUILayout.Button("服务器数据生成位置"))
        {
            string path = "";
            path = EditorUtility.OpenFolderPanel("服务器数据生成位置", Application.dataPath, "");
            if (path.Length != 0)
            {
                OutputPathServer = path;
            }
        }
        GUILayout.EndHorizontal();
    }
    /// <summary>
    /// 是否压缩数据
    /// </summary>
    void DoIsCompress()
    {
        EditorGUILayout.LabelField("压缩客户端表数据:", EditorStyles.boldLabel);
        IsNeedCompress = GUILayout.Toggle(IsNeedCompress, "是否需要压缩客户端表数据");
    }
    /// <summary>
    /// 导出客户端或服务器可用的数据
    /// </summary>
    void DoExprot()
    {
        EditorGUILayout.LabelField("数据导出选择:", EditorStyles.boldLabel);
        IsExportClient = GUILayout.Toggle(IsExportClient, "是否导出客户端所需数据");
        IsExportServer = GUILayout.Toggle(IsExportServer, "是否导出服务器所需数据");
    }
    /// <summary>
    /// 开始解析xml
    /// </summary>
    void DoParser()
    {
        //判断是否有文件夹，没有就创建，否则直接写入会报错
        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }
        if (IsNeedCompress && !Directory.Exists(OutputPathCompress))
        {
            Directory.CreateDirectory(OutputPathCompress);
        }
        if (!Directory.Exists(OutputPathServer))
        {
            Directory.CreateDirectory(OutputPathServer);
        }
        EditorGUILayout.LabelField("开始生成数据:", EditorStyles.boldLabel);
        if (GUILayout.Button("生成二进制数据"))
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
                bool _isHaveEmpty = false;
                string _tempS = "数据生成成功！";
                for (int i = 0; i < Excels.Count; i++)
                {
                    if (string.IsNullOrEmpty(Excels[i].ExcelPath))
                    {
                        _tempS += "但是存在没有选择excel路径的选项...";
                        _isHaveEmpty = true;
                        break;
                    }
                }
                if (EditorUtility.DisplayDialog("提示", _tempS, "确定"))
                {
                    if (!_isHaveEmpty)
                    {
                        window.Close();
                    }
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// 读取Excel
    /// </summary>
    void DoParserExcelData(ExcelOutputData data)
    {
        if (string.IsNullOrEmpty(data.ExcelPath))
        {
            return;
        }
        FileStream stream = File.Open(data.ExcelPath, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        //解析头
        var _tableName = data.ExcelPath.Substring(data.ExcelPath.LastIndexOf("/") + 1);
        _tableName = _tableName.Substring(0, _tableName.LastIndexOf("."));
        SetTableName(_tableName);
        //解析表内容
        if (string.IsNullOrEmpty(data.SheetNames.Trim()))
        {
            //sheetName没填默认第一个页
            var _table = result.Tables[0];
            if (IsExportClient)
            {
                //解析头
                DoParserCount(_table);
                //开始导出对应文件
                DoOutputFile(_table, EExprotType.C);
            }
            if (IsExportServer)
            {
                //解析头
                DoParserCount(_table);
                //开始导出对应文件
                DoOutputFile(_table, EExprotType.S);
            }
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
                        if (IsExportClient)
                        {
                            //解析头
                            DoParserCount(result.Tables[j]);
                            //开始导出对应文件
                            DoOutputFile(result.Tables[j], EExprotType.C);
                        }
                        if (IsExportServer)
                        {
                            //解析头
                            DoParserCount(result.Tables[j]);
                            //开始导出对应文件
                            DoOutputFile(result.Tables[j], EExprotType.S);
                        }
                        return;
                    }
                }
            }
        }
        ClearData();
        EditorUtility.DisplayDialog("提示", "Sheet页名称错误：" + data.SheetNames, "确定");
    }
    /// <summary>
    /// 解析当前表格有几条数据
    /// </summary>
    void DoParserCount(DataTable table)
    {
        NowBuffer = new ByteBuffer();
        NowBuffer.WriteUInt((uint)(table.Rows.Count - _HEADROWNUM));
        ////解析表头，以便放入对应长度的数据
        //DataNames.Clear();
        //DataType.Clear();
        //DataLength.Clear();
        //Remarks.Clear();
        //int columns = table.Columns.Count;
        //for (int i = 0; i < columns; i++)   //循环所有列
        //{
        //    if (table.Rows[1][i].ToString().Trim().ToLower() == "null")     //如果第2行填写null，则不打次列
        //    {
        //        continue;
        //    }
        //    //每行对应的参数不同，第2行预留
        //    DataNames.Add(table.Rows[2][i].ToString().Trim());      //第3行为属性名称
        //    DataType.Add(table.Rows[3][i].ToString().Trim());       //第4行为类型
        //    DataLength.Add(table.Rows[4][i].ToString().Trim());     //第5行为长度，仅对string有用（方便与byte[]互相转换）
        //    Remarks.Add(table.Rows[0][i].ToString().Trim());        //第1行为中文名，做注释使用
        //}
    }
    /// <summary>
    /// 开始导出对应文件
    /// </summary>
    void DoOutputFile(DataTable table, EExprotType type)
    {
        if (type == EExprotType.C && string.IsNullOrEmpty(OutputPath))
        {
            EditorUtility.DisplayDialog("提示", "当前不压缩数据的导出路径不存在", "确定");
            return;
        }
        if (type == EExprotType.C && string.IsNullOrEmpty(OutputPathCompress) && IsNeedCompress)
        {
            EditorUtility.DisplayDialog("提示", "当前压缩数据的导出路径不存在", "确定");
            return;
        }
        if (type == EExprotType.S && string.IsNullOrEmpty(OutputPathServer))
        {
            EditorUtility.DisplayDialog("提示", "当前服务器数据导出路径不存在", "确定");
            return;
        }
        if (type == EExprotType.C && OutputPath == OutputPathCompress)
        {
            EditorUtility.DisplayDialog("提示", "压缩数据路径不能与不压缩数据路径相同", "确定");
            return;
        }
        int _rowCount = table.Rows.Count;
        int _column = table.Columns.Count;
        //开始循环写数据
        for (int i = _HEADROWNUM; i < _rowCount; i++)
        {
            var _row = table.Rows[i];
            for (int j = 0; j < _column; j++)
            {
                var _s = table.Rows[1][j].ToString().Trim().ToLower();
                switch (type)
                {
                    case EExprotType.C:
                        if (_s == NOCS || _s == NOC)
                        {
                            continue;
                        }
                        break;
                    case EExprotType.S:
                        if (_s == NOCS || _s == NOS)
                        {
                            continue;
                        }
                        break;
                    default:
                        break;
                }
                string _type = table.Rows[3][j].ToString();
                if (_type == "string")
                {
                    WriteData(_type, _row[j].ToString(), uint.Parse(table.Rows[4][j].ToString()));
                }
                else
                {
                    WriteData(_type, _row[j].ToString());
                }
            }
        }
        if (type == EExprotType.C && IsExportClient)
        {
            //数据写入buff完毕，将buff写入不压缩文件
            IOTool.CreateFileBytes(OutputPath + "/" + _TableName + ".bytes", NowBuffer.ToBytes());
        }
        //若是压缩选项，则开始压缩
        if (type == EExprotType.C && IsNeedCompress)
        {
            ZipTool.Zip(ZipTool.ZipType.CompressBySevenZip, null, OutputPath + "/" + _TableName + ".bytes", OutputPathCompress + "/" + _TableName + ".bytes");
        }
        if (type == EExprotType.S && IsExportServer)
        {
            IOTool.CreateFileBytes(OutputPathServer + "/" + _TableName + ".bytes", NowBuffer.ToBytes());
        }
    }
    void WriteData(string type, string data, uint length = 0)
    {
        switch (type)
        {
            case "int":
                NowBuffer.WriteInt(int.Parse(data));
                break;
            case "uint":
                NowBuffer.WriteUInt(uint.Parse(data));
                break;
            case "string":
                NowBuffer.WriteString(data, length);
                break;
            case "byte":
                NowBuffer.WriteByte(byte.Parse(data));
                break;
            case "ushort":
                NowBuffer.WriteUShort(ushort.Parse(data));
                break;
            case "short":
                NowBuffer.WriteShort(short.Parse(data));
                break;
            case "float":
                NowBuffer.WriteFloat(float.Parse(data));
                break;
        }
    }
    /// <summary>
    /// 清除临时数据
    /// </summary>
    void ClearData()
    {
        _TableName = "";
    }

    /// <summary>
    /// 设置此表表明
    /// </summary>
    void SetTableName(string name)
    {
        _TableName = name + "Data";
    }
}
