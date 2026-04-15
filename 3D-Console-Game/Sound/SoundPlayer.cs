using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace _3D_Console_Game.Sound
{
    internal static class SoundPlayer
    {

        public static void PlaySound(string fileName, float vol = 1)
        {
            var audioFile = new AudioFileReader("./Sound/Resources/" + fileName);
            var outputDevice = new WaveOutEvent();
            outputDevice.Volume = vol;
            outputDevice.Init(audioFile);
            outputDevice.Play();
            
        }
    }
}
