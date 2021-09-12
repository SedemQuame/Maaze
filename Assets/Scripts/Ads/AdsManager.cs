using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    // Start is called before the first frame update
    [SerializeField] string _androidGameId = "4227891";
    [SerializeField] string _iOSGameId = "4227890";
    private string _gameId;
    private string _interstitialAds = "Interstitial_", _rewardedAds = "Rewarded_", _bannerAds = "Banner_";
    public PlayerController playerController;
    public GameObject player, overlayPanel, gameLostPanel, timer;
    // public GameObject rewarded_ads_btn;
    bool testMode = true, adShown = false, isPlayerResuscitated = false, isLevelReloaded = false;

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
        Advertisement.AddListener(this);
        Advertisement.Initialize(_gameId, testMode);

        // set player controller script
        playerController = player.GetComponent<PlayerController>();
    }

    // playing interstitial ads
    public void PlayAds(){
        if (Advertisement.IsReady(_interstitialAds))
        {
            Advertisement.Show(_interstitialAds);
        }
    }

    public void PlayRewardedAd(){
        if (Advertisement.IsReady(_rewardedAds))
        {
            Advertisement.Show(_rewardedAds);
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
    {}

    public void OnUnityAdsDidError(string message)
    {
        // throw new System.NotImplementedException();
        // error msg
        Debug.Log("Error occurred with unity ads.");
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // throw new System.NotImplementedException();
        // video started
        Debug.Log("Unity ads started.");        
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        adShown=true;
        if (placementId == _interstitialAds || placementId == _rewardedAds)
        {
            if(showResult == ShowResult.Finished && adShown){
                if(placementId == _rewardedAds){
                    if(!isPlayerResuscitated){
                        revivePlayer();

                        // activate player reward.
                        activatePlayerReward();

                        // hide the gamelost and overlay panel.
                        gameLostPanel.SetActive(false);
                        overlayPanel.SetActive(false);
                    }else{
                        if(!isLevelReloaded){
                            // reload active scene
                            reloadLevel();
                        }
                    }
                }


                if (placementId == _interstitialAds)
                {
                    changeLevel();
                }
                adShown=false;
            }
        }
    }

    private void changeLevel(){
        if(LevelDifficulty.levelDifficulty >= 33 && LevelDifficulty.levelDifficulty <= 40){
            SceneManager.LoadScene("Maaze Game Play Lighting Dark");
        }else{                    
            SceneManager.LoadScene("Maaze Game Play");
        }
    }

    private void reloadLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isLevelReloaded = true;
    }

    private void revivePlayer(){
        // revive player.
        player.SetActive(true);
        playerController.resusciatePlayer();
        isPlayerResuscitated = true;
    }

    private void activatePlayerReward(){
        // if in timer section add some more seconds.
        int rewardedTimeLimit = 40;
        if(LevelDifficulty.levelDifficulty >= 17 && LevelDifficulty.levelDifficulty <= 32){
            timer.GetComponent<Timer>().SetDuration(rewardedTimeLimit)
                .OnEnd(() => playerController.killPlayerOnTimeOut())
                .Begin();
        }
    }
}