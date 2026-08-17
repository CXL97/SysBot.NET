using PKHeX.Core;
using SysBot.Base;
using SysBot.Pokemon;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Bilibili;
public class BilibiliTradeNotifier<T> : IPokeTradeNotifier<T> where T : PKM, new()
{
    private T Data { get; }
    private PokeTradeTrainerInfo Info { get; }
    private int Code { get; }
    private string Username { get; }

    public BilibiliTradeNotifier(T data, PokeTradeTrainerInfo info, int code, string username)
    {
        Data = data;
        Info = info;
        Code = code;
        Username = username;
        LogUtil.LogText($"Created trade details for {Username} - {Code}");
    }

    public Action<PokeRoutineExecutor<T>>? OnFinish { private get; set; }

    public Task SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message)
    {
        LogUtil.LogText(message);
        return Task.CompletedTask;
    }

    public Task TradeCanceled(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeResult msg)
    {
        OnFinish?.Invoke(routine);
        var line = $"@{info.Trainer.TrainerName}: Trade canceled, {msg}";
        LogUtil.LogText(line);
        File.WriteAllText(@"msg.txt", $"等待命令");
        return Task.CompletedTask;
    }

    public Task TradeFinished(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;
        var message = $"@{info.Trainer.TrainerName}: " + (tradedToUser != 0
            ? $"Trade finished. Enjoy your {(Species) tradedToUser}!"
            : "Trade finished!");
        LogUtil.LogText(message);
        File.WriteAllText(@"msg.txt", $"等待命令");
        return Task.CompletedTask;
    }

    public Task TradeInitialize(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
        var msg =
            $"@{info.Trainer.TrainerName} (ID: {info.Id}): Initializing trade{receive} with you. Please be ready.";
        msg += $" Your trade code is: {info.Code:0000 0000}";
        LogUtil.LogText(msg);
        File.WriteAllText("msg.txt",
            $"派送:{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}\n密码:{info.Code:0000 0000}\n状态:初始化");
        return Task.CompletedTask;
    }

    public Task TradeSearching(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var name = Info.TrainerName;
        var trainer = string.IsNullOrEmpty(name) ? string.Empty : $", @{name}";
        var message = $"I'm waiting for you{trainer}! My IGN is {routine.InGameName}.";
        message += $" Your trade code is: {info.Code:0000 0000}";
        LogUtil.LogText(message);
        File.WriteAllText("msg.txt",
            $"派送:{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}\n密码:{info.Code:0000 0000}\n状态:搜索中");
        return Task.CompletedTask;
    }

    public Task SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeSummary message)
    {
        var msg = message.Summary;
        if (message.Details.Count > 0)
            msg += ", " + string.Join(", ", message.Details.Select(z => $"{z.Heading}: {z.Detail}"));
        LogUtil.LogText(msg);
        return Task.CompletedTask;
    }

    public Task SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string message)
    {
        var msg = $"Details for {result.FileName}: " + message;
        LogUtil.LogText(msg);
        return Task.CompletedTask;
    }
}
