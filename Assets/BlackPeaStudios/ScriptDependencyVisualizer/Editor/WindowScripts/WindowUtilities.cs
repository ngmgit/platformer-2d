using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace BPS
{
	public class VisualNode
	{
		public Node m_node;
		public Rect m_entityRect, m_drawableRect;
		public string m_entityTitle;
		static Texture monobehaviourDotTexture;
		Rect _openableRect;

		public VisualNode (Node node, DependencyAnalytics depAnalytics)
		{

			m_entityTitle = node.m_name;
			m_entityRect = new Rect (node.m_position.x, node.m_position.y, 100, 35);
			if (depAnalytics != null)
				m_entityTitle += depAnalytics.IsSingleton (m_entityTitle) ? " (s)" : "";
		
			m_node = node;
			if (monobehaviourDotTexture == null)
				monobehaviourDotTexture = Resources.Load ("MonobehaviourDot") as Texture;

			_openableRect = m_entityRect;
			_openableRect.size = new Vector2 (10f, 10f);
			m_drawableRect = m_entityRect;
		}

		static float _sizeMultiplier = 1f;
		public static int fontSize = 8;

		public static float sizeMultiplier {
			get { //Debug.Log ("Static Property");
				return _sizeMultiplier;
			}
			set { _sizeMultiplier = value; }
		}

		public void DrawEntity (Vector2 offset)
		{
			m_entityRect.position = m_node.m_position + offset;

			float f = m_entityTitle.Length / 20f;
			fontSize = 3 - Mathf.RoundToInt (f);
			fontSize += Mathf.RoundToInt (8f * _sizeMultiplier);
			//Debug.Log (fontSize);
			GUI.skin.box.fontSize = fontSize;

			//Rect drawRect = m_entityRect;
			//drawRect.position *= sizeMultiplier;
			//drawRect.size = m_entityRect.size * sizeMultiplier;
			m_drawableRect = m_entityRect;
			m_drawableRect.position *= sizeMultiplier;
			m_drawableRect.size *= sizeMultiplier;
			GUI.Box (m_drawableRect, m_entityTitle);
			GUI.skin.box.fontSize = 11;

			if (m_node.openable) {

				_openableRect.position = m_entityRect.position;
				Rect drawRect = _openableRect;
				drawRect.position *= _sizeMultiplier;
				drawRect.size *= _sizeMultiplier;
				GUI.DrawTexture (drawRect, monobehaviourDotTexture);

			}
		}

		public static List<VisualNode> DistinctNodeList (List<VisualNode> list)
		{
			List <VisualNode> tempReturnList = new List<VisualNode> ();
			foreach (VisualNode vn in list) {
				tempReturnList.Add (list.FindAll (x => x.m_node.m_name == vn.m_node.m_name) [0]);
			}
			return tempReturnList;
		}
	}

	public class VisualEdge
	{
		public DependencyEdge m_edge;
		static Texture _arrowTexture;
		string depType = "";

		public VisualEdge (DependencyEdge edge, DependencyAnalytics depAnalytics)
		{
			m_edge = edge;
			if (_arrowTexture == null)
				_arrowTexture = Resources.Load ("ArrowIconWhite") as Texture;
			depType += depAnalytics.IsTwoWayDependency (m_edge.m_source.m_name, m_edge.m_target.m_name) ? "2" : "1";
			depType += m_edge.m_dependencyType == DependencyType.Dirty ? "*" : "";


		}

		public VisualEdge (DependencyEdge edge)
		{
			
			m_edge = edge;
			if (_arrowTexture == null)
				_arrowTexture = Resources.Load ("ArrowIconWhite") as Texture;
			depType += m_edge.m_dependencyType == DependencyType.Circular ? "2" : "1";
			depType += m_edge.m_dependencyType == DependencyType.Dirty ? "*" : "";


		}

		Vector2 FindEdgePoint (Vector2 source, Vector2 target)
		{
			Vector2 rectSize = new Vector2 (50f, 17.5f) * VisualNode.sizeMultiplier;
			float y1, y0, x1, x0;
			y1 = target.y;
			y0 = source.y;
			x1 = target.x;
			x0 = source.x;
			float m = (y1 - y0) / (x1 - x0);
			float m_centerToCorner = rectSize.y / rectSize.x;
			Vector2 vec = Vector2.zero;
			if (y1 > y0 && x1 > x0) {
				if (m > m_centerToCorner) {
					vec.y = source.y + rectSize.y;
					vec.x = (vec.y - y0 + m * x0) / m;
				} else {
					vec.x = source.x + rectSize.x;
					vec.y = m * (vec.x - x0) + y0;
				}
			}
			if (y1 >= y0 && x1 <= x0) {
				if (m < -m_centerToCorner) {
					vec.y = source.y + rectSize.y;
					vec.x = (vec.y - y0 + m * x0) / m;
				} else {
					vec.x = source.x - rectSize.x;
					vec.y = m * (vec.x - x0) + y0;
				}
			}
			if (y1 <= y0 && x1 <= x0) {
				if (m < m_centerToCorner) {
					vec.x = source.x - rectSize.x;
					vec.y = m * (vec.x - x0) + y0;
				} else {
					vec.y = source.y - rectSize.y;
					vec.x = (vec.y - y0 + m * x0) / m;
				}
			}
			if (x1 >= x0 && y1 <= y0) {
				if (m < -m_centerToCorner) {
					vec.y = source.y - rectSize.y;
					vec.x = (vec.y - y0 + m * x0) / m;
				} else {
					vec.x = source.x + rectSize.x;
					vec.y = m * (vec.x - x0) + y0;
				}
			}
			return vec;
		}

		Vector2 sourceEdge, targetEdge;

		public void DrawEdge (ref Vector2 graphOffset)
		{

			Vector2 source, target;

			source = (m_edge.m_source.m_position + (new Vector2 (50f, 17.5f)) + graphOffset) * VisualNode.sizeMultiplier;
			target = (m_edge.m_target.m_position + (new Vector2 (50f, 17.5f)) + graphOffset) * VisualNode.sizeMultiplier;

			Vector2 source2 = FindEdgePoint (source, target);
			Vector2 target2 = FindEdgePoint (target, source);

			source = source2;
			target = target2;

			sourceEdge = source;
			targetEdge = target;
			Color handlesDefaultColor = UnityEditor.Handles.color;

			if (m_edge.m_dependencyType == DependencyType.PublicInstance)
				UnityEditor.Handles.color = Color.blue;
			if (m_edge.m_dependencyType == DependencyType.PublicStatic)
				UnityEditor.Handles.color = Color.yellow;
			if (m_edge.m_dependencyType == DependencyType.PrivateInstance)
				UnityEditor.Handles.color = new Color (0f, 1f, 1f);
			if (m_edge.m_dependencyType == DependencyType.PrivateStatic)
				UnityEditor.Handles.color = new Color (1f, 1f, .7f);
			if (m_edge.m_dependencyType == DependencyType.Dirty)
				UnityEditor.Handles.color = new Color (.75f, .25f, 0f);
			if (m_edge.m_dependencyType == DependencyType.Circular)
				UnityEditor.Handles.color = Color.red;

			UnityEditor.Handles.DrawLine (source, target);
			//Vector2 labelPosition = source + (target - source) / 2f + Vector2.up * 2f;
			Vector2 labelPosition = ((target - source) / 2f) + source - Vector2.one * 7.5f;
			labelPosition.x += 20f;
			GUI.Label (new Rect (labelPosition.x, labelPosition.y, 100f, 100f), depType);
			UnityEditor.Handles.color = handlesDefaultColor;

			//DrawArrowIcon (graphOffset);

		}

		public void DrawArrowIcon (Vector2 graphOffset)
		{
			Vector2 sourceCenter = sourceEdge; 
			//+ graphOffset;
			Vector2 targetCenter = targetEdge; 
			//+ graphOffset;


			Vector2 centerPosition = 1.25f * ((targetCenter - sourceCenter) / 2f) + sourceCenter - Vector2.one * 7.5f * VisualNode.sizeMultiplier;

			float angle;
			angle = 180f + Mathf.Acos (Vector2.Dot (Vector2.up, targetCenter - sourceCenter) / (targetCenter - sourceCenter).magnitude) * Mathf.Rad2Deg;
			if (targetCenter.x > sourceCenter.x)
				angle = 360f - angle;

			if (!float.IsNaN (angle))
				GUIUtility.RotateAroundPivot (angle, centerPosition + Vector2.one * 7.5f * VisualNode.sizeMultiplier);
			GUI.DrawTexture (new Rect (centerPosition.x, centerPosition.y, 15f * VisualNode.sizeMultiplier, 15f * VisualNode.sizeMultiplier), _arrowTexture);
			
			if (!float.IsNaN (angle))
				GUIUtility.RotateAroundPivot (-angle, centerPosition + Vector2.one * 7.5f * VisualNode.sizeMultiplier);
		}
	}

	public class DependencyEdge:Edge
	{
		public DependencyType m_dependencyType;

		public DependencyEdge (string name, Node source, Node target, float stiffness, DependencyType dependencyType) : base (name, source, target, stiffness)
		{
			m_dependencyType = dependencyType;
		}

		public bool IsNodeInEdge (ref Node node)
		{
			if (m_source.m_name == node.m_name)
				return true;
			if (m_target.m_name == node.m_name)
				return true;
			return false;
		}
	}

	public class Background
	{
		public Rect m_backgroundRect;
		public ScriptDependencyWindow m_window;

		public float m_lineOffset = 13f;
		const float c_overdrawSize = 5f;

		public Background (ref ScriptDependencyWindow window)
		{
			m_window = window;
		}

		public void DrawBackground (Vector2 offset)
		{
			Vector2 size = m_window.position.size + Vector2.one * c_overdrawSize;
			Vector2 position = -Vector2.one * c_overdrawSize;

			m_backgroundRect = new Rect (position.x, position.y, size.x, size.y);
			GUISkin skin = GUI.skin;
			GUI.skin = null;
			Color defaultColor = GUI.color;
			GUI.color = new Color (.102f, .102f, .102f);
			GUI.Box (m_backgroundRect, "");
			GUI.color = defaultColor;

			DrawLines (offset);
			GUI.skin = skin;
		}

		void DrawLines (Vector2 offset)
		{
			Vector2 size = m_window.position.size;

			int horizontalLines = Mathf.FloorToInt (size.y / m_lineOffset);
			int verticalLines = Mathf.FloorToInt (size.x / m_lineOffset);

			Color defaultColor = Handles.color;
			Handles.color = new Color (.2f, .2f, .2f);

			offset.x = Mathf.FloorToInt (offset.x) % (Mathf.FloorToInt (m_lineOffset) - 1);
			offset.y = Mathf.FloorToInt (offset.y) % (Mathf.FloorToInt (m_lineOffset) - 1);
			for (int i = -1; i < horizontalLines + 2; i++) {

				Vector2 start = new Vector3 (-m_lineOffset, i * m_lineOffset);
				Vector2 end = new Vector3 (size.x + m_lineOffset, i * m_lineOffset);
				Handles.DrawLine (start + offset, end + offset);

			}
			for (int i = -1; i < verticalLines + 2; i++) {

				Vector2 start = new Vector3 (i * m_lineOffset, -m_lineOffset);
				Vector2 end = new Vector3 (i * m_lineOffset, size.y + m_lineOffset);
				Handles.DrawLine (start + offset, end + offset);
			}
			Handles.color = defaultColor;
		}
	}

	public class SettingsPage
	{
		public bool m_allowLoners = true;
		public bool m_onlyMonos = true;
		public int m_maximumCycles = 1000;

		public List< string> m_namespaces;
		public List< string> m_typeNames;

		int maxCycles = 1000;
		bool allowLoners = true, onlyMonos = true;
		List< string> namespaces;
		List< string> typeNames;
		static Texture2D buttonBackgroundTexture;
		static Texture2D buttonBackgroundTextureActive;

		public SettingsPage (ScriptDependencyWindow window)
		{
			namespaces = new List<string> ();
			typeNames = new List<string> ();

			maxCycles = PlayerPrefs.GetInt ("BPS_maxCycles", 1000);
			allowLoners = PlayerPrefs.GetInt ("BPS_allowLoners", 0) == 1 ? true : false;
			onlyMonos = PlayerPrefs.GetInt ("BPS_onlyMonos", 1) == 1 ? true : false;

			string[] a_typeNames = PlayerPrefs.GetString ("BPS_typeNames", "UnityAction").Split ("|".ToCharArray ());
			foreach (string s in a_typeNames)
				typeNames.Add (s);
			if (typeNames.Count < 2)
				typeNames.Add ("");

			string[] a_namespaces = PlayerPrefs.GetString ("BPS_namespaces", "UnityEngine|UnityEngine.UI|UnityEngine.EventSystems|System").Split ("|".ToCharArray ());
			foreach (string s in a_namespaces)
				namespaces.Add (s);
			if (namespaces.Count < 2)
				namespaces.Add ("");

			SetClean ();
			scrollPosition = new Vector2 (window.position.width, window.position.height / 2f);
			buttonBackgroundTexture = Resources.Load ("BackgroundButton") as Texture2D;
			buttonBackgroundTextureActive = Resources.Load ("BackgroundButtonActive") as Texture2D;
		}

		void ResetValues ()
		{
			namespaces = new List<string> ();
			namespaces.Add ("UnityEngine");
			namespaces.Add ("UnityEngine.UI");
			namespaces.Add ("UnityEngine.EventSystems");
			namespaces.Add ("System");

			typeNames = new List<string> ();
			typeNames.Add ("UnityAction");
			typeNames.Add ("");

			allowLoners = false;
			onlyMonos = true;

			maxCycles = 1000;

		}

		Vector2 scrollPosition;

		public void Draw (ScriptDependencyWindow window, out bool active)
		{


			GUISkin skin = GUI.skin;
			GUI.skin = null;
			Color defaultColor = GUI.color;
			GUI.color = new Color (.2f, .2f, .2f, .9f);
			GUI.Box (new Rect (0, 0, window.position.width, window.position.height), "");
			GUI.color = defaultColor;

			GUI.skin = skin;
			scrollPosition = GUILayout.BeginScrollView (scrollPosition);

			Texture2D tempNormal = GUI.skin.button.normal.background;
			Texture2D tempActive = GUI.skin.button.active.background;
			Texture2D tempHover = GUI.skin.button.hover.background;
			
			GUI.skin.button.normal.background = buttonBackgroundTexture;
			GUI.skin.button.active.background = buttonBackgroundTextureActive;
			GUI.skin.button.hover.background = buttonBackgroundTexture;

			GUILayout.BeginHorizontal ();
			GUILayout.Space (50f);

			GUILayout.BeginVertical ();

			GUILayout.Space (20f);
						
			if (GUILayout.Button (IsDirty () ? "Apply" : "Close", GUILayout.MaxWidth (120f), GUILayout.Height (30f)))
				active = false;
			else
				active = true;
			
			GUILayout.Space (20f);

			if (GUILayout.Button ("Reset to Defaults", GUILayout.MaxWidth (120f), GUILayout.Height (30f)))
				ResetValues ();
			
			GUILayout.Space (20f);

			onlyMonos = GUILayout.Toggle (onlyMonos, "Only MonoBehaviours", GUILayout.MaxWidth (200f), GUILayout.Height (30f));
			allowLoners = GUILayout.Toggle (allowLoners, "Allow Loners", GUILayout.MaxWidth (200f), GUILayout.Height (30f));

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Max Cycles", GUILayout.MaxWidth (120f), GUILayout.Height (30f));
			GUILayout.Space (50f);
			maxCycles = int.Parse (GUILayout.TextField (maxCycles.ToString (), GUILayout.Width (60f), GUILayout.Height (30f)));
			GUILayout.EndHorizontal ();


			GUILayout.Label ("Exclude Namespaces", GUILayout.MaxWidth (120f), GUILayout.Height (30f));
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+", GUILayout.MaxWidth (30f), GUILayout.Height (30f)))
				namespaces.Add ("");
			GUILayout.Space (120f);
			namespaces [0] = GUILayout.TextField (namespaces [0], GUILayout.MaxWidth (200f), GUILayout.Height (30f));
			GUILayout.EndHorizontal ();
			for (int i = 1; i < namespaces.Count; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.Space (158f);
				namespaces [i] = GUILayout.TextField (namespaces [i], GUILayout.MaxWidth (200f), GUILayout.Height (30f));
				GUILayout.EndHorizontal ();
			}

			GUILayout.Label ("Exclude Type Names", GUILayout.MaxWidth (120f), GUILayout.Height (30f));
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+", GUILayout.MaxWidth (30f), GUILayout.Height (30f)))
				typeNames.Add ("");
			GUILayout.Space (120f);
			typeNames [0] = GUILayout.TextField (typeNames [0], GUILayout.MaxWidth (200f), GUILayout.Height (30f));
			GUILayout.EndHorizontal ();
			for (int i = 1; i < typeNames.Count; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.Space (158f);
				typeNames [i] = GUILayout.TextField (typeNames [i], GUILayout.MaxWidth (200f), GUILayout.Height (30f));
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndVertical ();
			GUILayout.EndScrollView ();
			GUILayout.EndHorizontal ();

			GUI.skin.button.normal.background = tempNormal;
			GUI.skin.button.active.background = tempActive;
			GUI.skin.button.hover.background = tempHover;
		}



		public bool IsDirty ()
		{
			bool dirty = (allowLoners != m_allowLoners) || (onlyMonos != m_onlyMonos) || (maxCycles != m_maximumCycles) || !ListsEqual (namespaces, m_namespaces) || !ListsEqual (typeNames, m_typeNames);
			if (dirty) {
				return true;
			}
			return false;
		}

		public void SetClean ()
		{
			m_allowLoners = allowLoners;
			m_onlyMonos = onlyMonos;
			m_maximumCycles = maxCycles;

			m_namespaces = new List<string> (namespaces);
			m_typeNames = new List<string> (typeNames);


		}

		public void SaveSettings ()
		{
			PlayerPrefs.SetInt ("BPS_maxCycles", maxCycles);
			PlayerPrefs.SetInt ("BPS_allowLoners", allowLoners ? 1 : 0);
			PlayerPrefs.SetInt ("BPS_onlyMonos", onlyMonos ? 1 : 0);

			string tempNamespaces = "";
			for (int i = 0; i < namespaces.Count; i++) {
				tempNamespaces += namespaces [i] + (i == (namespaces.Count - 1) ? "" : "|");
			}
			PlayerPrefs.SetString ("BPS_namespaces", tempNamespaces);

			string tempNames = "";
			for (int i = 0; i < typeNames.Count; i++) {
				tempNames += typeNames [i] + (i == (typeNames.Count - 1) ? "" : "|");
			}
			PlayerPrefs.SetString ("BPS_typeNames", tempNames);
		}

		void PrintSettings ()
		{
			Debug.Log ("Allow Loners: " + m_allowLoners + "\tOnlyMonoBehaviours: " + m_onlyMonos + "\tMaximumCycles: " + m_maximumCycles);

		}

		bool ListsEqual (List<string> list1, List<string> list2)
		{
			if (list1.Count != list2.Count)
				return false;
			for (int i = 0; i < list1.Count; i++) {
				if (list1 [i] != list2 [i])
					return false;
			}
			return true;
		}

	}
}
