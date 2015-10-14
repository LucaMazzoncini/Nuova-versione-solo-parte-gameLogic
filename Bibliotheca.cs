using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace GameLogic
{
    public class Bibliotheca
    {
        public LinkedList<Invocation> Invocations { get; set; } //lista di tutte le carte
              
        public Bibliotheca(LinkedList<string> xmlInvocations)
        {
            Invocations = new LinkedList<Invocation>();
            foreach (string xmlInvocation in xmlInvocations)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlInvocation);
                Invocation invocation = new Invocation(xmlDoc);
                Invocations.AddLast(invocation);
            }
        }

        public LinkedList<Invocation> getCards(List<Enums.Filter> filterList,Mana manaParam) //questa funzione filtra le carte e ritorna una lista 
        {
            if (filterList.Count == 0) // Se non ci sono filtri, ritorna la lista di tutte le carte.
                return Invocations;

            LinkedList<Invocation> InvTempList = new LinkedList<Invocation>();//lista temporanea su cui caricare le carte filtrate.
            

            foreach (Invocation InvTemp in Invocations)
            {
                int filterCount = 0; // contatore dei controlli sui filtri.

                if (filterList.Contains(Enums.Filter.Playable)) //Se tra i filtri c'è Playable, verifica che la carta possa essere pagata.
                {
                    if (manaParam.CanPay(InvTemp.manaCost))
                        filterCount++;
                    else
                        continue; // se la carta non è Playble, il foreach passa direttamente all'iterazione successiva senza controllare gli altri filtri.
                }
                foreach (Enums.Filter filtro in filterList)// filtra le carte in base alla lista di filtri
                {                                                  
                        if (containsType(InvTemp, filtro) ||
                        containsRole(InvTemp, filtro) ||
                        containsSubType(InvTemp, filtro) ||
                        InvTemp.RANK.Equals(filtro.ToString().ToUpper())) //da modificare perche' contiene rank1 non solo 1
                        filterCount++;             
                }

                if (filterCount == filterList.Count)//se tutti i controlli vengono passati, aggiunge la carta.
                    InvTempList.AddLast(InvTemp);
            }
            return InvTempList;
        }


        private bool containsType(Invocation inv,Enums.Filter param)
        {
            if (Enum.IsDefined(typeof(Enums.Type), param.ToString()))
                if (inv.type.Contains((Enums.Type)Enum.Parse(typeof(Enums.Type), param.ToString())))
                    return true;
                return false;
        }

        private bool containsRole(Invocation inv, Enums.Filter param)
        {
            if (Enum.IsDefined(typeof(Enums.Role), param.ToString()))
                if (inv.role.Contains((Enums.Role)Enum.Parse(typeof(Enums.Role), param.ToString())))
                    return true;
            return false;
        }
        private bool containsSubType(Invocation inv, Enums.Filter param)
        {
            if (Enum.IsDefined(typeof(Enums.SubType), param.ToString()))
                if (inv.subType.Contains((Enums.SubType)Enum.Parse(typeof(Enums.SubType), param.ToString())))
                    return true;
            return false;
        }

        public Card getCardByName(string name)
        {
            foreach (Invocation InvTemp in Invocations)
                if (InvTemp.name == name)
                {
                    Card card = new Card(name);
                    card = card.initFromInvocation(InvTemp);
                    return card;
                }
                              
            return null;
        }

       
    }
}
