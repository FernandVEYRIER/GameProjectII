using Assets.Scripts.Editor;
using System;
using UnityEngine;

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
        public SceneField gameScene;

        /// <summary>
        /// The description of the game.
        /// </summary>
        [TextArea]
        public string Description;
    }
}