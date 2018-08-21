using UnityEngine;
using System.Collections;

namespace BPS
{
	public class ForceLayoutGraph
	{
		public Graph m_graph;
		public bool m_withinThreshold = false;
		public int _count;
		public int _maxCycles = 1000;

		public const float stiffness = 80f;
		const float repulsion = 80000.0f;
		const float damping = 0.5f;
		const float threshold = 1f;


		public ForceLayoutGraph (Graph graph, int maxCycles)
		{
			m_graph = graph;

			_maxCycles = maxCycles;
			_count = 0;
		}

		void Repulse ()
		{
			foreach (Node node1 in m_graph.m_nodes) {
				foreach (Node node2 in m_graph.m_nodes) {
					if (node1 != node2) {
						Vector2 delta = node1.m_position - node2.m_position;
						float distance = delta.magnitude + .1f;
						Vector2 direction = delta.normalized;

						node1.AddForce ((direction * repulsion) / (distance * .5f));
						node2.AddForce ((direction * repulsion) / (distance * -.5f));

					}
				}
			}
		}

		void Attract ()
		{
			foreach (Edge edge in m_graph.m_edges) {
				Vector2 delta = edge.m_target.m_position - edge.m_source.m_position;
				float displacement = edge.m_length - delta.magnitude;
				Vector2 direction = delta.normalized;

				edge.m_source.AddForce (direction * (edge.m_stiffness * displacement * -.5f));
				edge.m_target.AddForce (direction * (edge.m_stiffness * displacement * .5f));
			}
		
			foreach (Node node in m_graph.m_nodes) {
				Vector2 direction = node.m_position * -1.0f;

				float displacement = direction.magnitude;
				direction = direction.normalized;
				node.AddForce (direction * (stiffness * displacement * 0.4f));
			}
		}

		void UpdateVelocity (float timeStep)
		{
			foreach (Node node in m_graph.m_nodes) {
				node.UpdateVelocity (timeStep, damping);
			}
		}

		void UpdatePosition (float timeStep)
		{
			foreach (Node node in m_graph.m_nodes) {
				node.UpdatePosition (timeStep);
			}
		}

		float TotalEnergy ()
		{
			float energy = 0.0f;
			foreach (Node node in m_graph.m_nodes) {
				float speed = node.m_velocity.magnitude;
				energy += 0.5f * 3f * speed * speed;
			}
			return energy;
		}

		public void Update (float timeStep)
		{
			if (!m_withinThreshold) {
				Repulse ();
				Attract ();
				UpdateVelocity (timeStep);
				UpdatePosition (timeStep);
			}
			_count++;

			m_withinThreshold = TotalEnergy () < threshold || _count > _maxCycles;
		}
	}
}