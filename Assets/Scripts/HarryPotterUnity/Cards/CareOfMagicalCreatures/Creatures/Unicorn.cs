namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class Unicorn : BaseCreature {

        public override void OnInPlayBeforeTurnAction()
        {
            Player.AddActions(1);
        }
    }
}
