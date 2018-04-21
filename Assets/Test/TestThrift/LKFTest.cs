using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Thrift;
using Thrift.Server;
using Thrift.Transport;
using Thrift.Protocol;
using Test;

public class LKFTest : MonoBehaviour
{
    public Transform obj;
    CommunicateTest.Client client;
    // Use this for initialization
    void Start()
    {
        Debug.Log(Time.time);
        //TTransport transport = new TSocket("127.0.0.1", 8899);
        TTransport transport = new TSocket("127.0.0.1", 9090);
        TProtocol protocol = new TBinaryProtocol(transport);
        client = new CommunicateTest.Client(protocol);
        transport.Open();
    }

    // Update is called once per frame
    void Update()
    {
        obj.Translate(Vector3.right * Time.deltaTime);
        //obj.Translate(Vector3.right * 0.02f);
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(Time.time);
            Loom.RunAsync(() =>
            {
                var s = client.echo("aaaaaaaaaaaaaaaaaaaaaaaa");
                Loom.QueueOnMainThread(() =>
                {
                    Debug.Log(Time.time);
                    Debug.Log(s);
                    var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    obj.transform.position = Vector3.zero;
                });
            });
        }
    }
}
