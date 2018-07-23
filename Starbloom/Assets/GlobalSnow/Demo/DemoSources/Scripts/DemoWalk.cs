using UnityEngine;
using System.Collections;

namespace GlobalSnowEffect {
				public class DemoWalk : MonoBehaviour {
								GlobalSnow snow;

								void Start () {
												snow = QuickFind.SnowHandler;
								}

								void Update () {
												if (Input.GetKeyDown (KeyCode.T)) {
																snow.enabled = !snow.enabled;
												}

								}
				}
}