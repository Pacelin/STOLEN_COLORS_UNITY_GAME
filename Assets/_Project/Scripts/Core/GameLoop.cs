using UniRx;
using Zenject;

namespace Core
{
    public class GameLoop : IInitializable
    {
        public IReadOnlyReactiveProperty<bool> Paused => _paused;

        private readonly IGameInitializeHandler[] _initializeHandlers;
        private readonly IGameAwakeHandler[] _awakeHandlers;
        private readonly IGamePauseHandler[] _pauseHandlers;
        private readonly ReactiveProperty<bool> _paused;

        public GameLoop(IGameInitializeHandler[] initializeHandlers, 
            IGameAwakeHandler[] awakeHandlers, IGamePauseHandler[] pauseHandlers)
        {
            _initializeHandlers = initializeHandlers;
            _awakeHandlers = awakeHandlers;
            _pauseHandlers = pauseHandlers;
            _paused = new(false);
        }
        
        void IInitializable.Initialize()
        {
            foreach (var initializeHandler in _initializeHandlers)
                initializeHandler.OnInitialize();
            foreach (var awakeHandler in _awakeHandlers)
                awakeHandler.OnAwake();
        }

        public void Pause()
        {
            if (_paused.Value)
                return;
            _paused.Value = true;
            foreach (var pauseHandler in _pauseHandlers)
                pauseHandler.OnPause();
        }

        public void Resume()
        {
            if (!_paused.Value)
                return;
            _paused.Value = false;
            foreach (var pauseHandler in _pauseHandlers)
                pauseHandler.OnResume();
        }
    }
}