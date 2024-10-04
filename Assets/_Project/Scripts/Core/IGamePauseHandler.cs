namespace Core
{
    public interface IGamePauseHandler
    {
        void OnPause();
        void OnResume();
    }
}