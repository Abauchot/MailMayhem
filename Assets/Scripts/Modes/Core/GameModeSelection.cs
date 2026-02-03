namespace Modes.Core
{
    /// <summary>
    /// Static bridge between Menu and Gameplay scenes.
    /// Menu sets the selected mode, Gameplay reads it.
    /// </summary>
    public static class GameModeSelection
    {
        public static GameModeType SelectedMode { get; set; } = GameModeType.Classic;
    }
}
