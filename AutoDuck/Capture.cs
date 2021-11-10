using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;

namespace AutoDuckProgram
{
    public class Capture
    {
        private WasapiCapture capture;
        private float volume;

        public float Volume { get => volume; }

        public Capture(WasapiCapture capture)
        {
            this.capture = capture;
            this.capture.StartRecording();
        }

        public void Stop() {
            capture.StopRecording();
        }
    }
}
