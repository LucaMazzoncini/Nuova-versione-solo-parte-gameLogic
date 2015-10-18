﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    using Params = Dictionary<string, string>;

    public static class MicroActions
    {
        public static Dictionary<string, Action<Params>> table;

        /*public MicroActions()
        {
            
        }*/

        //il richiamo a questa funzione sara' MicroAction.table["nomefunzione"](param);  Dove nomefunzione e' ad esempio Armor e invece param un dizionario stringa stringa
        //che in caso deve contenere anche il target

        static MicroActions()
        {
            MicroActions.table = new Dictionary<string, Action<Params>>();
            MicroActions.table.Add("ARMOR", armor);
            MicroActions.table.Add("HEALARMORELEMENTAL", HealArmorElemental);
            MicroActions.table.Add("KILL", kill);
            MicroActions.table.Add("DAMAGE", Damage);
            MicroActions.table.Add("DAMAGEELEMENTAL", DamageElemental);
            MicroActions.table.Add("DAMAGEENEMYELEMENTAL", DamageEnemyElemental);
            MicroActions.table.Add("DAMAGEPOISONELEMENTAL", DamagePoisonElemental);
            MicroActions.table.Add("DAMAGEPLAYER", DamagePlayer);
            MicroActions.table.Add("SELFDAMAGE", SelfDamage);
            MicroActions.table.Add("DISPEL", Dispel);
            MicroActions.table.Add("ADDCOS", AddCos);
            MicroActions.table.Add("DECSTR", DecStr);
            MicroActions.table.Add("INCURABLE", Incurable);
            MicroActions.table.Add("ASLEEP", Asleep);
            MicroActions.table.Add("SHIELD", Shield);
            MicroActions.table.Add("POISON", Poison);
            MicroActions.table.Add("HEALELEMENTAL", HealElemental);
            MicroActions.table.Add("HEAL", Heal);
            MicroActions.table.Add("HEALYOUANDALLALLIES", HealYouAndAllAllies);
            MicroActions.table.Add("ADDMANA", AddMana);
            MicroActions.table.Add("LOSTRANDOMELEMENT", LostRandomElement);
        }

        //Vanno aggiunti tutti i case delle microazioni
        public static List<Enums.Target> getTargetsByMicroaction(string microaction)
        {
            List<Enums.Target> targetsList = new List<Enums.Target>();

            switch (microaction)
            {
                case "DAMAGE":
                    targetsList.Add(Enums.Target.Player);
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "DAMAGEELEMENTAL":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "DAMAGEENEMYELEMENTAL":
                    targetsList.Add(Enums.Target.Elemental);
                    targetsList.Add(Enums.Target.Enemy);
                    break;
                case "DAMAGEPOISONELEMENTAL":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "DAMAGEPLAYER":
                    targetsList.Add(Enums.Target.Player);
                    break;
                case "DISPEL":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "ADDCOS":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "KILLALLY":
                    targetsList.Add(Enums.Target.Ally);
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "SELFDAMAGE":
                    
                    break;
                case "DECSTR":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "INCURABLE":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "SHIELD":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "POISON":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "ARMOR":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "HEAL":
                    targetsList.Add(Enums.Target.Player);
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "HEALELEMENTAL":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "HEALARMORELEMENTAL":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                case "HEALYOUANDALLALLIES": 
                    
                    break;
                case "ADDMANA":
                    targetsList.Add(Enums.Target.Player);
                    break;
                case "LOSTRANDOMELEMENT":
                    
                    break;
                case "ASLEEP":
                    targetsList.Add(Enums.Target.Elemental);
                    break;
                default:
                    break;
            }
            return targetsList;

        }

        public static List<Enums.Target> getTargets(string actions)
        {
            List<Enums.Target> targetsList = new List<Enums.Target>();

            //creo un vettore microActions con tutte le microazioni
            string[] microactions = actions.ToUpper().Split(' ');

            //filtro le microazioni togliendogli i parametri dopo il punto
            for (int i = 0; i < microactions.Length; i++)
            {
                string[] stringTemp = microactions[i].Split('.');
                microactions[i] = stringTemp[0];
                targetsList.AddRange(getTargetsByMicroaction(microactions[i]));
            }

            //tolgo le occorrenze inutili dalla lista
            targetsList = targetsList.Distinct<Enums.Target>().ToList<Enums.Target>();

            return targetsList; 
        }

        private static void armor(Params param)
        {
            int armorValue;
            if (param.ContainsKey("Value2"))
                armorValue = Int32.Parse(param["Value2"]);
            else
                armorValue = Int32.Parse(param["Value"]);
            int idCard = Int32.Parse(param["idTarget"]);
            Card cTemp = Game.FindTargetCardByID(idCard);
            if (cTemp.GetType() == typeof(Elemental))
            {
                Elemental elemTemp = (Elemental)cTemp;
                for (int i = 0; i < armorValue; i++)
                    elemTemp.properties.Add(Enums.Properties.Armor);
            }

            
        }
        private static void kill(Params param)
        {
            int idCard = Int32.Parse(param["idTarget"]);
            Card cTemp = Game.FindTargetCardByID(idCard);
            if (cTemp.GetType() == typeof(Elemental))
            {
                Elemental elemTemp = cTemp as Elemental;
                if (!elemTemp.buff.Contains(Enums.Buff.Immortal))
                    Game.RemoveCardById(elemTemp.id);
            }

        }
        private static void killAlly(Params param)
        {
            int idCard = Int32.Parse(param["idTarget"]);
            Card cTemp = Game.FindTargetCardByID(idCard);
            if (cTemp.GetType() == typeof(Elemental))
            {
                Elemental elemTemp = cTemp as Elemental;
                if (Game.IsAlly(elemTemp.id))
                    if (!elemTemp.buff.Contains(Enums.Buff.Immortal))
                        Game.RemoveCardById(elemTemp.id);
            }

        }
        private static void Damage(Params param)
        {
            int damageValue = Int32.Parse(param["Value"]);
            int idTarget = Int32.Parse(param["idTarget"]);
            if (idTarget < 2)
            {
                Player playerTemp = Game.FindTargetPlayerById(idTarget);
                playerTemp.hp -= damageValue;
            }

            if (idTarget >= 2)
            {
                Elemental elemTemp;
                elemTemp = (Elemental)Game.FindTargetCardByID(idTarget);
                for (int dmg = 0; dmg < damageValue; dmg++)
                {
                    if (elemTemp.properties.Contains(Enums.Properties.Armor))
                        elemTemp.properties.Remove(Enums.Properties.Armor);
                    else
                        elemTemp.hp -= 1;

                    if (elemTemp.hp == 0)
                        if (!elemTemp.buff.Contains(Enums.Buff.Immortal))
                        {
                            Game.RemoveCardById(elemTemp.id);
                            break;
                        }
                }
            }
        }
        private static void DamageElemental(Params param)
        {
            int damageValue = Int32.Parse(param["Value"]);
            int idTarget = Int32.Parse(param["idTarget"]);
            Elemental elemTemp;
            elemTemp = (Elemental)Game.FindTargetCardByID(idTarget);
            for (int dmg = 0; dmg < damageValue; dmg++)
            {
                if (elemTemp.properties.Contains(Enums.Properties.Armor))
                    elemTemp.properties.Remove(Enums.Properties.Armor);
                else
                    elemTemp.hp -= 1;

                if (elemTemp.hp == 0)
                {
                    Game.RemoveCardById(elemTemp.id);
                    break;
                }
            }
        }
        private static void DamageEnemyElemental(Params param)
        {
            DamageElemental(param);
        }
        private static void DamagePoisonElemental(Params param)
        {
            DamageElemental(param);
            Poison(param);
        }
        private static void DamagePlayer(Params param)
        {
            int damageValue = Int32.Parse(param["Value"]);
            int idTarget = Int32.Parse(param["idTarget"]);
            Player playerTemp = Game.FindTargetPlayerById(idTarget);
            playerTemp.hp -= damageValue;

        }
        private static void SelfDamage(Params param)
        {
            int damageValue = Int32.Parse(param["Value"]);
            Player playerTemp = Game.FindTargetPlayerById(0);
            playerTemp.hp -= damageValue;
        }
        private static void Dispel(Params param)
        {
            int idTarget = Int32.Parse(param["idTarget"]);
            Elemental elemTemp = (Elemental)Game.FindTargetCardByID(idTarget);
            if (Game.IsAlly(elemTemp.id))
            {
                if (elemTemp.debuff != null)
                {
                    foreach (Enums.Debuff Debuff in elemTemp.debuff)
                    {
                        if (Debuff == Enums.Debuff.DecreasedStr)
                            elemTemp.strength += 1;
                        if (Debuff == Enums.Debuff.DecreasedCon)
                            elemTemp.constitution += 1;
                    }
                    elemTemp.debuff.Clear();
                }
            }
            else if (elemTemp.buff != null)
            {
                foreach (Enums.Buff Buff in elemTemp.buff)
                {
                    if (Buff == Enums.Buff.IncreasedCon)
                        elemTemp.constitution -= 1;
                    if (Buff == Enums.Buff.IncreasedStr)
                        elemTemp.strength -= 1;
                }
                elemTemp.buff.Clear();
            }
        }
        private static void AddCos(Params param)
        {
            int cosValue = Int32.Parse(param["Value"]);
            int idTarget = Int32.Parse(param["idTarget"]);
            Elemental elemTemp = (Elemental)Game.FindTargetCardByID(idTarget);
            for (int i = 0; i < cosValue; i++)
            {
                elemTemp.constitution += 1;
                elemTemp.buff.Add(Enums.Buff.IncreasedCon);
            }
        }
        private static void DecStr(Params param)
        {
            int strValue = Int32.Parse(param["Value"]);
            int idTarget = Int32.Parse(param["idTarget"]);
            Elemental elemTemp = (Elemental)Game.FindTargetCardByID(idTarget);
            for (int i = 0; i < strValue; i++)
                if (elemTemp.strength > 0)
                {
                    elemTemp.strength -= 1;
                    elemTemp.debuff.Add(Enums.Debuff.DecreasedStr);
                }
        }
        private static void Incurable(Params param)
        {
            int idCard = Int32.Parse(param["idTarget"]);
            Card cTemp = Game.FindTargetCardByID(idCard);
            if (cTemp.GetType() == typeof(Elemental))
            {
                Elemental elemTemp = (Elemental)cTemp;
                elemTemp.debuff.Add(Enums.Debuff.Incurable);
            }

        }
        private static void Asleep(Params param)
        {
            int idCard = Int32.Parse(param["idTarget"]);
            Card cTemp = Game.FindTargetCardByID(idCard);
            if (cTemp.GetType() == typeof(Elemental))
            {
                Elemental elemTemp = (Elemental)cTemp;
                elemTemp.debuff.Add(Enums.Debuff.Asleep);
            }
        }
        private static void Shield(Params param)
        {
            int idCard = Int32.Parse(param["idTarget"]);
            Card cTemp = Game.FindTargetCardByID(idCard);
            if (cTemp.GetType() == typeof(Elemental))
            {
                Elemental elemTemp = (Elemental)cTemp;
                elemTemp.buff.Add(Enums.Buff.Shield);
            }
        }
        private static void Poison(Params param)
        {
            int idCard = Int32.Parse(param["idTarget"]);
            int poisonValue = Int32.Parse(param["Value"]);
            Card cTemp = Game.FindTargetCardByID(idCard);
            if (cTemp.GetType() == typeof(Elemental))
            {
                Elemental elemTemp = (Elemental)cTemp;
                int poisonCount = 0;
                foreach (Enums.Debuff debuff in elemTemp.debuff)
                    if (debuff.Equals(Enums.Debuff.PoisonShaman)) // la microazione assegna sempre PoisonShaman. i Poison assegnati dall'opponent devono essere visualizzati però come PoisonOpponent.
                        poisonCount += 1;
                for (int i = 0; i < poisonValue; i++)
                    if (poisonCount < 3)
                    {
                        elemTemp.debuff.Add(Enums.Debuff.PoisonShaman);
                        poisonCount += 1;
                    }
            }
        }
        private static void HealElemental(Params param)
        {
            int healValue = Int32.Parse(param["Value"]);
            int idTarget = Int32.Parse(param["idTarget"]);
            Card cardTemp = Game.FindTargetCardByID(idTarget);
            if (cardTemp.GetType() == typeof(Elemental))
            {
                Elemental elemTemp = (Elemental)cardTemp;
                if (!elemTemp.debuff.Contains(Enums.Debuff.Incurable))
                    for (int i = 0; i < healValue; i++)
                        if (elemTemp.hp < elemTemp.constitution)
                            elemTemp.hp += 1;
            }
        }
        private static void HealArmorElemental(Params param)
        {
            HealElemental(param);
            armor(param);
        }
        private static void Heal(Params param)
        {
            int healValue = Int32.Parse(param["Value"]);
            int idTarget = Int32.Parse(param["idTarget"]);
            if (idTarget < 2)
            {
                Player playerTemp = Game.FindTargetPlayerById(idTarget);
                for (int i = 0; i < healValue; i++)
                    if (playerTemp.hp < playerTemp.maxHp)
                        playerTemp.hp += 1;
            }

            if (idTarget >= 2)
            {
                Card cardTemp = Game.FindTargetCardByID(idTarget);
                if (cardTemp.GetType() == typeof(Elemental))
                {
                    Elemental elemTemp = (Elemental)cardTemp;
                    if (!elemTemp.debuff.Contains(Enums.Debuff.Incurable))
                        for (int i = 0; i < healValue; i++)
                            if (elemTemp.hp < elemTemp.constitution)
                                elemTemp.hp += 1;
                }
            }
        }
        private static void HealYouAndAllAllies(Params param)
        {
            int healValue = Int32.Parse(param["Value"]);
            Player playerTemp = Game.FindTargetPlayerById(0);
            for (int i = 0; i < healValue; i++)
            {
                if (playerTemp.hp < playerTemp.maxHp)
                    playerTemp.hp += 1;
                if (playerTemp.cardsOnBoard != null)
                    foreach (Elemental elemTemp in playerTemp.cardsOnBoard)
                        if (elemTemp.hp < elemTemp.constitution && !elemTemp.debuff.Contains(Enums.Debuff.Incurable))
                            elemTemp.hp += 1;
            }

        }
        private static void AddMana(Params param)
        {
            int manaValue = Int32.Parse(param["Value"]);
            Enums.Mana manaType = (Enums.Mana)Enum.Parse(typeof(Enums.Mana), param["Mana"], true);
            int idTarget = Int32.Parse(param["idTarget"]);
            Player playerTemp = Game.FindTargetPlayerById(idTarget);
            Dictionary<Enums.Mana, int> manaAdd = new Dictionary<Enums.Mana, int>();
            manaAdd.Add(manaType, manaValue);
            playerTemp.mana.AddMana(manaAdd);

        }
        private static void LostRandomElement(Params param)
        {
            
            Player playerTemp = Game.FindTargetPlayerById(1);
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
        }
    }
}

