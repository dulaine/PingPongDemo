

public class RpcPortManager
{
    private static  RpcPortManager _instance;
    private RPCThread m_RpcThread;
    public static RpcPortManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new RpcPortManager();
            }
            return _instance;
        }
    }

    private void Init()
    {
        m_RpcThread = new RPCThread();
    }

    private void StartThread()
    {
        if (m_RpcThread != null) m_RpcThread.Start();
    }
    private void EndThread()
    {
        if(m_RpcThread != null) m_RpcThread.SetTerminateFlag();
    }

    public void StartDataRecieving()
    {
        StartThread();
    }

    public void EndDataRecieving()
    {
        EndThread();
    }
}
