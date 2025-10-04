using System;

namespace ParasiteLost.Managers
{
    public static class GameEvents
    {
        public static Action OnGameStarted;           // When gameplay actually begins
        public static Action OnGameOver;              // When parasite dies / game ends
        public static Action OnGameRestarted;         // When restart triggered (before scene reload)
        public static Action OnParasiteDied;          // Parasite lifespan reached zero
    }
}
