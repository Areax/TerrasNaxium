using System.Linq;
using UnityEngine;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class SteelPlating: BaseCreature
    {
        public override void TakeDamage(int amount)
        {
            if (amount < Armor) return;
            else amount -= Armor;

            _health -= amount;
            _healthLabel.text = Mathf.Clamp(_health, 0, int.MaxValue).ToString();

            if (_health <= 0)
            {
                Player.Discard.Add(this);

            }
        }
    }
}