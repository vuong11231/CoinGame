#if SPINE

using Spine.Unity;
using System;
using System.Collections.Generic;
using Spine;
using UnityEngine;

namespace SteveRogers
{
    public class SpineTimescaleDuration
    {
        private float timescale = 1;
        private float originalDuration = 1;
        private float duration;

        public SpineTimescaleDuration(float originalDuration_s)
        {
            originalDuration = originalDuration_s;
            CalcDuration();
        }

        public SpineTimescaleDuration(SpineTimescaleDuration clone)
        {
            this.timescale = clone.timescale;
            this.originalDuration = clone.originalDuration;
            this.duration = clone.duration;
        }

        private void CalcDuration()
        {
            if (timescale == 0)
                duration = float.MaxValue;
            else
                duration = originalDuration / timescale;
        }

        public float Duration { get { return duration; } }

        public float Timescale
        {
            get { return timescale; }

            set
            {
                if (value < 0)
                    value = 0;

                timescale = value;
                CalcDuration();
            }
        }
    }

    public class SpineController
    {
        #region Variables / Core / Contructor (Private)

        // Element

        private SkeletonGraphic skeleton_s;                
        private SPINE_TAG tag = SPINE_TAG.NONE;

        private bool addedSetTimescaleFromDataEvent = false;
        private Dictionary<string, SpineTimescaleDuration> spineTimescaleDurations = null;

        private bool addedSetAnimationEndFromDataEvent = false;
        private Dictionary<string, float> animationEndTimes = null;

        // Contructor

        public SpineController(SkeletonGraphic skeleton, SPINE_TAG tag = SPINE_TAG.NONE)
        {
            if (skeleton == null)
            {
                throw new System.Exception("SpineController" + "Skeleton is null!");
            }

            // Skeleton

            this.skeleton_s = skeleton;

            // Set tags

            this.tag = tag;

            // Manager

            spineControllers.Add(this);
        }

        // Manager

        private static List<SpineController> spineControllers = new List<SpineController>();

        #endregion

        #region Manager core (Public)

        public static void PauseAll(SPINE_TAG tag)
        {
            foreach (SpineController i in spineControllers)
                if (i.HasTag(tag))
                    i.Pause();
        }

        public static void ResumeAll(SPINE_TAG tag)
        {
            foreach (SpineController i in spineControllers)
                if (i.HasTag(tag))
                    i.Resume();
        }

        public static void Remove(SpineController controller) // Main 
        {
            if (spineControllers.Contains(controller))
                spineControllers.Remove(controller);
        }

        #endregion

        #region Core (Private)

        private void SetTimescaleFromData(TrackEntry track)
        {
            track.TimeScale = spineTimescaleDurations[track.Animation.Name].Timescale;
        }

        private void SetAnimatinoEndFromData(TrackEntry track)
        {
            track.AnimationEnd = animationEndTimes[track.Animation.Name];
        }

        private TrackEntry CurrentTrack
        {
            get
            {
                return skeleton_s.AnimationState?.GetCurrent(0);
            }
        }

        #endregion

        #region Utils (Public)

        public bool HasTag(SPINE_TAG tag)
        {
            return this.tag == tag;
        }

        public bool NameIsNotExist(string name)
        {
            return skeleton_s.SkeletonData.Animations.Find(i => i.Name.Equals(name)) == null;
        }

        public string Current
        {
            get
            {
                var t = CurrentTrack;
                return t?.Animation.Name;
            }
        }

        public string Next
        {
            get
            {
                var t = CurrentTrack;
                return t?.Next?.Animation.Name;
            }
        }

        public bool IsComplete
        {
            get
            {
                var t = CurrentTrack;

                if (t == null)
                    return false;
                else
                    return t.IsComplete;
            }
        }

        public void ClearTrack()
        {
            if (skeleton_s.AnimationState == null)
            {
                Debug.LogError("Skeleton.AnimationState is null, so cant clear track of spine: " + skeleton_s.name);
                return;
            }

            skeleton_s.AnimationState.ClearTrack(0);
        }

        public Dictionary<string, SpineTimescaleDuration> GetDefaultTimescaleDurations()
        {
            if (skeleton_s.SkeletonData == null)
            {
                Debug.LogError(this + "GetDefaultTimescaleDurations - skeleton_s.SkeletonData null at go: " + skeleton_s.gameObject.RootNameGo());
                return null;
            }

            Dictionary<string, SpineTimescaleDuration> data = new Dictionary<string, SpineTimescaleDuration>();

            foreach (var i in skeleton_s.SkeletonData.Animations)
                data.Add(i.Name, new SpineTimescaleDuration(i.Duration));

            return data;
        }

        /// <summary>
        /// must be full of animations
        /// set = null to remove event
        /// </summary>
        public Dictionary<string, SpineTimescaleDuration> TimescaleOnStartData
        {
            set
            {
                spineTimescaleDurations = value;

                if (value == null)
                {
                    if (addedSetTimescaleFromDataEvent)
                    {
                        addedSetTimescaleFromDataEvent = false;
                        skeleton_s.AnimationState.Start -= SetTimescaleFromData;
                    }
                    //else
                    //    Debug.LogWarning("You have not register the event TimescaleOnStartData yet!");
                }
                else
                {
                    if (!addedSetTimescaleFromDataEvent)
                    {
                        addedSetTimescaleFromDataEvent = true;
                        skeleton_s.AnimationState.Start += SetTimescaleFromData;
                    }
                    //else
                    //    Debug.LogWarning("You're trying to register the event TimescaleOnStartData twice!");
                }
            }
        }

        /// <summary>
        /// NOT need to be full of animations
        /// set = null to remove event
        /// </summary>
        public Dictionary<string, float> AnimationEndData
        {
            set
            {
                animationEndTimes = value;

                if (value == null) // remove
                {
                    if (addedSetAnimationEndFromDataEvent)
                    {
                        addedSetAnimationEndFromDataEvent = false;
                        skeleton_s.AnimationState.Start -= SetAnimatinoEndFromData;
                    }
                    else
                        Debug.LogWarning("You have not register the event AnimationEndData yet!");
                }
                else // apply
                {
                    if (!addedSetAnimationEndFromDataEvent)
                    {
                        // fill for full of animations

                        foreach (var i in Animations)
                        {
                            if (!animationEndTimes.ContainsKey(i.Name)) // set default
                                animationEndTimes.Add(i.Name, i.Duration);
                        }
                        
                        // apply

                        addedSetAnimationEndFromDataEvent = true;
                        skeleton_s.AnimationState.Start += SetAnimatinoEndFromData;
                    }
                    else
                        Debug.LogWarning("You're trying to register the event AnimationEndData twice!");
                }
            }
        }

        public void UpdateTimescaleInstant()
        {
            TrackEntry cur = CurrentTrack;

            if (spineTimescaleDurations != null && cur != null)
                SetTimescaleFromData(cur);
        }

        public float Duration(string animation_s)
        {
            if (spineTimescaleDurations != null)
                return spineTimescaleDurations[animation_s].Duration;
            else
                return skeleton_s.SkeletonData.Animations.Find(i => i.Name.Equals(animation_s)).Duration;
        }

        public ExposedList<Spine.Animation> Animations
        {
            get { return skeleton_s.SkeletonData.Animations; }
        }

        public float CurrentTimescale
        {
            get
            {
                TrackEntry c = CurrentTrack;
                return c == null ? -1 : c.TimeScale;
            }
        }

        public float SkeletonTimescale
        {
            get
            {
                return skeleton_s.timeScale;
            }

            set { skeleton_s.timeScale = value; }
        }

        public bool IsPausing { get { return skeleton_s.freeze; } }

        public void Pause()
        {
            skeleton_s.freeze = true;
        }

        public void Resume()
        {
            skeleton_s.freeze = false;
        }

        public void Remove()
        {
            Remove(this);
        }

        public GameObject Go
        {
            get
            {
                return skeleton_s.gameObject;
            }
        }

        public void SetToFirstFrameAndTrackTimeScaleZero(bool set)
        {
            var c = CurrentTrack;

            if (c == null)
                return;

            if (set)
            {
                c.TrackTime = 0;
                c.TimeScale = 0;
            }
            else
            {
                c.TimeScale = 1;
            }
        }

        public void SetSetupPose()
        {
            skeleton_s.Skeleton.SetSlotAttachmentsToSetupPose();
        }

        public float Alpha
        {
            set {
                skeleton_s.SetAlpha(value);
            }
        }

        public void SetPoseAtPercent(float percent)
        {
            var c = CurrentTrack;
            
            if (c == null)
                return;
            
            c.TrackTime = c.AnimationEnd * Mathf.Clamp01(percent);
        }
        
        public float CurrentAnimationPercent
        {
            get
            {
                var c = CurrentTrack;

                if (c == null || c.AnimationEnd == 0)
                    return -1;

                return c.TrackTime / c.AnimationEnd;
            }
        }

        #endregion

        #region Add animation & action (Public)

        public void Add(string name_s)
        {
            if (name_s.Equals(Next))
            {
                skeleton_s.AnimationState.GetCurrent(0).Next.Loop = false;
                return;
            }

            skeleton_s.AnimationState.AddAnimation(0, name_s, false, 0);
        }

        public void AddLoop(string name_s)
        {
            if (name_s.Equals(Next))
            {
                skeleton_s.AnimationState.GetCurrent(0).Next.Loop = true;
                return;
            }

            skeleton_s.AnimationState.AddAnimation(0, name_s, true, 0);
        }

        public void AddStartAction(Action<Spine.TrackEntry> act_s)
        {
            skeleton_s.AnimationState.Start += (track) =>
            {
                act_s(track);
            };
        }

        public void AddCompleteAction(Action<Spine.TrackEntry> act_s)
        {
            skeleton_s.AnimationState.Complete += (track) =>
            {
                act_s(track);
            };
        }

        public void AddCompleteActionCurrentTrack(Action<Spine.TrackEntry> act_s)
        {
            var t = CurrentTrack;

            if (t != null)
            {
                t.Complete += (track) =>
                    {
                        act_s(track);
                    };
            }
        }

        #endregion

        #region Play (Public)

        public bool Play(string name_s, bool resetIfCurrent = false)
        {
            if (skeleton_s.AnimationState == null)
            {
                Debug.LogError("Cant play cus the go maybe not active in Hierachy! " + skeleton_s.name);
                return false;
            }
            var change = !name_s.Equals(Current);
             
            if (change || resetIfCurrent)
            {
                skeleton_s.AnimationState.SetAnimation(0, name_s, false);
                skeleton_s.Skeleton.SetSlotAttachmentsToSetupPose();
            }

            return change;
        }

        public void PlayWithCompleteAction(string name_s, Action done_s, bool resetIfCurrent = false)
        {
            if (skeleton_s.AnimationState == null)
            {
                Debug.LogError("Cant play cuz the go maybe not active in Hierachy! " + skeleton_s.name);
                return;
            }

            if (!name_s.Equals(Current) || resetIfCurrent)
                skeleton_s.AnimationState.SetAnimation(0, name_s, false)
                    .Complete += (track) =>
                    {
                        done_s();
                    };
        }

        public bool PlayLoop(string name_s, bool resetIfCurrent = false)
        {
            if (skeleton_s.AnimationState == null)
            {
                Debug.LogError("Cant play cus the go maybe not active in Hierachy! " + skeleton_s.name);
                return false;
            }

            var change = !name_s.Equals(Current);

            if (change || resetIfCurrent)
                skeleton_s.AnimationState.SetAnimation(0, name_s, true);

            return change;
        }

        public void PlayLoopWithCompleteAction(string name_s, Action done_s, bool resetIfCurrent = false)
        {
            if (skeleton_s.AnimationState == null)
            {
                Debug.LogError("Cant play cus the go maybe not active in Hierachy! " + skeleton_s.name);
                return;
            }

            if (!name_s.Equals(Current) || resetIfCurrent)
                skeleton_s.AnimationState.SetAnimation(0, name_s, true)
                    .Complete += (track) =>
                    {
                        done_s();
                    };
        }

        //public void PlayOneShot(string name_of_shot_not_current_s)
        //{
        //    if (skeleton_s.AnimationState == null)
        //    {
        //        Debug.LogError("Cant play cus the go maybe not active in Hierachy! " + skeleton_s.name);
        //        return;
        //    }

        //    string current = Current;

        //    if (name_of_shot_not_current_s.Equals(current) || !string.IsNullOrEmpty(Next))
        //        return;

        //    bool currentIsLoop = false;

        //    if (!string.IsNullOrEmpty(current))
        //        currentIsLoop = skeleton_s.AnimationState.GetCurrent(0).Loop;

        //    skeleton_s.AnimationState.SetAnimation(0, name_of_shot_not_current_s, false);

        //    if (!string.IsNullOrEmpty(current))
        //        skeleton_s.AnimationState.AddAnimation(0, current, currentIsLoop, 0);

        //}

        #endregion
    }
}

#endif