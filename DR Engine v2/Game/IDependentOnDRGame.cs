using System;
using Newtonsoft.Json;

namespace DREngine.Game
{
    public interface IDependentOnDRGame
    {

        public static DRGame CurrentGame;

        [JsonIgnore]
        DRGame Game { get; set; }
    }

}
