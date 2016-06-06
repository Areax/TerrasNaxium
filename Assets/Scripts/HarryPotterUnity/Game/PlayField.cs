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
        private readonly Vector2 _playPieceTL = new Vector2(100f, 0f);
        private readonly Vector2 _playPieceTM = new Vector2(100f, 0f);
        private readonly Vector2 _playPieceTR = new Vector2(100f, 0f);
        private readonly Vector2 _playPieceBL = new Vector2(100f, 0f);
        private readonly Vector2 _playPieceBM = new Vector2(100f, 0f);
        private readonly Vector2 _playPieceBR = new Vector2(100f, 0f);

        private GameObject TL;
        private PlayPiece TM;
        private PlayPiece TR;
        private PlayPiece BL;
        private PlayPiece BM;
        private PlayPiece BR;

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
        }
        private GameObject Instance = Resources.Load("AlbusDumbledore") as GameObject;

        private GameObject createPiece(Vector2 area)
        {
            var inst = Instantiate(Instance);
            var instpp = inst.AddComponent<PlayPiece>();

            //move place position and rotation according to the opponent? I think yes!
            inst.transform.parent = transform;
            inst.transform.localPosition = Vector3.back * -16f;
            inst.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));
            inst.transform.position += Vector3.back * 0.2f;

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
