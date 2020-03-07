using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public enum GameStateType
    {
        Root,
        SkillEditor,
    }

    public class GameStateData
    {
        public GameStateType mStateType;
        public Type mClassType;
        public bool mDefaultState;
        public GameStateData[] mSubStateData;
        public GameStateData(GameStateType stateType,Type classType,bool defaultState ,GameStateData[] subStateData = null)
        {
            this.mStateType = stateType;
            this.mClassType = classType;
            this.mDefaultState = defaultState;
            this.mSubStateData = subStateData;
        }
    }

    public partial class GameStateCfg
    {

        public static GameStateData GameState = new GameStateData(GameStateType.Root,typeof(StateContainerBase),false,
            new GameStateData[] {
                new GameStateData(GameStateType.SkillEditor,typeof(SkillEditorState),true),
            });
    }
}
