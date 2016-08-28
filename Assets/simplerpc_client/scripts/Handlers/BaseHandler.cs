using UnityEngine;
using System.Collections;

using SimpleJSON;

namespace LudoGear.SimpleRPC
{
	public abstract class BaseHandler
	{
		public abstract void call(Client client, string opname, JSONNode data);
	}
	
}
