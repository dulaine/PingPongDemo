

using System.Collections;
using System.Windows.Forms;
using UnityEngine;

public class OpenFileTest:MonoBehaviour
{
    public MeshRenderer render;
    public void Open()
    {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.InitialDirectory = UnityEngine.Application.streamingAssetsPath;// "file://" + UnityEngine.Application.dataPath;  //定义打开的默认文件夹位置//定义打开的默认文件夹位置
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            //显示打开文件的窗口
            Debug.Log(ofd.FileName);
            StartCoroutine(WaitLoad(ofd.FileName));
        }


    }

    IEnumerator WaitLoad(string fileName)
    {
        WWW wwwTexture = new WWW("file://" + fileName);

        Debug.Log(wwwTexture.url);

        yield return wwwTexture;

        render.material.mainTexture = wwwTexture.texture;
    }
}
