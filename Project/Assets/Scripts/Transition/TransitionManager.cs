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

        // private ConversationChecker conversationChecker;
        private CanvasGroup fadeCanvasGroup;
        private bool isFade;

        private bool canTransit = true;

        // public string GUID => GetComponent<DataGUID>().guid;

        //private override void Awake()
        //{
        //    base.Awake();
        //    SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        //}

        private void Start()
        {
            GameObject loadingPanel = GameObject.FindGameObjectWithTag("Loading");
            fadeCanvasGroup = loadingPanel.GetComponent<CanvasGroup>();

            // conversationChecker = GetComponent<ConversationChecker>();

            StartCoroutine(LoadSceneSetActive(startSceneName));

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
            //if (scene.name == startSceneName)
            //{
            //    EventHandler.CallAfterSceneLoadEvent();
            //}
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnloadScene());
        }

        private void OnStartNewGameEvent(int obj)
        {
            // StartCoroutine(LoadSaveDataScene(startSceneName));
        }


        private void OnTransitionEvent(string sceneToGo, string spawnPointID)
        {
            //if (conversationChecker)
            //    ;
            Debug.Log($"Transition triggered with flag{canTransit}");
            if ((!isFade) && canTransit)
            {
                StartCoroutine(Transition(sceneToGo, spawnPointID));

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
                    Debug.Log("Spawn Point Found!");
                    return spawnPoint;
                }
            }
            Debug.Log("Spawn Point NOT Found!");
            return null;
        }

        /// <summary>
        /// �����л�
        /// </summary>
        /// <param name="sceneName">Ŀ��λ��</param>
        /// <param name="spawnPointID">Ŀ��spawnpoint</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, string spawnPointID)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());// ж�ص�ǰ����

            yield return LoadSceneSetActive(sceneName);// ���س���������Ϊ����

            SpawnPoint spawnPoint = FindSpawnPointByID(spawnPointID);
            if (spawnPoint != null)
            {
                Vector3 targetPosition = spawnPoint.transform.position;
                //�ƶ���������
                EventHandler.CallMoveToPosition(targetPosition);

            }
            EventHandler.CallAfterSceneLoadEvent();
            yield return Fade(0);
            fadeCanvasGroup.blocksRaycasts = false;
        }


        /// <summary>
        /// ���س���������Ϊ����
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);

            // conversationChecker = GetComponent<ConversationChecker>();
        }

        /// <summary>
        /// Fade in or Fade out
        /// </summary>
        /// <param name="targetAlpha">1 = fade in��0 = fade out</param>
        /// <returns></returns>
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

        public IEnumerator LoadSaveDataScene(string sceneName)
        {
            yield return Fade(1f);

            if (SceneManager.GetActiveScene().name != "PersistentScene")//����Ϸ������ ����������Ϸ����
            {
                EventHandler.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }

            yield return LoadSceneSetActive(sceneName);
            EventHandler.CallAfterSceneLoadEvent();
            yield return Fade(0);
        }

        private IEnumerator UnloadScene()
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            yield return Fade(0);
        }

        //public GameSaveData GenerateSaveData()
        //{
        //    GameSaveData saveData = new GameSaveData();
        //    saveData.dataSceneName = SceneManager.GetActiveScene().name;

        //    return saveData;
        //}

        //public void RestoreData(GameSaveData saveData)
        //{
        //    //������Ϸ���ȳ���
        //    StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        //}
    }
}

