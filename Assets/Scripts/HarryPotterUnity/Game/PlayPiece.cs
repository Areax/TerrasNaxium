using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using UnityEngine;
using UnityLogWrapper;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Game
{
    public class PlayPiece: CardCollection
    {
        //make parent PlayField
        //
        public Player _player { get; set; }
        public byte _networkId { get; set; }

        public GameObject image;


        private readonly Vector2 _playFieldOffset = new Vector2(0f, 0f);

        public BaseCard card;

        private void Awake()
        {
            _player = transform.GetComponentInParent<Player>();
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(_playFieldOffset.x, _playFieldOffset.y, 0f);
            image = Resources.Load("AlbusDumbledore") as GameObject;
            image = Instantiate(image);
            image.transform.parent = transform;
        }
        

        public bool HaveCreature()
        {
            //has or doesn't have creature
            return true;
        }

        private void OnMouseUp()
        {
            //if highlight is set to true, find the highlighted card and move the position
            //call add function
            if (active == true && BaseCard.highlighted == true)
            {
                card = _player.Hand.FindHighlighted();
                if(card != null)
                {
                    Debug.Log(card);
                    Add(card);
                }
                
            }
        }

        private bool active = false;


        private void OnMouseOver()
        {
            //set variable to be true
            active = true;
        }

        private void OnMouseExit()
        {
            //set variable to be false
            active = false;
        }
        
        protected override void Remove(BaseCard card)
        {
            Cards.Remove(card);
        }

        /// <summary>
        /// Adds a card to the bottom of the deck
        /// </summary>
        public override void Add(BaseCard card)
        {
            MoveToThisCollection(card);

            Cards.Insert(0, card);

            card.transform.parent = transform;

            var cardPos = new Vector3(_playFieldOffset.x, _playFieldOffset.y, -16f);
            cardPos.z -= Cards.IndexOf(card) * 0.2f;

            var tween = new MoveTween
            {
                Target = card.gameObject,
                Position = cardPos,
                Time = 0.25f,
                Flip = FlipState.FaceUp,
                Rotate = TweenRotationType.NoRotate,
                OnCompleteCallback = () => card.State = State.InDeck
            };

            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        public override void AddAll(IEnumerable<BaseCard> cards)
        {
            //adds all cards??
            throw new NotImplementedException();
        }

        protected override void RemoveAll(IEnumerable<BaseCard> cards)
        {
            //remove all cards from the field??
            throw new NotImplementedException();
        }
    }
}
