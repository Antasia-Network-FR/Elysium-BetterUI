﻿namespace RespawnTimer
{
    using System;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using System.Linq;
    using MEC;
    using API.Features;
    using Exiled.Events.EventArgs.Player;

    public static class EventHandler
    {
        private static CoroutineHandle _timerCoroutine;

        internal static void OnWaitingForPlayers()
        {
            if (RespawnTimer.Singleton.Config.ReloadTimerEachRound)
                RespawnTimer.Singleton.OnReloaded();

            if (_timerCoroutine.IsRunning)
                Timing.KillCoroutines(_timerCoroutine);
        }

        internal static void OnRoundStart()
        {
            try
            {
                _timerCoroutine = Timing.RunCoroutine(TimerCoroutine());
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }

            Log.Debug($"RespawnTimer coroutine started successfully!");
        }

        internal static void OnDying(DyingEventArgs ev)
        {
            if (List.ContainsKey(ev.Player))
            {
                Timing.KillCoroutines(List[ev.Player]);
                List.Remove(ev.Player);
            }
            
            List.Add(ev.Player, Timing.CallDelayed(RespawnTimer.Singleton.Config.TimerDelay, () => List.Remove(ev.Player)));
        }

        private static IEnumerator<float> TimerCoroutine()
        {
            yield return Timing.WaitForSeconds(1f);

            Log.Debug("Start");

            while (true)
            {
                yield return Timing.WaitForSeconds(1f);

                Log.Debug("Tick");

                Spectators.Clear();
                Spectators.AddRange(Player.Get(x => !x.IsAlive));
                string text = TimerView.Current.GetText(Spectators.Count);

                foreach (Player player in Spectators)
                {
                    if (player.IsOverwatchEnabled && RespawnTimer.Singleton.Config.HideTimerForOverwatch)
                        continue;

                    if (API.API.TimerHidden.Contains(player.UserId))
                        continue;

                    if (List.ContainsKey(player))
                        continue;

                    player.ShowHint(text, 1.25f);
                }

                if (Round.IsEnded)
                    break;
            }
        }

        private static readonly List<Player> Spectators = new(25);
        private static readonly Dictionary<Player, CoroutineHandle> List = new(25);
    }
}