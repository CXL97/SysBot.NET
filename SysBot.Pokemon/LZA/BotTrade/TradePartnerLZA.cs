using System;
using System.Diagnostics;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

namespace SysBot.Pokemon;

public sealed class TradePartnerLZA
{
    public ulong NID { get; }
    public string TID7 { get; }
    public string SID7 { get; }
    public string TrainerName { get; }

    public int Game => (int)GameVersion.ZA;

    public int Gender { get; }

    public int Language { get; }

    public TradePartnerLZA(ulong ID, ReadOnlySpan<byte> TIDSID, ReadOnlySpan<byte> trainerNameObject, int gender, int language)
    {
        NID = ID;

        Debug.Assert(TIDSID.Length == 4);
        var tidsid = ReadUInt32LittleEndian(TIDSID);
        TID7 = $"{tidsid % 1_000_000:000000}";
        SID7 = $"{tidsid / 1_000_000:0000}";

        TrainerName = StringConverter8.GetString(trainerNameObject);

        Gender = gender;
        Language = language;
    }

    public const int MaxByteLengthStringObject = 26;
}
