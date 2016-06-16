using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.Cards
{
    public class BaseCreature : BaseCard, IPersistentCard {
        
        [Header("Creature Settings")]
        [SerializeField, UsedImplicitly]
        protected int _attack;

        [SerializeField, UsedImplicitly]
        protected int _health;

        [SerializeField, UsedImplicitly]
        protected int _speed;

        [SerializeField, UsedImplicitly]
        protected int _armor;

        private GameObject _uiCanvas;
        protected Text _healthLabel;
        protected Text _attackLabel;
        
        public int Attack { get { return _attack; } }

        public int Health { get { return _health; } }

        public int Speed { get { return _speed; } }

        public int Armor { get { return _armor; } }

        protected override void Start()
        {
            base.Start();
            LoadUiOverlay();
        }

        private void LoadUiOverlay()
        {
            var resource = Resources.Load("CreatureUIOverlay");
            _uiCanvas = (GameObject) Instantiate(resource);

            _uiCanvas.transform.position = transform.position + Vector3.back + Vector3.up * 10;
            _uiCanvas.transform.SetParent(transform, true);
            _uiCanvas.transform.localRotation = Player.IsLocalPlayer ? Quaternion.Euler(0f,0f,270f) : Quaternion.Euler(0f,0f,90f);
            _healthLabel = _uiCanvas.transform.FindChild("HealthLabel").gameObject.GetComponent<Text>();
            _attackLabel = _uiCanvas.transform.FindChild("AttackLabel").gameObject.GetComponent<Text>();
            _healthLabel.text = _health.ToString();
            _attackLabel.text = Attack.ToString();

            _uiCanvas.SetActive(true);
        }
        
        
        public virtual void TakeDamage(int amount)
        {
            _health -= amount;
            _healthLabel.text = Mathf.Clamp(_health, 0, int.MaxValue).ToString();

            if (_health <= 0)
            {
                Player.Discard.Add(this);

            }
        }

        public void Heal(int amount)
        {
            _health = Mathf.Clamp(_health + amount, 0, Health);
            _healthLabel.text = _health.ToString();
        }

        public virtual bool CanPerformInPlayAction() { return false; }
        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }
        public virtual void OnInPlayAction(List<BaseCard> targets = null) { }

        protected sealed override Type GetCardType()
        {
            return Type.Creature;
        }
    }
}
