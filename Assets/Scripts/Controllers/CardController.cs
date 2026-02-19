using System.Collections;
using MemoryGame.Events;
using MemoryGame.Models;
using MemoryGame.Views;
using UnityEngine;
using MemoryGame.Constants;

namespace MemoryGame.Controller
{
    /// <summary>
    /// Controller component for a single card in the memory game.
    /// Handles user input, card flipping animations, and communicates with the card model.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CardController : MonoBehaviour
    {
        /// <summary>
        /// Reference to the card's view component
        /// </summary>
        public CardView view;
        
        /// <summary>
        /// Duration of the card flip animation in seconds
        /// </summary>
        public float flipDuration ;

        private readonly CardModel _model = new CardModel();
        private bool _inputEnabled = true;

        /// <summary>
        /// Public accessor for the card's data model
        /// </summary>
        public CardModel Model => _model;

        /// <summary>
        /// Initializes the card with its ID, face sprite, and initial state
        /// </summary>
        /// <param name="id">Unique identifier for the card</param>
        /// <param name="faceSprite">Sprite to display when the card is face up</param>
        /// <param name="startFaceUp">Whether the card should start face up (default: false)</param>
        public void Init(string id, Sprite faceSprite, bool startFaceUp   = false)
        {
            _model.Set(id);
            if (view != null) view.BindFace(faceSprite);
            if (startFaceUp)
            {
                _model.FlipUp();
                view?.SetInstant(true);
            }
            else
            {
                _model.FlipDown();
                view?.SetInstant(false);
            }
            gameObject.SetActive(true);
            _inputEnabled = true;
            InputLock.Unlock();
        }

        /// <summary>
        /// Handles mouse click events on the card
        /// </summary>
        private void OnMouseUpAsButton()
        {
            // Debug.Log($"[CardController] OnMouseUpAsButton called for card {_model.Id}");
            // Debug.Log($"[CardController] _inputEnabled={_inputEnabled}, IsMatched={_model.IsMatched}, IsFaceUp={_model.IsFaceUp}");
            if (InputLock.IsLocked || !_inputEnabled || _model.IsMatched || _model.IsFaceUp) return;
            StartCoroutine(FlipRoutine(true));
            EventBus.Instance.Publish(new CardSelectedEvent(this));
        }

        /// <summary>
        /// Enables or disables user input for this card
        /// </summary>
        /// <param name="enabled">True to enable input, false to disable</param>
        public void SetInput(bool enabled) => _inputEnabled = enabled;

        /// <summary>
        /// Coroutine that handles the card flip animation
        /// </summary>
        /// <param name="faceUp">True to flip face up, false to flip face down</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        public IEnumerator FlipRoutine(bool faceUp)
        {
            yield return view.StartCoroutine(view.AnimateFlip(faceUp, flipDuration));
            if (faceUp) _model.FlipUp(); else _model.FlipDown();
        }

        /// <summary>
        /// Locks the card as matched, preventing further interactions
        /// </summary>
        public void Lock() => _model.Lock();
        
        /// <summary>
        /// Instantly sets the card's face state without animation
        /// </summary>
        /// <param name="faceUp">True to show face, false to show back</param>
        public void FaceInstant(bool faceUp)
        {
            view?.SetInstant(faceUp);
            if (faceUp) _model.FlipUp(); else _model.FlipDown();
        }
    }
}