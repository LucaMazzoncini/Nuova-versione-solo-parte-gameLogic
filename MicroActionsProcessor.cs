using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic
{
    public static class MicroActionsProcessor
    {
        public static Communication.Communicator comm;
        public static int index = 0;
        public static List<string> microactions = new List<string>(); // stora la lista di microazioni del Power da processare.
        public static List<List<Enums.Target>> targets = new List<List<Enums.Target>>(); // store tutti i target validi di tutte le microazioni di cui è composto il Power. Se una Microaction è associata ad una lista di target vuota, significa che non richiede target in risoluzione.
        public static Dictionary<string, string> microactionParams = new Dictionary<string, string>(); // store di tutti i Param da spedire alle funzioni di MicroActions.table
        //public static List<int> TargetId = new List<int>();

        
        public static object AcquireMicroactionsParams()
        {

            if (index < microactions.Count)
            {
                char[] separator = new char[] { '.' };
                string[] Split;
                if (microactions != null)
                {
                    
                    Split = microactions[index].ToUpper().Split(separator);
                    if (!Split[0].Equals("COOLDOWN"))
                    {
                        if (Split.Length > 1)
                            microactionParams.Add("Value", Split[1]);
                        if (Split.Length > 2)
                            microactionParams.Add("Value2", Split[2]); // alcune microazioni fanno 2 cose con 2 valori diversi, ma stesso bersaglio. tipo "HealArmorElemental.3.1"
                    }

                    if (!targets[index].Contains(Enums.Target.None)) // se ha un bersaglio lo chiede.
                    {
                        Game.SendCommTargets(Game.FindAllValidTargetsId(targets[index]));// -- invia a comm la lista dei target validi tramite id  
                                          
                    }
                    else // se NON ha un bersaglio da scegliere.
                    {
                        // qui esegue Microazione e aggiorna bersagli

                        char sep = '.';
                        string[] splitted = microactions[index].ToUpper().Split(sep);
                        string MicroActionName = splitted[0];
                        if (Split[0] == "ADDMANA")
                        {
                            comm = Communication.Communicator.getInstance();
                            comm.ChoseMana(Enums.ManaEvent.AddMana);
                            return null;
                        }
                        MicroActions.table[MicroActionName](microactionParams); // CHIAMATA

                        if (targets[index].Contains(Enums.Target.Self)) // aggiorna shaman.
                        {
                            Game.UpdateCommPlayers(0, Game.FindTargetPlayerById(0).hp);
                            comm.sendMana(Game.FindTargetPlayerById(0).mana);
                        }
                        if (targets[index].Contains(Enums.Target.Opponent)) // aggiorna opponent.
                        {
                            Game.UpdateCommPlayers(1, Game.FindTargetPlayerById(1).hp);
                            comm.sendMana(Game.FindTargetPlayerById(1).mana);
                        }
                        if (targets[index].Contains(Enums.Target.AllAllies)) // aggiorna All Allies.
                        {
                            Game.UpdateCommPlayers(0, Game.FindTargetPlayerById(0).hp);
                            if (Game.FindTargetPlayerById(0).cardsOnBoard != null)
                                foreach (Elemental elemTemp in Game.FindTargetPlayerById(0).cardsOnBoard)
                                    if (elemTemp.type == Enums.Type.Elemental)
                                        Game.UpdateCommElemental(elemTemp);
                        }
                        if (targets[index].Contains(Enums.Target.AllEnemies)) // aggiorna All Enemies.
                        {
                            Game.UpdateCommPlayers(0, Game.FindTargetPlayerById(1).hp);
                            if (Game.FindTargetPlayerById(1).cardsOnBoard != null)
                                foreach (Card cardTemp in Game.FindTargetPlayerById(1).cardsOnBoard)
                                    if (cardTemp.type == Enums.Type.Elemental)
                                        Game.UpdateCommElemental((Elemental)cardTemp);
                        }

                        microactions.RemoveAt(index); // svuota la posizione [0] di tutte le liste.
                        targets.RemoveAt(index);
                        microactionParams.Clear();
                        //index += 1;
                        AcquireMicroactionsParams();
                    }
                }
            }
            return null;          
        }
        public static void AcquireValidTargets(List<List<Enums.Target>> validTargets)
        {
            targets = validTargets;
        }
        public static void AcquireMicroactions(List<string> microActions)
        {
            microactions = microActions;
        }
        public static bool canProcessMicroactions() // verifica che tutte le microazioni del potere, che richiedano bersaglio, abbiano almeno 1 bersaglio valido.
        {
            bool canProcess = false;
            bool[] canProc = new bool[targets.Count];
            bool norAllynorEnemy = false;
            int indexTemp = 0;

            do
            {
                norAllynorEnemy = !targets[indexTemp].Contains(Enums.Target.Ally) && !targets[indexTemp].Contains(Enums.Target.Enemy);
                if (targets[indexTemp].Contains(Enums.Target.None))
                {
                    canProc[indexTemp] = true;
                    indexTemp += 1;
                    if (indexTemp < canProc.Length)
                        continue;
                    else
                        break;
                }
                foreach (Enums.Target target in targets[indexTemp])
                {
                    if (target == Enums.Target.Player || target == Enums.Target.Shaman || target == Enums.Target.Opponent)
                    {
                        canProc[indexTemp] = true;
                        indexTemp += 1;
                        if (indexTemp < canProc.Length)
                            continue;
                        else
                            break;
                    }
                    if (targets[indexTemp].Contains(Enums.Target.Ally) || norAllynorEnemy)
                    {
                        foreach (Enums.Target allyTarget in Game.AllyElementals)
                            if (target == allyTarget)
                            {
                                canProc[indexTemp] = true;
                                break;
                            }
                        foreach (Enums.Target allyTarget in Game.AllySpirits)
                            if (target == allyTarget)
                            {
                                canProc[indexTemp] = true;
                                break;
                            }
                    }
                    if (targets[indexTemp].Contains(Enums.Target.Enemy) || norAllynorEnemy)
                    {
                        foreach (Enums.Target EnemyTarget in Game.EnemyElementals)
                            if (target == EnemyTarget)
                            {
                                canProc[indexTemp] = true;
                                break;
                            }
                        foreach (Enums.Target EnemyTarget in Game.EnemySpirits)
                            if (target == EnemyTarget)
                            {
                                canProc[indexTemp] = true;
                                break;
                            }
                    }
                    if (canProc[indexTemp])
                        break;
                }
                if (indexTemp < canProc.Length)
                indexTemp += 1;               
            } while (indexTemp < targets.Count);

            int countTrue = 0;
            for (int i = 0; i < canProc.Length; i++)
                if (canProc[i] == true)
                    countTrue += 1;
            if (countTrue == canProc.Length)
                canProcess = true;
            return canProcess;
        }
    } 
}    


    
