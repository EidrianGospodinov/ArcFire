namespace _Scripts.Multiplayer
{
    public class CommandContext
    {

        // References to your main managers/systems
        public MultiPlayerLevelManager LevelManager { get; private set; }

        public CommandContext(MultiPlayerLevelManager levelManager)
        {
            LevelManager = levelManager;

        }
    }

}