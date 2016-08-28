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
		client.OnFail(handleFail);
		client.OnDisconnect(handleDisconnect);
		client.OnConnect(handleConnect);
		client.Start();
	}

	void handleWelcome(JSONNode data)
	{
		Debug.LogFormat("The server welcomed me: {0}", data["msg"]);

		JSONClass args = new JSONClass();
		args.Add("msg", "Thanks for having me simplerpc");
		client.rpc("thanks", args);
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
