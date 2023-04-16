using UnityEngine ;
using UnityEngine.UI ;
using UnityEngine.Events ;
using System.Collections ;
using TMPro;

public class Timer : MonoBehaviour {
   [Header ("Timer UI references :")]
//    [SerializeField] private Image uiFillImage ;
   public TMP_Text uiText ;
   public AudioClip tickSound;

   public int Duration { get; private set; }

   public bool IsPaused { get; private set; }

   private int remainingDuration ;

   // Events --
   private UnityAction onTimerBeginAction ;
   private UnityAction<int> onTimerChangeAction ;
   private UnityAction onTimerEndAction ;
   private UnityAction<bool> onTimerPauseAction ;
    private AudioSource source;

   private void Awake () {
        ResetTimer () ;
        source = GetComponent<AudioSource>();
   }

   private void ResetTimer () {
      uiText.text = "00:00" ;
    //   uiFillImage.fillAmount = 0f ;

      Duration = remainingDuration = 0 ;

      onTimerBeginAction = null ;
      onTimerChangeAction = null ;
      onTimerEndAction = null ;
      onTimerPauseAction = null ;

      IsPaused = false ;
   }

   public void SetPaused (bool paused) {
      IsPaused = paused ;

      if (onTimerPauseAction != null)
         onTimerPauseAction.Invoke (IsPaused) ;
   }


   public Timer SetDuration (int seconds) {
      Duration = remainingDuration = seconds ;
      return this ;
   }

   //-- Events ----------------------------------
   public Timer OnBegin (UnityAction action) {
      onTimerBeginAction = action ;
      return this ;
   }

   public Timer OnChange (UnityAction<int> action) {
      onTimerChangeAction = action ;
      return this ;
   }

   public Timer OnEnd (UnityAction action) {
      onTimerEndAction = action ;
      return this ;
   }

   public Timer OnPause (UnityAction<bool> action) {
      onTimerPauseAction = action ;
      return this ;
   }

   public void Begin () {
      if (onTimerBeginAction != null)
         onTimerBeginAction.Invoke () ;

      StopAllCoroutines () ;
      StartCoroutine (UpdateTimer ()) ;
   }

   private IEnumerator UpdateTimer () {
      while (remainingDuration > 0) {
         if (!IsPaused) {
            if (onTimerChangeAction != null)
               onTimerChangeAction.Invoke (remainingDuration) ;

            UpdateUI (remainingDuration) ;
            PlayTickSound();
            remainingDuration-- ;
         }
         yield return new WaitForSeconds (1f) ;
      }
      End () ;
   }

   private void UpdateUI (int seconds) {
      uiText.text = string.Format ("{0:D2}:{1:D2}", seconds / 60, seconds % 60) ;
    //   uiFillImage.fillAmount = Mathf.InverseLerp (0, Duration, seconds) ;
   }

   private void PlayTickSound(){
        source.PlayOneShot(tickSound, 0.8f);
   }

   public void End () {
      if (onTimerEndAction != null)
         onTimerEndAction.Invoke () ;

      ResetTimer () ;
   }


   private void OnDestroy () {
      StopAllCoroutines () ;
   }
}
