namespace Photon.Voice.Unity
{
    using ExitGames.Client.Photon;

    public interface ILoggable
    {
        DebugLevel LogLevel { get; set; }
        VoiceLogger Logger { get; }
    }
}