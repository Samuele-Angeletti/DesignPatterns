public class PauseMessage : IPublisherMessage
{
    public PauseMessage(bool gamePaused)
    {
        GamePaused = gamePaused;
    }

    public bool GamePaused { get; }
}