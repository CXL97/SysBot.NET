using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PKHeX.Core;

namespace SysBot.Pokemon;

public static class ShowdownUtil
{
    public static ShowdownSet? ConvertToShowdown(string setstring)
        => TryConvertSingleLine(setstring, out var set) ? set : null;

    /// <summary>
    /// Converts a single line to a showdown set
    /// </summary>
    /// <param name="setstring">single string</param>
    /// <param name="set">output ShowdownSet object</param>
    /// <returns>True if conversion was successful, otherwise false</returns>
    public static bool TryConvertSingleLine(string setstring, [NotNullWhen(true)] out ShowdownSet? set)
    {
        set = null;
        // LiveStreams remove new lines, so we are left with a single line set
        var restorenick = string.Empty;

        var nickIndex = setstring.LastIndexOf(')');
        if (nickIndex > -1)
        {
            restorenick = setstring[..(nickIndex + 1)];
            if (restorenick.TrimStart().StartsWith('('))
                return false;
            setstring = setstring[(nickIndex + 1)..];
        }

        foreach (string i in splittables)
        {
            if (setstring.Contains(i))
                setstring = setstring.Replace(i, $"\r\n{i}");
        }

        var finalset = RemoveEmptyLines(restorenick + setstring);

        // The split table below only supports English, so we don't need to try parsing in all languages.
        var localization = BattleTemplateLocalization.GetLocalization(LanguageID.English);
        set = new ShowdownSet(finalset, localization);
        return set.Species != 0;
    }
    private static string RemoveEmptyLines(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // 按行拆分 → 跳过空行/空白行 → 重新拼接
        var lines = text.Split(["\r\n", "\n"], StringSplitOptions.None)
                        .Where(line => !string.IsNullOrWhiteSpace(line));

        return string.Join("\r\n", lines);
    }

    private static readonly string[] splittables =
    [
        "Ability:", "EVs:", "IVs:", "Shiny:", "Gigantamax:", "Ball:", "- ", "Level:",
        "Happiness:", "Friendship", "Language:", "OT:", "OTGender:", "TID:", "SID:", "Alpha:", "Tera Type:",
        "Adamant Nature", "Bashful Nature", "Brave Nature", "Bold Nature", "Calm Nature",
        "Careful Nature", "Docile Nature", "Gentle Nature", "Hardy Nature", "Hasty Nature",
        "Impish Nature", "Jolly Nature", "Lax Nature", "Lonely Nature", "Mild Nature",
        "Modest Nature", "Naive Nature", "Naughty Nature", "Quiet Nature", "Quirky Nature",
        "Rash Nature", "Relaxed Nature", "Sassy Nature", "Serious Nature", "Timid Nature",
    ];
}
