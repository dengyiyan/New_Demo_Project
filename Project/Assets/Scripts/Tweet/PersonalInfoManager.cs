
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PersonalInfoManager : MonoBehaviour
{
    public GameObject selfUsername;
    public GameObject selfSubtitle;
    public GameObject followerCount;
    // Start is called before the first frame update
    void Start()
    {
        selfUsername.GetComponent<Text>().text = GameStateManager.PlayerName + "777";
        selfSubtitle.GetComponent<Text>().text = Settings.TweetzSubtitle;

        OnFollowerChangeEvent();
    }

    private void OnEnable()
    {
        EventHandler.FollowerChangeEvent += OnFollowerChangeEvent;
    }

    private void OnDisable()
    {
        EventHandler.FollowerChangeEvent -= OnFollowerChangeEvent;

    }

    private void OnFollowerChangeEvent()
    {
        int startValue = int.Parse(followerCount.GetComponent<Text>().text);
        int endValue = GameStateManager.Followers;

        // Animate the follower count over 0.5 seconds
        DOTween.To(() => startValue, x => startValue = x, endValue, 0.5f).OnUpdate(() =>
        {
            followerCount.GetComponent<Text>().text = startValue.ToString();
        }).OnComplete(() =>
        {
            followerCount.GetComponent<Text>().text = endValue.ToString();
        });
    }

}
