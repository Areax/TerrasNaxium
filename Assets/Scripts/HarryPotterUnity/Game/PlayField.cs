using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using UnityEngine;
using System.Linq;
using UnityLogWrapper;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Game
{
    public class PlayField: MonoBehaviour
    {
        private Player _player;

        private readonly Vector2 _playFieldOffset = new Vector2(0f, -85f);
        //TL = top left, BR = bottom right, TM = top middle
        private readonly Vector3 _playPieceTL = new Vector3(-150f, -50f, 0f);
        private readonly Vector3 _playPieceTM = new Vector3(0f, -50f, 0f);
        private readonly Vector3 _playPieceTR = new Vector3(150f, -50f, 0f);
        private readonly Vector3 _playPieceBL = new Vector3(-150f, -130f, 0f);
        private readonly Vector3 _playPieceBM = new Vector3(0f, -130f, 0f);
        private readonly Vector3 _playPieceBR = new Vector3(150f, -130f, 0f);
        /*private int _playPieceTL = 100;
        private int _playPieceTM = 200;
        private int _playPieceTR = 300;
        private int _playPieceBL = 400;
        private int _playPieceBM = 500;
        private int _playPieceBR = 600;*/



        public GameObject TL;
        public GameObject TM;
        public GameObject TR;
        public GameObject BL;
        public GameObject BM;
        public GameObject BR;


        public event Action<Player> OnHandIsOutOfCards;

        private void OnDestroy()
        {
            OnHandIsOutOfCards = null;
        }

        public PlayPiece findId(byte networkid)
        {
            foreach(GameObject ob in GameManager.PlayPieces)
            {
                if (ob.GetComponent<PlayPiece>()._networkId == networkid) return ob.GetComponent<PlayPiece>();
            }
            return null;
        }

        private void Awake()
        {
            _player = transform.GetComponentInParent<Player>();
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(550f, 150f, 0f);
            col.center = new Vector3(_playFieldOffset.x, _playFieldOffset.y, 0f);
            //create 6 PlayPieces
            PiecetoList(TL, _playPieceTL);
            PiecetoList(TM, _playPieceTM);
            PiecetoList(TR, _playPieceTR);
            PiecetoList(BL, _playPieceBL);
            PiecetoList(BM, _playPieceBM);
            PiecetoList(BR, _playPieceBR);

            /*TL = createPiece(_playPieceTL);
            TL.name = "TL";
            PlayPieces.Add(TL);
            TM = createPiece(_playPieceTM);
            TM.name = "TM";
            TR = createPiece(_playPieceTR);
            TR.name = "TR";
            BL = createPiece(_playPieceBL);
            BL.name = "BL";
            BM = createPiece(_playPieceBM);
            BM.name = "BM";
            BR = createPiece(_playPieceBR);
            BR.name = "BR";*/
        }

        public void PiecetoList(GameObject ob, Vector3 area)
        {
            ob = createPiece(area);
            ob.name = ob.ToString();
            GameManager.PlayPieces.Add(ob);
        }

        public void HandtoField(BaseCard card, PlayPiece field)
        {
            if (card == null) return;

            field.MoveToThisCollection(card);

            field.Cards.Insert(0, card);

            card.transform.parent = field.transform;
            Debug.Log("moving card from PlayPiece");

            var tween = new MoveTween
            {
                Target = card.gameObject,
                Position = transform.parent.position + Vector3.back * 1.5f,
                Time = 0.25f,
                Flip = FlipState.FaceUp,
                Rotate = TweenRotationType.NoRotate,
                OnCompleteCallback = () => card.State = State.InDeck
            };

            GameManager.TweenQueue.AddTweenToQueue(tween);
        }


        private GameObject Instance = Resources.Load("AlbusDumbledore") as GameObject;

        private GameObject createPiece(Vector3 area)
        {
            var inst = Instantiate(Instance);
            var instpp = inst.AddComponent<PlayPiece>();
            //move place position and rotation according to the opponent? I think yes!
            inst.transform.parent = transform;
            inst.transform.localPosition = area;//Vector3.down * 80f + (Vector3.right - Vector3.right/4) * area;
            inst.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _player.transform.rotation.eulerAngles.z));


            instpp._player = _player;
            
            
            instpp._networkId = GameManager.NetworkIdCounter++;
            //add piece to playfield
            return inst;
        }
        
        private bool NoCreatures()
        {
            //TODO if no creatures and there is no card being played, left school is discarded
            return true;
        }
        

        public void AddAll(IEnumerable<BaseCard> cards)
        {
            //adds all cards??
            throw new NotImplementedException();
        }

        protected void RemoveAll(IEnumerable<BaseCard> cards)
        {
            //remove all cards from the field??
            throw new NotImplementedException();
        }
    }
}
