namespace RespawnTimer
{
    using System;
    using System.IO;
    using API.Features;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Loader;


    public class BetterUI : Plugin<Configs.Config>
    {
        public static BetterUI Singleton;
        public EventHandler EventHandler;
        public static string RespawnTimerDirectoryPath { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;

            RespawnTimerDirectoryPath = Path.Combine(Paths.Configs, "BetterUI");
            EventHandler = new EventHandler();

            if (!Directory.Exists(RespawnTimerDirectoryPath))
            {
                // Log.Warn("BetterUI directory does not exist. Creating...");
                Log.Info("BetterUI directory does not exist. Creating...");
                Directory.CreateDirectory(RespawnTimerDirectoryPath);
            }

            RueiHelper.Refresh();

            Exiled.Events.Handlers.Map.Generated += EventHandler.OnGenerated;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandler.OnRoundStart;
            Exiled.Events.Handlers.Player.Dying += EventHandler.OnDying;
            Exiled.Events.Handlers.Server.ReloadedConfigs += OnReloaded;

            if (!Config.ReloadTimerEachRound)
                OnReloaded();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Map.Generated -= EventHandler.OnGenerated;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandler.OnRoundStart;
            Exiled.Events.Handlers.Player.Dying -= EventHandler.OnDying;
            Exiled.Events.Handlers.Server.ReloadedConfigs -= OnReloaded;

            EventHandler = null;
            Singleton = null;

            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            if (Config.Timers.IsEmpty())
            {
                Log.Error("Timer list is empty!");
                return;
            }

            TimerView.CachedTimers.Clear();

            foreach (string name in Config.Timers.Values)
                TimerView.AddTimer(name);
        }

        public override string Name => "BetterUI";
        public override string Author => "tgbhy";
        public override Version Version => new(1, 0, 0);
        public override Version RequiredExiledVersion => new(8, 9, 6);
        public override PluginPriority Priority => PluginPriority.Last;
    }
}