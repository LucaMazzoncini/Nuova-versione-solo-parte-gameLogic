using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public class Card
    {
        //aggiungere il cast limit
        public string name;
        public int id; //identificatore univoco che viene valorizzato quando entra in gioco
        public List<Power> powers = new List<Power>();
        public bool CanUsePowers = true;
        public Dictionary<Enums.Mana, int> manaCost;
        public Enums.Type type;
        public Enums.SubType subtype;
        public int castLimit;
        public Enums.Target target;
        public string Power_A = "";
        public string Power_B = "";
        public string Power_C = "";

        string card; // stringa di info parsate da Xml, inizializzata da invocation

        public Card() { }

        public Card(string name)
        {
            this.name = name;
        }

        public virtual bool canAttackPlayer(Player targetPlayer)
        {
            return false;
        } // funzioni virtuali per l'override in "Elemental".
        public virtual Player attackPlayer(Player targetPlayer)
        {
            return null;
        }
        public virtual Elemental attackElemental(Elemental targetElem)
        {
            return null;
        }
        public virtual bool canAttackElem(Elemental targetElem, Player controller)
        {
            return false;
        }
        public virtual bool canAttack()
        {
            return false;
        }
        public Card initFromInvocation(Invocation InvTemp)
        {

            if (InvTemp.type[0] == Enums.Type.Elemental)
            {
                Elemental card = new Elemental(InvTemp.name);
                card.name = (string)InvTemp.name.Clone();
                card.manaCost = new Dictionary<Enums.Mana, int>(InvTemp.manaCost);
                card.type = InvTemp.type[0];
                card.subtype = InvTemp.subType[0];
                card.powers = new List<Power>(InvTemp.powers);
                card.castLimit = InvTemp.castLimit;
                card.card = (string)InvTemp.getCard().Clone();

                card.strength = InvTemp.strength;
                card.constitution = InvTemp.constitution;
                card.hp = card.constitution;
                card.rank = InvTemp.rank;
                if (card.rank > 0)
                    card.from = (string)InvTemp.from.Clone();
                card.role = InvTemp.role;
                card.properties = new List<Enums.Properties>(InvTemp.properties);
                card.onAppear = new List<string>(InvTemp.onAppear);
                card.onDeath = new List<string>(InvTemp.onDeath);
                return card;
            }

            if (InvTemp.type[0] == Enums.Type.Ritual)
            {
                Ritual card = new Ritual(InvTemp.name);
                card.name = (string)InvTemp.name.Clone();
                card.manaCost = new Dictionary<Enums.Mana, int>(InvTemp.manaCost);
                card.type = InvTemp.type[0];
                card.subtype = InvTemp.subType[0];
                card.powers = new List<Power>(InvTemp.powers);
                card.castLimit = InvTemp.castLimit;
                card.card = (string)InvTemp.getCard().Clone();

                return card;
            }

            if (InvTemp.type[0] == Enums.Type.Spirit)
            {
                Spirit card = new Spirit(InvTemp.name);
                card.name = (string)InvTemp.name.Clone();
                card.manaCost = new Dictionary<Enums.Mana, int>(InvTemp.manaCost);
                card.type = InvTemp.type[0];
                card.subtype = InvTemp.subType[0];
                card.powers = new List<Power>(InvTemp.powers);
                card.castLimit = InvTemp.castLimit;
                card.card = (string)InvTemp.getCard().Clone();
                Spirit SpiritTemp = new Spirit(this.name);
                SpiritTemp.name = (string)InvTemp.name.Clone();
                SpiritTemp.manaCost = new Dictionary<Enums.Mana, int>(InvTemp.manaCost);
                SpiritTemp.type = InvTemp.type[0];
                SpiritTemp.subtype = InvTemp.subType[0];
                SpiritTemp.powers = new List<Power>(InvTemp.powers);
                SpiritTemp.castLimit = InvTemp.castLimit;
                SpiritTemp.card = InvTemp.getCard();
                SpiritTemp.essence = InvTemp.essence;
                SpiritTemp.onAppear = new List<string>(InvTemp.onAppear);
                SpiritTemp.onDeath = new List<string>(InvTemp.onDeath);
                return card;
            }

            return null;

        }
       
    }
}
