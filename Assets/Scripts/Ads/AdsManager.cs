using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    // Start is called before the first frame update
    [SerializeField] string _androidGameId = "4227891";
    [SerializeField] string _iOSGameId = "4227890";
    private string _gameId;
    private string _interstitialAds = "Interstitial_", _rewardedAds = "Rewarded_", _bannerAds = "Banner_";
    public ButtonBehaviour buttonBehaviour;
    bool testMode = true;
    void Start()
    {
        // branching depending on environment type.
        #if UNITY_IOS
            _gameId = _iOSGameId;
            _interstitialAds += "iOS";
            _rewardedAds += "iOS";
            _bannerAds += "iOS"; 
        #else
            _gameId = _androidGameId;
            _interstitialAds += "Android";
            _rewardedAds += "Android";
            _bannerAds += "Android";
        #endif

        Debug.Log(_interstitialAds);
        Debug.Log(_rewardedAds);
        Debug.Log(_bannerAds);

        Advertisement.AddListener(this);
        Advertisement.Initialize(_gameId, testMode);
    }

    // playing interstitial ads
    public void PlayAds(){
        if (Advertisement.IsReady(_interstitialAds))
        {
            Debug.Log("Show Ads");
            Advertisement.Show(_interstitialAds);
        }else{
            Debug.Log("Unable to show ads");
            buttonBehaviour.loadNextGamePlayLevelcene();
        }
    }

    public void PlayRewardedAd(){
        if (Advertisement.IsReady(_rewardedAds))
        {
            Advertisement.Show(_rewardedAds);
        }else{
            Debug.Log("Unable to show ads");
            buttonBehaviour.loadNextGamePlayLevelcene();
        }
    }

    public void ShowBanner(){
        if (Advertisement.IsReady(_bannerAds))
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(_bannerAds);
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
        // if(placementId == _interstitialAds){
        //     PlayAds();
        // }else if(placementId == _rewardedAds){
        //     PlayRewardedAd();
        // }
        // throw new System.NotImplementedException();
        // ads are ready
        // Debug.Log("");
    }

    public void OnUnityAdsDidError(string message)
    {
        // throw new System.NotImplementedException();
        // error msg
        Debug.Log("");
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // throw new System.NotImplementedException();
        // video started
        Debug.Log("");        
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // throw new System.NotImplementedException();


        if (placementId == _interstitialAds || placementId == _rewardedAds)
        {
            if(showResult == ShowResult.Finished){
                // grant some extra coins
                // other rewards
                Debug.Log("Perform reward function");
            }
            buttonBehaviour.loadNextGamePlayLevelcene();
        }
    }
}