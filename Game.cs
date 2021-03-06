﻿ using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;


namespace GameLogic
{
    public class Game
    {

        #region var
        public delegate void GenericEventHandler();
        public delegate void ResultEventHandler(int result);

        public event ResultEventHandler sendDiceResult;
        public event GenericEventHandler requestXmlForBibliotheca;

        private static Player shaman;
        private static Player opponent;
        private int diceResult;
        private int opponentDiceResult = 0;
        private bool myRound = false;
        Communication.Communicator comm;
        private static bool manaChosen; //questo flag ci dice se il mana e' gia' stato scelto
        private static bool opponentReady = false;
        private bool unityReady = false;
        Bibliotheca bibliotheca;
        public static List<Enums.Target> AllyElementals = new List<Enums.Target>();
        public static List<Enums.Target> EnemyElementals = new List<Enums.Target>();
        public static List<Enums.Target> AllySpirits = new List<Enums.Target>();
        public static List<Enums.Target> EnemySpirits = new List<Enums.Target>();
        public static List<Card> AllCardsOnBoard = new List<Card>();

        private static Game _instance = null;

        public static void init(string name)
        {
            _instance = new Game(name);
        }

        public static Game getInstance()
        {
            return _instance; // This may return null if not initialized
        }

        #endregion
        public Game(string name)
        {
            shaman = new Player(name, 0);         //vanno inizializzati
            opponent = new Player("Opponent", 1); //vanno inizializzati
            comm = Communication.Communicator.getInstance();


        }
        #region setGet
        public bool isMyRound()
        {
            return myRound;
        }
        public string GetOppenentName()
        {
            return opponent.Name;
        }

        public string GetShamanName()
        {
            return shaman.Name;
        }
        public void SetOpponentName(string name) //da tenere
        {
            opponent.Name = name;
        }
        public void SetOpponent(Player param)
        {
            opponent = param;
        }
        public bool getRoundFlag()
        {
            return myRound;
        }
        #endregion
        #region Who start
        public void ThrowDice()
        {
            ThrowDice(Player.ThrowDice(999999));
        }

        public void ThrowDice(int diceValue)
        {
            diceResult = diceValue;
        }

        public void OnOpponentDiceResult(int opponentDiceResult) //in questa funzione viene stabilito di chi e' il turno
        {

            this.opponentDiceResult = opponentDiceResult;
            comm = Communication.Communicator.getInstance();
            comm.sendMana(shaman.mana);
            myRound = false;
            if (diceResult == opponentDiceResult)//questa parte andra' ricontrollata il problema era che nn era inizializzato Comm
            {
                ThrowDice();
                comm.game_diceResult(diceResult); //si invia nuovamente il risultato del dado
                if (diceResult > opponentDiceResult)
                {
                    myRound = true;
                    FirstRoundStart();
                }

            }
            else
            {
                if (diceResult > opponentDiceResult)
                {
                    myRound = true;
                }
                FirstRoundStart();
            }

        }

        #endregion

        #region Metodi chiamati da Comunicator
        public void UpdateElemental(Elemental ele)
        {
            int indShaman = 0;
            int indOppo = 0;
            foreach (Elemental eleTemp in shaman.cardsOnBoard)
            {
                if (eleTemp.id == ele.id)
                {
                    shaman.cardsOnBoard.RemoveAt(indShaman);
                    shaman.cardsOnBoard.Insert(0, ele);
                    break;
                }
                indShaman += 1;

            }
            foreach (Elemental eleTemp in opponent.cardsOnBoard)
            {
                if (eleTemp.id == ele.id)
                {
                    opponent.cardsOnBoard.RemoveAt(indOppo);
                    opponent.cardsOnBoard.Insert(0, ele);
                    break;
                }
                indOppo += 1;
            }


            //Da ricontrollare se va' bene qui
            if (ele.hp < 1)
                RemoveCardById(ele.id);

        }

        public void CanAttack(int id)
        {
            foreach (Card cardTemp in shaman.cardsOnBoard)
                if (cardTemp.id == id)
                    if (cardTemp.canAttack())
                        comm.YouCanAttack(id);

        }

        public void OpponentPlayCard(Card card)
        {

            //qui va' presa la carta ed aggiunta all'opponent
            opponent.cardsOnBoard.Add(card);

            Game.AllCardsOnBoard.Add(card);
            Game.EnemyElementals.Add(card.target); //!!!!!!ATTENTO IL TARGET E' DA VALORIZZARE aggiunge a lista di bersagli validi sul board.

        }
        public void GetPlayerMana()
        {
            comm.ManaPlayerCallback(shaman.mana);
        }

        public void CreateShamanPool(Enums.Mana mana)
        {
            if (shaman.mana.canCreatePool(mana))
            {
                if (shaman.mana.createPool(mana)) //se la creazione e' andata a buon fine viene visualizzata altrimenti no
                {
                    comm.sendMana(shaman.mana);
                    comm.sendOpponentPool(mana, shaman.mana.poolList[mana]);
                    comm.DisplayPool(mana, shaman.mana.poolList[mana]); //fa' visualizzare dalla grafica le polle
                }
            }
        }
        public void CanCreateManaPool(Enums.Mana mana) //questa funzione e' chiamata dalla grafica per sapere se puo' creare una polla di un tipo
        {
            if (shaman.mana.canCreatePool(mana) && myRound == true)
                comm.YesYouCanCreateManaPool(mana);
        }

        public LinkedList<Invocation> MenuFiltered(List<Enums.Filter> param) //questa funzione ritorna una linkedList delle carte filtrate
        {
            return bibliotheca.getCards(param, shaman.mana);
        }
        public void EndTourn() //viene chiamato quando shaman passa il turno
        {
            comm = Communication.Communicator.getInstance();
            myRound = false;
            comm.setRound(myRound);
            comm.EndRound(); //chiamata per cambiare il round
        }
        public void StartTourn()
        {
            comm = Communication.Communicator.getInstance();
            myRound = true;
            shaman.mana.setPoolFlag(false);
            if (shaman.cardsOnBoard != null)
                if (shaman.cardsOnBoard.Count != 0)
                {
                    foreach (Elemental elemTemp in shaman.cardsOnBoard)
                        if (elemTemp.type == Enums.Type.Elemental)
                        {
                            elemTemp.hasAttacked = false;
                            elemTemp.hasAttackedThunderborn = false;
                            elemTemp.hasWeakness = false;
                        }
                    foreach (Card cardTemp in shaman.cardsOnBoard)
                        cardTemp.CanUsePowers = true;
                }
            
            PowersCooldowns(); // scala clock poteri.

            if (opponent.cardsOnBoard != null)
                if (opponent.cardsOnBoard.Count != 0)
                    foreach (Elemental elemTemp in opponent.cardsOnBoard) // poison scalato.
                        if (elemTemp.debuff.Contains(Enums.Debuff.Poison))
                        {
                            foreach (Enums.Debuff deBuff in elemTemp.debuff)
                                elemTemp.hp -= 1;
                            comm.UpdateElemental(elemTemp);
                        }




            comm.setRound(myRound);//invio la chiamata in locale
            comm.ChoseMana(Enums.ManaEvent.NewRound); //Chiedo di selezionare il mana che prendo in manaAtStart
        }
        public void PowersCooldowns() // scala di 1 il cooldownClock dei powers.
        {
            if (isMyRound())
                foreach (Card cardTemp in shaman.cardsOnBoard)
                    if (cardTemp.powers != null)
                        if (cardTemp.powers.Count > 0)
                        {
                            cardTemp.CanUsePowers = true;
                            foreach (Power powTemp in cardTemp.powers)
                                if (powTemp.clock > 0)
                                    powTemp.clock -= 1;
                        }
        }

        public void manaChoosen(Enums.Mana manaParam, Enums.ManaEvent manaEventparam) //mi passa il mana selezionato
        {
            if (manaEventparam == Enums.ManaEvent.NewRound)
            {
                comm = Communication.Communicator.getInstance();
                shaman.mana.incMana(manaParam);    //se ha raggiunto il mana max non viene aggiunto il mana
                comm.SendOpponentManaChosen(manaParam); //invio il mana scelto all'avversario
                comm.sendMana(shaman.mana);  //invio l'update del mana
                Enums.Mana manaTemp = shaman.mana.addRandomMana(); //aggiungo il mana random allo shamano
                comm.sendMana(shaman.mana);  //invio l'update del mana

                //Aggiungo il mana delle polle
                shaman.mana.addManaPool();
                comm.sendMana(shaman.mana);  //invio l'update del mana
                                             //Ricordati che ho fatto 3 send mana invece di uno perche' cosi' possiamo fare 3 animazioni distinte in base al mana che viene aggiunto
            }

            if (manaEventparam == Enums.ManaEvent.AddMana)
            {
                comm = Communication.Communicator.getInstance();
                MicroActionsProcessor.microactionParams.Add("Mana", manaParam.ToString());
                char separator = '.';
                string[] splitted = MicroActionsProcessor.microactions[MicroActionsProcessor.index].ToUpper().Split(separator);
                string MicroActionName = splitted[0];
                // qui chiama la MicroAzione e aggiorna i bersagli.

                MicroActions.table[MicroActionName](MicroActionsProcessor.microactionParams); // CHIAMATA

                comm.sendMana(shaman.mana);
                comm.SendOpponentManaChosen(manaParam);// player si aggiorna.

                MicroActionsProcessor.microactions.RemoveAt(MicroActionsProcessor.index); // svuota la posizione [0] di tutte le liste.

                MicroActionsProcessor.targets.RemoveAt(MicroActionsProcessor.index);

                MicroActionsProcessor.microactionParams.Clear();

                MicroActionsProcessor.AcquireMicroactionsParams(); //callback a AcquireMicroactionsParam.
            }
        }

        public void PlayCard(string name)
        {
            if (CanPlayCard(name))
            {
                Card playedCard = bibliotheca.getCardByName(name);
                shaman.PlayCard(playedCard);
                comm.sendMana(shaman.mana);
                if (playedCard.type != Enums.Type.Elemental)
                    comm.SendPlayedCard(playedCard);
                else
                {
                    Elemental ele = (Elemental)playedCard;
                    if (ele.rank < 2)
                        comm.SendPlayedCard(playedCard);
                    else
                        comm.UpdateElemental(ele);
                }
            }
        }
        public bool CanPlayCard(string name)
        {
            return shaman.CanPlayCard(bibliotheca.getCardByName(name)) && isMyRound();
        }


        public void CanCastPower(int idCard)
        {
            if (idCard > 1)
            {
                List<string> Pows = new List<string>();
                Card cardTemp = FindTargetCardByID(idCard);
                int powIndex = 0;

                if (cardTemp.powers != null)
                    if (cardTemp.powers.Count > 0)
                        foreach (Power powTemp in cardTemp.powers)
                        {
                            string powString = powIndex.ToString() + "." + powTemp.cooldown.ToString() + ".Ability.";
                            if (powTemp.IsCastable() && cardTemp.CanUsePowers)
                                powString += "true";
                            else
                                powString += "false";
                            Pows.Add(powString);
                            powIndex += 1;
                        }
                comm.CastablePowers(Pows);
            }
                //ogni string in CastablePowers deve avere questo formato: "powerIndex.powercooldown.texture.boolcastabile"
       }
        public void CastPower(string power, int idElemental)
        {
            if (isMyRound())
            {
                int powIndex = Int32.Parse(power);
                Card cardTemp = FindTargetCardByID(idElemental);
                Elemental elemTemp = (Elemental)cardTemp;
                List<List<Enums.Target>> validTargets = new List<List<Enums.Target>>();

                if (cardTemp.CanUsePowers)
                {
                    if (!(elemTemp.type == Enums.Type.Elemental && elemTemp.debuff.Contains(Enums.Debuff.Asleep)))
                    {
                        if (cardTemp.powers[powIndex].IsCastable())
                            foreach (string microAct in cardTemp.powers[powIndex].microActions)
                            {
                                validTargets.Add(MicroActions.getTargets(microAct));
                            }
                        cardTemp.powers[powIndex].clock = cardTemp.powers[powIndex].cooldown;
                        cardTemp.CanUsePowers = false;
                        List<string> microaction = new List<string>(cardTemp.powers[powIndex].microActions);
                        MicroActionsProcessor.AcquireMicroactions(microaction);
                        MicroActionsProcessor.AcquireValidTargets(validTargets);
                    if (MicroActionsProcessor.canProcessMicroactions())// controlla se le microazioni hanno tutte almeno 1 target valido.
                            MicroActionsProcessor.AcquireMicroactionsParams();
                    }
                }
            }

        }

        public void TargetEvent(int idTarget) //questa funzione riceve il target richiesto precedentemente
        {
            shaman.target = FindTargetById(idTarget);
            shaman.TargetUpdated(); //questo evento viene chiamato per avvertire il player che la carta e' arrivata
        }

        public void AttackTarget(int idAttacker, int idTarget)
        {
            if (isMyRound())
            {
                int indexAttacker = 0;
                int indexTarget = 0;
                if (shaman.cardsOnBoard != null)
                    if (opponent.cardsOnBoard != null)
                    {
                        for (indexAttacker = 0; indexAttacker < shaman.cardsOnBoard.Count; indexAttacker++)
                            if (shaman.cardsOnBoard[indexAttacker].id == idAttacker)
                                break;
                        for (indexTarget = 0; indexTarget < opponent.cardsOnBoard.Count; indexTarget++)
                            if (opponent.cardsOnBoard[indexTarget].id == idTarget)
                                break;

                        if (shaman.cardsOnBoard[indexAttacker].canAttackElem((Elemental)opponent.cardsOnBoard[indexTarget], opponent))
                            shaman.cardsOnBoard[indexAttacker].attackElemental((Elemental)opponent.cardsOnBoard[indexTarget]);
                    }
                comm.ResultAttackElemental((Elemental)shaman.cardsOnBoard[indexAttacker], (Elemental)opponent.cardsOnBoard[indexTarget]); // ResultAttack al momento non esiste.

                Elemental temp = (Elemental)shaman.cardsOnBoard[indexAttacker];
                if (temp.hp < 1)
                    RemoveCardById(temp.id);
                temp = (Elemental)opponent.cardsOnBoard[indexTarget];
                if (temp.hp < 1)
                    RemoveCardById(temp.id);
            }
        }

        public void TargetReturn(int id)
        {

            MicroActionsProcessor.microactionParams.Add("idTarget", id.ToString()); //aggiunge target alla lista di parametri
            char separator = '.';
            string[] splitted = MicroActionsProcessor.microactions[MicroActionsProcessor.index].ToUpper().Split(separator);
            string MicroActionName = splitted[0];
            // qui chiama la MicroAzione e aggiorna i bersagli.

            MicroActions.table[MicroActionName](MicroActionsProcessor.microactionParams); // CHIAMATA

            if (id == 0 || id == 1)
                UpdateCommPlayers(id, FindTargetPlayerById(id).hp); // se il bersaglio era player lo aggiorna.

            if (id > 1)
                UpdateCommElemental((Elemental)FindTargetCardByID(id)); // se il bersaglio era elementale lo aggiorna. 

            //MicroActionsProcessor.index += 1; // incrementa index
            MicroActionsProcessor.microactions.RemoveAt(MicroActionsProcessor.index); // svuota la posizione [0] di tutte le liste.

            MicroActionsProcessor.targets.RemoveAt(MicroActionsProcessor.index);

            MicroActionsProcessor.microactionParams.Clear();

            MicroActionsProcessor.AcquireMicroactionsParams(); //callback a AcquireMicroactionsParam.

            comm.PlaySpell("Spell", id); //aggiorna la grafica, al posto di spell ci va' la carta giocata


        }

        public void LostManaRandomOpponent()
        {
            //comm.LostManaRandomOpponent();

        }
        public void LostManaRandom()
        {
            Player playerTemp = Game.FindTargetPlayerById(0);
            Random random = new Random();
            Boolean lost = false;
            do
            {
                int randomNumber = random.Next(1, 6);
                Enums.Mana manaTemp = new Enums.Mana();
                if (playerTemp.mana.GetTotalMana() > 0)
                {
                    switch (randomNumber)
                    {
                        case 1:
                            manaTemp = Enums.Mana.Water;
                            break;

                        case 2:
                            manaTemp = Enums.Mana.Earth;
                            break;

                        case 3:
                            manaTemp = Enums.Mana.Fire;
                            break;

                        case 4:
                            manaTemp = Enums.Mana.Life;
                            break;

                        case 5:
                            manaTemp = Enums.Mana.Death;
                            break;

                        default:
                            break;
                    }
                    if (playerTemp.mana.valueList[manaTemp] > 0)
                    {
                        playerTemp.mana.decMana(manaTemp);
                        lost = true;
                    }
                }
            } while (lost == false);
            comm.sendMana(FindTargetPlayerById(0).mana);   
        }
    


        public void GetValidAttackableTarget(int idAttacker)
        {
            List<int> idList = new List<int>();
            Elemental elemTemp = (Elemental)FindTargetCardByID(idAttacker);
            if (elemTemp.canAttackPlayer(opponent))
                idList.Add(1);
            foreach (Elemental elem in opponent.cardsOnBoard)
                if (elemTemp.canAttackElem(elem, opponent))
                    idList.Add(elem.id);
            comm.SendValidAttackableTarget(idList);
        }

        public void AttackPlayer(int idAttacker)
        {
            if (isMyRound())
            {
                int indexAttacker = 0;
                if (shaman.cardsOnBoard != null)
                {
                    for (indexAttacker = 0; indexAttacker < shaman.cardsOnBoard.Count; indexAttacker++)
                        if (shaman.cardsOnBoard[indexAttacker].id == idAttacker)
                            break;

                    if (shaman.cardsOnBoard[indexAttacker].canAttackPlayer(opponent))
                        shaman.cardsOnBoard[indexAttacker].attackPlayer(opponent);
                }
                comm.ResultAttackPlayer(shaman.cardsOnBoard[indexAttacker], opponent); // da implementare.
            }
        }

        public void ShamanHpUpdate(int hp)
        {
            shaman.hp = hp;
        }
        public void OpponentHpUpdate(int hp)
        {
            opponent.hp = hp;
        }


        public static List<int> FindAllValidTargetsId(List<Enums.Target> targetList) // gli passi una lista di tipi di bersagli validi e ti ritorna la lista degli ID dei bersagli effettivamente validi.
        {
            List<int> idList = new List<int>();
            if (targetList != null)
                if (targetList.Contains(Enums.Target.Shaman))
                    idList.Add(0);
            if (targetList.Contains(Enums.Target.Opponent))
                if (IsDamageAction(MicroActionsProcessor.microactions[0]))
                {
                    if (!IsAnyGuardian(opponent.cardsOnBoard))
                        idList.Add(1);
                }
                else
                    idList.Add(1);
            if (targetList.Contains(Enums.Target.Player))
            {
                idList.Add(0);

                if (IsDamageAction(MicroActionsProcessor.microactions[0]))
                    {
                        if (!IsAnyGuardian(opponent.cardsOnBoard))
                            idList.Add(1);
                    }
                else
                    idList.Add(1);
            }
            if (targetList.Contains(Enums.Target.Elemental))
            {
                char separator = '.';
                string[] splitted = MicroActionsProcessor.microactions[0].ToUpper().Split(separator);

                if (targetList.Contains(Enums.Target.Ally))
                    foreach (Card cardTemp in shaman.cardsOnBoard)
                        if (cardTemp.type == Enums.Type.Elemental)
                            idList.Add(cardTemp.id);
                if (targetList.Contains(Enums.Target.Enemy))
                    foreach (Elemental elemTemp in opponent.cardsOnBoard)
                    {
                        if (elemTemp.type == Enums.Type.Elemental)
                            if (IsDamageAction(MicroActionsProcessor.microactions[0]))
                            {
                                if (IsAnyGuardian(opponent.cardsOnBoard))
                                    if (elemTemp.properties.Contains(Enums.Properties.Guardian))
                                        idList.Add(elemTemp.id);
                            }
                            else
                                idList.Add(elemTemp.id);
                    }
                if (!targetList.Contains(Enums.Target.Ally) && !targetList.Contains(Enums.Target.Enemy))
                {
                    foreach (Card cardTemp in shaman.cardsOnBoard)
                        if (cardTemp.type == Enums.Type.Elemental)
                            idList.Add(cardTemp.id);
                    foreach (Elemental elemTemp in opponent.cardsOnBoard)
                    {
                        if (elemTemp.type == Enums.Type.Elemental)
                            if (IsDamageAction(MicroActionsProcessor.microactions[0]))
                            {
                                if (IsAnyGuardian(opponent.cardsOnBoard))
                                    if (elemTemp.properties.Contains(Enums.Properties.Guardian))
                                        idList.Add(elemTemp.id);
                            }
                            else
                                idList.Add(elemTemp.id);
                    }
                }
            }
                if (targetList.Contains(Enums.Target.Spirit))
            {
                if (targetList.Contains(Enums.Target.Ally))
                    foreach (Card cardTemp in shaman.cardsOnBoard)
                        if (cardTemp.type == Enums.Type.Spirit)
                            idList.Add(cardTemp.id);
                if (targetList.Contains(Enums.Target.Enemy))
                    foreach (Card cardTemp in opponent.cardsOnBoard)
                        if (cardTemp.type == Enums.Type.Spirit)
                            idList.Add(cardTemp.id);
                if (!targetList.Contains(Enums.Target.Ally) && !targetList.Contains(Enums.Target.Enemy))
                    foreach (Card cardTemp in AllCardsOnBoard)
                        if (cardTemp.type == Enums.Type.Spirit)
                            idList.Add(cardTemp.id);
            }


            return idList;
        }


        public static void SendCommTargets(List<int> idTargetsList)
        {
            Game.getInstance().comm.SendValidTargets(idTargetsList);      
        } 

        public static void UpdateCommElemental(Elemental elemTemp) // incapsula UpdCommElem. (accessibilità)
        {
            Game.getInstance().comm.UpdateElemental(elemTemp);
        }

        public static void UpdateCommPlayers (int idPlayer, int hpPlayer)
        {
            Game.getInstance().comm.UpdatePlayers(idPlayer, hpPlayer);
        }

        public static Card FindTargetCardByID(int idTemp)
        {
            if (idTemp > 1)
                if (shaman.cardsOnBoard != null)
                    foreach (Card cardTemp in shaman.cardsOnBoard)
                        if (cardTemp.id == idTemp)
                            switch (cardTemp.type)
                            {
                                case Enums.Type.Elemental:
                                    Elemental ElemTemp = (Elemental)cardTemp;
                                    return ElemTemp;

                                case Enums.Type.Spirit:
                                    Spirit SpiritTemp = (Spirit)cardTemp;
                                    return SpiritTemp;
                            }
            if (opponent.cardsOnBoard != null)
                foreach (Card cardTemp in opponent.cardsOnBoard)
                    if (cardTemp.id == idTemp)
                        switch (cardTemp.type)
                        {
                            case Enums.Type.Elemental:
                                Elemental ElemTemp = (Elemental)cardTemp;
                                return ElemTemp;

                            case Enums.Type.Spirit:
                                Spirit SpiritTemp = (Spirit)cardTemp;
                                return SpiritTemp;
                        }
            return null;
        }

        public static Player FindTargetPlayerById(int idTemp)
        {
            if (idTemp == 0)
                return shaman;
            if (idTemp == 1)
                return opponent;

            return null;
        }

        public static void RemoveCardById(int idTemp)
        {
            if (idTemp > 1)
                if (shaman.cardsOnBoard != null)
                    foreach (Card cardTemp in shaman.cardsOnBoard)
                    {
                        if (cardTemp.id == idTemp)
                        {
                            if (cardTemp.GetType() == typeof(Elemental))
                            {
                                if (AllyElementals != null)
                                    foreach (Enums.Target targTemp in AllyElementals)
                                        if (targTemp == cardTemp.target)
                                        {
                                            AllyElementals.Remove(targTemp); // rimuove dai target validi
                                            break;
                                        }
                            }
                            if (cardTemp.GetType() == typeof(Spirit))
                            {
                                if (AllySpirits != null)
                                    foreach (Enums.Target targTemp in AllySpirits)
                                        if (targTemp == cardTemp.target)
                                        {
                                            AllySpirits.Remove(targTemp); // rimuove dai target validi
                                            break;
                                        }
                            }
                            shaman.cardsOnBoard.Remove(cardTemp); // rimuove dal board
                            break;
                        }
                    }

            if (opponent.cardsOnBoard != null)
                foreach (Card cardTemp in opponent.cardsOnBoard)
                {
                    if (cardTemp.id == idTemp)
                    {
                        if (cardTemp.GetType() == typeof(Elemental))
                        {
                            if (EnemyElementals != null)
                                foreach (Enums.Target targTemp in EnemyElementals)
                                    if (targTemp == cardTemp.target)
                                    {
                                        EnemyElementals.Remove(targTemp); // rimuove dai target validi
                                        break;
                                    }
                        }
                        if (cardTemp.GetType() == typeof(Spirit))
                        {
                            if (EnemySpirits != null)
                                foreach (Enums.Target targTemp in EnemySpirits)
                                    if (targTemp == cardTemp.target)
                                    {
                                        EnemySpirits.Remove(targTemp); // rimuove dai target validi
                                        break;
                                    }
                        }
                        opponent.cardsOnBoard.Remove(cardTemp); // rimuove dal board
                        break;
                    }
                }
            if (AllCardsOnBoard != null)
                foreach (Card cardTemp in AllCardsOnBoard)
                    if (cardTemp.id == idTemp)
                    {
                        AllCardsOnBoard.Remove(cardTemp); // rimuove dalla lista di tutte le carte sul board                    
                        break;
                    }
        }

        public static bool IsAlly(int idTemp)
        {
            foreach (Card cardTemp in shaman.cardsOnBoard)
                if (cardTemp.id == idTemp)
                {
                    return true;
                }
            return false;
        }
        public static bool IsAnyGuardian(List<Card> cardList)
        {
            foreach (Card cardTemp in cardList)
                if (cardTemp.type == Enums.Type.Elemental)
                {
                    Elemental elemTemp = (Elemental)cardTemp;
                    if (elemTemp.properties.Contains(Enums.Properties.Guardian))
                        return true;
                }
            return false;                   
        }
        public static bool IsDamageAction(string microaction)
        {
            string[] splitted = microaction.ToUpper().Split('.');
            if (splitted[0] == "DAMAGE" || splitted[0] == "DAMAGEELEMENTAL" || splitted[0] == "DAMAGEENEMYELEMENTAL" || splitted[0] == "DAMAGEPOISONELEMENTAL" ||  splitted[0] == "DAMAGEPLAYER")
                return true;
            return false;
        }
        #endregion

        #region Metodi chiamati da altri oggetti a Comunicator
        public void GetAnyTarget()
        {
            comm = Communication.Communicator.getInstance();
            comm.GetAnyTarget();
        }

        public void GetPlayerTarget()
        {
            comm = Communication.Communicator.getInstance();
            comm.GetPlayersTarget();
        }
        public void GetElementalTarget()
        {
            comm = Communication.Communicator.getInstance();
            comm.GetElementalTarget();
        }

        public void GetSpiritTarget()
        {
            comm = Communication.Communicator.getInstance();
            comm.GetAllyElementalTarget();
        }



        #endregion

        #region Metodi Chiamati da Interface
        public void MenuRequest(List<Enums.Filter> filtrerList, Mana mana)
        {
            LinkedList<Invocation> cardList = bibliotheca.getCards(filtrerList, mana);
            comm.MenuFiltered(cardList);
        }
        #endregion
        public void LoadBibliotheca(LinkedList<string> xmlInvocations)
        {
            bibliotheca = new Bibliotheca(xmlInvocations);
        }
        private Target FindTargetById(int id)
        {
            Target ret = new Target();
            ret.id = id;
            if (id == 0)
            {
                ret.name = "SHAMAN";
                ret.target = Enums.Target.Player;
            }
            if (ret.id == 1)
            {
                ret.name = "OPPONENT";
                ret.target = Enums.Target.Player;
            }
            if (ret.id > 1)
            {


                foreach (Card card in shaman.cardsOnBoard)


                {
                    if (card.id == id)
                    {
                        ret.name = card.name;
                        ret.target = card.target;
                        return ret;
                    }
                }


                foreach (Card card in opponent.cardsOnBoard)

                {
                    if (card.id == id)
                    {
                        ret.name = card.name;
                        ret.target = card.target;
                        return ret;
                    }
                }

            }

            return ret;
        } 

        #region Inizio round
        public void FirstRoundStart()  //viene chiamato solo la prima volta
        {
            //invio i miei dati all'opponent
            comm = Communication.Communicator.getInstance();
            //comm.sendPlayerInfo(shaman);
            comm.setRound(myRound);   //setto il round per la grafica
            comm.sendMana(shaman.mana);  //Primo round invio il mana alla grafica
            if (myRound)
            { 
                Enums.Mana manaTemp = shaman.mana.addRandomMana(); //aggiungo il mana random allo shamano
                comm.sendMana(shaman.mana);  //invio l'update del mana
            }
        }

        public void OpponentIsReady()
        {
            opponentReady = true;
            if(unityReady)
            {
                comm.Loaded();
                starMatch();
            }
        }

        public void UnityReady()
         {
            unityReady = true;
            comm.UnityOpponentIsReady(); //invio all'opponent che il mio dispositivo e' pronto
            if (opponentReady)
            {
                comm.Loaded();
                starMatch();
            }            
        }

        public void starMatch()
        {
            XmppCommunicator.Utils.Log("Partita Iniziata");
            comm.SendOpponentName(shaman.Name);
            ThrowDice(); //lancio il dado per vedere chi inizia

            requestXmlForBibliotheca();
            comm.game_diceResult(diceResult);
            shaman.InitCastCounter(bibliotheca);  //inizializza i CastCounter.
            opponent.InitCastCounter(bibliotheca);

            #endregion
        }
    }
}
