using UnityEngine;
using System.Collections;

namespace BPS
{
	public class Node
	{
		public string m_name;

		public Vector2 m_position, m_velocity = Vector2.zero, m_acceleration = Vector2.zero;
		bool _openable = false;
		public bool openable{ get { return _openable; } }
		public Node (string name, bool openable = false)
		{
			m_name = name;
			m_position = Random.insideUnitCircle * 5000f;
			_openable = openable;

		}
		public Node (Scripts script)
		{
			m_name = script.m_type.Name;
			m_position = Random.insideUnitCircle * 5000f;
			_openable = script.openable;
			
		}

		public void AddForce (Vector2 force)
		{
			m_acceleration += (force / 3f);
		}

		public void UpdateVelocity (float timeStep, float damping)
		{
			m_velocity += (m_acceleration * timeStep);
			m_velocity *= damping;
			m_acceleration = Vector2.zero;
		}

		public void UpdatePosition (float timeStep)
		{
			m_position += (m_velocity * timeStep);
		}
	}
}