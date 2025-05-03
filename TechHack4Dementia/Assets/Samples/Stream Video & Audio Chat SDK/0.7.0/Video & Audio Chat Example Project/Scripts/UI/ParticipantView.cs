﻿using System;
using StreamVideo.Core;
using StreamVideo.Core.StatefulModels;
using StreamVideo.Core.StatefulModels.Tracks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreamVideo.ExampleProject.UI
{
    public class ParticipantView : MonoBehaviour
    {
        public IStreamVideoCallParticipant Participant { get; private set; }

        public void Init(IStreamVideoCallParticipant participant)
        {
            if (Participant != null)
            {
                throw new NotSupportedException("reusing participant view for new participant is not supported yet");
            }

            Participant = participant ?? throw new ArgumentNullException(nameof(participant));

            foreach (var track in Participant.GetTracks())
            {
                OnParticipantTrackAdded(Participant, track);
            }
            
            Participant.TrackAdded += OnParticipantTrackAdded;

            _name.text = Participant.Name;
        }

        public void UpdateIsDominantSpeaker(bool isDominantSpeaker)
        {
            var frameColor = isDominantSpeaker ? _dominantSpeakerFrameColor : _defaultSpeakerFrameColor;
            _videoFrame.color = frameColor;
        }

        /// <summary>
        /// Call this for local participant only. We will not receive the `Participant.TrackAdded` event for the local participant.
        /// So in order to show the stream from a local camera we hook it up separately
        /// </summary>
        public void SetLocalCameraSource(WebCamTexture localWebCamTexture)
        {
            if (localWebCamTexture == null)
            {
                _video.texture = null;
                return;
            }
            
            _video.texture = localWebCamTexture;
        }
        
        // Called by Unity Engine
        protected void Awake()
        {
            _videoRectTransform = _video.GetComponent<RectTransform>();
        }

        // Called by Unity Engine
        protected void Update()
        {
            var rect = _videoRectTransform.rect;
            var videoRenderedSize = new Vector2(rect.width, rect.height);
            if (videoRenderedSize != _lastVideoRenderedSize)
            {
                _lastVideoRenderedSize = videoRenderedSize;
                var videoResolution = new VideoResolution((int)videoRenderedSize.x, (int)videoRenderedSize.y);
                
                // To optimize bandwidth we always request the video resolution that matches what we're actually rendering
                Participant.UpdateRequestedVideoResolution(videoResolution);
                Debug.Log($"Rendered resolution changed for participant `{Participant.UserId}`. Requested video resolution update to: {videoResolution}");
            }
        }

        // Called by Unity Engine
        protected void OnDestroy()
        {
            if (Participant != null)
            {
                Participant.TrackAdded -= OnParticipantTrackAdded;
            }
        }

        [SerializeField]
        private TMP_Text _name;

        [SerializeField]
        private RawImage _video;
        
        [SerializeField]
        private RawImage _videoFrame;
        
        [SerializeField]
        private Color32 _dominantSpeakerFrameColor;
        
        [SerializeField]
        private Color32 _defaultSpeakerFrameColor;

        private AudioSource _audioSource;
        private RectTransform _videoRectTransform;
        private Vector2 _lastVideoRenderedSize;

        private void OnParticipantTrackAdded(IStreamVideoCallParticipant participant, IStreamTrack track)
        {
            Debug.Log($"Track received from `{participant.UserId}`, type: {track.GetType()}");
            switch (track)
            {
                case StreamAudioTrack streamAudioTrack:
                    if (_audioSource != null)
                    {
                        //StreamTodo: debug why we're sometimes getting multiple audio tracks despite publishing a single audio track
                        //StreamTodo: handle multiple audio tracks. This is a valid use case
                        Debug.LogError("Multiple audio track!");
                        return;
                    }

                    _audioSource = gameObject.AddComponent<AudioSource>();
                    streamAudioTrack.SetAudioSourceTarget(_audioSource);
                    break;

                case StreamVideoTrack streamVideoTrack:
                    streamVideoTrack.SetRenderTarget(_video);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(track));
            }
        }
    }
}