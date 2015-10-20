using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic
{
    public static class MicroActionsProcessor
    {
        public static int index = 0;
        public static List<string> microactions = new List<string>(); // stora la lista di microazioni del Power da processare.
        public static List<List<Enums.Target>> targets = new List<List<Enums.Target>>(); // store tutti i target validi di tutte le microazioni di cui è composto il Power. Se una Microaction è associata ad una lista di target vuota, significa che non richiede target in risoluzione.
        public static Dictionary<string, string> microactionParams = new Dictionary<string, string>(); // store di tutti i Param da spedire alle funzioni di MicroActions.table
        //public static List<int> TargetId = new List<int>();

        
        public static void AcquireMicroactionsParams()
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
                        int id = -5;
                        if (targets[index].Contains(Enums.Target.Self))
                            id = -1; // aggiorna solo shaman.
                        if (targets[index].Contains(Enums.Target.Opponent))
                            id = -2; // aggiorna solo opponent.
                        if (targets[index].Contains(Enums.Target.AllAllies))
                            id = -3; // aggiorna tutti elementali ally.
                        if (targets[index].Contains(Enums.Target.AllEnemies))
                            id = -4; // aggiorna tutti elementali enemy.

                        // qui esegue Microazione e aggiorna bersagli

                        char sep = '.';
                        string[] splitted = microactions[index].ToUpper().Split(sep);
                        string MicroActionName = splitted[0];
                        MicroActions.table[MicroActionName](microactionParams); // CHIAMATA

                        if (id == -1) // aggiorna shaman.
                            Game.UpdateCommPlayers(0, Game.FindTargetPlayerById(0).hp);
                        if (id == -2) // aggiorna opponent.
                            Game.UpdateCommPlayers(1, Game.FindTargetPlayerById(1).hp);
                        if (id == -3) // aggiorna All Allies.
                        {
                            if (Game.FindTargetPlayerById(0).cardsOnBoard != null)
                                foreach (Card cardTemp in Game.FindTargetPlayerById(0).cardsOnBoard)
                                    if (cardTemp.type == Enums.Type.Elemental)
                                        Game.UpdateCommElemental((Elemental)cardTemp);
                        }
                        if (id == -4) // aggiorna All Enemies.
                        {
                            if (Game.FindTargetPlayerById(1).cardsOnBoard != null)
                                foreach (Card cardTemp in Game.FindTargetPlayerById(1).cardsOnBoard)
                                    if (cardTemp.type == Enums.Type.Elemental)
                                        Game.UpdateCommElemental((Elemental)cardTemp);
                        }

                        MicroActionsProcessor.microactions.RemoveAt(index); // svuota la posizione [0] di tutte le liste.
                        MicroActionsProcessor.targets.RemoveAt(index);
                        MicroActionsProcessor.microactionParams.Clear();
                        //index += 1;
                        AcquireMicroactionsParams();
                    }
                }
            }
            //else
                //ProcessMicroactions(); // finito di acquisire tutto, comincia a processare.           
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
       /* public static void ProcessMicroactions()
        {
            List<string> callMicroations = new List<string>(); // come microactions, ma senza .values, solo nome microazione
            char separator = '.';
            index = 0;
            foreach (string stringTemp in microactions)
            {
                string[] splitted = stringTemp.ToUpper().Split(separator);
                callMicroations.Add(splitted[0]);
            }
            while (callMicroations.Count > 0)
            {                                          
                MicroActions.table[callMicroations[index]](microactionParams[index]);
                if (TargetId[index] == 0 || TargetId[index] == 1)
                    Game.UpdateCommPlayers(TargetId[index], Game.FindTargetPlayerById(TargetId[index]).hp); // se il bersaglio era player lo aggiorna.
                if(TargetId[index] > 1)
                    Game.UpdateCommElemental((Elemental)Game.FindTargetCardByID(TargetId[index])); // se il bersaglio era elementale lo aggiorna.
                if (TargetId[index] == -1) // aggiorna shaman.
                    Game.UpdateCommPlayers(0, Game.FindTargetPlayerById(0).hp);
                if (TargetId[index] == -2) // aggiorna opponent.
                    Game.UpdateCommPlayers(1, Game.FindTargetPlayerById(1).hp);
                if (TargetId[index] == -3) // aggiorna All Allies.
                {
                    if (Game.FindTargetPlayerById(0).cardsOnBoard != null)
                        foreach (Card cardTemp in Game.FindTargetPlayerById(0).cardsOnBoard)
                            if (cardTemp.type == Enums.Type.Elemental)
                                Game.UpdateCommElemental((Elemental)cardTemp);
                }
                if (TargetId[index] == -4) // aggiorna All Enemies.
                {
                    if (Game.FindTargetPlayerById(1).cardsOnBoard != null)
                        foreach (Card cardTemp in Game.FindTargetPlayerById(1).cardsOnBoard)
                            if (cardTemp.type == Enums.Type.Elemental)
                                Game.UpdateCommElemental((Elemental)cardTemp);
                }



                TargetId.RemoveAt(index);
                targets.RemoveAt(index);
                microactionParams.RemoveAt(index);
                microactions.RemoveAt(index);
                callMicroations.RemoveAt(index);// svuota le liste
            } */
    } 
}    


    
