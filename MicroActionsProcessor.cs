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
        public static List<Dictionary<string, string>> microactionParams = new List<Dictionary<string, string>>(); // store di tutti i Param da spedire alle funzioni di MicroActions.table
        public static int TargetId; // questo ID viene rivalorizzato ogni volta che il giocatore seleziona bersaglio sull'interfaccia.
                                    // Game.SendCommTargets -- innesca una serie di chiamate che alla fine valorizzano TargetId.

        public static void AcquireTarget()// questa invia target validi a comm e aspetta ritorno TargetResult. BASTA. le altre, vengono
                                          // chiamate dentro Target Result!
        {
        }
        public static void AcquireMicroactionsParams()
        {
            char[] separator = new char[] { '.' };
            string[] Split;
            
            if (microactions != null)
            do
            {
                    Dictionary<string, string> dictTemp = new Dictionary<string, string>();
                    microactionParams.Add(dictTemp);
                    Split = microactions[index].ToUpper().Split(separator);
                    if (!Split[0].Equals("COOLDOWN"))
                        if (Split.Length > 1)
                        {

                            microactionParams[index].Add("Value", Split[1]);
                        }
                    if (targets[index].Count > 0)
                        {
                            Game.SendCommTargets(Game.FindAllValidTargetsId(targets[index]));// -- invia a comm la lista dei target validi tramite id                           
                            microactionParams[index].Add("Target", TargetId.ToString());
                        }                   
                index += 1;
            } while (index < microactions.Count);
        }
        public static List<List<Enums.Target>> AcquireValidTargets(List<List<Enums.Target>> validTargets)
        {
            targets = validTargets;
            return validTargets;
        }
        public static List<string> AcquireMicroactions(List<string> microActions)
        {
            microactions = microActions;
            return microActions;
        }
        public static void AcquireData(List<string> micrAct, List<List<Enums.Target>> targ)
        {
            AcquireMicroactions(micrAct);
            AcquireValidTargets(targ);
            AcquireMicroactionsParams();
        }
        public static bool canProcessMicroactions() // verifica che tutte le microazioni del potere, che richiedano bersaglio, abbiano almeno 1 bersaglio valido.
        {
            bool canProcess = false;
            bool[] canProc = new bool[targets.Count];
            bool norAllynorEnemy = false;
            int index = 0;

            do
            {
                norAllynorEnemy = !targets[index].Contains(Enums.Target.Ally) && !targets[index].Contains(Enums.Target.Enemy);
                if (targets[index].Count == 0)
                {
                    canProc[index] = true;
                    index += 1;
                    if (index < canProc.Length)
                        continue;
                    else
                        break;
                }
                foreach (Enums.Target target in targets[index])
                {
                    if (target == Enums.Target.Player || target == Enums.Target.Shaman || target == Enums.Target.Opponent)
                    {
                        canProc[index] = true;
                        index += 1;
                        if (index < canProc.Length)
                            continue;
                        else
                            break;
                    }
                    if (targets[index].Contains(Enums.Target.Ally) || norAllynorEnemy)
                    {
                        foreach (Enums.Target allyTarget in Game.AllyElementals)
                            if (target == allyTarget)
                            {
                                canProc[index] = true;
                                break;
                            }
                        foreach (Enums.Target allyTarget in Game.AllySpirits)
                            if (target == allyTarget)
                            {
                                canProc[index] = true;
                                break;
                            }
                    }
                    if (targets[index].Contains(Enums.Target.Enemy) || norAllynorEnemy)
                    {
                        foreach (Enums.Target EnemyTarget in Game.EnemyElementals)
                            if (target == EnemyTarget)
                            {
                                canProc[index] = true;
                                break;
                            }
                        foreach (Enums.Target EnemyTarget in Game.EnemySpirits)
                            if (target == EnemyTarget)
                            {
                                canProc[index] = true;
                                break;
                            }
                    }
                    if (canProc[index])
                        break;
                }
                if (index < canProc.Length)
                index += 1;               
            } while (index < targets.Count);

            int countTrue = 0;
            for (int i = 0; i < canProc.Length; i++)
                if (canProc[i] == true)
                    countTrue += 1;
            if (countTrue == canProc.Length)
                canProcess = true;
            return canProcess;
        }
        public static void ProcessMicroactions()
        {
            AcquireMicroactionsParams();
            int index = 0;
            while (microactions.Count > 0)
            {               
                MicroActions.table[microactions[index]](microactionParams[index]);
                if (TargetId < 2)
                    Game.UpdateCommPlayer(TargetId, Game.FindTargetPlayerById(TargetId).hp); // se il bersaglio era player lo aggiorna.
                else
                    Game.UpdateCommElemental((Elemental)Game.FindTargetCardByID(TargetId));
                targets.RemoveAt(index);
                microactionParams.RemoveAt(index);
                microactions.RemoveAt(index); // svuota le liste
            }
        } 
   }    
}

    
