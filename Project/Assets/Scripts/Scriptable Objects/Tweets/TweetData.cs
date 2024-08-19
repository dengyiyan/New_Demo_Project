using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTweetData", menuName = "Tweet Data", order = 1)]
public class TweetData : ScriptableObject
{
    [Header("Tweet Information")]
    [TextArea]
    public string tweetText;

    [Tooltip("List of follow-up tweets that will appear after this tweet is sent.")]
    public List<TweetData> followUpTweets;

    [Header("User Information")]
    public Sprite avatar;      // Avatar image for the tweet
    public string username;    // Username for the tweet
    public string subtitle;    // Subtitle for the tweet, e.g., "Average Citizen | Hello world!"

    [Header("Tweet Impact")]
    public List<int> likesList = new List<int>();      // List of likes counts over time
    public List<int> commentsList = new List<int>();   // List of comments counts over time
    public List<int> retweetsList = new List<int>();   // List of retweet counts over time

    public int likesCount;
    public int commentsCount;
    public int retweetCount;

    private int likesIndex = 0;
    private int commentsIndex = 0;
    private int retweetsIndex = 0;

    [Header("Follower Change")]
    public int followerChange = 0;  // The base amount by which followers will change

    public int followerChangeMin = -5;  // Minimum fluctuation range
    public int followerChangeMax = 5;   // Maximum fluctuation range

    public bool isPlayer = false;
    public bool isIan = false;

    private void OnEnable()
    {
        likesIndex = 0;
        commentsIndex = 0;
        retweetsIndex = 0;

        //EventHandler.StaticsUpdateEvent += UpdateStatics;
    }

    private void OnDisable()
    {
        //EventHandler.StaticsUpdateEvent -= UpdateStatics;
    }

    public void UpdateStatics()
    {
        if (likesIndex < likesList.Count)
        {
            likesCount = likesList[likesIndex];
            likesIndex++;
        }

        // Update the comments count if the index is within range
        if (commentsIndex < commentsList.Count)
        {
            commentsCount = commentsList[commentsIndex];
            commentsIndex++;
        }

        // Update the retweets count if the index is within range
        if (retweetsIndex < retweetsList.Count)
        {
            retweetCount = retweetsList[retweetsIndex];
            retweetsIndex++;
        }
    }
}
