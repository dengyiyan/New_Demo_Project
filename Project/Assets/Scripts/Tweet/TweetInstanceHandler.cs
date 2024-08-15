using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TweetInstanceHandler : MonoBehaviour
{
    private TweetData tweetData;
    private Transform statics;

    public void Initialize(TweetData data)
    {
        tweetData = data;
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
        tweetData.UpdateStatics();  // Update the statics in TweetData

        UpdateTextField(statics.Find("LikesCount").GetComponent<Text>(), tweetData.likesCount);
        UpdateTextField(statics.Find("CommentsCount").GetComponent<Text>(), tweetData.commentsCount);
        UpdateTextField(statics.Find("RetweetCount").GetComponent<Text>(), tweetData.retweetCount);
        //statics.Find("LikesCount").GetComponent<Text>().text = tweetData.likesCount.ToString();
        //statics.Find("CommentsCount").GetComponent<Text>().text = tweetData.commentsCount.ToString();
        //statics.Find("RetweetCount").GetComponent<Text>().text = tweetData.retweetCount.ToString();
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
}
