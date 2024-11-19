using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Involved.HTF.Common
{
    public class BattleTeamsDto
    {
        public List<TeamMember> TeamA { get; set; }
        public List<TeamMember> TeamB { get; set; }
    }

    public class TeamMember
    {
        public int Strength { get; set; }
        public int Speed { get; set; }
        public int Health { get; set; }
    }
}
