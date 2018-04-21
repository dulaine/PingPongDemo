using UnityEngine;
using System.Collections;

/// <summary>
/// 所有lua类型的根类型
/// </summary>
public interface ICmdXmlParser
{
    void ParserXml(string filePath, bool isOneFile = false);
}
