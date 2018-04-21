using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 版本文件比对工具
/// </summary>
public class VersionComparisonTool
{
    /// <summary>
    /// 传入旧、新文件
    /// 返回两个版本文件的增删改
    /// </summary>
    public static void Comparison(string _oldStr, string _newStr, out List<string> _add, out List<string> _del, out List<string> _diff)
    {
        Dictionary<string, string> _BaseVersionDic = new Dictionary<string, string>();  //基本版本
        Dictionary<string, string> _NewVersionDic = new Dictionary<string, string>();   //最新版本
        _add = new List<string>();
        _del = new List<string>();
        _diff = new List<string>();

        var _baseStrs = _oldStr.Split('\n');
        for (int i = 0; i < _baseStrs.Length; i++)
        {
            if (!string.IsNullOrEmpty(_baseStrs[i]))
            {
                string[] kv = _baseStrs[i].Split('|');
                _BaseVersionDic.Add(kv[0], kv[1]);
            }
        }
        var _newStrs = _newStr.Split('\n');
        for (int i = 0; i < _newStrs.Length; i++)
        {
            if (!string.IsNullOrEmpty(_newStrs[i]))
            {
                string[] kv = _newStrs[i].Split('|');
                _NewVersionDic.Add(kv[0], kv[1]);
            }
        }
        var _baseKeys = _BaseVersionDic.Keys.ToList();
        var _newKeys = _NewVersionDic.Keys.ToList();
        _add = _newKeys.Except(_baseKeys).ToList();     //新的有，旧的没有说明是新增的
        _del = _baseKeys.Except(_newKeys).ToList();     //旧的有，新的没有说明是被删除的
        var _checkKeys = _newKeys.Intersect(_baseKeys).ToList();    //存储相同的key，对比md5
        for (int i = 0; i < _checkKeys.Count; i++)
        {
            string _key = _checkKeys[i];
            if (_BaseVersionDic[_key] != _NewVersionDic[_key])
            {
                _diff.Add(_key);
            }
        }
    }
}
