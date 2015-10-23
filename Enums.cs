using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic
{
    public class Enums
    {
        public enum Type
        {
            None, Ritual, Elemental, Spirit, Shaman
        }

        public enum SubType
        {
            None, Water, Earth, Fire, Magma, Vapor, Flora, Ancestral
        }

        public enum Role
        {
            None, Tank, Healer, Dps
        }

        public enum Mana
        {
            None, Water, Earth, Fire, Life, Death
        }

        public enum ManaEvent //questo enum definisco per quali eventi puo' essere richiesto il mana
        {
            None,NewRound,AddMana
        }

        public enum Properties
        {
            None, Quickness, Guardian ,Penetrate ,Thunderborn ,Thorns , Armor
        }
        public enum Buff
        {
            None, Shield, Immortal, Quickness, Guardian, Penetrate, Thunderborn, Thorns, IncreasedStr, IncreasedCon 
                // Lista di appoggio. Quando viene aggiunto una proprietà come Buff, viene aggiunta anche la Property corrispondente. 
                // ES: un rituale mi conferisce Guardian. Check su properties: se NON c'è Guardian aggiungo Guardian in buff E in properties.
        }
        public enum Debuff
        {
            None, Incurable, Asleep, Poison, DecreasedStr, DecreasedCon
        }
        public enum Filter
        {
            None, Water, Earth, Fire, Magma, Vapor, Flora, Ancestral, Ritual, Elemental, Spirit, Tank, Healer, Dps, Playable, Rank1, Rank2, Rank3
        }
        public enum Target
        {
            None, Player, Shaman, Opponent, Elemental, Spirit, Mana, Ally, Enemy, Other, Self, AllAllies, AllEnemies
                /* se tra i target validi di una microazione o di un attacco c'è ALLY, si ciclano le liste ALLY se c'è ENEMY le liste ENEMY*/
        }
    }
}
