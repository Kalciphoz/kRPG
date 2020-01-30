using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kRPG.Enums
{
    public enum Message : byte
    {
        AddXP,
        CreateProjectile,
        SwordInit,
        StaffInit,
        BowInit,
        SyncHit,
        SyncCritHit,
        SyncLevel,
        InitProjEleDmg,
        SyncStats,
        SyncSpear,
        PrefixNPC,
        NPCEleDmg
    };

}
