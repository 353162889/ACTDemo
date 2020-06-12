using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class ColliderBehaviour : PlayableBehaviour
    {
        public GameObject gameObject = null;
        public static ScriptPlayable<ColliderBehaviour> Create(PlayableGraph graph, GameObject gameObject)
        {
            var handle = ScriptPlayable<ColliderBehaviour>.Create(graph);
            var playable = handle.GetBehaviour();
            playable.gameObject = gameObject;
            return handle;
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to Playables.PlayState.Playing.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        /// <param name="info">The information about this frame</param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (gameObject == null)
                return;

            gameObject.SetActive(true);
        }

        /// <summary>
        /// This function is called during the ProcessFrame phase of the PlayableGraph.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        /// <param name="userData">unused</param>
        public override void ProcessFrame(Playable playable, FrameData info, object userData)
        {
            if (gameObject != null)// && !gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (gameObject != null && info.effectivePlayState == PlayState.Paused)
                gameObject.SetActive(false);
        }

        /// <summary>
        /// This function is called when the Playable that owns the PlayableBehaviour is destroyed.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        public override void OnPlayableDestroy(Playable playable)
        {
            if(gameObject!= null)
                gameObject.SetActive(false);
        }
    }
}