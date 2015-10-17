using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameLogic
{
    using Params = Dictionary<string, string>;
   
    public class Invocation
    {
        #region variables
        public string name { get; set; }
        public string from { get; set; }
        public string RANK { get; set; }
        public int rank { get; set; }
        public int strength { get; set; }
        public int constitution { get; set; }
        public int essence { get; set; }
        public int castLimit { get; set; }
        public Dictionary<Enums.Mana,int> manaCost{ get; set; }
        public List<Enums.Type> type{ get; set; }
        public List<Enums.SubType> subType{ get; set; }
        public List<Enums.Role> role{ get; set; }
        public List<Enums.Properties> properties { get; set; }
        public List<Power> powers { get; set; }
        public List<string> onAppear { get; set; }
        public List<string> onDeath { get; set; }
        public string flavour { get; set; }
        public string Power_A;
        public string Power_B;
        public string Power_C;
        private System.Xml.XmlDocument xmlDoc;

        private string card; //in questa stringa ci vanno tutte le informazioni lette che vanno passate all'interfaccia
        #endregion


        public string getCard()
        {
            return card;
        }

        public Invocation(System.Xml.XmlDocument xmlDoc)
        {
            //ATTENTO quando si chiama il costruttore si deve inizializzare anche card con un formato da decidere
            this.xmlDoc = xmlDoc;
            name = GetDataFromXml("Name");
            card += ("NAME:" + name);
            from = GetDataFromXml("From");
            card += (" FROM:" + from);
            manaCost = LoadCostFromString(GetDataFromXml("Cost"));
            card += (" MANACOST:" + (GetDataFromXml("Cost")));
            type = LoadTypeFromString(GetDataFromXml("Type"));
            card += (" TYPE:" + (GetDataFromXml("Type")));
            subType = LoadSubTypeFromString(GetDataFromXml("SubType"));
            card += (" SUBTYPE:" + (GetDataFromXml("SubType")));
            role = LoadRoleFromString(GetDataFromXml("Role"));
            card += (" ROLE:" + (GetDataFromXml("Role")));
            if (GetDataFromXml("Rank")!="")
                rank = Int32.Parse(GetDataFromXml("Rank"));
                RANK = "RANK" + (GetDataFromXml("Rank"));
                card += (" RANK:" + (GetDataFromXml("Rank")));
            if (GetDataFromXml("Strength") != "")
                strength = Int32.Parse(GetDataFromXml("Strength"));
                card += (" STRENGTH:" + (GetDataFromXml("Strength")));
            if (GetDataFromXml("Constitution") != "")
                constitution = Int32.Parse(GetDataFromXml("Constitution"));
                card += (" CONSTITUTION:" + (GetDataFromXml("Constitution")));
            if (GetDataFromXml("Essence") != "")
                essence = Int32.Parse(GetDataFromXml("Essence"));
            card += (" ESSENCE:" + (GetDataFromXml("Essence")));
            if (GetDataFromXml("CastLimit") != "")
                castLimit = Int32.Parse(GetDataFromXml("CastLimit"));
                card += (" CASTLIMIT:" + (GetDataFromXml("CastLimit")));
            properties = LoadPropertiesFromString(GetDataFromXml("Properties"));
            card += (" PROPERTIES:" + (GetDataFromXml("Properties")));
            powers = LoadPowersFromString(GetDataFromXml("Powers"));
            card += (" POWERS:" + (GetDataFromXml("Powers")));
            onAppear = LoadActionsAndEffectFromString(GetDataFromXml("OnAppear"));
            card += (" ONAPPEARACTIONS:" + (GetDataFromXml("OnAppear")));
            onDeath = LoadActionsAndEffectFromString(GetDataFromXml("OnDeath"));
            card += (" ONDEATH:" + (GetDataFromXml("OnDeath")));
            flavour = GetDataFromXml("Flavour");
            card += (" FLAVOUR:" + (GetDataFromXml("Flavour")));

        }

        public string returnStringFormat()
        {
            return card;
        }
        /* esempio di chiamata con i funtori
        public void test()
        {
            // Prendo una carta -> param

            List<string> powers = new List<string>() { "Armor.1", "Kill" };

            foreach(string power in powers)
            {
                string[] tmp = power.Split('.');
                // If carta ha il potere armor
                var powerAction = MicroActions.table[tmp[0]];
                Params param = new Params();

                if (tmp.Length > 1) { 
                    // Parsing dei parametri
                    // Dictionary<string, string> prm  = new Dictionary<string, string>();
                    
                    param.Add("value", "5");
                }

                powerAction(param);
            }
           
        }
        */
        #region methods
        private List<Enums.Role> LoadRoleFromString(string roleString)
        {
            List<Enums.Role> list = new List<Enums.Role>();
            string[] roleArray = roleString.ToUpper().Split(' ');
            foreach (string rString in roleArray)
            {
                switch (rString)
                {
                    case "TANK":
                        list.Add(Enums.Role.Tank);
                        break;
                    case "HEALER":
                        list.Add(Enums.Role.Healer);
                        break;
                    case "DPS":
                        list.Add(Enums.Role.Dps);
                        break;
                    default:
                        break;
                }
            }
            return list;
        }

        private List<Enums.SubType> LoadSubTypeFromString(string subTypeString)
        {
            List<Enums.SubType> list = new List<Enums.SubType>();
            string[] subTypeArray = subTypeString.ToUpper().Split(' ');
            foreach (string sString in subTypeArray)
            {
                switch (sString)
                {
                    case"FLORA":
                        list.Add(Enums.SubType.Flora);
                        break;
                    case"MAGMA":
                        list.Add(Enums.SubType.Magma);
                        break;
                    case"VAPOR":
                        list.Add(Enums.SubType.Vapor);
                        break;
                    case"EARTH":
                        list.Add(Enums.SubType.Earth);
                        break;
                    case"WATER":
                        list.Add(Enums.SubType.Water);
                        break;
                    case"FIRE":
                        list.Add(Enums.SubType.Fire);
                        break;
                    case"ANCESTRAL":
                        list.Add(Enums.SubType.Ancestral);
                        break;
                    default:
                        break;
                }
            }
            return list;
        }
        


        /// <summary>
        /// If no node is found, the method returns ""
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private string GetDataFromXml(string nodeName)
        {
            try
            {
                return xmlDoc.GetElementsByTagName(nodeName).Item(0).InnerText.Trim();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private List<Enums.Properties> LoadPropertiesFromString(string properties)
        {
            List<Enums.Properties> list = new List<Enums.Properties>();
            string[] propertiesArray = properties.ToUpper().Split(' ');
            foreach (string prop in propertiesArray)
            {
                switch (prop)
                {
                    case"QUICKNESS":
                        list.Add(Enums.Properties.Quickness);
                        break;
                    case"GUARDIAN":
                        list.Add(Enums.Properties.Guardian);
                        break;
                    case "PENETRATE":
                        list.Add(Enums.Properties.Penetrate);
                        break;
                    case "THUNDERBORN":
                        list.Add(Enums.Properties.Thunderborn);
                        break;
                    case "THORNS.1":
                        list.Add(Enums.Properties.Thorns);  //ATTENTO qui ci sara' un problema se si utilizza Thorns.x ma per motivi di tempo per adesso e' cosi
                        break;
                    default:
                        list.Add(Enums.Properties.None);
                        break;
                }
            }
            return list;
        }
        private List<Power> LoadPowersFromString(string properties)
        {
            List<Power> list = new List<Power>();
            string[] propertiesArray = properties.ToUpper().Split(' ');
            Power temp = null;
            foreach (string prop in propertiesArray)
            {
                if( prop.Contains("COOLDOWN."))
                {
                    if(temp != null)
                        list.Add(temp);
                        temp = new Power();
                        string[] tmpStr = prop.Split('.');
                        temp.cooldown = Int32.Parse(tmpStr[1]);
                }
                else
                if(temp != null)
                {
                    temp.microActions.Add(prop); //aggiungo le proprietà del potere
                }          
            }
            if (temp != null)
                list.Add(temp); //inserisco l'ultimo processato che altrimenti andrebbe perso
            return list;
        }

        private List<string> LoadActionsAndEffectFromString(string properties)
        {
            List<string> list = new List<String>();
            string[] propertiesArray = properties.ToUpper().Split(' ');
            foreach (string prop in propertiesArray)
            {
                list.Add(prop);
            }
            return list;
        }

        private List<Enums.Type> LoadTypeFromString(string typeString)
        {
            List<Enums.Type> list = new List<Enums.Type>();
            string[] typeArray = typeString.ToUpper().Split(' ');
            foreach (string tString in typeArray)
            {
                switch (tString)
                {
                    case "RITUAL":
                        list.Add(Enums.Type.Ritual);
                        break;
                    case "ELEMENTAL":
                        list.Add(Enums.Type.Elemental);
                        break;
                    case "SPIRIT":
                        list.Add(Enums.Type.Spirit);
                        break;
                    default:
                        break;
                }
            }
            return list;
        }

        private Dictionary<Enums.Mana, int> LoadCostFromString(string manaString)
        {
            Dictionary<Enums.Mana, int> costDic = new Dictionary<Enums.Mana, int>();
            char[] manaCharArray = manaString.ToUpper().ToCharArray();
            foreach (char manaChar in manaCharArray)
            {
                switch (manaChar)
                {
                    case'E':
                        AddManaCost(costDic, Enums.Mana.Earth);
                        break;
                    case'W':
                        AddManaCost(costDic, Enums.Mana.Water);
                        break;
                    case'F':
                        AddManaCost(costDic, Enums.Mana.Fire);
                        break;
                    case'L':
                        AddManaCost(costDic, Enums.Mana.Life);
                        break;
                    case'D':
                        AddManaCost(costDic, Enums.Mana.Death);
                        break;
                    default:
                        break;
                }
            }
            return costDic;
        }

        private void AddManaCost(Dictionary<Enums.Mana, int> costDic, Enums.Mana type)
        {
            if (costDic.ContainsKey(type))
            {
                costDic[type] += 1;
            }
            else
            {
                costDic.Add(type, 1);
            }
        }
        #endregion
    }
}
