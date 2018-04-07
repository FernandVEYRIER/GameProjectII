using System;
//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Represents one game entry.
    /// </summary>
    [Serializable]
    public class GameEntry
    {
        /// <summary>
        /// Color for the wheel slice.
        /// </summary>
        public Color color;

        /// <summary>
        /// Scene to load for this item.
        /// </summary>
        public Scene gameScene;
    }
}