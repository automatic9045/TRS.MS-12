using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

using Codeplex.Data;

using TRS.TMS12.Interfaces;
using TRS.Tickets;

namespace TRS.TMS12.Plugins.TRS
{
    public class TimeInquiringInfo
    {
        public Game Game { get; private set; }
        public int TimeNumber { get; private set; }
        public string String { get; private set; }

        public TimeInquiringInfo(Game game, int timeNumber, string str)
        {
            Game = game;
            TimeNumber = timeNumber;
            String = str;
        }
    }

    public partial class Connector : IPlugin
    {
        private string[] gameNames = new string[] { "トレインシミュレーター", "模型体験運転", };

        public List<TimeInquiringInfo> InquireTimeNumbers(Game game)
        {
            dynamic json = null;
            List<TimeInquiringInfo> result = new List<TimeInquiringInfo>();

            try
            {
                json = Communicator.Check();
            }
            catch
            {
                return new List<TimeInquiringInfo>();
            }

            try
            {
                int[] availability = game switch
                {
                    Game.TrainSimulator => json.availability.ts,
                    Game.ModelTrainDriving => json.availability.mk,
                    _ => throw new ArgumentOutOfRangeException(),
                };

                for (int i = 0; i < availability.Length; i++)
                {
                    if (availability[i] >= 0)
                    {
                        string title = gameNames[(int)game - 1] + "　" + Strings.StrConv((i + 1).ToString(), VbStrConv.Wide);
                        result.Add(new TimeInquiringInfo(game, i + 1, (result.Count + 1).ToString() + ":" +
                            title.PadRight(16, '　') +
                            json.availability.time1[(int)game - 1][i] + " " + json.availability.time2[(int)game - 1][i] + "　 " +
                            Strings.StrConv(json.availability.type[(int)game - 1][i], VbStrConv.Wide) + "　" + availability[i].ToString().PadLeft(3, '0')));
                    }
                }
            }
            catch
            {
                return new List<TimeInquiringInfo>();
            }

            return result;
        }
    }
}
