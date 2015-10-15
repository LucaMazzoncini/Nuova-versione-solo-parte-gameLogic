using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic
{
    
    public class Player
    {
        #region variables
        public const int MAXHP = 6;
        public string Name { get; set; }
        public int hp { get; set; }
        public int maxHp { get; set; }
        public Mana mana { get; set; }
        private int id;
        public Target target = new Target();
        public List<Card> cardsOnBoard = new List<Card>();  //nella 4 carta ci sta' lo spirito
        public Dictionary<string,int> castCounter = new Dictionary<string, int>(); //lista che serve per verificare il castLimit
        
        #endregion
        #region utils methods
        public static int ThrowDice(int numberOfFaces)
        {
            return new Random().Next(1,numberOfFaces+1);
        }
        #endregion
       
        
        public Player(string name, int idTemp)
        {
            this.Name = name;
            this.hp = MAXHP;
            this.id = idTemp;
            mana = new Mana(); //alloca e genera i mana random 
                         
            //metti tutta la lista cards a null
        }
        public Player() { }

        public Card PlayCard(Card cardTemp)
        {
            if(CanPlayCard(cardTemp))
            {
                this.mana.PayMana(cardTemp.manaCost); // paghi il costo di mana.
                if (cardTemp.castLimit > 0)
                    castCounter[cardTemp.name] -= 1;
                int idTemp = 2;
                List<List<Enums.Target>> validTargets = new List<List<Enums.Target>>(); // target validi delle microazioni contenute in OnAppear. A stessono indice corrispondo Microazione e Target validi.

                switch (cardTemp.type)
                {
                    case Enums.Type.Elemental:
                        Elemental ElemTemp = (Elemental)cardTemp; // cast a Elemental.
                        foreach (Card cTemp in Game.AllCardsOnBoard) // assegna ID all'elementale.
                            if (idTemp <= cTemp.id)
                                idTemp = cTemp.id + 1;
                        ElemTemp.id = idTemp;
                        cardsOnBoard.Insert(0, ElemTemp); // lo mette sul board inserendolo in prima posizione.
                        Game.AllCardsOnBoard.Add(ElemTemp); // lo mette nella listona di tutte le carte sul board.
                        Game.AllyElementals.Add(ElemTemp.target); // aggiunge a lista di bersagli validi sul board.
                        if (ElemTemp.onAppear != null) // controlla se ci sono microazioni in OnAppear.
                            if (ElemTemp.onAppear.Count > 0)
                            {
                                foreach (string microAct in ElemTemp.onAppear)
                                    validTargets.Add(MicroActions.getTargets(microAct));
                                MicroActionsProcessor.AcquireValidTargets(validTargets); //stora bersagli.
                                MicroActionsProcessor.AcquireMicroactions(ElemTemp.onAppear); // stora microazioni.
                                if (MicroActionsProcessor.canProcessMicroactions()) // controlla se le microazioni hanno tutte almeno 1 target valido.
                                    MicroActionsProcessor.ProcessMicroactions();
                                                             
                            }
                        return ElemTemp;

                    case Enums.Type.Spirit:
                        Spirit SpiritTemp = (Spirit)cardTemp; // cast a Spirit
                        foreach (Card cTemp in Game.AllCardsOnBoard) // assegna ID all'elementale.
                            if (idTemp <= cTemp.id)
                                idTemp = cTemp.id + 1;
                        SpiritTemp.id = idTemp;
                        cardsOnBoard.Add(SpiritTemp); // lo mette sul board in ultima posizione.
                        Game.AllCardsOnBoard.Add(SpiritTemp);
                        Game.AllySpirits.Add(SpiritTemp.target);//aggiunge a lista bersagli validi sul board.
                        return SpiritTemp;

                    case Enums.Type.Ritual:
                        Ritual ritualTemp = (Ritual)cardTemp; // cast a Ritual
                        foreach (Power powTemp in ritualTemp.powers)
                            foreach (string microAct in powTemp.microActions)
                                validTargets.Insert(0, MicroActions.getTargets(microAct));
                        //stesso discorso che per gli Elementals.
                        return ritualTemp;

                    default:
                        return null;                        
                }                       
            }
            return null;
        }
        public bool CanPlayCard(Card cardTemp)
        {
            bool canPlay = true;
            if(cardTemp != null)
            if (this.mana.CanPay(cardTemp.manaCost) && cardsOnBoard != null) //controlla che si possa pagare
            {
                if (cardTemp.castLimit > 0) // check sul CastCounter. Se hai già raggiunto il castLimit, ritorna false.
                {
                    foreach (KeyValuePair<string, int> ktemp in castCounter)
                        if (ktemp.Key == cardTemp.name)
                            if (ktemp.Value == 0)  // ogni volta che si gioca una carta, si decrementa questo valore. Se è a 0 è raggiunto il castlimit.
                                canPlay = false;
                }
                switch (cardTemp.type)
                {
                    case Enums.Type.Elemental:

                        if (cardsOnBoard.Count < 4) // controlla che il board non sia pieno
                        {
                            int elemCount = 0; // se è 3, ci sono già 3 elementali sul tuo board e ritorna false.
                            foreach (Card cTemp in cardsOnBoard)
                                if (cTemp.type == Enums.Type.Elemental)
                                    elemCount += 1;
                            if (elemCount == 3)
                            {
                                canPlay = false;
                                break;
                            }

                            Elemental elemCard = (Elemental)cardTemp;
                                int count = 0;
                                foreach (Elemental elemTemp in cardsOnBoard)
                                    if (elemCard.rank > 1 && elemCard.from != elemTemp.name)
                                        count += 1;
                                if (count == elemCount)
                                    canPlay = false;

                        }
                        break;
                    case Enums.Type.Spirit:

                        if (cardsOnBoard.Count < 4) // controlla che il board non sia pieno
                            foreach (Card Ctemp in cardsOnBoard)
                                if (Ctemp.type == Enums.Type.Spirit)
                                    canPlay = false;
                        break;
                    case Enums.Type.Ritual:

                        List<Enums.Target> targetList = null;
                        foreach (Power powTemp in cardTemp.powers)
                            foreach (string microaction in powTemp.microActions)
                                foreach (Enums.Target targTemp in MicroActions.getTargets(microaction))
                                    targetList.Add(targTemp);
                        if (targetList != null)
                            foreach (Enums.Target tTemp in targetList)
                            {
                                if (tTemp == Enums.Target.Enemy)
                                    if (Game.EnemyElementals.Count == 0)
                                        canPlay = false;
                                if (tTemp == Enums.Target.Ally)
                                    if (Game.AllyElementals.Count == 0)
                                        canPlay = false;
                                if (tTemp == Enums.Target.Spirit)
                                    if (Game.EnemySpirits.Count == 0 && Game.AllySpirits.Count == 0)
                                        canPlay = false;
                            }
                        break;
                }
            }
            else
                canPlay = false;
            else
                canPlay = false;

            return canPlay;        
        }                
        public void TargetUpdated()
        {
            //qui si deve sbloccare la microazione
            // target -> param 
            // Card.power -> indice per locare la function nelle microactions
            // Microactions.table["power"](target come  param)     
        }
        public void InitCastCounter(Bibliotheca invList) //inizializza il castCounter
        {

            if(invList != null)
            foreach (Invocation invTemp in invList.Invocations)
                if (invTemp.castLimit > 0)
                    castCounter.Add(invTemp.name, invTemp.castLimit);
        }
    }

        
}

