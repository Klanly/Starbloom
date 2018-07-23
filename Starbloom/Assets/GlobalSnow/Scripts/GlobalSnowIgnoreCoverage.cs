using UnityEngine;
using System.Collections;

namespace GlobalSnowEffect
{
    [ExecuteInEditMode]
    public class GlobalSnowIgnoreCoverage : MonoBehaviour
    {
        public bool rendHasSubmeshes;
        GlobalSnow snow;

        [HideInInspector]
        public int layer;

        void OnEnable()
        {
            snow = QuickFind.SnowHandler;
            AddToExclusionList();
        }

        void Update()
        {
            if (snow == null)
            {
                snow = QuickFind.SnowHandler;
                AddToExclusionList();
            }
            this.layer = gameObject.layer;
        }

        private void OnDisable()
        {
            if (snow != null)
                snow.UseGameObject(this);
        }


        void AddToExclusionList()
        {
            if (snow != null)
                snow.IgnoreGameObject(this);
        }
    }
}