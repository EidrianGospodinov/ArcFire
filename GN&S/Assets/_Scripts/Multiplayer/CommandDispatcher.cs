using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.Multiplayer
{
    public interface IGameCommand
    {
        string Name { get; }
        string Execute(CommandContext commandContext, string[] args); 
    }
    #region Commands
    public class HealCommand : IGameCommand
    {
        public string Name => "heal";
        
        public string Execute(CommandContext commandContext, string[] args)
        {
            Debug.Log("Health set to 100!"); 
            return "You have been fully healed!";
        }
    }
    public class TimeCommand : IGameCommand
    {
        public string Name => "time";
        public string Execute(CommandContext commandContext, string[] args)
        {
            var value = args[0];
            if (float.TryParse(value, out float result))
            {
                commandContext.LevelManager.timer.UpdateTimer(result);
                Debug.Log("Set time to: " + args[0]);
                return $"time set to {args[0]}";
            }

            return "Invalid data used";


        }
    }
    public class QuitCommand : IGameCommand
    {
        public string Name => "quit";
        public string Execute(CommandContext commandContext, string[] args)
        {
            Debug.Log("quit game"); 
            return "Quitting game";
        }
    }
    public class KickCommand : IGameCommand
    {
        public string Name => "kick";
        public string Execute(CommandContext commandContext, string[] args)
        {
            foreach (var player in GameManager.Instance.PlayersInGame)
            {
               var multiplayer = player.GetComponent<MultyPlayer>();
               if (args[0] == multiplayer.name)
               {
                   PhotonNetwork.CloseConnection(multiplayer.PV.Owner);
               }
                
            }
            Debug.Log("quit game"); 
            return "Quitting game";
        }
    }
    #endregion
    public class CommandDispatcher
    {
        private Dictionary<string, IGameCommand> commands = new Dictionary<string, IGameCommand>();
        CommandContext context;
        public CommandDispatcher(MultiPlayerLevelManager multiPlayerLevelManager)
        {
            context = new CommandContext(multiPlayerLevelManager);
            RegisterCommand(new HealCommand());
            RegisterCommand(new TimeCommand());
            RegisterCommand(new KickCommand());
            RegisterCommand(new QuitCommand());
        }

        private void RegisterCommand(IGameCommand command)
        {
            commands.Add(command.Name.ToLower(), command);
        }

        public string Dispatch(string input)
        {
            string[] parts = input.Trim().Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) return string.Empty;

            string commandName = parts[0].ToLower();
            string[] args = parts.Skip(1).ToArray();

            if (commands.TryGetValue(commandName, out IGameCommand command))
            {
                return command.Execute(context,args);
            }
            else
            {
                return $"Error: Unknown command '{commandName}'.";
            }
        }
    
       
    }
    
}