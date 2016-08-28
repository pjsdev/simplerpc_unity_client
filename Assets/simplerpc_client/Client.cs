using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

using SimpleJSON;

namespace LudoGear.SimpleRPC 
{
	public class Client 
	{
		public BaseHandler handler;

		const int readSize = 1024;
		private byte[] readBuffer = new byte[readSize];
		TcpClient tcp;
		System.Action<JSONNode> failCallback;
		System.Action disconnectCallback;
		System.Action connectCallback;
		string dangling = "";

		string host;
		int port;

		public Client (string host, int port, BaseHandler handler) 
		{
			tcp = new TcpClient();
			this.host = host;
			this.port = port;
			this.handler = handler;
		}

		public void Start()
		{
			tcp.BeginConnect(host, port, new AsyncCallback(ConnectCallback), tcp);
		}

		void ConnectCallback(IAsyncResult ar)
		{
			if(connectCallback != null)
			{
				connectCallback();
			}

			tcp.GetStream().BeginRead(readBuffer, 0, readSize, new AsyncCallback(DoRead), null);
		}

		void OnApplicationQuit()
		{
			tcp.Close();
		}

		public void OnFail(System.Action<JSONNode> cb)
		{
			failCallback = cb;	
		}

		public void OnConnect(System.Action cb)
		{
			connectCallback = cb;
		}

		public void OnDisconnect(System.Action cb)
		{
			disconnectCallback = cb;
		}
			
		public void rpc(string opname, JSONNode data)
		{
			string payload = Payload.toString(opname, data);
			this.SendData(payload);
		}

		private void SendData(string data)
		{
			StreamWriter writer = new StreamWriter(tcp.GetStream());
			writer.Write(data);
			writer.Flush();
		}

		private void DoRead(IAsyncResult ar)
		{ 
			int BytesRead;
			try
			{
				// Finish asynchronous read into readBuffer and return number of bytes read.
				BytesRead = tcp.GetStream().EndRead(ar);
				if (BytesRead < 1) 
				{
					tcp.Close();
					if(disconnectCallback != null)
					{
						disconnectCallback();
					}
					return;
				}
					
				string buf = Encoding.ASCII.GetString(readBuffer, 0, BytesRead);
				List<string> pkgs = new List<string>();

				// extract packages from this buffer
				while(true)
				{
					int eof = buf.IndexOf('\n');
					if(eof == -1) // no end of msg found
					{
						dangling += buf;
						break;
					}

					string pkg = dangling + buf.Substring(0, eof);
					buf = buf.Substring(eof+1);
					dangling = "";

					pkgs.Add(pkg);
				}
					
				// dispatch each pkg to handler
				foreach(string pkg in pkgs)
				{
					RPCData rpc = Payload.fromString(pkg);

					if(rpc.opname == "FAIL" && failCallback != null)
					{
						failCallback(rpc.data);	
					}
					else 
					{
						this.handler.call(this, rpc.opname, rpc.data);
					}
				}

				// Start a new asynchronous read into readBuffer.
				tcp.GetStream().BeginRead(readBuffer, 0, readSize, new AsyncCallback(DoRead), null);
			} 
			catch (Exception e)
			{
				Debug.Log("Error during read");
				Debug.Log(e);
			}
		}
	}
}

