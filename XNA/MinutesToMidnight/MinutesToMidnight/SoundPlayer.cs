
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace MinutesToMidnight
{
    class SoundPlayer
    {
        
        static private SoundPlayer instance;
        private int talk_counter_max;
        private int current_talk_counter;
        private int walk_counter_max;
        private int current_walk_counter;
        private int max_speaking_length;
        private int speaking_counter;
        static public SoundPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SoundPlayer();
                }
                return instance;
            }
        }


        private SoundEffect bgMusic;
        private SoundEffect talking;
        private SoundEffect walking;
        private SoundEffectInstance walk_effect;
        private SoundEffectInstance talk_effect;
        private SoundEffectInstance currentMusic;

        public SoundPlayer()
        {
            max_speaking_length = 100;
            talk_counter_max = 2;
            current_talk_counter = 0;
            walk_counter_max = 30;
            current_walk_counter = 0;
            speaking_counter = 0;
        }

        public void LoadContent(ContentManager cmanager)
        {
            bgMusic = cmanager.Load<SoundEffect>("audio//Eureka");
            currentMusic = bgMusic.CreateInstance();
            currentMusic.IsLooped = true;
            bgMusic.Play(0.5f, 0.0f, 0.0f);

            talking = cmanager.Load<SoundEffect>("audio//speaking");
            talk_effect = talking.CreateInstance();
            talk_effect.Volume = 0.2f;
            talk_effect.IsLooped = false;

            walking = cmanager.Load<SoundEffect>("audio//walking");
            walk_effect = walking.CreateInstance();
            walk_effect.Volume = 0.5f;
            walk_effect.IsLooped = false;
        }

        public void StartTalking()
        {
            Random r = new Random();
            if (speaking_counter < max_speaking_length)
            {
                if (talk_effect.State != SoundState.Playing)
                {
                    if (talk_counter_max <= current_talk_counter)
                    {
                        talk_effect.Play();
                        // talking.Play(0.5f, (float)r.NextDouble(), 0f);
                        float ptch = (float)(r.NextDouble() - 0.5);
                        talk_effect.Pitch = ptch;
                        current_talk_counter = 0;
                    }
                    else
                    {
                        current_talk_counter++;
                    }
                }
                speaking_counter++;
            }
        }

        public void StopTalking()
        {
            talk_effect.Stop();
            speaking_counter = 0;
        }

        public void playWalkSound()
        {
            if (walk_effect.State != SoundState.Playing)
            {
                Random r = new Random();
                if (walk_counter_max <= current_walk_counter)
                {
                    walk_effect.Play();
                    // talking.Play(0.5f, (float)r.NextDouble(), 0f);
                    float ptch = (float)(r.NextDouble() * 0.5 - 0.25);
                    walk_effect.Pitch = ptch;
                    current_walk_counter = 0;
                }
                else
                {
                    current_walk_counter++;
                }
            }
        }

        public void stopWalkSound()
        {
            talk_effect.Stop();
            speaking_counter = 0;
        }
    }
}