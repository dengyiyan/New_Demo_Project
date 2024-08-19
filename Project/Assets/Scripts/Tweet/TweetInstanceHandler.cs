using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweetInstanceHandler : MonoBehaviour
{
    private List<TweetData> tweetDataList;
    private Transform statics;
    private int currentTweetIndex = 0;

    public void Initialize(List<TweetData> dataList)
    {
        tweetDataList = dataList;
        statics = transform.Find("TweetBody/Statics");

        // Register to the statics update event
        EventHandler.RegisterToStaticsUpdate(UpdateTweetStatics);
    }

    private void OnDestroy()
    {
        // Unregister from the event when this instance is destroyed
        EventHandler.UnregisterFromStaticsUpdate(UpdateTweetStatics);
    }

    private void UpdateTweetStatics()
    {
        //if (currentTweetIndex >= tweetDataList.Count)
        //    return;  // No more tweets to update

        TweetData currentTweetData = tweetDataList[currentTweetIndex];
        currentTweetData.UpdateStatics();  // Update the statics in TweetData

        UpdateTextField(statics.Find("LikesCount").GetComponent<Text>(), currentTweetData.likesCount);
        UpdateTextField(statics.Find("CommentsCount").GetComponent<Text>(), currentTweetData.commentsCount);
        UpdateTextField(statics.Find("RetweetCount").GetComponent<Text>(), currentTweetData.retweetCount);
    }

    private void UpdateTextField(Text text, int count)
    {
        int startValue = int.Parse(text.text);
        int endValue = count;

        DOTween.To(() => startValue, x => startValue = x, endValue, 0.5f).OnUpdate(() =>
        {
            text.text = startValue.ToString();
        }).OnComplete(() =>
        {
            text.text = endValue.ToString();
        });
    }

    public void AddIndex()
    {
        currentTweetIndex = (currentTweetIndex + 1)%(tweetDataList.Count);
    }
}
