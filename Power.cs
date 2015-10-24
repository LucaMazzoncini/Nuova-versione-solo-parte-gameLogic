using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public class Power
    // dentro un Power possono esserci più MicroAzioni (come Stringhe). Se un Power ha CoolDown >= 0, è il Potere di una Creatura. 
    //Se Cooldown =-1, è l'effetto di un rituale o di un trigger OnAppear / OnDeath.
    {
        public int cooldown; // se cooldown = -1 non è un POTERE ma semplicemente quarcosa.
        public int clock; // se il clock = 0 il potere è castabile. quando si casta il potere, clock diventa = cooldown e scala ogni turno.
        public List<string> microActions;
        public Power()
        {
            clock = 0;
            cooldown = 0;
            microActions = new List<string>();
        }
        public bool IsCastable()
        {
         bool castable = false;
            if (clock == 0)
            {
                int validTarg = 0;
                foreach (string microaction in microActions)
                    if (MicroActions.HasValidTarget(microaction))
                        validTarg += 1;
                if (validTarg == microActions.Count)
                    castable = true;
            }
            return castable;
        }
    }
}
