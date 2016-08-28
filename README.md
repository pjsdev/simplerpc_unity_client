# simplerpc unity client

A client for the python [simplerpc server](https://github.com/pjsdev/simplerpc). 

## How to use

```c#
using UnityEngine;

using SimpleJSON;
using LudoGear.SimpleRPC;

public class Sample : MonoBehaviour 
{
    Client client;
    MessageHandler messages;

    void Start () 
    {
        messages = new MessageHandler();
        messages.on("welcome", handleWelcome);

        client = new Client("127.0.0.1", 8000, messages);
        client.OnDisconnect(handleDisconnect);
        client.OnConnect(handleConnect);
        client.Start();
    }

    void resCB(Client _, string opname, JSONNode data)
    {
        Debug.LogFormat("Response received: {0}", opname);  
    }

    void handleWelcome(JSONNode data)
    {
        Debug.LogFormat("The server welcomed me: {0}", data["msg"]);

        JSONClass args = new JSONClass();
        args.Add("msg", "Thanks for having me simplerpc");
        client.rpc("thanks", args);

        args["msg"] = "This time with a CB";
        client.rpc("thanks", args, resCB);
    }

    void handleConnect()
    {
        Debug.Log("Connected...");
    }

    void handleDisconnect()
    {
        Debug.Log("Disconnected...");
    }

    void handleFail(JSONNode _data)
    {
        Debug.LogFormat("Failure: {0}, {1}", _data["reason"], _data["message"]);
    }
}

```
