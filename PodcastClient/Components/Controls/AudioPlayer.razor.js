let audio = null;

function InitializeAudio(control) {
    audio = control;
}

function Play() {
    audio.play();
}

function Pause() {
    audio.pause();
}

function GetCurrentTime() {
    return audio.currentTime;
}

function SetCurrentTime(value) {
    audio.currentTime = value;
}

function SetVolume(value) {
    audio.volume = value;
    localStorage.setItem("audio-volume", value);
}

function SetPlaySpeed(value) {
    audio.playbackRate = value;
    localStorage.setItem("audio-speed", value);
}