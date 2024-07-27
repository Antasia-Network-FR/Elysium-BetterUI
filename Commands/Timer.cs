namespace RespawnTimer.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;

    using static API.API;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Timer : ICommand
    {
        public string Command => "timer";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Affiche / cache RespawnTimer.";

        public bool SanitizeResponse => false;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Player.Get(sender).UserId;

            if (!TimerHidden.Remove(userId))
            {
                TimerHidden.Add(userId);

                response = "<color=red>Les informations de spectateurs ont été caché !</color>";
                return true;
            }

            response = "<color=green>Les informations de spectateurs ont été révéllé !</color>";
            return true;
        }
    }
}