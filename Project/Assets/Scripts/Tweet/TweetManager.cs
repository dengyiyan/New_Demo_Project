using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TweetManager : MonoBehaviour
{
    public Transform chatWindow;  // The parent for tweet bubbles (Content of the ScrollView)
    public GameObject tweetPrefab;  // Prefab for a tweet bubble
    public TweetData startingTweetData;  // Starting tweet data

    private TweetData currentTweetData;  // The current tweet being displayed
    private GameObject currentTweetInstance;  // The current tweet instance in the UI
    private int currentTweetIndex = 0;
    private List<TweetData> playerTweetOptions = new List<TweetData>();  // List of player tweet options
    //private bool waitingForNextClick = false;

    void Start()
    {
        // Start with the initial tweet
        AddTweet(startingTweetData);
        //HandleFollowUpTweets(startingTweetData);
    }
    //void AddClickListenerToChatWindow()
    //{
    //    EventTrigger trigger = chatWindow.gameObject.AddComponent<EventTrigger>();

    //    EventTrigger.Entry entry = new EventTrigger.Entry();
    //    entry.eventID = EventTriggerType.PointerClick;
    //    entry.callback.AddListener((eventData) => { OnChatWindowClick(); });

    //    trigger.triggers.Add(entry);
    //}

    //void OnChatWindowClick()
    //{
    //    Debug.Log("Clicked!");
    //    if (waitingForNextClick)
    //    {
    //        waitingForNextClick = false;

    //        // Display the next tweet
    //        HandleFollowUpTweets(currentTweetData);
    //    }
    //}

    void DisplayTweet(TweetData tweetData)
    {
        // If a tweet is already being displayed, use the existing instance
        if (currentTweetInstance == null)
        {
            currentTweetInstance = Instantiate(tweetPrefab, chatWindow);
        }

        // Reference to the main components
        Transform tweetBody = currentTweetInstance.transform.Find("TweetBody");
        Transform panel = tweetBody.Find("Panel");
        Transform statics = tweetBody.Find("Statics");
        Transform buttons = currentTweetInstance.transform.Find("Buttons");

        // Set avatar, username, subtitle, and tweet text
        panel.Find("Avatar").GetComponent<Image>().sprite = tweetData.avatar;
        panel.Find("Username").GetComponent<Text>().text = tweetData.username;
        panel.Find("Subtitle").GetComponent<Text>().text = tweetData.subtitle;
        tweetBody.Find("Text").GetComponent<Text>().text = tweetData.tweetText;

        // Set stats (you can expand this if you have real data for likes, comments, retweets)
        statics.Find("LikesCount").GetComponent<Text>().text = "0";  // Example default value
        statics.Find("CommentsCount").GetComponent<Text>().text = "0";  // Example default value
        statics.Find("RetweetCount").GetComponent<Text>().text = "0";  // Example default value

        // Hide buttons if it's not a player tweet
        Button publishButton = buttons.Find("PublishButton").GetComponent<Button>();
        Button pickAnotherOneButton = buttons.Find("PickAnotherOneButton").GetComponent<Button>();
        publishButton.gameObject.SetActive(false);
        pickAnotherOneButton.gameObject.SetActive(false);

        currentTweetData = tweetData;
        //waitingForNextClick = true;
    }

    void DisplayTweetFinal(TweetData tweetData)
    {
        DisplayTweet(tweetData);
        Transform buttons = currentTweetInstance.transform.Find("Buttons");
        GameObject.Destroy(buttons.gameObject);
        currentTweetInstance = null;
        HandleFollowUpTweets(tweetData);
    }


    void HandleFollowUpTweets(TweetData tweetData)
    {
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
        // Use the follow-up TweetData objects directly
        playerTweetOptions = new List<TweetData>(tweetData.followUpTweets);

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
        // If a tweet is already being displayed, use the existing instance
        if (currentTweetInstance == null)
        {
            currentTweetInstance = Instantiate(tweetPrefab, chatWindow);
        }

        // Reference to the main components
        Transform tweetBody = currentTweetInstance.transform.Find("TweetBody");
        Transform panel = tweetBody.Find("Panel");
        Transform buttons = currentTweetInstance.transform.Find("Buttons");

        // Set avatar, username, subtitle, and tweet text
        panel.Find("Avatar").GetComponent<Image>().sprite = tweetData.avatar;
        panel.Find("Username").GetComponent<Text>().text = tweetData.username;
        panel.Find("Subtitle").GetComponent<Text>().text = tweetData.subtitle;
        tweetBody.Find("Text").GetComponent<Text>().text = tweetData.tweetText;

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
        currentTweetIndex = (currentTweetIndex + 1) % playerTweetOptions.Count;
        UpdateSampleTweet();
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
    }
}
