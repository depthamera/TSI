using TSI.Core.Scene;
using UnityEngine;

namespace TSI.Game
{
    public class GameSessionOptions
    {
        public SceneProfile InitialStage { get; }

        public GameSessionOptions(SceneProfile initialStage)
        {
            InitialStage = initialStage;
        }
    }
}
