let video = null;
let videoPlayer = null;

window.InitializeVideo = () => {
    video = document.querySelector('video');
    videoPlayer = document.getElementById("video-player");

    video.addEventListener('keydown', (e) => {
        if (e.key == ' ' && e.target === video) {
            e.preventDefault();
        }
    });

    videoPlayer.addEventListener('fullscreenchange', () => {
        if (document.fullscreenElement == videoPlayer) {
            video.focus();
        }
    });
};

window.PlayVideo = () => {
    video.play();
};

window.PauseVideo = () => {
    video.pause();
};

window.GetVideoCurrentTime = () => {
    return video.currentTime;
};

window.SetVideoCurrentTime = (value) => {
    video.currentTime = value;
};

window.SetVideoVolume = (value) => {
    video.volume = value;
};

window.SetVideoPlaySpeed = (value) => {
    video.playbackRate = value;
};

window.SwitchFullscreen = () => {
    if (document.fullscreenElement) {
        document.exitFullscreen();
    }
    else {
        videoPlayer.requestFullscreen();
    }
};