﻿using Jay.Dumping;
using Jay.Dumping.Interpolated;

namespace Jay.Reflection.Dumping;

public sealed class EventInfoDumper : Dumper<EventInfo>
{
    protected override void DumpImpl(ref DumpStringHandler stringHandler, [DisallowNull] EventInfo eventInfo, DumpFormat format)
    {
        stringHandler.Dump(eventInfo.EventHandlerType, format);
        stringHandler.Write(' ');
        if (format.IsWithType)
        {
            stringHandler.Dump(eventInfo.OwnerType(), format);
            stringHandler.Write('.');
        }
        stringHandler.Write(eventInfo.Name);
    }
}