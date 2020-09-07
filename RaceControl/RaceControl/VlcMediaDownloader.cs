﻿using LibVLCSharp.Shared;
using Prism.Mvvm;
using RaceControl.Enums;
using RaceControl.Interfaces;
using System;
using System.Threading.Tasks;

namespace RaceControl
{
    public class VlcMediaDownloader : BindableBase, IMediaDownloader
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;

        private DownloadStatus _status = DownloadStatus.Pending;
        private float _progress;
        private bool _disposed;

        public VlcMediaDownloader(LibVLC libVLC, MediaPlayer mediaPlayer)
        {
            _libVLC = libVLC;
            _mediaPlayer = mediaPlayer;
            _mediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
            _mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
        }

        public DownloadStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public float Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public async Task StartDownloadAsync(string streamUrl, string filename)
        {
            if (streamUrl == null)
            {
                Status = DownloadStatus.Failed;
                return;
            }

            var option = $":sout=#std{{access=file,mux=ts,dst=\"{filename}\"}}";
            var media = new Media(_libVLC, streamUrl, FromType.FromLocation, option);
            await media.Parse();
            Status = _mediaPlayer.Play(media) ? DownloadStatus.Downloading : DownloadStatus.Failed;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _mediaPlayer.Media?.Dispose();
                _mediaPlayer.Dispose();
            }

            _disposed = true;
        }

        private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            Progress = e.Position * 100;
        }

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            Status = DownloadStatus.Failed;
            Progress = 0;
        }

        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {
            Status = DownloadStatus.Finished;
            Progress = 100;
        }
    }
}