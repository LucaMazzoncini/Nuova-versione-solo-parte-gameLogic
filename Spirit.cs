using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    class Spirit : Card
    {
        public Spirit() : base()
        {
            target = Enums.Target.Spirit;
        }
        public Spirit(string name) : base(name)
        {
            target = Enums.Target.Spirit;
        }
        public int essence;
        public List<string> onAppear = new List<string>();
        public List<string> onDeath = new List<string>();
        public override bool canAttack()
        {
            return false;
        }
        public Spirit initFromInv(Invocation invTemp)
        {
            Card cardTemp = base.initFromInvocation(invTemp);
            return (Spirit)cardTemp;
        }
    }
}
