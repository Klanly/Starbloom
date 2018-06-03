//using Pathfinding;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace VoidCrystal
//{
//	[RequireComponent(typeof(Seeker))]
//	public class PathController : MonoBehaviour
//	{
//		#region Events
//		public event Action OnPathReleased;
//		public event Action OnPathSearching;
//		public event Action<Path> OnPathReady;
//		#endregion

//		#region Settings
//		public Seeker m_Seeker;
//		public bool m_Repath = true;
//		public float m_RepathFrequency = 0.5f;
//		public Vector3 m_Destination = Vector3.zero;
//		public Transform m_Target = null;
//		public bool m_Debug = false;
		
//		public enum eTravelType
//		{
//			Destination,
//			Target
//		}
//		public eTravelType m_TravelType = eTravelType.Destination;
//		#endregion

//		#region Properties
//		// Convert target/destination for a homogenized reference
//		public Vector3 TargetDestPoint { get { return eTravelType.Destination == m_TravelType ? m_Destination : null != m_Target ? m_Target.transform.position : transform.position; } }
//		#endregion

//		#region Members
//		protected Path m_Path = null;
//		protected bool m_WaitingForPath = false;
//		protected bool m_StartHasRun = false;
//		protected bool m_CanSearchPath = false;
//		protected float m_PrevRepathTime = 0f;
//		#endregion

//		#region Unity Events
//		void Start()
//		{
//			if (null == m_Seeker)
//				m_Seeker = GetComponent<Seeker>();

//			m_StartHasRun = true;
//			m_Destination = transform.position;
//			OnEnable();
//		}

//		protected virtual void OnEnable()
//		{
//			m_PrevRepathTime = 0f;
//			m_WaitingForPath = false;
//			m_CanSearchPath = true;

//			if (m_StartHasRun)
//			{
//				//Make sure we receive callbacks when paths complete
//				m_Seeker.pathCallback += OnPathComplete;
//				StartCoroutine(SearchPaths());
//			}
//		}

//		void Update()
//		{
//		}
//		#endregion

//		#region Event Handlers
//		private void OnPathComplete(Path _path)
//		{
//			m_Path = _path;
//			m_Path.Claim(this);
//			m_WaitingForPath = false;
//			if (null != OnPathReady)
//				OnPathReady(_path);
//		}
//		#endregion

//		#region Interface
//		public void SetDestination(Vector3 _dest)
//		{
//			// Don't recalc our current destination
//			if (null != m_Path && _dest == m_Destination)
//				return;

//			m_TravelType = eTravelType.Destination;
//			m_Destination = _dest;

//			UpdatePath();
//		}

//		public void SetTarget(Transform _target)
//		{
//			m_TravelType = eTravelType.Target;
//			m_Target = _target;

//			UpdatePath();
//		}

//		public void ReleasePath()
//		{
//			if (null != m_Path)
//			{
//				m_Path.Release(this);
//				m_Path = null;
//				if (null != OnPathReleased)
//					OnPathReleased();
//			}
//		}
//		#endregion

//		#region Internal
//		IEnumerator SearchPaths()
//		{
//			while (true)
//			{
//				while (!m_Repath || m_WaitingForPath || !m_CanSearchPath || null == m_Path || Time.time - m_PrevRepathTime < m_RepathFrequency)
//					yield return null;

//				if( m_Debug )
//					Debug.Log("[PathController] Repath update");

//				//canSearchPath = false;

//				//waitingForPathCalc = true;
//				//lastRepath = Time.time;
//				//seeker.StartPath (tr.position, target.position);
//				UpdatePath();

//				yield return null;
//			}
//		}

//		public virtual void UpdatePath()
//		{
//			Path p = m_Seeker.GetCurrentPath();

//			//Cancel any eventual pending pathfinding request
//			if (p != null && !m_Seeker.IsDone())
//			{
//				p.Error();
//				// Make sure it is recycled. We won't receive a callback for this one since we
//				// replace the path directly after this
//				p.Claim(this);
//				ReleasePath();
//			}

//			m_WaitingForPath = true;
//			m_PrevRepathTime = Time.time;

//			m_Seeker.StartPath(transform.position, TargetDestPoint);

//			if (null != OnPathSearching)
//				OnPathSearching();
//		}


//		#endregion
//	}
//}