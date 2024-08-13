using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneChangeOnTimelineComplete : MonoBehaviour
{
    [SceneName]
    public string sceneToLoad; // The name of the scene to load
    public string spawnPointID;
    public AnimationSequence newStartingSequence = null;
    private PlayableDirector playableDirector;

    private bool isPlaying;

    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();


        if (playableDirector != null)
        {
            playableDirector.stopped += OnPlayableDirectorStopped;
        }
        else
        {
            //Debug.LogError("PlayableDirector component not found.");
        }
    }

    void Update()
    {
        if (playableDirector != null && playableDirector.state == PlayState.Playing && !isPlaying)
        {
            OnPlayableDirectorPlayed(playableDirector);
        }
    }

    void OnDestroy()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;
            playableDirector.played -= OnPlayableDirectorPlayed;
        }
    }

    void OnPlayableDirectorPlayed(PlayableDirector director)
    {
        //Debug.Log("PlayableDirector started.");
        if (director == playableDirector)
        {
            isPlaying = true;
        }
    }

    void OnPlayableDirectorStopped(PlayableDirector director)
    {
        //Debug.Log("PlayableDirector stopped.");
        if (director == playableDirector)
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                EventHandler.CallTransitionEvent(sceneToLoad, spawnPointID, newStartingSequence);
            }

            playableDirector.enabled = false;

            isPlaying = false;
        }
    }
}
