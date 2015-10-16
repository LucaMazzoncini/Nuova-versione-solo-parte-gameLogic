using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic
{
    public static class MicroActionsProcessor
    {
        public static List<string> microactions = new List<string>(); // stora la lista di microazioni del Power da processare.
        public static List<List<Enums.Target>> targets = new List<List<Enums.Target>>(); // store tutti i target validi di tutte le microazioni di cui è composto il Power. Se una Microaction è associata ad una lista di target vuota, significa che non richiede target in risoluzione.
        public static List<Dictionary<string, string>> microactionParams = new List<Dictionary<string, string>>(); // store di tutti i Param da spedire alle funzioni di MicroActions.table

        public static void AcquireMicroactionsParams(List<string> microaction)
        {
            char[] separator = new char[] { '.', ' ' };
            int index = 0;
            string[] Split;
            do
            {
                Split = microaction[index].ToUpper().Split(separator);
                if (!Split[0].Equals("COOLDOWN"))
                microactionParams[index].Add("Value", Split[1]);
                //if (targets[index].Count > 0)
                    //TargetAcquired();
            } while (index < microaction.Count);
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
      /*  public static void ProcessMicroactions()
        {
            int index = 0;
            do
            {
                if (targets[index].Count == 0)
                {
                    //MicroActions.table[microactions[index]](/*dizionario di roba parsata );
                    microactions.RemoveAt(index);
                    targets.RemoveAt(index);
                    continue;
                }

                else
                {

                }


            } while (microactions.Count > 0);
        } */
   










        /* a index di "microactions" corrisponde relativa lista di target validi in "targets".
        DO
             canProcessMicroaction(string microaction)
               che chiama: processMicroaction(microaction)
                    che chiama: comm.getTarget(targets[0])
                    WHILE microactions.count() > 0





    /*canProcessMicroaction()
    {CHECK 1 - controlla che tutte le microazioni di microactions abbiano almeno 1 bersaglio VALIDO. 
    SE CHECK 1 = TRUE --> processo microactions[0];

        ProcessMicroaction() 
        { SE microactions[0] ha bisogno di Target, chiedo il / i target EFFETTIVO/I della microactions[0];
                    CHIEDO TARGET a Comm. (es. "comm.getTarget(targets[0])); (ATTENTO se non ci sono target da prendere chiamo direttamente target acquired con -1)
                            FINE}

    public static void TargetAcquired(int id -- è questo l'id del target, passato come argomento)
    {  qui perparo i parametri per eseguire la microazione, il target lo ho, mi mancano gli altri come es: damage.1
    a questo punto in base alla microazione faccio questo:

        param sono i parametri che ho preparato
            MicroAction.table["nomefunzione"](param); 

        cancello da entrambe le liste l'elemento 0

        se ci sono piu di 0 elementi nella lista delle microazioni richiamo Process microaction














        */
    }
}

    
