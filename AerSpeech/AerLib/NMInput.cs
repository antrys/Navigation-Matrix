﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using AerSpeech;

namespace AerSpeech
{
    /// <summary>
    /// Simple class that handles speech input.
    /// </summary>
    public class NMInput
    {
        protected SpeechRecognitionEngine RecognitionEngine;
        public RecognitionResult LastResult;
        public bool NewInput;

        private int _ResponseSpeed;
        private string _CultureInfo;

        //I know that GrammarLoaded is bad, but there's no good way to get the delegate surfaced out of AerInput in to AerTalk yet.
        // This could be solved with a service registry, but I haven't thought that through yet.
        // It could also be solved by using RecognitionEngine.LoadGrammar() instead of the Async version again, but
        // I rather like the async version.
        

        public NMInput(string pathToGrammar = @"Grammars\", EventHandler<LoadGrammarCompletedEventArgs> GrammarLoaded = null)
        {
            LoadSettings();
            try
            { 
                RecognitionEngine = new SpeechRecognitionEngine(new CultureInfo(_CultureInfo));
            }
            catch (ArgumentException e)
            {
                NMDebug.LogError("Could not load speech recognizer with the current Culture settings (" + _CultureInfo + "), using default");
                RecognitionEngine = new SpeechRecognitionEngine();
            }

            RecognitionEngine.SetInputToDefaultAudioDevice();
            LoadGrammar(pathToGrammar, GrammarLoaded);
            RecognitionEngine.SpeechRecognized += this.SpeechRecognized_Handler;


            RecognitionEngine.UpdateRecognizerSetting("ResponseSpeed", _ResponseSpeed);
            NewInput = false;
            RecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Loads the grammar in to the recognition engine.
        /// </summary>
        protected virtual void LoadGrammar(string pathToGrammar, EventHandler<LoadGrammarCompletedEventArgs> GrammarLoaded)
        {

            NMDebug.Log("Loading Grammar...");
            Grammar grammar = new Grammar(pathToGrammar + @"Default.xml");
            RecognitionEngine.LoadGrammarCompleted += GrammarLoaded;
            RecognitionEngine.LoadGrammarAsync(grammar);
        }

        /// <summary>
        /// Pulls data out of the settings file
        /// </summary>
        protected virtual void LoadSettings()
        {
            string rspSpeed = Settings.Load(Settings.RESPONSE_SPEED_NODE, "750");
            if (rspSpeed == null)
            {
                NMDebug.LogError("Could not load ResponseSpeed from settings file!");
                rspSpeed = "750";
            }

            try
            {
                _ResponseSpeed = int.Parse(rspSpeed);
            }
            catch (Exception e)
            {
                NMDebug.LogError("Invalid ResponseSpeed in settings file!");
                NMDebug.LogException(e);
                _ResponseSpeed = 750; //Default if it breaks.
            }

            _CultureInfo = Settings.Load(Settings.CULTURE_NODE, "en-US");
        }
        
        /// <summary>
        /// Handles the SpeechRecognized event from the Recognition Engine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SpeechRecognized_Handler(object sender, SpeechRecognizedEventArgs e)
        {
            string text = e.Result.Text;
            SemanticValue semantics = e.Result.Semantics;

            NewInput = true;
            LastResult = e.Result;
        }

    }
}
