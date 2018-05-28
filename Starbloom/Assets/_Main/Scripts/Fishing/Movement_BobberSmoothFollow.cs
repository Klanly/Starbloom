using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VC_Movement
{
    public class Movement_BobberSmoothFollow : MonoBehaviour
    {
        public Transform Bobber = null;
        public float SlerpSpeed = .1f;



        void Update()
        {
            if (Bobber == null)
                return;

            Vector3 SlerpPos = Vector3.Slerp(transform.position, Bobber.position, SlerpSpeed);
            transform.position = SlerpPos;
        }


    }
}
