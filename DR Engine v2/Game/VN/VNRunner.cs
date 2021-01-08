using System.Collections;
using GameEngine;
using GameEngine.Game.Coroutine;
using Newtonsoft.Json;

namespace DREngine.Game.VN
{
    /// <summary>
    ///     An entity that begins running VN Scripts and it don't stop till ya drop.
    /// </summary>
    public class VNRunner
    {
        private CoroutineRunner _coroutineRunner = new CoroutineRunner();

        private DRGame _game;
        public VNState State;
        
        [JsonIgnore]
        public bool IsScriptActive { get; private set; }

        public VNRunner(DRGame game)
        {
            _game = game;
            State = new VNState();

            // Start running commands and keep running them.
            _coroutineRunner.Run(CommandRunRoutine());
        }

        public void CallScript(VNScript script, int commandIndex = 0)
        {
            State.Push(script, commandIndex);
        }

        public void GotoScript(VNScript script, int commandIndex = 0)
        {
            State.ReplaceTop(script, commandIndex);
        }

        public void PopScript()
        {
            State.Pop();
        }

        public void OnTick()
        {
            _coroutineRunner.OnTick();
        }

        private IEnumerator CommandRunRoutine()
        {
            // Just keep going forever.
            while (true)
            {
                // Keep going until
                while (!State.IsEmpty)
                {
                    IsScriptActive = true;
                    // Run commands until our current state is done.
                    while (!State.CurrentFrame.IsFinished)
                    {
                        yield return RunCommand(State.CurrentFrame.GetCurrentCommand());
                        State.CurrentFrame.ProgressOneLine();
                    }

                    // Pop off the frame since we're done with the last script.
                    State.Pop();
                }
                IsScriptActive = false;
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator RunCommand(VNCommand command)
        {
            yield return command.Run(_game);
        }

        /// <summary>
        ///     Called after a load is done.
        /// </summary>
        internal void OnLoad()
        {
            // Not necessary for now. I think our system works fine.
            // TODO: Start next (current) script?
        }
    }
}