using System.Collections.Generic;
using MemoryGame;
using MemoryGame.Config;
using MemoryGame.Events;
using UnityEngine;
using MemoryGame.Constants;
using System.Linq;
using UnityEngine.Pool;

namespace MemoryGame.Controller
{
    /// <summary>
    /// Controller responsible for building and managing the game board.
    /// Handles card placement, layout, and board configuration.
    /// </summary>
    public class BoardController
    {
        private readonly Transform _root;
        private readonly GameConfig _cfg;
        private readonly CardSet _set;
        private readonly ObjectPool<CardController> _pool;
        private readonly BoardFrame _frame; 

        private readonly BoardSettings _settings = new BoardSettings();

        /// <summary>
        /// List of all card controllers on the board
        /// </summary>
        public List<CardController> Cards { get; } = new List<CardController>();



        /// <summary>
        /// Constructor for the BoardController
        /// </summary>
        /// <param name="root">Transform to parent all cards under</param>
        /// <param name="cfg">Game configuration settings</param>
        /// <param name="set">Card set containing available card faces</param>
        /// <param name="pool">Object pool for card controllers</param>
        /// <param name="frame">Optional board frame for layout constraints</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public BoardController(Transform root, GameConfig cfg, CardSet set, ObjectPool<CardController> pool, BoardFrame frame,BoardSettings settings)
        {
            _root = root; _cfg = cfg; _set = set; _pool = pool; _frame = frame;
            _settings = settings;
        }

        /// <summary>
        /// Build or rebuild the board using only the required number of cards.
        /// Reuses already-instantiated cards when possible, and returns extras to the pool.
        /// </summary>
        /// <param name="rows">Number of rows in the board grid</param>
        /// <param name="cols">Number of columns in the board grid</param>
        public void Build(int rows, int cols)
        {
            int totalSlots = rows * cols;
            int usable = (totalSlots / 2) * 2; // enforce even count
            int pairs = usable / 2;

            if (usable != totalSlots)
            {
                Debug.LogWarning($"[BoardController] Odd grid detected ({rows}x{cols}={totalSlots}). Using {usable} cards (pairs={pairs}) and leaving {totalSlots - usable} empty slot(s).");
            }

            // Prepare ids
            var ids = PickIds(pairs);
            Shuffle(ids);

            // Pool sizing
            while (Cards.Count < usable)
            {
                var card = _pool.Get();
                card.transform.SetParent(_root, false);
                Cards.Add(card);
            }
            if (Cards.Count > usable)
            {
                for (int i = Cards.Count - 1; i >= usable; i--)
                {
                    _pool.Release(Cards[i]);
                    Cards.RemoveAt(i);
                }
            }

            if (_frame != null && _frame.HasBounds(out var inner))
            {
                LayoutInsideRect(inner, rows, cols, ids);

            }
            else
            {
                float scale = CalculateCardScale(rows, cols);
                for (int i = 0; i < usable; i++)
                {
                    var card = Cards[i];
                    var pos = IndexToLocal(i, rows, cols, _cfg.DefaultCellX, _cfg.DefaultCellY);
                    card.transform.localPosition = pos;
                    card.transform.localScale = Vector3.one * scale;

                    var id = ids[i];
                    var face = _set.GetFaceById(id);
                    card.Init(id, face, startFaceUp: false);
                    card.flipDuration = _cfg.FlipDuration;
                }
            }
            if (_frame)
            {
                float scale = CalculateFramecale(rows);
                _frame.transform.localScale = Vector3.one * scale;
                Debug.Log($"[BoardController] Layout with BoardFrame at scale {scale:F2}");
            }
            EventBus.Instance.Publish(new BoardBuiltEvent(pairs));
        }

        /// <summary>
        /// Layout cards within a defined rectangular area
        /// </summary>
        /// <param name="innerWorld">The rectangular area to layout cards within</param>
        /// <param name="rows">Number of rows in the grid</param>
        /// <param name="cols">Number of columns in the grid</param>
        /// <param name="ids">List of card IDs to assign</param>
        private void LayoutInsideRect(Rect innerWorld, int rows, int cols, List<string> ids)
        {
            int totalSlots = rows * cols;
            int usable = (totalSlots & 1) == 1 ? totalSlots - 1 : totalSlots; // ensure even

            // Cell size from inner rect
            float cellW = innerWorld.width / cols;
            float cellH = innerWorld.height / rows;

            // Dynamic extra padding for tighter grids (prevents edge bleed in 5×5+)
            // Effective padding = boardFrame.cardPadding + a small grid-based term
            float gridPad = Mathf.Clamp01(_settings.GridPaddingFactor * Mathf.Max(rows, cols));   // ~0.06 at 5x5
            float pad = Mathf.Clamp01((_frame != null ? _frame.cardPadding : _settings.DefaultPadding) + gridPad);

            float boxW = cellW * (1f - pad);
            float boxH = cellH * (1f - pad);

            for (int i = 0; i < usable; i++)
            {
                int r = i / cols;
                int c = i % cols;

                // Center of each cell in world space
                float cx = innerWorld.xMin + (c + 0.5f) * cellW;
                float cy = innerWorld.yMax - (r + 0.5f) * cellH;
                Vector3 worldPos = new Vector3(cx, cy, 0f);
                Vector3 localPos = _root.InverseTransformPoint(worldPos);

                var card = Cards[i];
                card.transform.localPosition = localPos;

                // Make sure child renderers are at scale 1 to avoid double scaling
                card.view?.NormalizeChildScale();

                // Measure natural size at scale 1 (see helper below)
                Vector2 nat = GetCardNaturalSize(card);
                float scale = Mathf.Min(boxW / nat.x, boxH / nat.y);
                card.transform.localScale = Vector3.one * scale;

                // Init content
                var id = ids[i];
                var face = _set.GetFaceById(id);
                card.Init(id, face, false);
                card.flipDuration = _cfg.FlipDuration;
            }
        }

        /// <summary>
        /// Gets the natural size of a card at scale 1
        /// </summary>
        /// <param name="card">The card controller to measure</param>
        /// <returns>The natural size of the card</returns>
        private  Vector2 GetCardNaturalSize(CardController card)
        {
            var view = card.view;
            if (view != null)
            {
                if (view.back && view.back.sprite)
                {
                    // sprite.bounds.size is in local sprite units; multiply by child localScale if not 1
                    var s = view.back.sprite.bounds.size;
                    var ls = view.back.transform.localScale;
                    return new Vector2(s.x * ls.x, s.y * ls.y);
                }
                if (view.face && view.face.sprite)
                {
                    var s = view.face.sprite.bounds.size;
                    var ls = view.face.transform.localScale;
                    return new Vector2(s.x * ls.x, s.y * ls.y);
                }
            }
            // conservative default aspect (taller than wide)
            return new Vector2(_settings.DefaultCardWidth, _settings.DefaultCardHeight);
        }

        /// <summary>
        /// Clears all cards from the board and returns them to the pool
        /// </summary>
        public void ClearAll()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                _pool.Release(Cards[i]);
            }
            Cards.Clear();
        }

        /// <summary>
        /// Picks a specified number of unique card IDs from the available set
        /// </summary>
        /// <param name="pairCount">Number of pairs (unique IDs) to pick</param>
        /// <returns>List of IDs, with each ID appearing twice for pairing</returns>
        private List<string> PickIds(int pairCount)
        {
            // FIX: Use Distinct() to ensure the source list contains only truly unique IDs.
            // If _set.GetAllIds() accidentally returned duplicates (e.g., from multiple sources),
            // this prevents those duplicates from being considered for the 'unique' list.
            var uniqueSource = _set.GetAllIds().Distinct().ToList();

            // 1. Pick the unique IDs
            var unique = new List<string>();

            // Shuffle the unique source list before picking
            Shuffle(uniqueSource);

            // Pick the required number of pairs (unique IDs)
            for (int i = 0; i < pairCount && i < uniqueSource.Count; i++)
                unique.Add(uniqueSource[i]);

            // 2. Create the final list by adding each unique ID twice
            var list = new List<string>(pairCount * 2);
            foreach (var id in unique)
            {
                list.Add(id); list.Add(id);
            }

            Debug.Log($"[BoardController] Final Board IDs (pre-shuffle): {string.Join(", ", list)}");

            return list;
        }

        /// <summary>
        /// Calculates an appropriate scale for cards based on grid dimensions
        /// </summary>
        /// <param name="rows">Number of rows in the grid</param>
        /// <param name="cols">Number of columns in the grid</param>
        /// <returns>Scale factor for cards</returns>
        private float CalculateCardScale(int rows, int cols)
        {
            // Base reference: 4x4 grid = scale 1.0, shrink when more cells
            int maxDim = Mathf.Max(rows, cols);
            return Mathf.Clamp(4f / maxDim, 0.4f, 1f);
        }

        /// <summary>
        /// Converts a linear index to a local position in the grid
        /// </summary>
        /// <param name="index">Linear index of the card</param>
        /// <param name="rows">Number of rows in the grid</param>
        /// <param name="cols">Number of columns in the grid</param>
        /// <param name="dx">Horizontal spacing between cards</param>
        /// <param name="dy">Vertical spacing between cards</param>
        /// <returns>Local position for the card</returns>
        private static Vector3 IndexToLocal(int index, int rows, int cols, float dx, float dy)
        {
            int r = index / cols;
            int c = index % cols;
            float width = (cols - 1) * dx;
            float height = (rows - 1) * dy;
            float x = -width * 0.5f + c * dx;
            float y = height * 0.5f - r * dy;
            return new Vector3(x, y, 0);
        }

        /// <summary>
        /// Shuffles a list in place using the Fisher-Yates shuffle algorithm
        /// </summary>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        /// <param name="list">List to shuffle</param>
        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        /// <summary>
        /// Calculates an appropriate scale for cards based on grid dimensions using a dynamic formula.
        /// This formula is designed to approximate the requested scaling factors (1.7 for 2x2, 0.85 for 4x4, 0.55 for 7x7)
        /// while ensuring smooth scaling for all other grid sizes.
        /// </summary>
        /// <param name="rows">Number of rows in the grid</param>
        /// <param name="cols">Number of columns in the grid</param>
        /// <returns>Scale factor for cards</returns>
        private float CalculateFramecale(int rowCount)
        {

            // Safety check: Avoid division by zero, though unlikely with grid dimensions
            if (rowCount < 1)
            {
                return 1.0f;
            }

            // Formula: S = 3.4 / (D + 0.1)
            // This formula is specifically tuned to hit the requested scale points:
            // D=2  -> 3.4 / 2.1  ~ 1.61 (Close to 1.7)
            // D=4  -> 3.4 / 4.1  ~ 0.83 (Close to 0.85)
            // D=7  -> 3.4 / 7.1  ~ 0.48 (Close to 0.55)

            // To hit 1.7 more accurately for small grids, we can adjust the numerator slightly:
            // Let's use 3.5 / (D + 0.1) for better small-grid fit and clamp it.

            // The divisor offset (0.1) prevents the scale from growing too fast for small dimensions.
            float calculatedScale = _settings.CardScaleNumerator / (rowCount + _settings.CardScaleDivisorOffset);

            // Clamp the scale to a sensible range to prevent cards from becoming too large or too small.
            // Max scale limit (e.g., 2.0) prevents huge cards on a 1x1 grid.
            // Min scale limit (e.g., 0.2) ensures visibility on very large grids (e.g., 20x20).
            return Mathf.Clamp(calculatedScale, _settings.MinCardScale, _settings.MaxCardScale);
        }
    }

    
}