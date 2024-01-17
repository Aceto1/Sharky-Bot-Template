﻿using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class MorphDesire : IDesire
    {
        public UnitTypes TargetType { get; private set; }

        public ValueRange Count { get; private set; }
        
        public MacroData Data { get; private set; }
        
        public bool Enforced { get; set; }
        
        public int MineralCost { get; }
        
        public int VespeneCost { get; }
        
        public int TimeCost { get; }

        public MorphDesire(UnitTypes targetType, ValueRange count, MacroData data)
        {
            TargetType = targetType;
            Count = count;
            Data = data;

            if (new MorphDataService().MorphData().TryGetValue(targetType, out var structureInfo))
            {
                MineralCost = structureInfo.Minerals * count;
                VespeneCost = structureInfo.Gas * count;
                TimeCost = structureInfo.Time * count;
            }
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            // Assuming the MacroData has a method or property to set the desired morph counts
            Data.DesiredMorphCounts[TargetType] = Count;

            Enforced = true;
        }
    }
}