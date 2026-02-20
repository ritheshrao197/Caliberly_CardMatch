using System;
using System.Collections.Generic;
using MemoryGame.Config;
using MemoryGame.Controller;
using UnityEngine;

namespace MemoryGame.Runtime
{
    /// <summary>
    /// Owns board construction lifecycle and card pooling.
    /// </summary>
    public class BoardRuntime
    {
        private readonly BoardController _boardController;

        public BoardRuntime(
            Transform boardRoot,
            CardSet cardSet,
            CardController cardPrefab,
            BoardFrame frame,
            GameConfig config)
        {
            if (boardRoot == null) throw new ArgumentNullException(nameof(boardRoot));
            if (cardSet == null) throw new ArgumentNullException(nameof(cardSet));
            if (cardPrefab == null) throw new ArgumentNullException(nameof(cardPrefab));

            var pool = new ObjectPool<CardController>(cardPrefab, boardRoot, 0);
            _boardController = new BoardController(boardRoot, cardSet, pool, frame, config);
        }

        public IReadOnlyList<CardController> Build(int rows, int cols)
        {
            _boardController.Build(rows, cols);
            return _boardController.Cards;
        }

        public void Clear()
        {
            _boardController.ClearAll();
        }
    }
}
