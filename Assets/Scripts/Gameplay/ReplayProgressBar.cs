using DG.Tweening;
using UnityEngine;
using Michsky.MUIP;
using TMPro;
using UltimateReplay;
using UltimateReplay.Storage;


public class ReplayProgressBar : MonoBehaviour
{
    public bool isPlay = false;

    [SerializeField] private ButtonManager playBut;
    [SerializeField] private SliderManager progressSlider;
    [SerializeField] private SliderManager playSpeedSlider;
    [SerializeField] private SwitchManager reverseSwitcher;
    [SerializeField] private TextMeshProUGUI playTimeStamp;

    public Sprite playIcon;
    public Sprite pauseIcon;

    public bool m_Completed = false;
    [SerializeField] private ReplayControls replayController;


    private void Awake()
    {
        playBut.onClick.AddListener(OnPlayButClick);
        reverseSwitcher.onValueChanged.AddListener(OnReserveSwitchChanged);
    }

    // Update is called once per frame
    void Update()
    {
        var handle = replayController.PlaybackHandle;
        if (!handle.IsDisposed)
        {
            var target = ReplayManager.GetReplayStorageTarget(handle);
            var playbackTime = ReplayManager.GetPlaybackTime(handle);
            var normalizedTime = ReplayManager.GetPlaybackTime(handle).NormalizedTime;

            if (!progressSlider.isDragging)
            {
                progressSlider.Value = normalizedTime;
            }
            else
            {
                // replayController.OnBtnPausePlayback();
                ReplayManager.SetPlaybackTimeNormalized(replayController.PlaybackHandle, progressSlider.Value);
            }

            if (progressSlider.Value > 0.99f)
            {
                m_Completed = true;
            }

            if (m_Completed)
            {
                m_Completed = false;
                MoveBack();

                replayController.OnBtnLive();
            }

            UpdatePlaySpeed(handle, progressSlider.Value);
            UpdateTimeStampText(target, playbackTime);
        }
    }

    // click the button, and decide play or pause
    void OnPlayButClick()
    {
        if (!replayController.PlaybackHandle.IsDisposed)
        {
            Debug.Log("Play Button Clicked!");
            if (isPlay)
                playBut.SetIcon(pauseIcon);
            else
                playBut.SetIcon(playIcon);

            isPlay = !isPlay;

            if (!replayController.PlaybackHandle.IsDisposed)
            {
                if (isPlay)
                    replayController.OnBtnResumePlayback();
                else
                    replayController.OnBtnPausePlayback();
            }
        }
    }

    void OnReserveSwitchChanged(bool v)
    {
        if (!replayController.PlaybackHandle.IsDisposed)
        {
            Debug.Log("Reserve Switch value changed to : " + v);

            // Set direction for playback
            ReplayManager.SetPlaybackDirection(replayController.PlaybackHandle, v
                ? ReplayManager.PlaybackDirection.Backward
                : ReplayManager.PlaybackDirection.Forward);
        }
    }

    void UpdateTimeStampText(ReplayStorageTarget target, ReplayTime replayTime)
    {
        string currentTime = ReplayTime.GetCorrectedTimeValueString(replayTime.Time);
        string totalTime = ReplayTime.GetCorrectedTimeValueString((target != null)
            ? target.Duration
            : 0);

        playTimeStamp.SetText(currentTime + "/" + totalTime);
    }

    void UpdatePlaySpeed(ReplayHandle handle, float speed)
    {
        ReplayManager.SetPlaybackTimeScale(handle, speed);
    }

    // Start is called before the first frame update
    public void MoveInto()
    {
        Vector3 targetPosition = new Vector3(462.0f, 100.0f, 0);

        transform.DOMove(targetPosition, 1);
    }


    public void MoveBack()
    {
        Vector3 targetPosition = new Vector3(462.0f, -100.0f, 0);

        transform.DOMove(targetPosition, 1);
    }
}