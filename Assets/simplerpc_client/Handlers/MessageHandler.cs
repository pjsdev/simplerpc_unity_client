using System;
using System.Collections.Generic;
using SimpleJSON;

namespace LudoGear.SimpleRPC
{
	public class MessageHandler : BaseHandler
	{
		Dictionary<string, List< Action<JSONNode> > > msgTable;

		public MessageHandler()
		{
			msgTable = new Dictionary<string, List<Action<JSONNode>>>();
		}

		public void on(string opname, Action<JSONNode> cb)
		{
			if(!msgTable.ContainsKey(opname))
			{
				this.msgTable[opname] = new List<Action<JSONNode>>();
			}

			this.msgTable[opname].Add(cb);
		}

		public void clear(string opname)
		{
			if(msgTable.ContainsKey(opname))
			{
				msgTable[opname].Clear();	
			}
		}

		public void clearAll()
		{
			msgTable = new Dictionary<string, List<Action<JSONNode>>>();
		}

		public override void call(Client client, string opname, JSONNode data)
		{
			if(this.msgTable.ContainsKey(opname))
			{
				foreach(var cb in this.msgTable[opname])
				{
					cb(data);
				}
			}	
		}
	}

}
