let video = null;
let videoPlayer = null;

function InitializeVideo(control) {
    video = control;
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
}

function PlayVideo() {
    video.play();
}

function PauseVideo() {
    video.pause();
}

function GetVideoCurrentTime() {
    return video.currentTime;
}

function SetVideoCurrentTime(value) {
    video.currentTime = value;
}

function SetVideoVolume(value) {
    video.volume = value;
    localStorage.setItem("video-volume", value);
}

function SetVideoPlaySpeed(value) {
    video.playbackRate = value;
    localStorage.setItem("video-speed", value);
}

function SwitchFullscreen() {
    if (document.fullscreenElement) {
        document.exitFullscreen();
    }
    else {
        videoPlayer.requestFullscreen();
    }
}