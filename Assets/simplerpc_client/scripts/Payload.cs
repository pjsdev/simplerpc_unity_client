using UnityEngine;
using System.Collections;

using SimpleJSON;

namespace LudoGear.SimpleRPC
{
	public class RPCData 
	{
		public string opname;
		public JSONNode data;
	}

	public class Payload
	{	
		public static string toString(string opname, JSONNode data)
		{
			return string.Format("{0}{1}\n", opname, data.ToString());
		}

		public static RPCData fromString(string _data)
		{
			RPCData rpc = new RPCData();

			int json_start = _data.IndexOf("{");
			rpc.opname = _data.Substring(0, json_start);
			rpc.data = JSON.Parse(_data.Substring(json_start));

			return rpc;
		}
	}	
}
