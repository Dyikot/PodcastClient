let audio = null;

window.InitializeAudio = (control) => {
    audio = control;
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
    localStorage.setItem("audio-volume", value);
};

window.SetPlaySpeed = (value) => {
    audio.playbackRate = value;
    localStorage.setItem("audio-speed", value);
};