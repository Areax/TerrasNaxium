﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.DeckGeneration;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityLogWrapper;
using Type = HarryPotterUnity.Enums.Type;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HarryPotterUnity.Cards
{
    [SelectionBase]
    public abstract class BaseCard : MonoBehaviour
    {
        [Header("Deck Generation Options")]
        [SerializeField, UsedImplicitly]
        private ClassificationTypes _classification;
        
        [Header("Card Settings")]
        [SerializeField, EnumFlags]
        [UsedImplicitly]
        private Tag _tags;

        public State State { get; set; }
        public ClassificationTypes Classification { get { return _classification; } set { _classification = value; } }

        public Type Type { get { return GetCardType(); } }
        protected abstract Type GetCardType();

        public FlipState FlipState { private get; set; }

        public Player Player { get; set; }

        //will return to where the card is put
        public Transform parentToReturnTo = null;
        public Transform placeholderParent = null;
        //GameObject placeholder = null;
        public static bool moveable = true;
        public bool isDefending;


        private List<IDeckGenerationRequirement> _deckGenerationRequirements;
        public List<IDeckGenerationRequirement> DeckGenerationRequirements
        {
            get
            {
                return _deckGenerationRequirements ??
                       (_deckGenerationRequirements =
                           GetComponents<MonoBehaviour>().OfType<IDeckGenerationRequirement>().ToList());
            }
        }

        public string CardName { get { return string.Format("{0}: {1}", Type, transform.name.Replace("(Clone)", "")); } }
        public byte NetworkId { get; set; }

        //private InputGatherer _inputGatherer;
        private int _fromHandActionInputRequired;
        private int _inPlayActionInputRequired;

        private static readonly Vector2 _colliderSize = new Vector2(50f, 70f);

        private GameObject _cardFace;
        // the entire whitening of the card showing which card is currently highlighted
        private GameObject _outline;
        // the yellow indicating the defending card
        public GameObject _highlight;
        // the white light indicating the card that is being attacked
        public GameObject _dlight;
        public GameObject arrow;

        private List<ICardPlayRequirement> PlayRequirements { get; set; }

        private CardCollection _collection;
        public CardCollection PreviousCollection { get; private set; }
        public CardCollection CurrentCollection
        {
            get
            {
                if (_collection == null)
                {
                    throw new Exception("collection is null for card: " + CardName);
                }
                return _collection;
            }
            set
            {
                PreviousCollection = _collection;
                _collection = value;
            }

        }

        private int ActionCost
        {
            get
            {
                switch (GetCardType())
                {
                    case Type.Adventure:
                    case Type.Character:
                        return 2;
                    default:
                        return 1;
                }
            }
        }

        protected virtual void Start()
        {
            FlipState = FlipState.FaceDown;

            gameObject.layer = GameManager.CARD_LAYER;
            _cardFace = transform.FindChild("Front").gameObject;

            AddCollider();

            LoadPlayRequirements();

            AddOutlineComponent();
            AddHighlightComponent();
            AddDlightComponent();
            isDefending = false;
        }

        private void AddOutlineComponent()
        {
            var tmp = Resources.Load("Outline");

            _outline = (GameObject)Instantiate(tmp);
            _outline.transform.position = transform.position + Vector3.back * 0.3f;
            _outline.transform.parent = transform;

            _outline.SetActive(false);
        }

        // defending light?
        private void AddDlightComponent()
        {
            var tmp = Resources.Load("Outline");

            _dlight = (GameObject)Instantiate(tmp);
            _dlight.transform.position = transform.position - Vector3.back * 0.2f;
            _dlight.transform.parent = transform;

            _dlight.SetActive(false);
        }

        private void AddHighlightComponent()
        {
            var tmp = Resources.Load("Highlight");

            _highlight = (GameObject)Instantiate(tmp);
            _highlight.transform.position = transform.position - Vector3.back;
            _highlight.transform.parent = transform;

            _highlight.SetActive(false);
        }


        private void LoadPlayRequirements()
        {
            PlayRequirements = GetComponents<MonoBehaviour>().OfType<ICardPlayRequirement>().ToList();

            var inputRequirement = PlayRequirements.OfType<InputRequirement>().SingleOrDefault();

            if (inputRequirement != null)
            {
                //_inputGatherer = GetComponent<InputGatherer>();
                _fromHandActionInputRequired = inputRequirement.FromHandActionInputRequired;
                _inPlayActionInputRequired = inputRequirement.InPlayActionInputRequired;
            }
        }

        private void AddCollider()
        {
            if (gameObject.GetComponent<Collider>() == null)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(_colliderSize.x, _colliderSize.y, 0.2f);
            }
        }

        private bool stillOnCard = false;
        public static bool highlighted = false;
        public bool noCylinder = true;

        public void OnMouseOver()
        {
            stillOnCard = true;
        }

        public void OnMouseExit()
        {
            stillOnCard = false;
        }

        // the total whitnenss
        public bool isHighlight()
        {
            return _outline.activeSelf ? true : false;
        }

        // the yellow background
        public bool isDlighted()
        {
            return _highlight.activeSelf ? true : false;
        }

        public void OnMouseDown()
        {
            if (noCylinder && isHighlight() && GameManager.Phase_localP == Phase.Attack && transform.parent.GetComponent<PlayPiece>() != null && !isDefending)
            {
                noCylinder = false;
                CreateCylinder();
            }

            if (arrow != null && isHighlight() && stillOnCard)
                arrow.GetComponent<Arrow>().movingarrow = true;

        }

        public void CreateCylinder()
        {
            arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            arrow.AddComponent<Arrow>();
            arrow.transform.localScale = new Vector3(.005f, 1f, .005f);
            arrow.tag = "Arrow";
            arrow.GetComponent<Arrow>().attack = GetComponent<BaseCreature>().Attack;
            arrow.GetComponent<Arrow>().origin = this;
        }

        public List<GameObject> arrows;

        public void CleanDiscard()
        {
            RemoveHighlight();
            /*while(arrows.Count != 0)
            {
                Destroy(arrows[0]);
            }*/
            //unhighlight
            //tell it that it's not highlighted anymore
        }

        public bool isFrontRow()
        {
            if (transform.parent.name == "TL" || transform.parent.name == "TR" || transform.parent.name == "TM")
                return true;
            return false;
        }

        public bool canAttackHighlightedCard()
        {
            if (Player.PlayField.hasDefending())
            {
                Debug.Log("there are defending cards");
                if (isDlighted())
                {
                    Debug.Log("I am highlighted! " + Player.NetworkId);
                    return true;
                }
                else return false;
            }
            else return true;

        }

        public void OnMouseUp()
        {
            if (GameManager.curHi != null && GameManager.curHi.arrow != null)
            {
                GameManager.curHi.arrow.GetComponent<Arrow>().movingarrow = false;
                //Debug.Log(highlighted + " " + GameManager.curHi.Player + " " + Player);
                // two things here: either actually ADD raycasts or change the attacking to click click 
                

                if (highlighted && GameManager.curHi.Player != Player && IsCreature() && GameManager.Phase_localP == Phase.Attack
                        && canAttackHighlightedCard())
                {
                    Debug.Log("i attack you!");
                    if (GameManager.curHi.arrow.transform.parent != null)
                        GameManager.curHi.arrow.transform.parent.GetComponent<BaseCard>()._dlight.SetActive(false);
                    arrows.Add(GameManager.curHi.arrow);
                    GameManager.curHi.arrow.transform.parent = transform;
                    _dlight.SetActive(true);
                }

            }

            //static booleans screw up when you exit the game lol
            //if this card is not yours you can't highlight it silly
            if (GameManager.IsInputGathererActive) return; //Do call OnMouseDown if cursor has left the object
            //Player clicked on this card as a target, not to activate its effect.
            if (FlipState == FlipState.FaceDown) return;
            //if (FlipState == FlipState.FaceDown) return;

            //if player successfully attacks card

            // if highlighted already
            if (highlighted && _outline.activeSelf == true && stillOnCard)
                Removehighlighted();
            else if(stillOnCard && Player.IsLocalPlayer && GameManager.Phase_localP == Phase.Defense && _highlight.activeSelf == false && isFrontRow())
            {
                // highlight card to show its defending
                // make sure it's only front row :)
                // 
                //if(transform.parent == )
                _highlight.SetActive(true);
                isDefending = true;
            }
            else if(stillOnCard && Player.IsLocalPlayer && GameManager.Phase_localP == Phase.Defense && _highlight.activeSelf == true)
            {
                _highlight.SetActive(false);
                isDefending = false;
            }
            else if (!highlighted && stillOnCard && Player.IsLocalPlayer) // if not highlighted, highlight!
            {
                _outline.SetActive(true);
                highlighted = true;
                GameManager.curHi = this;
            }
           /* else if (highlighted && GameManager.curHi.Player != Player && IsCreature() && GameManager.Phase_localP == Phase.Attack && GameManager.curHi.arrow != null)
            {
                //GetComponent<BaseCreature>().TakeDamage(GameManager.curHi.GetComponent<BaseCreature>().Attack);
                //GameManager.curHi.arrow.GetComponent<Arrow>().enabled = false;
                arrows.Add(GameManager.curHi.arrow);
                GameManager.curHi.arrow.transform.parent = transform;
            }*/
                    
        }

        private bool IsCreature()
        {
            if (this is BaseCreature) return true;
            return false;
        }

        private bool IsActivatable()
        {
            return State == State.InPlay
                   && Player.IsLocalPlayer
                   && ((IPersistentCard)this).CanPerformInPlayAction()
                   && GetInPlayActionTargets().Count >= _fromHandActionInputRequired;
        }

        private bool IsPlayableFromHand()
        {
            bool meetsPlayRequirements = PlayRequirements.Count == 0 ||
                                     PlayRequirements.All(req => req.MeetsRequirement());

            return Player.IsLocalPlayer &&
                   State == State.InHand &&
                   Player.CanUseActions(ActionCost) &&
                   meetsPlayRequirements &&
                   IsUnique() &&
                   MeetsAdditionalPlayRequirements();
        }

        public bool HasTag(Tag t)
        {
            return (_tags & t) == t;
        }

        private bool IsUnique()
        {
            if (HasTag(Tag.Unique) == false) return true;

            var allInPlayCards = Player.InPlay.Cards.Concat(
                Player.OppositePlayer.InPlay.Cards)
                .Select(c => c.CardName);

            return allInPlayCards.Contains(CardName) == false;
        }

        public void PlayFromHand(List<BaseCard> targets = null)
        {
            foreach (var requirement in PlayRequirements)
            {
                requirement.OnRequirementMet();
            }

            OnPlayFromHandAction(targets);

            Player.UseActions(ActionCost);

        }

        protected virtual void OnPlayFromHandAction(List<BaseCard> targets)
        {
            if (this is IPersistentCard)
            {
                Player.InPlay.Add(this);
            }
            else
            {
                throw new Exception("OnPlayFromHandAction must be overriden in cards that do not implement IPersistentCard!");
            }
        }

        private void ShowPreview()
        {
            _cardFace.layer = GameManager.PREVIEW_LAYER;

            if (FlipState == FlipState.FaceUp && iTween.Count(gameObject) == 0)
                GameManager.PreviewCamera.ShowPreview(this);
            else HidePreview();
        }

        private void HidePreview()
        {
            _cardFace.layer = GameManager.CARD_LAYER;
            GameManager.PreviewCamera.HidePreview();
        }

        public void Disable()
        {
            gameObject.layer = GameManager.IGNORE_RAYCAST_LAYER;
            _cardFace.GetComponent<Renderer>().material.color = new Color(0.35f, 0.35f, 0.35f);
        }

        public void Enable()
        {
            gameObject.layer = GameManager.CARD_LAYER;
            _cardFace.GetComponent<Renderer>().material.color = Color.white;
        }

        public void SetSelected()
        {
            gameObject.layer = GameManager.CARD_LAYER;
            _cardFace.GetComponent<Renderer>().material.color = Color.yellow;
        }

        public virtual List<BaseCard> GetFromHandActionTargets()
        {
            if (_fromHandActionInputRequired == 0)
            {
                return new List<BaseCard>();
            }

            throw new NotSupportedException("Card with from hand input did not define valid targets");
        }

        public virtual List<BaseCard> GetInPlayActionTargets()
        {
            if (_inPlayActionInputRequired == 0)
            {
                return new List<BaseCard>();
            }

            throw new NotSupportedException("Card with in play input did not define valid targets.");
        }

        protected virtual bool MeetsAdditionalPlayRequirements()
        {
            return true;
        }

        public void SetHighlight()
        {
            if (_highlight) _highlight.SetActive(true);
            else Log.Error("Highlight component has not yet been added.");
        }


        public void Reset_Attack()
        {
            noCylinder = true;
        }

        public void RemoveHighlight()
        {
            //if (_highlight) _highlight.SetActive(false);
            //Debug.Log("removing highlights for player " + Player + " name me " + transform.parent.name);
            highlighted = false;
            _outline.SetActive(false);
            _dlight.SetActive(false);
            _highlight.SetActive(false);
            GameManager.curHi = null;
        }

        //doesn't remove the defense light, aka _highlight
        public void Removehighlighted()
        {
            //if (_highlight) _highlight.SetActive(false);
            //Debug.Log("removing highlights for player " + Player + " name me " + transform.parent.name);
            highlighted = false;
            _outline.SetActive(false);
            _dlight.SetActive(false);
            GameManager.curHi = null;
        }

    }

}
