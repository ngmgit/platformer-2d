using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace BPS
{
	public class DependencyAnalytics
	{
		public ScriptData m_scriptData;

		public Dictionary<string, bool> m_singletonData;

		public Dictionary<string, int> m_dependencyDataIndex;

		public NodeDependencyInfo[,] m_nodeDepInfo;

		public DependencyAnalytics ()
		{
			
		}

		public DependencyAnalytics (ScriptData scriptData)
		{
			if (scriptData != null) {
				m_scriptData = scriptData;

				m_dependencyDataIndex = new Dictionary<string, int> ();

				m_singletonData = new Dictionary<string, bool> ();

				GenerateSingletonData ();
				GenerateNodeDependencyInfo ();

			}
		}

		void GenerateNodeDependencyInfo ()
		{
			if (m_scriptData.m_scripts != null) {
				int numScripts = m_scriptData.m_scripts.Count;
				m_nodeDepInfo = new NodeDependencyInfo[numScripts, numScripts];
				for (int i = 0; i < numScripts; i++) {
					try {
						m_dependencyDataIndex.Add (m_scriptData.m_scripts [i].m_type.Name, i);
					} catch {
					}
				}
				for (int i = 0; i < numScripts; i++) {
					for (int j = 0; j < numScripts; j++) {
						foreach (Type f in  m_scriptData.m_scripts[j].m_privateInstanceTypes) {
							if (f == m_scriptData.m_scripts [i].m_type)
								m_nodeDepInfo [i, j].depType = (m_nodeDepInfo [i, j].depType == DependencyType.None) ? DependencyType.PrivateInstance : DependencyType.Dirty;
						}
						foreach (Type f in  m_scriptData.m_scripts[j].m_publicInstanceTypes) {
							if (f == m_scriptData.m_scripts [i].m_type)
								m_nodeDepInfo [i, j].depType = (m_nodeDepInfo [i, j].depType == DependencyType.None) ? DependencyType.PublicInstance : DependencyType.Dirty;
						}
						foreach (Type f in  m_scriptData.m_scripts[j].m_privateStaticTypes) {
							if (f == m_scriptData.m_scripts [i].m_type)
								m_nodeDepInfo [i, j].depType = (m_nodeDepInfo [i, j].depType == DependencyType.None) ? DependencyType.PrivateStatic : DependencyType.Dirty;
						}
						foreach (Type f in  m_scriptData.m_scripts[j].m_publicStaticTypes) {
							if (f == m_scriptData.m_scripts [i].m_type) {
								m_nodeDepInfo [i, j].depType = (m_nodeDepInfo [i, j].depType == DependencyType.None) ? DependencyType.PublicStatic : DependencyType.Dirty;
								m_nodeDepInfo [i, j].singleton = true;
							}
						}

					}
				}
			}

		}

		public bool IsTwoWayDependency (string type1Name, string type2Name)
		{
			int i = 0; 
			m_dependencyDataIndex.TryGetValue (type1Name, out i);
			int j = 0; 
			m_dependencyDataIndex.TryGetValue (type2Name, out j);

			return (m_nodeDepInfo [i, j].depType != DependencyType.None) && (m_nodeDepInfo [j, i].depType != DependencyType.None);
		}




		void GenerateSingletonData ()
		{
			if (m_scriptData != null)
			if (m_scriptData.m_scripts != null)
				foreach (Scripts s in m_scriptData.m_scripts) {
					try {
						m_singletonData.Add (s.m_type.Name, false);
					} catch {
					}
					foreach (FieldInfo f in s.m_publicStaticFields) {
						if (f.FieldType == s.m_type)
							m_singletonData [s.m_type.Name] = true;
					}
				}
		}

		public bool IsSingleton (string typeName)
		{
			bool value = false;
			bool success = m_singletonData.TryGetValue (typeName, out value);
			
			if (success) {
				return value;
			} else {
				m_singletonData.Add (typeName, false);
				return false;
			}
		}
	}

	public struct NodeDependencyInfo
	{
		public NodeDependencyInfo (DependencyType depType, bool singleton)
		{
			this.depType = depType;
			this.singleton = singleton;
			edgeAssigned = false;
		}

		public DependencyType depType;
		public bool singleton;
		public bool edgeAssigned;

	}

	public enum DependencyType
	{
		None,
		PublicInstance,
		PrivateInstance,
		PublicStatic,
		PrivateStatic,
		Dirty,
		Circular
	}
}