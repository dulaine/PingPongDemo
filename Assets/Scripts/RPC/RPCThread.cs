using System.Net.Sockets;
using System.Threading;

public class RPCThread
{
    private Thread m_thread = null;
    private bool m_terminateFlag = false;
    private System.Object m_terminateFlagMutex = null;

    //构造
    public RPCThread()
    {
        m_thread = new Thread(ThreadProc);
        m_terminateFlag = false;
        m_terminateFlagMutex = new System.Object();
    }

    //线程执行函数
    static protected void ThreadProc(object obj)
    {
        RPCThread me = (RPCThread)obj;
        me.Main();
    }

    //线程启动函数
    public void Start()
    {
        m_thread.Start(this);
    }

    //执行函数
    protected virtual void Main()
    {
        while (!IsTerminateFlagSet())
        {
            //接收信息
            //收集RandouInfo, 记录 
            //DataManager.Instance.UpdateRoundInfo();

            Thread.Sleep(5);
        }
    }

    //阻塞 等完成
    public void WaitTermination()
    {
        m_thread.Join();
    }

    //设置 结束标记
    public void SetTerminateFlag()
    {
        lock (m_terminateFlagMutex)
        {
            m_terminateFlag = true;
        }
    }

    //是否 结束
    protected bool IsTerminateFlagSet()
    {
        lock (m_terminateFlagMutex)
        {
            return m_terminateFlag;
        }
    }
}
