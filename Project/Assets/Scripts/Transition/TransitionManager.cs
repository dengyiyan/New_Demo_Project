using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyProject.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;
        public string startSpawnID = "0";
        public AnimationSequence startingSequence = null;

        // private ConversationChecker conversationChecker;
        private CanvasGroup fadeCanvasGroup;
        private bool isFade;

        private bool isNotFirstTime = false; //to prevent the loading become strange, not useful in final version

        private bool canTransit = true;

        // public string GUID => GetComponent<DataGUID>().guid;

        //private override void Awake()
        //{
        //    base.Awake();
        //    SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        //}

        private void Awake()
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        private void Start()
        {
            GameObject loadingPanel = GameObject.FindGameObjectWithTag("Loading");
            fadeCanvasGroup = loadingPanel.GetComponent<CanvasGroup>();

            // conversationChecker = GetComponent<ConversationChecker>();

            StartCoroutine(LoadSceneSetActive(startSceneName, startSpawnID, startingSequence));
            //if (!string.IsNullOrEmpty(startSpawnID))
            //{
            //    MovePlayerToSpawnPoint(startSpawnID);
            //}

        }

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
            EventHandler.FadeInEvent += OnFadeInEvent;
            EventHandler.FadeOutEvent += OnFadeOutEvent;
            EventHandler.EnableTransitionEvent += OnEnableTransitionEvent;
            EventHandler.DisableTransitionEvent += OnDisableTransitionEvent;


            SceneManager.sceneLoaded += OnSceneLoaded;
            // EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            // EventHandler.EndGameEvent += OnEndGameEvent;
        }
        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
            EventHandler.FadeInEvent -= OnFadeInEvent;
            EventHandler.FadeOutEvent -= OnFadeOutEvent;
            EventHandler.EnableTransitionEvent -= OnEnableTransitionEvent;
            EventHandler.DisableTransitionEvent -= OnDisableTransitionEvent;


            SceneManager.sceneLoaded -= OnSceneLoaded;
            // EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            // EventHandler.EndGameEvent -= OnEndGameEvent;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == startSceneName && !isNotFirstTime)
            {
                EventHandler.CallAfterSceneLoadEvent();
                isNotFirstTime = true;
                // Debug.Log("loaded");
            }
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnloadScene());
        }

        private void OnStartNewGameEvent(int obj)
        {
            // StartCoroutine(LoadSaveDataScene(startSceneName));
        }


        private void OnTransitionEvent(string sceneToGo, string spawnPointID, AnimationSequence sequence, Direction direction)
        {
            //if (conversationChecker)
            //    ;
            //Debug.Log($"Transition triggered with flag{canTransit}");
            if ((!isFade) && canTransit)
            {
                StartCoroutine(Transition(sceneToGo, spawnPointID, sequence, direction));

            }
            // if ()
        }

        private void OnFadeInEvent()
        {
            StartCoroutine(Fade(1f));
        }
        private void OnFadeOutEvent()
        {
            StartCoroutine(Fade(0));
        }

        private void OnDisableTransitionEvent()
        {
            canTransit = false;
        }

        private void OnEnableTransitionEvent()
        {
            canTransit = true;
        }
        private SpawnPoint FindSpawnPointByID(string spawnPointID)
        {
            var spawnPoints = FindObjectsOfType<SpawnPoint>();
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint.spawnPointID == spawnPointID)
                {
                    // Debug.Log("Spawn Point Found!");
                    return spawnPoint;
                }
            }
            // Debug.Log("Spawn Point NOT Found!");
            return null;
        }

        
        private IEnumerator Transition(string sceneName, string spawnPointID, AnimationSequence sequence = null, Direction direction = Direction.None)
        {
            fadeCanvasGroup.blocksRaycasts = true;

            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1);
            EventHandler.CallAfterFadeOutEvent();
            EventHandler.CallPlayerFaceEvent(direction);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());// 卸载当前场景

            yield return LoadSceneSetActive(sceneName, spawnPointID, sequence);// 加载场景并设置为激活

            EventHandler.CallAfterSceneLoadEvent();
            EventHandler.CallBeforeFadeInEvent();

            yield return Fade(0);



            fadeCanvasGroup.blocksRaycasts = false;
        }

        private void MovePlayerToSpawnPoint(string spawnPointID)
        {
            SpawnPoint spawnPoint = FindSpawnPointByID(spawnPointID);
            if (spawnPoint != null)
            {
                Vector3 targetPosition = spawnPoint.transform.position;
                //移动人物坐标
                EventHandler.CallMoveToPosition(targetPosition);

            }
        }

        
        private IEnumerator LoadSceneSetActive(string sceneName, string spawnPointID, AnimationSequence sequence = null)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);

            MovePlayerToSpawnPoint(spawnPointID);

            var animationManager = FindObjectOfType<AnimationManager>();
            // Debug.LogWarning($"Animation manager: {animationManager}");
            if (animationManager != null && sequence != null)
            {
                animationManager.SetStartingSequence(sequence);
            }
            // conversationChecker = GetComponent<ConversationChecker>();
        }

        
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;


            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.loadingFadeDuration;

            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }


            isFade = false;
        }

        //public IEnumerator LoadSaveDataScene(string sceneName)
        //{
        //    yield return Fade(1f);

        //    if (SceneManager.GetActiveScene().name != "PersistentScene")//在游戏过程中 加载另外游戏进度
        //    {
        //        EventHandler.CallBeforeSceneUnloadEvent();
        //        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //    }

        //    yield return LoadSceneSetActive(sceneName);
        //    EventHandler.CallAfterSceneLoadEvent();
        //    yield return Fade(0);
        //}

        private IEnumerator UnloadScene()
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            yield return Fade(0);
        }

        
    }
}

