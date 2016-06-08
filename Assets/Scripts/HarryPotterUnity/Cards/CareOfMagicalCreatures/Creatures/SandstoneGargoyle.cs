namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class SandstoneGargoyle : BaseCreature
    {
        private bool _hasAddedDamage;

        public override void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.Creatures.Count != 0) return;

            _attack += 2;
            _attackLabel.text = (_attack).ToString();
            _hasAddedDamage = true;
        }

        public override void OnInPlayAfterTurnAction()
        {
            if (!_hasAddedDamage) return;

            _attack -= 2;
            _attackLabel.text = (_attack).ToString();
            _hasAddedDamage = false;
        }
    }
}
