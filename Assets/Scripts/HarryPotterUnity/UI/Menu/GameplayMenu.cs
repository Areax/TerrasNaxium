using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityLogWrapper;

namespace HarryPotterUnity.UI.Menu
{
    public class GameplayMenu : BaseMenu
    {
        private Player _localPlayer;
        private Player _remotePlayer;
        public Player LocalPlayer
        {
            private get { return _localPlayer; }
            set
            {
                _localPlayer = value;

                /*_localPlayer.OnTurnStart += () =>
                {
                    _actionsLeftLabelLocal.GetComponent<Animator>().SetBool("IsOpen", true);
                    _skipActionButton.interactable = true;
                };
                
                _localPlayer.OnTurnEnd   += () =>
                {
                    _actionsLeftLabelLocal.GetComponent<Animator>().SetBool("IsOpen", false);
                    _skipActionButton.interactable = false;
                };*/
                
                _localPlayer.InPlay.OnCardEnteredPlay += card =>
                {
                    if (card is ILessonProvider)
                    {
                        UpdateLessonPanel();
                    }
                };

                _localPlayer.InPlay.OnCardExitedPlay += card =>
                {
                    if (card is ILessonProvider)
                    {
                        UpdateLessonPanel();
                    }
                };

                //_localPlayer.Deck.OnDeckIsOutOfCards += ShowGameOverMessage;
            }
        }


        private void ShowGameOverMessage(Player loser)
        {
            //TODO: Show game over message
            //Unsub from the other game over message here, only show the win/lose based on who died first
        }

        private void UpdateLessonPanel()
        {
            //TODO: Update player hud lessons panel
        }

        public Player RemotePlayer
        {
            private get { return _remotePlayer; }
            set
            {
                _remotePlayer = value;
                _remotePlayer.OnTurnStart += () => _actionsLeftLabelRemote.GetComponent<Animator>().SetBool("IsOpen", true);
                _remotePlayer.OnTurnEnd   += () => _actionsLeftLabelRemote.GetComponent<Animator>().SetBool("IsOpen", false);

                //_remotePlayer.Deck.OnDeckIsOutOfCards += ShowGameOverMessage;
            }
        }

        private Text _actionsLeftLabelLocal;
        private Text _actionsLeftLabelRemote;

        private Text _cardsLeftLabelLocal;
        private Text _cardsLeftLabelRemote;

        private Text curPhase;

        private Button _skipActionButton;

        private Text _mainMenuTitle;
        private Image _mainMenuBackground;

        private Image _gameBackground;
        private Phase nextPhase;
        private bool waitingForOpponent;

        protected override void Awake()
        {
            base.Awake();

            nextPhase = Phase.Preparation;
            waitingForOpponent = false;

            var allText = FindObjectsOfType<Text>();

            _mainMenuBackground = FindObjectsOfType<Image>().FirstOrDefault(i => i.name.Contains("MainMenuBackground"));
            _mainMenuTitle = allText.FirstOrDefault(i => i.name.Contains("MainMenuTitle"));

            _actionsLeftLabelLocal = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Local"));
            _actionsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Remote"));

            _cardsLeftLabelLocal = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Local"));
            _cardsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Remote"));

            curPhase = allText.FirstOrDefault(t => t.name.Contains("CurPhase"));

            _skipActionButton = FindObjectsOfType<Button>().FirstOrDefault(b => b.name.Contains("SkipActionButton"));

            if (_actionsLeftLabelLocal  == null || 
                _actionsLeftLabelRemote == null || 
                _cardsLeftLabelLocal    == null || 
                _cardsLeftLabelRemote   == null || 
                _mainMenuTitle          == null || 
                _mainMenuBackground     == null ||
                curPhase                == null ||
                _skipActionButton       == null)
            {
                Log.Error("Could not find all needed HUD elements in gameplay menu, report this error!");
            }
        }

        private void Start()
        {
            //_skipActionButton.interactable = false;
            GameManager.Phase_localP = Phase.Placement;
            GameManager.Phase_opponentP = Phase.Placement;
        }

        protected override void Update()
        {
            base.Update();

            if ((((int)GameManager.Phase_localP) + 1) % 6 == (int)nextPhase % 6)
                curPhase.text = GameManager.Phase_localP.ToString();
            else curPhase.text = "Waiting For Opponent...";


            if (LocalPlayer != null)
            {
                //Update Local Player Properties
                _actionsLeftLabelLocal.text = string.Format("Actions Left: {0}", LocalPlayer.ActionsAvailable);
                //_cardsLeftLabelLocal.text = string.Format("Cards Left: {0}", LocalPlayer.Deck.Cards.Count);
            }

            if (RemotePlayer != null)
            {
                //Update Remote Plater Properties
                _actionsLeftLabelRemote.text = string.Format("Actions Left: {0}", RemotePlayer.ActionsAvailable);
                //_cardsLeftLabelRemote.text = string.Format("Cards Left: {0}", RemotePlayer.Deck.Cards.Count);
            }
        }

        
        [UsedImplicitly]
        public void SkipAction()
        {
            //need to send curPhase
            //LocalPlayer.

            //var player = LocalPlayer.CanUseActions() ? LocalPlayer : RemotePlayer;

            /*if (player.ActionsAvailable == 1)
            {
                _skipActionButton.interactable = false;
            }*/
            //if opponent's board is empty, you win!
            //LOL This only will wokr if we go through the RPC because the local player has no idea what the other player has

            //_localPlayer.UseActions();
            //_skipActionButton.enabled = false;

            if (GameManager.Phase_localP == Phase.Persistence) nextPhase = Phase.Placement;
            else nextPhase = GameManager.Phase_localP + 2;

            if (_remotePlayer.PlayField.NoCreatures()) Debug.Log("LOCAL PLAYER WINS!");
            GameManager.Network.RPC("ExecuteSkipAction", PhotonTargets.All, _localPlayer.NetworkId);
        }

        public override void OnShowMenu()
        {
            _mainMenuBackground.GetComponent<Animator>().SetBool("IsVisible", false);
            _mainMenuTitle.GetComponent<Animator>().SetBool("IsVisible", false);
        }

        public override void OnHideMenu()
        {
            _mainMenuBackground.GetComponent<Animator>().SetBool("IsVisible", true);
            _mainMenuTitle.GetComponent<Animator>().SetBool("IsVisible", true);
        }
    }
}
