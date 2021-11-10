using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;

namespace AutoDuckProgram
{
    public class Capture
    {
        private WasapiCapture capture;

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
