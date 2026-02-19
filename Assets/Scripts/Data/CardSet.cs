using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame
{
    /// <summary>
    /// ScriptableObject that holds a collection of card face sprites for the memory game.
    /// Automatically generates unique IDs for each card based on sprite names.
    /// </summary>
    [CreateAssetMenu(fileName = "CardSet", menuName = "MemoryGame/CardSet")]
    public class CardSet : ScriptableObject
    {
        /// <summary>
        /// List of card face sprites. IDs will be auto-generated from sprite names.
        /// Just drop all card face sprites here.
        /// </summary>
        [Tooltip("Just drop all card face sprites here. IDs will auto-generate from names.")]
        public List<Sprite> faces = new List<Sprite>();

        /// <summary>
        /// Returns the sprite based on id (auto-generated as sprite.name).
        /// </summary>
        /// <param name="id">The ID of the card face to retrieve</param>
        /// <returns>The sprite associated with the given ID, or null if not found</returns>
        public Sprite GetFaceById(string id)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                if (faces[i] != null && faces[i].name == id)
                    return faces[i];
            }
            return null;
        }

        /// <summary>
        /// Returns a list of all IDs (auto-generated from sprite names).
        /// </summary>
        /// <returns>A list containing all card IDs</returns>
        public List<string> GetAllIds()
        {
            var list = new List<string>();
            for (int i = 0; i < faces.Count; i++)
            {
                if (faces[i] != null)
                    list.Add(faces[i].name);
            }
            return list;
        }
    }
}