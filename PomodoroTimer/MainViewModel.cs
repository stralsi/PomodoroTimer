using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace PomodoroTimer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private DateTime remainingTime;
        public DateTime RemainingTime
        {
            get
            {
                return remainingTime;
            }
            private set
            {
                if (remainingTime != value)
                {
                    remainingTime = value;
                    RaisePropertyChanged("RemainingTime");
                }
            }
        }

        private double volume = 0.5;
        public double Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (volume != value)
                {
                    volume = value;

                    if (mediaPlayer != null)
                    {
                        mediaPlayer.Volume = volume;
                    }

                    RaisePropertyChanged("Volume");
                }
            }
        }

        public void StartCountdown(int seconds)
        {
            StopCountDownIfWasRunning();

            StartNewCountDown(seconds);
        }

        public void Stop()
        {
            StopCountDownIfWasRunning();
        }

        private void StartNewCountDown(int seconds)
        {
            RemainingTime = new DateTime(seconds * TimeSpan.TicksPerSecond);

            StartTimer();

            StartTickingSound();
        }

        private DispatcherTimer dispatcherTimer;

        private void StartTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);//1 second
            dispatcherTimer.Tick += (sender, eventArgs) =>
            {
                if (remainingTime.Ticks == 0)
                {
                    EndOfCountDown();
                }
                else
                {
                    remainingTime = remainingTime.AddSeconds(-1);
                    RaisePropertyChanged("RemainingTime");
                }
            };
            dispatcherTimer.Start();
        }

        private MediaPlayer mediaPlayer;

        private void StartTickingSound()
        {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(@"Resources\audio\analog-timer-tick.mp3", UriKind.Relative)); //sound from https://www.freesound.org/people/EagleStealthTeam/sounds/209698/
            mediaPlayer.Volume = Volume;
            mediaPlayer.MediaEnded += (sender, eventArgs) =>
            {
                //loop
                mediaPlayer.Position = new TimeSpan(0, 0, 0);
                mediaPlayer.Play();
            };
            mediaPlayer.Play();
        }

        private void EndOfCountDown()
        {
            StopCountDownIfWasRunning();
            PlayEndOfCountDownSound();
        }

        private void PlayEndOfCountDownSound()
        {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(@"Resources\audio\analog-timer-ring.mp3", UriKind.Relative)); //sound from https://www.freesound.org/people/EagleStealthTeam/sounds/209698/
            mediaPlayer.Volume = Volume;
            mediaPlayer.Play();
        }

        private void StopCountDownIfWasRunning()
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
            }

            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
