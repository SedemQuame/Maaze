using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    // Start is called before the first frame update
    [SerializeField] string _androidGameId = "4227891";
    [SerializeField] string _iOSGameId = "4227890";

    private string _gameId;
    void Start()
    {
        // branching depending on environment type.
        #if UNITY_IOS
            _gameId = _iOSGameId;
        #else
            _gameId = _androidGameId;
        #endif
        Advertisement.Initialize(_gameId);
        Advertisement.AddListener(this);
    }

    // playing interstitial ads
    public void PlayAds(){
        if (Advertisement.IsReady("_adType"))
        {
            Advertisement.Show("_adType");
        }
    }

    public void PlayRewardedAd(){
        if (Advertisement.IsReady("_adType"))
        {
            Advertisement.Show("_adType");
        }else{
            Debug.Log("Rewarded ad is not Ready.");
        }
    }

    public void ShowBanner(){
        if (Advertisement.IsReady("_bannerAdType"))
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show("_bannerAdType");
        }else{
            StartCoroutine(RepeatShowBanner());
        }
    }

    public void HideBanner(){
        Advertisement.Banner.Hide();
    }

    IEnumerator RepeatShowBanner(){
        yield return new WaitForSeconds(1);
        ShowBanner();
    }

    public void OnUnityAdsReady(string placementId)
    {
        // throw new System.NotImplementedException();
        // ads are ready
    }

    public void OnUnityAdsDidError(string message)
    {
        // throw new System.NotImplementedException();
        // error msg
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // throw new System.NotImplementedException();
        // video started
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // throw new System.NotImplementedException();
        if(placementId == "rewardedVideo" && showResult == ShowResult.Finished){
            // grant some extra coins
            // other rewards
        }
    }
}