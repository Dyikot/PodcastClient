let audio = null;

window.InitializeAudio = () => {
    audio = document.querySelector('audio');
};

window.Play = () => {
    audio.play();
};

window.Pause = () => {
    audio.pause();
};

window.GetCurrentTime = () => {
    return audio.currentTime;
};

window.SetCurrentTime = (value) => {
    audio.currentTime = value;
};

window.SetVolume = (value) => {
    audio.volume = value;
};

window.SetPlaySpeed = (value) => {
    audio.playbackRate = value;
};