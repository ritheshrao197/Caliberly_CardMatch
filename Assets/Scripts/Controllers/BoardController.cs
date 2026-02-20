using System;
using System.Collections.Generic;
using System.Linq;
using MemoryGame;
using MemoryGame.Config;
using MemoryGame.Events;
using MemoryGame.Constants;
using UnityEngine;
using UnityEngine.Pool;

namespace MemoryGame.Controller
{
    public class BoardController
    {
        private readonly Transform _root;
        private readonly CardSet _set;
        private readonly ObjectPool<CardController> _pool;
        private readonly BoardFrame _frame;
        private readonly GameConfig _config;
        private bool _oddGridInfoLogged;
        private bool _frameFallbackWarned;
        private bool _simpleLayoutWarned;

        public List<CardController> Cards { get; } = new List<CardController>();

        public BoardController(
            Transform root,
            CardSet set,
            ObjectPool<CardController> pool,
            BoardFrame frame,
            GameConfig config)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
            _set = set ?? throw new ArgumentNullException(nameof(set));
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            _frame = frame;
            _config = config;
        }

        public void Build(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Rows and columns must be greater than 0.");

            int totalSlots = rows * cols;
            int usable = (totalSlots / 2) * 2;
            int pairs = usable / 2;

            if (usable != totalSlots && !_oddGridInfoLogged)
            {
                Debug.Log(
                    $"[BoardController] Odd-sized grids use even card counts. " +
                    $"Example {rows}x{cols} -> {usable} cards.");
                _oddGridInfoLogged = true;
            }

            var ids = PickIds(pairs);
            Shuffle(ids);

            ResizePool(usable);

            if (_frame != null)
            {
                float frameScale = CalculateFrameScale(rows, cols);
                _frame.transform.localScale = Vector3.one * frameScale;

                if (_frame.HasBounds(out var inner))
                {
                    LayoutInsideRect(inner, rows, cols, ids);
                }
                else
                {
                    if (!_frameFallbackWarned)
                    {
                        Debug.LogWarning("[BoardController] Frame bounds invalid. Using simple grid fallback.");
                        _frameFallbackWarned = true;
                    }

                    UseSimpleLayout(rows, cols, usable, ids, false);
                }
            }
            else
            {
                UseSimpleLayout(rows, cols, usable, ids, true);
            }


            EventBus.Instance.Publish(new BoardBuiltEvent(pairs));
        }

        private void UseSimpleLayout(
            int rows,
            int cols,
            int usable,
            List<string> ids,
            bool warnIfNoFrame)
        {
            if (warnIfNoFrame && !_simpleLayoutWarned)
            {
                Debug.LogWarning("[BoardController] No frame configured. Using simple grid layout.");
                _simpleLayoutWarned = true;
            }

            float scale = CalculateCardScale(rows, cols);

            for (int i = 0; i < usable; i++)
            {
                var card = Cards[i];

                card.transform.localPosition =
                    IndexToLocal(i, rows, cols,
                    BoardConstants.DefaultCellX,
                    BoardConstants.DefaultCellY);

                card.transform.localScale = Vector3.one * scale;

                InitCard(card, ids[i]);
            }
        }

        private void ResizePool(int required)
        {
            while (Cards.Count < required)
            {
                var card = _pool.Get();
                card.transform.SetParent(_root, false);
                Cards.Add(card);
            }

            while (Cards.Count > required)
            {
                var card = Cards[Cards.Count - 1];
                _pool.Release(card);
                Cards.RemoveAt(Cards.Count - 1);
            }
        }

        private void LayoutInsideRect(
            Rect innerWorld,
            int rows,
            int cols,
            List<string> ids)
        {
            float cellW = innerWorld.width / cols;
            float cellH = innerWorld.height / rows;

            float gridPad =
                Mathf.Clamp01(BoardConstants.GridPaddingFactor *
                              Mathf.Max(rows, cols));

            float pad =
                Mathf.Clamp01(BoardConstants.DefaultCardPadding + gridPad);

            float boxW = cellW * (1f - pad);
            float boxH = cellH * (1f - pad);

            for (int i = 0; i < Cards.Count; i++)
            {
                int r = i / cols;
                int c = i % cols;

                float cx = innerWorld.xMin + (c + 0.5f) * cellW;
                float cy = innerWorld.yMax - (r + 0.5f) * cellH;

                Vector3 world = new Vector3(cx, cy, 0f);
                Vector3 local = _root.InverseTransformPoint(world);

                var card = Cards[i];
                card.transform.localPosition = local;

                card.view?.NormalizeChildScale();

                Vector2 natural = GetCardNaturalSize(card);
                float scale = Mathf.Min(boxW / natural.x, boxH / natural.y);

                card.transform.localScale = Vector3.one * scale;

                InitCard(card, ids[i]);
            }
        }

        private void InitCard(CardController card, string id)
        {
            var face = _set.GetFaceById(id);
            card.Init(id, face, false);
            card.flipDuration = _config != null
                ? _config.flipDuration
                : BoardConstants.DefaultFlipDuration;
        }

        private Vector2 GetCardNaturalSize(CardController card)
        {
            var view = card.view;
            if (view != null)
            {
                if (view.back && view.back.sprite)
                {
                    var s = view.back.sprite.bounds.size;
                    return new Vector2(s.x, s.y);
                }

                if (view.face && view.face.sprite)
                {
                    var s = view.face.sprite.bounds.size;
                    return new Vector2(s.x, s.y);
                }
            }

            return new Vector2(
                BoardConstants.DefaultCardWidth,
                BoardConstants.DefaultCardHeight);
        }

        private List<string> PickIds(int pairCount)
        {
            var source = _set.GetAllIds().Distinct().ToList();

            if (pairCount > source.Count)
                throw new InvalidOperationException(
                    "Not enough unique card IDs to build board.");

            Shuffle(source);

            var result = new List<string>(pairCount * 2);

            for (int i = 0; i < pairCount; i++)
            {
                result.Add(source[i]);
                result.Add(source[i]);
            }

            return result;
        }

        private float CalculateCardScale(int rows, int cols)
        {
            int maxDim = Mathf.Max(rows, cols);
            return Mathf.Clamp(4f / maxDim, 0.4f, 1f);
        }

        private float CalculateFrameScale(int rows, int cols)
        {
            int maxDim = rows;

            if (maxDim <= 0)
                return 5f;

            // // Make 2x2 and 3x3 intentionally larger
            // if (maxDim <= 2)
            //     return 3f;   // Big and satisfying

            // if (maxDim == 3)
            //     return 2f;

            // if (maxDim == 4)
            //     return 1.5f;

            // if (maxDim == 5)
            //     return 1.5f;

            // if (maxDim == 6)
            //     return 1f;

            float raw =
                BoardConstants.CardScaleNumerator /
                (maxDim + BoardConstants.CardScaleDivisorOffset);

            return Mathf.Clamp(
                raw,
                BoardConstants.MinCardScale,
                BoardConstants.MaxCardScale);
        }

        public void ClearAll()
        {
            foreach (var card in Cards)
                _pool.Release(card);

            Cards.Clear();
        }

        private static Vector3 IndexToLocal(
            int index,
            int rows,
            int cols,
            float dx,
            float dy)
        {
            int r = index / cols;
            int c = index % cols;

            float width = (cols - 1) * dx;
            float height = (rows - 1) * dy;

            float x = -width * 0.5f + c * dx;
            float y = height * 0.5f - r * dy;

            return new Vector3(x, y, 0);
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
