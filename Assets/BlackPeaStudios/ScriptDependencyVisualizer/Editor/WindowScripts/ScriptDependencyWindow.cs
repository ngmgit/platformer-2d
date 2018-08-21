using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace BPS
{
	public class ScriptDependencyWindow : EditorWindow
	{
		ScriptData m_scriptData;

		Graph m_graph;

		List<VisualNode> m_visualNodes;
		List<VisualEdge> m_visualEdges;

		ForceLayoutGraph m_forceData;

		float stiffness{ get { return ForceLayoutGraph.stiffness; } }

		DependencyAnalytics dependencyAnalytics;

		Background m_background;


		Vector2 targetGraphOffset = Vector2.zero;
		Vector2 graphOffset = Vector2.zero;
		VisualNode _nodeOnPointer;

		bool settingsPanelActive = false;
		SettingsPage _settingsPage;

		string searchFocus = "";
		bool searchTriggered = false, saveState = false;
		int searchVisualDurationFrames = 0;
		VisualNode focusNode;

		static GUISkin skin;
		static ScriptDependencyWindow window;

		static Texture _regenerateButtonTexture, _settingsButtonTexture, _loadLayoutButtonTexture, _saveLayoutButtonTexture, _centerButtonTexture;

		[MenuItem ("Window/Black Pea Studios/Script Dependency Visualizer")]
		public static void Init ()
		{
			window = (ScriptDependencyWindow)EditorWindow.GetWindow (typeof(ScriptDependencyWindow), false, "Visualizer");

			Serializer.Init (window.GetSavePath ());
			window.InitScriptData ();
			window.antiAlias = 4;
			window.minSize = new Vector2 (600, 450);
			window.weaveStartTime = Time.realtimeSinceStartup;
			skin = Resources.Load ("GUISKin")as GUISkin;

			LoadTextures (true);
			window.Show ();
		}

		string GetSavePath ()
		{
			var script = MonoScript.FromScriptableObject (this);
			string path = AssetDatabase.GetAssetPath (script);
			int lastIndex = path.IndexOf ("ScriptDependencyVisualizer/") + "ScriptDependencyVisualizer/".Length;

			return path.Remove (lastIndex);
		}

		static void LoadTextures (bool forced = false)
		{
			if (forced) {
				_regenerateButtonTexture = Resources.Load ("RegenerateButton") as Texture;
				_settingsButtonTexture = Resources.Load ("SettingsButton") as Texture;
				_loadLayoutButtonTexture = Resources.Load ("LoadLayoutButton") as Texture;
				_saveLayoutButtonTexture = Resources.Load ("SaveLayoutButton") as Texture;
				_centerButtonTexture = Resources.Load ("CenterButton") as Texture;
				
			} else {
				if (_regenerateButtonTexture == null)
					_regenerateButtonTexture = Resources.Load ("RegenerateButton") as Texture;
				if (_settingsButtonTexture == null)
					_settingsButtonTexture = Resources.Load ("SettingsButton") as Texture;
				if (_loadLayoutButtonTexture == null)
					_loadLayoutButtonTexture = Resources.Load ("LoadLayoutButton") as Texture;
				if (_saveLayoutButtonTexture == null)
					_saveLayoutButtonTexture = Resources.Load ("SaveLayoutButton") as Texture;
				if (_centerButtonTexture == null)
					_centerButtonTexture = Resources.Load ("CenterButton") as Texture;
			}
		}

		void ReCenter ()
		{
			targetGraphOffset = window.position.size / 2f;
		}

		void InitScriptData (bool doWork = false)
		{
			saveState = false;
			LoadTextures ();
			graphOffset = Vector2.zero;
			ReCenter ();
			if (_settingsPage == null)
				_settingsPage = new SettingsPage (window);

			m_graph = new Graph ();	
			m_background = new Background (ref window);

			if (doWork) {
				ReGenerateData ();
			} else if (Serializer.CurrentFileExists ()) {
				if (!Serializer.Deserialize (out m_visualNodes, out m_visualEdges)) {
					InitScriptData (true);
				} else {
					saveState = true;
					dependencyAnalytics = new DependencyAnalytics ();
				}
			} else
				InitScriptData (true);
		}

		void ReGenerateData ()
		{
			m_scriptData = new ScriptData (_settingsPage.m_onlyMonos, _settingsPage.m_namespaces, _settingsPage.m_typeNames);
			dependencyAnalytics = new DependencyAnalytics (m_scriptData);
			GenerateNodes ();
			GenerateEdges ();
			m_forceData = new ForceLayoutGraph (m_graph, _settingsPage.m_maximumCycles);
		}

		bool saveChecked = false;

		void OnGUI ()
		{
			try {

				graphOffset = Vector2.Lerp (graphOffset, targetGraphOffset, .05f);
				GUI.skin = skin;





				if (m_forceData != null)
				if (m_forceData.m_withinThreshold) {
					if (!saveChecked) {
						if (!Serializer.CurrentFileExists ()) {
							Serializer.Serialize (m_visualNodes, m_visualEdges, graphOffset, out saveState);
							_settingsPage.SaveSettings ();
						}
						saveChecked = true;
					} else {
						Repaint ();
						saveChecked = false;
					
					}
				}
				DrawScriptDataElements ();
			} catch {
				Init ();
			}
			try {
				DrawUIElements ();
			} catch {
				
			}
		}



		void GenerateNodes ()
		{
			m_visualNodes = new List<VisualNode> ();
			m_visualEdges = new List<VisualEdge> ();
			if (m_scriptData.m_scripts != null)
				foreach (Scripts s in m_scriptData.m_scripts) {
					//Node node = new Node (s.m_type.Name);
					Node node = new Node (s);
					m_graph.AddNode (node);
					m_visualNodes.Add (new VisualNode (node, dependencyAnalytics));
				}

		}

		void GenerateEdges ()
		{
			if (m_scriptData.m_scripts != null) {
				int size = m_scriptData.m_scripts.Count;
				NodeDependencyInfo[,] table = dependencyAnalytics.m_nodeDepInfo;
				for (int i = 0; i < size; i++) {
					for (int j = 0; j < size; j++) {
						if (i != j) {
							if (table [i, j].depType != DependencyType.None && table [j, i].depType != DependencyType.None) {
								//circular
								string targetName = m_scriptData.m_scripts [i].m_type.Name;
								string sourceName = m_scriptData.m_scripts [j].m_type.Name;
								DependencyEdge e = new DependencyEdge (sourceName + " ---> " + targetName, m_graph.GetNode (sourceName), m_graph.GetNode (targetName), stiffness, table [i, j].depType = DependencyType.Circular);
								m_graph.AddEdge (e);
								m_visualEdges.Add (new VisualEdge (e, dependencyAnalytics));

							} else if (table [i, j].depType != DependencyType.None) {
								string targetName = m_scriptData.m_scripts [i].m_type.Name;
								string sourceName = m_scriptData.m_scripts [j].m_type.Name;
								DependencyEdge e = new DependencyEdge (sourceName + " ---> " + targetName, m_graph.GetNode (sourceName), m_graph.GetNode (targetName), stiffness, table [i, j].depType);
								m_graph.AddEdge (e);
								m_visualEdges.Add (new VisualEdge (e, dependencyAnalytics));
							}

						}
					}
				}
		

				if (!_settingsPage.m_allowLoners) {
					List<VisualNode> tempList = new List<VisualNode> (m_visualNodes);
					foreach (VisualNode vn in m_visualNodes) {
						bool hasEdge = false;
						foreach (VisualEdge v in m_visualEdges) {
							if (v.m_edge.IsNodeInEdge (ref vn.m_node))
								hasEdge = true;
						}

						if (!hasEdge) {
							tempList.Remove (vn);
						}
					}
					tempList = VisualNode.DistinctNodeList (tempList);
					m_visualNodes = tempList;
				}
			}
		}

		void DrawScriptDataElements ()
		{
			m_background.DrawBackground (graphOffset);

			if (m_forceData != null) {
				m_forceData.Update (.05f);
			}
			foreach (VisualEdge edge in m_visualEdges) {
				edge.DrawEdge (ref graphOffset);
			}
			foreach (VisualEdge edge in m_visualEdges) {
				edge.DrawArrowIcon (graphOffset);
			}
			foreach (VisualNode n in m_visualNodes) {
				n.DrawEntity (graphOffset);
			}
		}

		//float zoomSlider = 1f;
		float weaveStartTime = -1f;

		void DrawUIElements ()
		{
			Event e = Event.current;
			Rect settingsButtonRect = new Rect (10f, position.size.y * (.15f + .1f), 45f, 45f);
			Rect centerRect = new Rect (10f, position.size.y * (.15f + .2f), 45f, 45f);
			Rect generateRect = new Rect (10f, position.size.y * (.15f + .3f), 45f, 45f);
			Rect saveRect = new Rect (10f, position.size.y * (.15f + .4f), 45f, 45f);
			Rect restoreRect = new Rect (10f, position.size.y * (.15f + .5f), 45f, 45f);

			Vector2 mousePosition = Event.current.mousePosition;
			//Tooltips
			if (settingsButtonRect.Contains (mousePosition))
				GUI.Box (new Rect (new Vector2 (settingsButtonRect.xMax, settingsButtonRect.yMin), new Vector2 (70f, 25f)), "Settings");
			if (centerRect.Contains (mousePosition))
				GUI.Box (new Rect (new Vector2 (centerRect.xMax, centerRect.yMin), new Vector2 (70f, 25f)), "Re-Center");
			if (generateRect.Contains (mousePosition))
				GUI.Box (new Rect (new Vector2 (generateRect.xMax, generateRect.yMin), new Vector2 (70f, 25f)), "Regenerate");
			if (saveRect.Contains (mousePosition))
				GUI.Box (new Rect (new Vector2 (saveRect.xMax, saveRect.yMin), new Vector2 (80f, 25f)), "Save Layout");
			if (restoreRect.Contains (mousePosition))
				GUI.Box (new Rect (new Vector2 (restoreRect.xMax, restoreRect.yMin), new Vector2 (80f, 25f)), "Load Layout");

			GUI.DrawTexture (new Rect (10f, 10f, 100f, 100f), Resources.Load ("AssetLogo")as Texture);

			if (GUI.Button (settingsButtonRect, _settingsButtonTexture) || settingsPanelActive) {
				if (_settingsPage == null)
					_settingsPage = new SettingsPage (window);
				_settingsPage.Draw (window, out settingsPanelActive);
				if (!settingsPanelActive && _settingsPage.IsDirty ()) {
					_settingsPage.SetClean ();
					InitScriptData (true);
				}

			} else {
				if (GUI.Button (centerRect, _centerButtonTexture))// "Center"))
					ReCenter ();
				if (GUI.Button (generateRect, _regenerateButtonTexture)) {// "Generate"))
					weaveStartTime = Time.realtimeSinceStartup;
					InitScriptData (true);
				}
				if (GUI.Button (saveRect, _saveLayoutButtonTexture)) {// "SaveLayout")) {
					if (m_forceData != null)
						m_forceData._count = m_forceData._maxCycles;
					Serializer.Serialize (m_visualNodes, m_visualEdges, graphOffset, out saveState);
					_settingsPage.SaveSettings ();
				}
				if (GUI.Button (restoreRect, _loadLayoutButtonTexture)) {// "RestoreLayout")) {
					InitScriptData ();
				}
			}
			if (m_forceData != null) {
				Color color = GUI.color;
				GUISkin skin = GUI.skin;
				GUI.color = Color.black;
				GUI.skin = null;
				GUI.Box (new Rect (0f, position.size.y - 30f, position.size.x * .2f + 100f + (saveState ? 40f : 0f), 30f), "");
				GUI.skin = skin;
				GUI.color = color;
				GUI.Box (new Rect (10f, position.size.y - 20f, position.size.x * .2f, 10f), "");
				GUI.color = Color.green;
				GUI.Box (new Rect (10f, position.size.y - 20f, position.size.x * .2f * (m_forceData.m_withinThreshold ? 1f : ((float)m_forceData._count / m_forceData._maxCycles)), 10f), "");
				GUI.color = Color.white;

				GUI.Label (new Rect (position.size.x * .2f + 30f, position.size.y - 25f, 100f, 50f), m_forceData.m_withinThreshold ? "Complete" : Mathf.RoundToInt (((float)m_forceData._count / m_forceData._maxCycles) * 100) + "%");
				GUI.Label (new Rect (position.size.x * .2f + 100f, position.size.y - 25f, 100f, 50f), saveState ? "Saved" : "");
				if (!m_forceData.m_withinThreshold) {

					GUIStyle savedStyle = GUIStyle.none;

					GUI.Box (new Rect (0f, 0f, position.size.x, position.size.y), "");
					Rect weavingRect = new Rect (position.size / 2f + new Vector2 (-75f, -50f - (position.size.y / 4f - 45f) - 20f), new Vector2 (200f, 50f));
					GUI.Box (weavingRect, "");
					if (Mathf.RoundToInt (Time.realtimeSinceStartup - weaveStartTime) % 2 == 0) {
						savedStyle.alignment = TextAnchor.MiddleCenter;
						savedStyle.fontSize = 20;
						savedStyle.normal.textColor = Color.white;
						GUI.Label (weavingRect, "Weaving...", savedStyle);
					}

					if (GUI.Button (new Rect (position.size.x * .2f + 60f, position.size.y - 25f, 20f, 20f), Resources.Load ("StopIcon") as Texture))
						m_forceData._count = m_forceData._maxCycles;
					Repaint ();

					if (e.button == 0)
						targetGraphOffset += e.delta / 2f;
					
					return;


				}
				GUI.color = color;
			}

			if (!settingsPanelActive) {
				float posX = position.size.x;
				#region zoomCode
	
				float sizeMultiplier = VisualNode.sizeMultiplier;
				Rect zoomOutRect = new Rect (posX - 202f, 70f, 50f, 50f);
				Rect zoomInRect = new Rect (posX - 62f, 70f, 50f, 50f);
				if (GUI.Button (zoomInRect, "+")) {
					if (sizeMultiplier < 1.2f) {
						sizeMultiplier += .1f;

						targetGraphOffset += Vector2.one * (-60f);
						graphOffset = targetGraphOffset;
					} else
						sizeMultiplier = 1.2f;
				}
				if (GUI.Button (zoomOutRect, "-")) {
					if (sizeMultiplier > .5f) {
						sizeMultiplier -= .1f;
						targetGraphOffset += Vector2.one * (60f);
						graphOffset = targetGraphOffset;
					} else
						sizeMultiplier = .5f;
				}
				VisualNode.sizeMultiplier = sizeMultiplier;

				GUIStyle sizeMultGuiStyle = GUIStyle.none;
				sizeMultGuiStyle.fontSize = 15;
				sizeMultGuiStyle.fontStyle = FontStyle.Bold;
				sizeMultGuiStyle.alignment = TextAnchor.MiddleCenter;
				sizeMultGuiStyle.normal.textColor = Color.white;
				GUI.Label (new Rect (zoomOutRect.xMax + (zoomInRect.xMin - zoomOutRect.xMax) / 4f, 70f, 50f, 50f), ((Mathf.Round (sizeMultiplier * 100f * 2f - 100f)) > 0 ? (Mathf.Round (sizeMultiplier * 100f * 2f - 100f)) : 1).ToString () + '%', sizeMultGuiStyle);

				#endregion


				bool alreadyOpened = false;
				if (e.clickCount == 2) {
					foreach (VisualNode n in m_visualNodes) {
						if (n.m_drawableRect.Contains (e.mousePosition)) {
							if (n.m_node.openable) {

								string[] guids = AssetDatabase.FindAssets (n.m_node.m_name);
								string[] paths = new string[guids.Length];
								for (int i = 0; i < guids.Length; i++) {
									paths [i] = AssetDatabase.GUIDToAssetPath (guids [i]);
								}
								foreach (string path in paths) {
									string[] splits = path.Split ("/".ToCharArray ());
									if (splits [splits.Length - 1] == n.m_node.m_name + ".cs" || splits [splits.Length - 1] == n.m_node.m_name + ".js") {
										if (!alreadyOpened) {
											UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal (path, 1);
											alreadyOpened = true;
											break;
										}
									}
								}
							}
						}
						if (alreadyOpened)
							break;
					}
				}
				if (e.button == 0) {
					if (e.type == EventType.MouseDown) {
						foreach (VisualNode n in m_visualNodes) {
							if (n.m_drawableRect.Contains (e.mousePosition)) {
								_nodeOnPointer = n;
							}
						}
					}
				
					if (e.type == EventType.MouseUp) {
						_nodeOnPointer = null;
					}
					if (_nodeOnPointer == null)
						targetGraphOffset += e.delta / 2f;
					else {
						_nodeOnPointer.m_node.m_position += e.delta / 2f / VisualNode.sizeMultiplier;
						saveState = false;
						Color defaultColor = GUI.color;
						GUI.color = Color.yellow;
						GUI.skin.box.fontSize = VisualNode.fontSize;
						GUI.Box (_nodeOnPointer.m_drawableRect, _nodeOnPointer.m_entityTitle);
						GUI.color = defaultColor;
					}
					Repaint ();
				}

				searchFocus = GUI.TextField (new Rect (posX - 202f, 10f, 150f, 45f), searchFocus);
				if (GUI.Button (new Rect (posX - 54.5f, 10.5f, 43f, 43f), Resources.Load ("SearchButton") as Texture)) {//, "Find")) {
					foreach (VisualNode node in m_visualNodes)
						if (node.m_node.m_name.Contains (searchFocus)) {
							targetGraphOffset = targetGraphOffset - node.m_entityRect.position + window.position.size / 2f;
							searchVisualDurationFrames = 0;
							searchTriggered = true;
							focusNode = node;
							break;
						}
				}
				if (searchTriggered) {
					if (searchVisualDurationFrames < 200) {
						Color defaultColor = GUI.color;
						GUI.color = Color.yellow;
						GUI.skin.box.fontSize = VisualNode.fontSize;
						GUI.Box (focusNode.m_drawableRect, focusNode.m_entityTitle);
						GUI.color = defaultColor;
						searchVisualDurationFrames++;
					} else {
						searchVisualDurationFrames = 0;
						searchTriggered = false;
					}
				}


			}
		}
	}
}