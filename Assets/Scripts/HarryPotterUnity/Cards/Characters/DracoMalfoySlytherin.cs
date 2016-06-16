﻿using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Characters
{
    public class DracoMalfoySlytherin : BaseCharacter
    {
        private bool HasEffectActivated { get; set; }

        private void AddActionOnItemPlayed(BaseCard card, List<BaseCard> targets)
        {
            if (HasEffectActivated == false && card.Type == Type.Item)
            {
                HasEffectActivated = true;

                Player.AddActions(1);
            }
        }

        public override void OnInPlayAfterTurnAction()
        {
            HasEffectActivated = false;
        }
    }
}