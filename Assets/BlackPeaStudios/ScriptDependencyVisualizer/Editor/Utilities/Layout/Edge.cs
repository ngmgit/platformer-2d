using UnityEngine;
using System.Collections;

namespace BPS
{
	public class Edge
	{

		public string m_name;
		public Node m_source, m_target;

		public float m_length = 130f, m_stiffness = 0f;

		public Edge (string name, Node source, Node target, float stiffness)
		{
			m_name = name;
			m_source = source;
			m_target = target;
			m_stiffness = stiffness;

		}
	}
}