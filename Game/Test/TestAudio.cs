using DREngine.Game.Audio;
using DREngine.Game.Input;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game
{
    public class TestAudio : IGameRunner
    {
        private GamePlus _game;

        private AudioMixer BGM;
        private AudioMixer SFX;

        private AudioClip music0, music1, click;

        private AudioSource clicker;
        private AudioSource music;

        public void Initialize(GamePlus game)
        {
            _game = game;

            BGM = new AudioMixer(game.AudioOutput);
            SFX = new AudioMixer(game.AudioOutput);

            music0 = new AudioClip(game.AudioOutput, new EnginePath("default_resources/Audio/DONOTPUSH_beautiful_dead.wav"), AudioClipType.Streamed);
            music1 = new AudioClip(game.AudioOutput, new EnginePath("default_resources/Audio/DONOTPUSH_trial.wav"), AudioClipType.Streamed);

            click = new AudioClip(game.AudioOutput, new EnginePath("default_resources/Audio/DONOTPUSH_bullet_click0.wav"), AudioClipType.Cached);
            clicker = new AudioSource(SFX);

            music = new AudioSource(BGM);
        }

        public void Update(float deltaTime)
        {
            if (RawInput.KeyPressed(Keys.Space))
            {
                Debug.Log("CLICK!");
                clicker.Play(click);
            }

            if (RawInput.KeyPressed(Keys.NumPad0))
            {
                Debug.Log("STOP da music");
                music.Stop();
            }

            if (RawInput.KeyPressed(Keys.NumPad1))
            {
                Debug.Log("Playing Music 0");
                music.Play(music0);
            }
            if (RawInput.KeyPressed(Keys.NumPad2))
            {
                Debug.Log("Playing Music 1");
                music.Play(music1);
            }
        }

        public void Draw()
        {
            // Do nothing.
        }
    }
}
