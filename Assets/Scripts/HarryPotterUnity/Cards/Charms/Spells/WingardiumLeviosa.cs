﻿using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Demigods.Spells
{
    public class WingardiumLeviosa : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.TypeImmunity.Add(Type.Creature);

            Player.OnNextTurnStart += () => Player.TypeImmunity.Remove(Type.Creature);

        }
    }
}
