using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TreeEditor;

public class TweetManager : MonoBehaviour
{
    public Transform chatWindow;  // The parent for tweet bubbles (Content of the ScrollView)
    public GameObject tweetPrefab;  // Prefab for a tweet bubble
    public GameObject replyPrefab;
    public TweetData startingTweetData;  // Starting tweet data
    public ScrollRect scrollRect;

    private TweetData currentTweetData;  // The current tweet being displayed
    private GameObject currentTweetInstance;  // The current tweet instance in the UI
    private int currentTweetIndex = 0;
    private List<TweetData> playerTweetOptions = new List<TweetData>();  // List of player tweet options
    //private bool waitingForNextClick = false;

    [SerializeField] private Sprite PlayerAvatar;
    //[SerializeField] private string PlayerUsername;
    //[SerializeField] private string PlayerSubtitle;
    [SerializeField] private Sprite IanAvatar;
    [SerializeField] private string IanUsername;
    [SerializeField] private string IanSubtitle;
    [SerializeField] [SceneName] private string nextScene;
    //[SerializeField]
    private TweetInstanceHandler instanceHandler;

    void Start()
    {
        playerTweetOptions = new List<TweetData> { startingTweetData };
        // Start with the initial tweet
        AddTweet(startingTweetData);
        //HandleFollowUpTweets(startingTweetData);
    }

    void SetTweetFields(TweetData tweetData)
    {
        // Determine which prefab to use based on the type of tweetData
        GameObject prefabToUse = (tweetData is ReplyData) ? replyPrefab : tweetPrefab;

        // If a tweet is already being displayed, use the existing instance
        if (currentTweetInstance == null)
        {
            currentTweetInstance = Instantiate(prefabToUse, chatWindow);
            instanceHandler = currentTweetInstance.AddComponent<TweetInstanceHandler>();
            instanceHandler.Initialize(playerTweetOptions);
            //EventHandler.RegisterToStaticsUpdate(() => UpdateTweetInstance(currentTweetInstance, tweetData));
        }
        // Reference to the main components
        Transform tweetBody = currentTweetInstance.transform.Find("TweetBody");
        Transform panel = tweetBody.Find("Panel");
        // Transform statics = tweetBody.Find("Statics");

        // Set avatar, username, subtitle, and tweet text
        if (tweetData.isPlayer)
            panel.Find("Avatar").GetComponent<Image>().sprite = PlayerAvatar;
        else if (tweetData.isIan)
            panel.Find("Avatar").GetComponent<Image>().sprite = IanAvatar;
        else if (tweetData.avatar)
            panel.Find("Avatar").GetComponent<Image>().sprite = tweetData.avatar;


        if (tweetData.isPlayer)
            panel.Find("Username").GetComponent<Text>().text = GameStateManager.PlayerName + "777";
        else if (tweetData.isIan)
            panel.Find("Username").GetComponent<Text>().text = IanUsername;
        else if (!string.IsNullOrEmpty(tweetData.username))
            panel.Find("Username").GetComponent<Text>().text = tweetData.username;


        if (tweetData.isPlayer)
            panel.Find("Subtitle").GetComponent<Text>().text = Settings.TweetzSubtitle;
        else if (tweetData.isIan)
            panel.Find("Subtitle").GetComponent<Text>().text = IanSubtitle;
        else if (!string.IsNullOrEmpty(tweetData.subtitle))
            panel.Find("Subtitle").GetComponent<Text>().text = tweetData.subtitle; 

        tweetBody.Find("Text").GetComponent<Text>().text = tweetData.tweetText;
    }

    void DisplayTweet(TweetData tweetData)
    {
        SetTweetFields(tweetData);

        Transform buttons = currentTweetInstance.transform.Find("Buttons");

        // Set stats (you can expand this if you have real data for likes, comments, retweets)
        //statics.Find("LikesCount").GetComponent<Text>().text = "0";  // Example default value
        //statics.Find("CommentsCount").GetComponent<Text>().text = "0";  // Example default value
        //statics.Find("RetweetCount").GetComponent<Text>().text = "0";  // Example default value

        // Hide buttons if it's not a player tweet
        Button publishButton = buttons.Find("PublishButton").GetComponent<Button>();
        Button pickAnotherOneButton = buttons.Find("PickAnotherOneButton").GetComponent<Button>();
        publishButton.gameObject.SetActive(false);
        pickAnotherOneButton.gameObject.SetActive(false);

        currentTweetData = tweetData;
        //waitingForNextClick = true;
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();  // Ensure UI is updated before changing scroll position
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatWindow.GetComponent<RectTransform>());
        scrollRect.verticalNormalizedPosition = 0f;  // Scroll to the bottom
    }


    //void UpdateTweetInstance(GameObject tweetInstance, TweetData tweetData)
    //{
    //    Transform statics = tweetInstance.transform.Find("TweetBody/Statics");
    //    tweetData.UpdateStatics();  // Update the statics in TweetData
    //    UpdateTweetStatics(statics, tweetData);  // Update the UI based on the new statics
    //}

    void UpdateTweetStatics(Transform statics, TweetData tweetData)
    {
        // Update statics using values from tweetData
        Debug.Log($"Get text field: {statics.Find("LikesCount").GetComponent<Text>().text}");
        Debug.Log($"Updated like: {tweetData.likesCount}");
        statics.Find("LikesCount").GetComponent<Text>().text = tweetData.likesCount.ToString();
        statics.Find("CommentsCount").GetComponent<Text>().text = tweetData.commentsCount.ToString();
        statics.Find("RetweetCount").GetComponent<Text>().text = tweetData.retweetCount.ToString();
    }

    void DisplayTweetFinal(TweetData tweetData)
    {
        DisplayTweet(tweetData);
        EventHandler.CallStaticsUpdate();

        if (tweetData.followerChange != 0)
            ApplyFollowerChange(tweetData.followerChange, tweetData.followerChangeMin, tweetData.followerChangeMax);

        Transform buttons = currentTweetInstance.transform.Find("Buttons");
        GameObject.Destroy(buttons.gameObject);
        currentTweetInstance = null;
        HandleFollowUpTweets(tweetData);
        ScrollToBottom();
    }

    void ApplyFollowerChange(int baseChange, int minFluctuation, int maxFluctuation)
    {
        // Create a random fluctuation within the given range
        int fluctuation = Random.Range(minFluctuation, maxFluctuation + 1);

        // Calculate the final change in followers
        int finalChange = baseChange + fluctuation;

        // Update the current follower count
        GameStateManager.Followers += finalChange;
        if (GameStateManager.Followers < 0)
            GameStateManager.Followers = 0;

        EventHandler.CallFollowerChangeEvent();
        // Optionally, you can add code here to update the UI with the new follower count
        //Debug.Log($"Followers changed by {finalChange}. New follower count: {currentFollowers}");
    }

    void HandleFollowUpTweets(TweetData tweetData)
    {
        // Use the follow-up TweetData objects directly
        playerTweetOptions = new List<TweetData>(tweetData.followUpTweets);

        if (tweetData.followUpTweets.Count > 1)
        {
            // More than one follow-up tweet means these are player choices
            SetPlayerTweetOptions(tweetData);
        }
        else if (tweetData.followUpTweets.Count == 1)
        {
            // Only one follow-up tweet, automatically display it
            AddTweet(tweetData.followUpTweets[0]); 
            //waitingForNextClick = true;
        }
        else
        {
            // No follow-up tweets, nothing else to display
            EndTweetChain();
        }
    }

    void SetPlayerTweetOptions(TweetData tweetData)
    {
        // Start allowing the player to choose
        BeginPlayerTweetSelection();
    }

    void BeginPlayerTweetSelection()
    {
        currentTweetIndex = 0;
        UpdateSampleTweet();
    }

    void UpdateSampleTweet()
    {
        TweetData currentOption = playerTweetOptions[currentTweetIndex];
        DisplaySampleTweet(currentOption);
    }

    void DisplaySampleTweet(TweetData tweetData)
    {
        SetTweetFields(tweetData);

        Transform buttons = currentTweetInstance.transform.Find("Buttons");

        Button publishButton = buttons.Find("PublishButton").GetComponent<Button>();
        Button pickAnotherOneButton = buttons.Find("PickAnotherOneButton").GetComponent<Button>();

        // Enable buttons for publishing or picking another tweet
        publishButton.gameObject.SetActive(true);
        pickAnotherOneButton.gameObject.SetActive(true);

        // Set button listeners
        publishButton.onClick.RemoveAllListeners();  // Clear previous listeners to avoid stacking
        pickAnotherOneButton.onClick.RemoveAllListeners();

        publishButton.onClick.AddListener(() => SendTweet(tweetData));
        pickAnotherOneButton.onClick.AddListener(ChangeTweet);

    }

    void SendTweet(TweetData selectedTweet)
    {
        //AddTweet(selectedTweet);  // Update the current instance with the selected tweet data
        //HandleFollowUpTweets(selectedTweet);
        DisplayTweetFinal(selectedTweet);
    }

    void ChangeTweet()
    {
        instanceHandler.AddIndex();
        currentTweetIndex = (currentTweetIndex + 1) % playerTweetOptions.Count;
        UpdateSampleTweet();
        ScrollToBottom();
    }

    void AddTweet(TweetData newTweetData)
    {
        // This method is now called to update the existing tweet instance with new data
        DisplayTweet(newTweetData);

        Transform buttons = currentTweetInstance.transform.Find("Buttons");

        //buttons.gameObject.SetActive(true);

        Button pickAnotherOneButton = buttons.Find("PickAnotherOneButton").GetComponent<Button>();
        Button publishButton = buttons.Find("PublishButton").GetComponent<Button>();

        pickAnotherOneButton.gameObject.SetActive(true);
        publishButton.gameObject.SetActive(false);

        pickAnotherOneButton.onClick.RemoveAllListeners();
        pickAnotherOneButton.GetComponentInChildren<Text>().text = "Next";  // Rename button to "Next"
        pickAnotherOneButton.onClick.AddListener(() => DisplayTweetFinal(newTweetData));  // Set up listener for the next tweet

        //HandleFollowUpTweets(newTweetData);
    }

    void EndTweetChain()
    {
        Debug.Log("Tweet chain ended.");
        if (!string.IsNullOrEmpty(nextScene))
            EventHandler.CallTransitionEvent(nextScene, null);
    }
}
