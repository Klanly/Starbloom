// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Saves the active/inactive state of a GameObject. This component should be
    /// on a different GameObject that's guaranteed to be active, or it won't
    /// take effect.
    /// </summary>
    [AddComponentMenu("")]
    public class ActiveSaver : Saver
    {

        [Serializable]
        public class Data
        {
            public bool active;
        }

        [Tooltip("GameObject to watch.")]
        [SerializeField]
        private GameObject m_gameObjectToWatch;

        public GameObject gameObjectToWatch
        {
            get { return m_gameObjectToWatch; }
            set { m_gameObjectToWatch = value; }
        }

        private Data m_data = new Data();

        public override string RecordData()
        {
            var value = (gameObjectToWatch != null) ? gameObjectToWatch.activeSelf : false;
            m_data.active = value;
            return SaveSystem.Serialize(m_data);
        }

        public override void ApplyData(string s)
        {
            if (gameObjectToWatch == null || string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s, m_data);
            if (data == null) return;
            m_data = data;
            gameObjectToWatch.SetActive(data.active);
        }

    }
}
