using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BPS
{
	public class Graph
	{
		public List<Node> m_nodes;
		public List<Edge> m_edges;

		public Graph ()
		{
			m_nodes = new List<Node> ();
			m_edges = new List<Edge> ();
		}

		public Node AddNode (Node node)
		{
			m_nodes.Add (node);
			return node;
		}

		public Edge AddEdge (Edge edge)
		{
			m_edges.Add (edge);
			return edge;
		}

		public Node GetNode (string name)
		{
			foreach (Node n in m_nodes) {
				if (n.m_name == name)
					return n;
			}
			return null;
		}
	}

}