//using Pathfinding;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace VoidCrystal
//{
//	public abstract class IMovement : MonoBehaviour
//	{
//		#region Events
//		public event Action<Path> OnPathAcquired;
//		public event Action OnDestinationReached;

//		protected virtual void InvokeOnPathAcquired(Path _path) { if (null != OnPathAcquired) OnPathAcquired(_path); }
//		protected virtual void InvokeOnDestinationReached() { if (null != OnDestinationReached) OnDestinationReached(); }
//		#endregion

//		#region Settings
//		public float MaxSpeed = 50f;// { get; set; }
//		public float TurnSpeed = 0f;// { get; set; }

//		[Tooltip("Distance from node until arrived")]
//		public float WaypointThreshold = 1f;// { get; set; }

//		[Tooltip("Distance from destination until arrived")]
//		public float DestThreshold = 1f;// { get; set; }

//		[Tooltip("Whether repathing is allowed")]
//		public bool CanRepath;// { get; set; }

//		[Tooltip("How frequently (in seconds) should the path be recalculated")]
//		public float RepathFrequency;// { get; set; }
//		#endregion

//		#region Interface
//		public abstract void Stop();
//		public abstract void ResetPath();
//		#endregion

//		#region Feedback Info
//		public abstract Vector3 Destination { get; set; }
//		public abstract Transform Target { get; set; }
//		public abstract bool IsMoving { get; }
//		public abstract bool HasPath { get; }
//		public abstract float Speed { get; }
//		#endregion
//	}
//}