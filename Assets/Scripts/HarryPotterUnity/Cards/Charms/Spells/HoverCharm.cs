﻿using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Demigods.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class HoverCharm : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return Player.OppositePlayer.InPlay.CardsExceptStartingCharacter;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            Player.OppositePlayer.Hand.Add(target);
        }
    }
}
