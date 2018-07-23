using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalSnowEffect
{
    public class GlobalSnowCollisionDetector : MonoBehaviour
    {

        struct SnowColliderInfo
        {
            public Vector3 position;
            public float markSize;
        }

        Dictionary<GameObject, SnowColliderInfo> collisionCache = new Dictionary<GameObject, SnowColliderInfo>();

        void OnCollisionStay(Collision collision)
        {
            GlobalSnow snow = QuickFind.SnowHandler;
            GameObject go = collision.gameObject;
            if (snow != null && go != null)
            {
                // Ensure this object is visible for the camera

                for (int i = 0; i < QuickFind.SnowHandler.snowCams.Length; i++)
                {

                    if (QuickFind.SnowHandler.snowCams[i] != null && (QuickFind.SnowHandler.snowCams[i].cullingMask & (1 << go.layer)) == 0)
                        return;

                }

                // Check gameobject position change
                Vector3 collisionPoint = go.transform.position;
                SnowColliderInfo colliderInfo;
                if (collisionCache.TryGetValue(go, out colliderInfo))
                {
                    Vector3 oldPosition = colliderInfo.position;
                    float diff = (collisionPoint - oldPosition).sqrMagnitude;
                    if (diff < 0.1f)
                        return;
                }
                SnowColliderInfo newColliderInfo;
                newColliderInfo.position = collisionPoint;
                GlobalSnowColliderExtraInfo ci = go.GetComponent<GlobalSnowColliderExtraInfo>();
                if (ci != null)
                {
                    newColliderInfo.markSize = ci.markSize;
                }
                else
                {
                    newColliderInfo.markSize = -1;
                }
                collisionCache[go] = newColliderInfo;
                ContactPoint[] cps = collision.contacts;
                if (cps != null)
                {
                    for (int k = 0; k < cps.Length && k < 5; k++)
                    {
                        ContactPoint cp = cps[k];
                        snow.MarkSnowAt(cp.point, newColliderInfo.markSize);
                    }

                }

            }
        }
    }

}