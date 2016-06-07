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

        private readonly Vector2 _playFieldOffset = new Vector2(100f, 0f);
        //TL = top left, BR = bottom right, TM = top middle
        private int _playPieceTL = 100;
        private int _playPieceTM = 200;
        private int _playPieceTR = 300;
        private int _playPieceBL = 400;
        private int _playPieceBM = 500;
        private int _playPieceBR = 600;

        private GameObject TL;
        private GameObject TM;
        private GameObject TR;
        private GameObject BL;
        private GameObject BM;
        private GameObject BR;

        public event Action<Player> OnHandIsOutOfCards;

        private void OnDestroy()
        {
            OnHandIsOutOfCards = null;
        }

        private void Awake()
        {
            _player = transform.GetComponentInParent<Player>();
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(_playFieldOffset.x, _playFieldOffset.y, 0f);
            //create 6 PlayPieces
            TL = createPiece(_playPieceTL);
            TM = createPiece(_playPieceTM);
            TR = createPiece(_playPieceTR);
            BL = createPiece(_playPieceBL);
            BM = createPiece(_playPieceBM);
            BR = createPiece(_playPieceBR);
        }
        private GameObject Instance = Resources.Load("AlbusDumbledore") as GameObject;

        private GameObject createPiece(int area)
        {
            var inst = Instantiate(Instance);
            var instpp = inst.AddComponent<PlayPiece>();

            //move place position and rotation according to the opponent? I think yes!
            inst.transform.parent = transform;
            inst.transform.localPosition = Vector3.down * 80f + (Vector3.right - Vector3.right/4) * area;
            inst.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _player.transform.rotation.eulerAngles.z));
            inst.transform.position += Vector3.back * -.02f;

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
