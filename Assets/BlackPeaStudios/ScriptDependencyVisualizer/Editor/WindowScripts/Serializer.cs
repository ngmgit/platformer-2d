using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEditor.SceneManagement;

namespace BPS
{
	public static class Serializer
	{
		static string _dynamicPath;

		public static void Init (string dynamicPath)
		{
			_dynamicPath = Application.dataPath.Replace ("Assets", dynamicPath);
			//Debug.Log ("DataPath: " + _dynamicPath);
		}

		public static void Serialize (List<VisualNode> nodes, List<VisualEdge> edges, Vector2 offset, out bool saveState)
		{
			try {
				MemoryStream m = new MemoryStream ();
				BinaryWriter bw = new BinaryWriter (m);
				bw.Write (nodes.Count);
				bw.Write (edges.Count);
				foreach (VisualNode n in nodes) {
					bw.Write (n.m_entityRect.position.x - offset.x);
					bw.Write (n.m_entityRect.position.y - offset.y);
					bw.Write (n.m_node.m_name);
					bw.Write (n.m_node.openable);
				}

				foreach (VisualEdge e in edges) {
					bw.Write (e.m_edge.m_name);
					bw.Write (e.m_edge.m_source.m_name);
					bw.Write (e.m_edge.m_target.m_name);
					bw.Write ((int)e.m_edge.m_dependencyType);
				}

				byte[] byteArray = m.ToArray ();

				string dataPath = DataPath ();
				File.WriteAllBytes (dataPath, byteArray);
				saveState = true;
			} catch {
				saveState = false;
			}
		}

		public static bool Deserialize (out List<VisualNode> nodes, out List<VisualEdge> edges)//, out Vector2 offset)
		{
			nodes = new List<VisualNode> ();
			edges = new List<VisualEdge> ();
			try {

				if (File.Exists (DataPath ())) {
				
					byte[] byteArray = File.ReadAllBytes (DataPath ());
					MemoryStream m = new MemoryStream (byteArray);
					BinaryReader br = new BinaryReader (m);

					int numNodes = br.ReadInt32 ();
					int numEdges = br.ReadInt32 ();

					for (int i = 0; i < numNodes; i++) {
						
						Vector2 position;
						position.x = br.ReadSingle ();
						position.y = br.ReadSingle ();

						string nodeName = br.ReadString ();
						Node node = new Node (nodeName, br.ReadBoolean ());

						node.m_position = position;
						VisualNode v = new VisualNode (node, null);
						nodes.Add (v);

					} 


					for (int i = 0; i < numEdges; i++) {
						string name = br.ReadString ();
						string sourceName = br.ReadString ();
						string targetName = br.ReadString ();
						DependencyType depType = (DependencyType)br.ReadInt32 ();

						DependencyEdge edge = new DependencyEdge (name, FindNodeByName (sourceName, nodes), FindNodeByName (targetName, nodes), 1f, depType);
						VisualEdge vEdge = new VisualEdge (edge);

						edges.Add (vEdge);

					}


					return true;
				} else {
					return false;
				}
			} catch {
				return false;
			}
		}

		static Node FindNodeByName (string name, List<VisualNode> nodes)
		{
			foreach (VisualNode n in nodes) {
				if (n.m_node.m_name == name)
					return n.m_node;
			}
			return null;
		}

		static string DataPath ()
		{
			//return Application.dataPath + paths [0].Replace ("Assets", "") + "/SDV_data_" + GetCurrentSceneName () + ".d";
			return _dynamicPath + "/SDV_data_" + GetCurrentSceneName () + ".d";
		}

		public static bool CurrentFileExists ()
		{
			//return System.IO.File.Exists (Application.dataPath + "/BlackPeaStudios/ScriptDependencyVisualizer/SDV_data_" + GetCurrentSceneName () + ".d");
			return System.IO.File.Exists (DataPath ());
		}

		public static void GetCurrentFiles (bool nameOnly)
		{
			string[] s = System.IO.Directory.GetFiles (Application.dataPath + "/BlackPeaStudios/ScriptDependencyVisualizer");
			foreach (string files in s) {
				if (!files.Contains (".meta") && files.Contains ("SDV_d")) {
					if (nameOnly) {
						Debug.Log (GetSceneNameFromPath (files));
					} else {
						Debug.Log (files);
					}
				}
			}
		}

		static string GetCurrentSceneName ()
		{
			try {
				string s;
//				#if !UNITY_5_4_OR_NEWER
				//s = UnityEditor.EditorApplication.currentScene;
//				#elif
				s = EditorSceneManager.GetActiveScene ().path;
//				#endif


				int extensionStart = s.IndexOf (".unity");
			
				s = s.Remove (extensionStart);
			
				string[] splitted = s.Split ("/".ToCharArray ());

				return splitted [splitted.Length - 1];
			} catch {
				Debug.Log ("GetCurrentSceneName() error: The scene has not been saved. Defaulting to fallback layout.");
				return "fallback";
			}
		}

		static string GetSceneNameFromPath (string s)
		{
			int extensionStart = s.IndexOf (".d");
			
			s = s.Remove (extensionStart);
			
			string[] splitted = s.Split ("_".ToCharArray ());
			
			return splitted [splitted.Length - 1];
		}


	}
}