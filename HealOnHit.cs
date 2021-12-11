using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace HealOnHit
{
    public class HealOnHit : MissionBehavior
    {
        public static float ConvertRate = 0.2f;

        public override MissionBehaviorType BehaviorType
        {
            get
            {
                return MissionBehaviorType.Other;
            }
        }

        public static void Log(string text)
        {
            InformationManager.DisplayMessage(new InformationMessage(text, new Color(0f, 1f, 0f, 0.5f)));
        }

        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon affectorWeapon)
        {
            try
            {
                if (affectorAgent != null && affectedAgent != null)
                {
                    if (affectorAgent.Character != null && affectedAgent.Character != null)
                    {
                        if (affectorAgent == Agent.Main && damage > 0)
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
            HealOnHit.Log("HealOnHit: Running.");
            string path;
            try
            {
                 path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            }
            catch (Exception)
            {
                HealOnHit.Log("HealOnHit: cannot parse config.xml, use default value.");
                return;
            };

            int loc = path.LastIndexOf('\\');
            if (loc != -1)
            {
                path = path.Substring(0, loc);
                path += "\\config.xml";
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
                catch (Exception)
                {
                    HealOnHit.Log("HealOnHit: cannot parse config.xml, use default value.");
                    return;
                }
            }
            else
            {
                HealOnHit.Log("HealOnHit: path error");
                return;
            }
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            mission.AddMissionBehavior(new HealOnHit());
        }
    }
}
