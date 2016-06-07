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
        }
        

        public bool HaveCreature()
        {
            //has or doesn't have creature
            return true;
        }

        private bool hasSomething = false;

        public bool HasSomething()
        {
            //if has child, return true :)
            return hasSomething ? true : false;
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

            if (BaseCard.highlighted == true)//&& clicked on empty PlayField)
            {
                return;
            }
            if (HasSomething())
            {
                //????
                //stop the clicking of objects

                //_inputGatherer.GatherInput(InputGatherMode.FromHandAction);

                //if true, move to specific location, indicated by card, set parent
            }
            else
            {
                //allow card to come :D
                //GameManager.Network.RPC("ExecutePlayActionById", PhotonTargets.All, NetworkId);
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

            var cardPos = new Vector3(_playFieldOffset.x, _playFieldOffset.y, -1);
            //cardPos.z += Cards.IndexOf(card) * 10f;

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
