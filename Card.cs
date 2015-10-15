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
        public Dictionary<Enums.Mana, int> manaCost;
        public Enums.Type type;
        public Enums.SubType subtype;
        public int castLimit;
        public Enums.Target target;

        string card; // stringa di info parsate da Xml, inizializzata da invocation

        public Card(){}

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
                card.name = InvTemp.name;
                card.manaCost = InvTemp.manaCost;
                card.type = InvTemp.type[0];
                card.subtype = InvTemp.subType[0];
                card.powers = InvTemp.powers;
                card.castLimit = InvTemp.castLimit;
                card.card = InvTemp.getCard();

                card.strength = InvTemp.strength;
                card.constitution = InvTemp.constitution;
                card.hp = card.constitution;
                card.rank = InvTemp.rank;
                if (card.rank > 0)
                    card.from = InvTemp.from;
                card.role = InvTemp.role;
                card.properties = InvTemp.properties;
                card.onAppear = InvTemp.onAppear;
                card.onDeath = InvTemp.onDeath;
                return card;
            }

            if (InvTemp.type[0] == Enums.Type.Ritual)
            {
                Ritual card = new Ritual(InvTemp.name);
                card.name = InvTemp.name;
                card.manaCost = InvTemp.manaCost;
                card.type = InvTemp.type[0];
                card.subtype = InvTemp.subType[0];
                card.powers = InvTemp.powers;
                card.castLimit = InvTemp.castLimit;
                card.card = InvTemp.getCard();

                return card;
            }

            if (InvTemp.type[0] == Enums.Type.Spirit)
            {
               Spirit card = new Spirit(InvTemp.name);
                card.name = InvTemp.name;
                card.manaCost = InvTemp.manaCost;
                card.type = InvTemp.type[0];
                card.subtype = InvTemp.subType[0];
                card.powers = InvTemp.powers;
                card.castLimit = InvTemp.castLimit;
                card.card = InvTemp.getCard();
                Spirit SpiritTemp = new Spirit(this.name);
                SpiritTemp.name = InvTemp.name;
                SpiritTemp.manaCost = InvTemp.manaCost;
                SpiritTemp.type = InvTemp.type[0];
                SpiritTemp.subtype = InvTemp.subType[0];
                SpiritTemp.powers = InvTemp.powers;
                SpiritTemp.castLimit = InvTemp.castLimit;
                SpiritTemp.card = InvTemp.getCard();
                SpiritTemp.essence = InvTemp.essence;
                SpiritTemp.onAppear = InvTemp.onAppear;
                SpiritTemp.onDeath = InvTemp.onDeath;
                return card;
            }

            return null;

        }     
        public void processMicroaction(List<string> paramList) //questa funzione processa e prepara le microazioni
        {

            //deve parsare la stringa 
            foreach (string microaction in paramList) //deve ciclare 
            {
                MicroActions.table[microaction](null);
            }
        }
    }
}
