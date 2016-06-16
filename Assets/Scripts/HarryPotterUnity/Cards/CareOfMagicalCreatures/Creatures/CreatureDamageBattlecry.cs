using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class CreatureDamageBattlecry : BaseCreature
    {
        [UsedImplicitly, SerializeField, Space(10)]
        private int _battlecryDamage;

    }
}
