using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.DeckGeneration;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;
using UnityLogWrapper;
using Type = HarryPotterUnity.Enums.Type;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Player : MonoBehaviour
    {
        public Player OppositePlayer { get; set; }
        public Hand Hand { get; private set; }
        public Deck Deck { get; private set; }
        public InPlay InPlay { get; private set; }
        public Discard Discard { get; private set; }
        public PlayField PlayField { get; private set; }

        public List<BaseCard> AllCards
        {
            get { return Hand.Cards.Concat(Deck.Cards).Concat(InPlay.Cards).Concat(Discard.Cards).ToList(); }
        }

        private readonly HashSet<LessonTypes> _lessonTypesInPlay = new HashSet<LessonTypes>(); 
        public HashSet<LessonTypes> LessonTypesInPlay
        {
            get
            {
                _lessonTypesInPlay.Clear();
                foreach (var providers in InPlay.Cards.OfType<ILessonProvider>())
                {
                    _lessonTypesInPlay.Add(providers.LessonType);
                }
                return _lessonTypesInPlay;
            }
        }
 
        public int AmountLessonsInPlay
        {
            get
            {
                return InPlay.Cards.OfType<ILessonProvider>().Sum(card => card.AmountLessonsProvided);
            }
        }

        public HashSet<Type> TypeImmunity { get; set; }

        // Actions Available will now be cards available to play lol
        public int ActionsAvailable { get; set; }

        public bool IsLocalPlayer { get; set; }
        public byte NetworkId { get; set; }


        public delegate void TurnEvents();
        public delegate void CardPlayedEvent(BaseCard card, List<BaseCard> targets = null);
        public delegate void DamageTakenEvent(BaseCard source, int amount);

        public event TurnEvents OnNextTurnStart;
        public event TurnEvents OnTurnStart;
        public event TurnEvents OnTurnEnd;
        public event CardPlayedEvent OnCardPlayed;
        public event DamageTakenEvent OnDamageTaken;

        private List<BaseCard> prebuiltCards;

        public void OnDestroy()
        {
            OnNextTurnStart = null;
            OnTurnStart = null;
            OnTurnEnd = null;
            OnCardPlayed = null;
            OnDamageTaken = null;
        }

        public void Awake()
        {
            nextTurn = true;
            ActionsAvailable = 2;

            Hand = transform.GetComponentInChildren<Hand>();
            //Deck = transform.GetComponentInChildren<Deck>();
            InPlay = transform.GetComponentInChildren<InPlay>();
            Discard = transform.GetComponentInChildren<Discard>();
            PlayField = transform.GetComponentInChildren<PlayField>();

            TypeImmunity = new HashSet<Type>();

            
            
        }

        public void InitDeck(List<LessonTypes> selectedLessons)
        {
            prebuiltCards = GameManager.GetPlayerTestDeck(NetworkId);
            //List<BaseCard> cards;


            if (GameManager.DebugModeEnabled)
            {
                DrawInitialHand();
                //cards = DeckGenerator.GenerateDeck(selectedLessons);
                //cards = DeckGenerator.GenerateDeck(prebuiltCards, selectedLessons);
                //startingCharacter = GameManager.GetPlayerTestCharacter(NetworkId);
            }
            else
            {
                //cards = DeckGenerator.GenerateDeck(selectedLessons);

            }

            //Deck.Initialize( cards, startingCharacter);
        }

        /// <summary>
        /// Remember to always call this function after the card's effect,
        /// otherwise the action will be used before the card activates.
        /// possibly beginning the next turn and causing a chain of actions to occur
        /// such as drawing the opponent's card and dealing his creature's damage before your card's effect activates.
        /// </summary>
        /// <param name="amount"></param>
       

        public void InvokeCardPlayedEvent(BaseCard card, List<BaseCard> targets = null)
        {
            if (OnCardPlayed != null) OnCardPlayed(card, targets);
        }

        public void BeginTurn()
        {
            if (OnNextTurnStart != null)
            {
                OnNextTurnStart();
                OnNextTurnStart = null;
            }

            if (OnTurnStart != null)
            {
                OnTurnStart();
            }

            foreach (var card in InPlay.Cards.Cast<IPersistentCard>())
            {
                card.OnInPlayBeforeTurnAction();
            }

            //Deck.DrawCard();
            //AddActions(2);

            /* if (ActionsAvailable < 1)
            {
                ActionsAvailable = 1;
            }

            foreach (var creature in InPlay.Creatures.Cast<BaseCreature>())
            {
                OppositePlayer.TakeDamage(creature, creature.Attack);
            }*/
        }

        private void EndTurn()
        {
            if (OnTurnEnd != null)
            {
                OnTurnEnd();
            }
            // ActionsAvailable = 0;

            foreach (var card in InPlay.Cards.Cast<IPersistentCard>())
            {
                card.OnInPlayAfterTurnAction();
            }
            
            Hand.AdjustHandSpacing();
            OppositePlayer.BeginTurn();
        }

        private bool nextTurn;


        //here are all skip sequences

        public void ResetActions()
        {
            ActionsAvailable = 2;
        }


        public bool UseAction()
        {
            if(ActionsAvailable > 0)
            {
                ActionsAvailable--;
                return true;
            }
            return false;
        }

        public void UseActions(int amount = 1)
        {
            nextTurn = false;
        }

        public void AddActions(int amount)
        {
            nextTurn = true;
        }

        public bool CanUseActions(int amount = 1)
        {
            return nextTurn;
        }



        public void DrawInitialHand()
        {
            Hand.Initialize(prebuiltCards);
            /*for (int i = 0; i < 7; i++)
            {
                //var card = Deck.TakeTopCard();
                var card = prebuiltCards[i];


                Hand.Add(card, preview: false, adjustSpacing: false);
            }*/       
        }
        
        public void TakeDamage(BaseCard sourceCard, int amount)
        {
            if (amount <= 0) return;
            
            var cards = new List<BaseCard>();

            for (int i = 0; i < amount; i++)
            {
                if (TypeImmunity.Contains(sourceCard.Type))
                {
                    continue;
                }

                var card = Deck.TakeTopCard();

                if (card == null)
                {
                    Log.Write("Game Over");
                    break;
                }
                cards.Add(card);
            }

            Discard.AddAll(cards);

            if (OnDamageTaken != null && sourceCard != null)
            {
                OnDamageTaken(sourceCard, cards.Count); 
            }
        }
        
        public void DisableAllCards()
        {
            Deck.gameObject.layer = GameManager.IGNORE_RAYCAST_LAYER;
            GameManager.DisableCards(Hand.Cards);
            GameManager.DisableCards(InPlay.Cards);
        }

        public void EnableAllCards()
        {
            Deck.gameObject.layer = GameManager.DECK_LAYER;
            GameManager.EnableCards(Hand.Cards);
            GameManager.EnableCards(InPlay.Cards);
        }

        public void ClearHighlightComponent()
        {
            foreach (var card in Hand.Cards)
            {
                card.RemoveHighlight();
            }

            //
            PlayField.RemoveAllHighlighted();

            foreach (var card in Discard.Cards)
            {
                card.RemoveHighlight();
            }
        }
    }
}
