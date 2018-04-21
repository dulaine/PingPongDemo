

using System;
using System.Collections;
using System.Windows.Forms;
using UnityEngine;

public class OpenFileManager : MonoBehaviour
{
    private static OpenFileManager _instance;

    public static OpenFileManager Instance
    {
        get
        {

            return _instance;
        }
    }

    private Renderer m_TargetRenderer;
    void Awake()
    {
        _instance = this;
    }
    public void ChooseFileFor(Renderer render)
    {
        m_TargetRenderer = render;
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
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

        m_TargetRenderer.sharedMaterial.mainTexture = wwwTexture.texture;

        //保存图片信息
        EnumAdImage imageType =
            m_TargetRenderer.gameObject.name == ResourceManager.Instance.BillboardObj.name ? EnumAdImage.Billboard : EnumAdImage.Stadium;
        SaveAdConfig(imageType, fileName);
    }


    public void LoadAd(EnumAdImage type, string filePath)
    {
        switch (type)
        {
            case EnumAdImage.Billboard:
                m_TargetRenderer = ResourceManager.Instance.GetBillboard().transform.GetComponent<Renderer>();
                break;
            case EnumAdImage.Stadium:
                m_TargetRenderer = ResourceManager.Instance.GetStadium().transform.GetComponent<Renderer>();
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
        
        StartCoroutine(WaitLoad(filePath));
    }

    private void SaveAdConfig(EnumAdImage image, string fileName)
    {
        ConfigManager.Instance.UpdateAdConfig(EnumAdImage.Billboard, fileName);
        ConfigManager.Instance.SaveAdCofig();
    }
}
