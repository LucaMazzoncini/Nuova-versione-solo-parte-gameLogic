using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace GameLogic
{
    public class Power 
        // dentro un Power possono esserci più MicroAzioni (come Stringhe). Se un Power ha CoolDown >= 0, è il Potere di una Creatura. 
        //Se Cooldown =-1, è l'effetto di un rituale o di un trigger OnAppear / OnDeath.
        {
            public int cooldown; // se cooldown = -1 non è un POTERE ma semplicemente quarcosa.
            public List<string> microActions;
            public Power()
            {
                cooldown = 0;
                microActions = new List<string>();
            }
        }
}
