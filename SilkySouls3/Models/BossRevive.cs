using System.Collections.Generic;
using System.Numerics;
using SilkySouls3.Enums;

namespace SilkySouls3.Models
{
    public class BossRevive
    {
        public DlcRequirement DlcRequirement { get; set; }
        public string Area { get; set; }
        public string BossName { get; set; }
        public int BlockId { get; set; }
        public List<BossFlag> FirstEncounterFlags { get; set; }
        public List<BossFlag> BossFlags { get; set; }
        public int BonfireId { get; set; }
        public Vector3? Coords { get; set; }
        public float Angle { get; set; }
    }
}
