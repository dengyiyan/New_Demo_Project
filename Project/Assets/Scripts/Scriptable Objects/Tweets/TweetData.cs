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
}
