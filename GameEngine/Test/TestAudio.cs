using GameEngine.Game;
using GameEngine.Game.Audio;
using GameEngine.Game.Input;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
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

            // fuuck why didn't I pick these songs from the getgo this makes debugging so much more tolerable
            music0 = new AudioClip(game.AudioOutput, new EnginePath("default_resources/Audio/dominoline.wav"), AudioClipType.Streamed);
            music1 = new AudioClip(game.AudioOutput, new EnginePath("default_resources/Audio/Chameleon.wav"), AudioClipType.Streamed);

            click = new AudioClip(game.AudioOutput, new EnginePath("default_resources/Audio/boof.wav"), AudioClipType.Cached);
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

            if (RawInput.KeyPressing(Keys.Up))
            {
                BGM.Volume += 0.2f * deltaTime;
                BGM.Volume = System.Math.Clamp(BGM.Volume, 0, 1);
                Debug.Log($"VOLUME: {BGM.Volume}");
            } else if (RawInput.KeyPressing(Keys.Down))
            {
                BGM.Volume -= 0.2f * deltaTime;
                BGM.Volume = System.Math.Clamp(BGM.Volume, 0, 1);
                Debug.Log($"VOLUME: {BGM.Volume}");
            }

        }

        public void Draw()
        {
            // Do nothing.
        }
    }
}
