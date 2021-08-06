using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace HealOnHit
{
    public class HealOnHit : MissionBehaviour
    {
        public static float ConvertRate = 0.2f;

        public override MissionBehaviourType BehaviourType
        {
            get
            {
                return MissionBehaviourType.Other;
            }
        }

        public static void Log(string text)
        {
            InformationManager.DisplayMessage(new InformationMessage(text, new Color(0f, 1f, 0f, 0.5f)));
        }

        public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, BoneBodyPartType victimHitBodyPart)
        {
            try
            {
                if (affectorAgent != null && affectedAgent != null)
                {
                    if (affectorAgent.Character != null && affectedAgent.Character != null)
                    {
                        if (affectorAgent == Agent.Main && damage > 0f)
                        {
                            float num = affectorAgent.Health + damage * HealOnHit.ConvertRate;
                            if (num >= affectorAgent.HealthLimit)
                            {
                                affectorAgent.Health = affectorAgent.HealthLimit;
                            }
                            else
                            {
                                affectorAgent.Health = num;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                HealOnHit.Log("HealOnHit:OnScoreHit Exception");
            }
        }
    }
    public class HealOnHitModuleBase : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            //HealOnHit.Log("HealOnHitModuleBase.OnGameStart");
            HealOnHit.Log("HealOnHit: Running.");
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            int loc = path.LastIndexOf('\\');
            if (loc != -1)
            {
                path = path.Substring(0, loc);
                path = path + "\\config.xml";
                //HealOnHit.Log(path);
                try
                {
                    XmlDocument config = new XmlDocument();
                    config.Load(path);
                    XmlNode rootNode = config.SelectSingleNode("HealOnHit");
                    XmlNode node =rootNode.SelectSingleNode("ConvertRate");
                    float rate = float.Parse(node.InnerText);
                    if (rate >= 0)
                    {
                        HealOnHit.ConvertRate = rate;
                        HealOnHit.Log("HealOnHit: convert rate=" + rate.ToString());
                    }
                }
                catch (Exception e)
                {
                    //HealOnHit.Log("HealOnHit: cannot parse config.xml, " + e.ToString());
                    HealOnHit.Log("HealOnHit: cannot parse config.xml, use default value.");
                }
            }
            else
            {
                HealOnHit.Log("HealOnHit: path error");
            }
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);
            mission.AddMissionBehaviour(new HealOnHit());
            //HealOnHit.Log("Debug: HealOnHitModuleBase.OnMissionBehaviourInitialize");
        }
    }
}
