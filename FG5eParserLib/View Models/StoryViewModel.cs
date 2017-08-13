﻿using FG5eParserLib.Utility;
using FG5eParserModels.DM_Modules;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FG5eParserLib.View_Models
{
    public class StoryViewModel : INotifyPropertyChanged
    {
        public string storyTextPath { get; set; }
        public ObservableCollection<string> EntryListItems { get; set; }
        private bool overrightFlg = false; // WARNING! Enabling this will OVERRIGHT any details present in the save to file
        private string tableTextPath = string.Empty;
        private string locationCommandText = string.Empty;
        private string NPCEntries = string.Empty;
        private string currentParameter = string.Empty;

        // Lists and Objects
        private Story StoryObj { get; set; }
        public Story StoryObject
        {
            get
            {
                return StoryObj;
            }
            set
            {
                StoryObj = value;
                OnPropertyChanged("StoryObject");
            }
        }
        private string showDataTableFlg { get; set; }
        public bool _showDataTableFlg
        {
            get
            {
                return Convert.ToBoolean(showDataTableFlg);
            }
            set
            {
                showDataTableFlg = value.ToString();
                OnPropertyChanged("_showDataTableFlg");
            }
        }

        public ObservableCollection<string> EntryNames { get; set; }

        // Relay Commands
        public RelayCommand AddStoryEntry { get; set; } // Save Button
        public RelayCommand SelectTableData { get; set; }
        public RelayCommand AddSelectedTableItem { get; set; }
        public RelayCommand LoadAllEntries { get; set; }
        public RelayCommand AddNewStoryBlock { get; set; }
        public RelayCommand AddNewStoryEntry { get; set; }
        public RelayCommand DisplayEntriesList { get; set; }
        public RelayCommand AddSelectedItem { get; set; }

        // Output
        private string Output { get; set; }
        public string _Output
        {
            get
            {
                return Output;
            }
            set
            {
                Output = value;
                OnPropertyChanged("_Output");
            }
        }

        // Constructor
        public StoryViewModel()
        {
            _showDataTableFlg = true;
            EntryListItems = new ObservableCollection<string>() { "TEST" };

            // Class Object
            StoryObj = new Story();

            // Relay Commands Init
            AddStoryEntry = new RelayCommand(addStoryEntry);
            LoadAllEntries = new RelayCommand(loadAllEntries);
            AddNewStoryBlock = new RelayCommand(addNewStoryBlock);
            AddNewStoryEntry = new RelayCommand(addNewStoryEntry);
            DisplayEntriesList = new RelayCommand(displayEntriesList);
            AddSelectedItem = new RelayCommand(addSelectedItem);
        }

        private void addSelectedItem(object obj)
        {
            if (obj != null)
            {
                StringBuilder _sb = new StringBuilder();
                _sb.Append(_Output);

                if (!string.IsNullOrEmpty(_Output))
                {
                    _sb.Append(Environment.NewLine);
                }

                // NPCS
                if (currentParameter == "npc")
                {
                    _sb.Append(string.Format("#zal;NPC;*;NPC:{0};{0}", obj.ToString()));
                }

                _Output = _sb.ToString();
            }
        }

        private void displayEntriesList(object obj)
        {
            Readers _reader = new Readers();

            // NPC LIST
            if (obj.ToString().ToLower() == "npc")
            {
                if (string.IsNullOrEmpty(NPCEntries))
                {
                    OpenFileDialog _ofd = new OpenFileDialog() { Title = "Please select a file that contains NPC's" };
                    if (_ofd.ShowDialog() == true)
                    {
                        NPCEntries = _ofd.FileName;
                    }
                }

                if (!string.IsNullOrEmpty(NPCEntries))
                {
                    EntryListItems.Clear();
                    foreach (string item in _reader.getNPCList(NPCEntries))
                    {
                        if (item != null)
                        {
                            EntryListItems.Add(item);
                        }
                    }

                    currentParameter = obj.ToString().ToLower();
                }
            }            
        }

        private void addNewStoryEntry(object obj)
        {
            StringBuilder _sb = new StringBuilder();
            _sb.Append(_Output);

            if (!string.IsNullOrEmpty(_Output))
            {
                _sb.Append(Environment.NewLine);
            }
            _sb.Append("##;<Story Entry Header goes here>");
            _sb.Append(Environment.NewLine);
            _sb.Append("<Story Description>");

            _Output = _sb.ToString();
        }

        private void addNewStoryBlock(object obj)
        {
            StringBuilder _sb = new StringBuilder();
            _sb.Append(_Output);

            if (!string.IsNullOrEmpty(_Output))
            {
                _sb.Append(Environment.NewLine);
            }
            _sb.Append("#@;<Major Story Category goes here>");
            _sb.Append(Environment.NewLine);
            _sb.Append("##;<Story Entry Header goes here>");
            _sb.Append(Environment.NewLine);
            _sb.Append("<Story Description>");

            _Output = _sb.ToString();
        }

        private void loadAllEntries(object obj)
        {
            if (!string.IsNullOrEmpty(storyTextPath))
            {
                StringBuilder _sb = new StringBuilder();
                overrightFlg = true;
                var _lines = File.ReadLines(storyTextPath);

                foreach (string item in _lines)
                {
                    _sb.Append(item);
                    _sb.Append(Environment.NewLine);
                }

                _Output = _sb.ToString();
            }
        }

        private void addStoryEntry(object obj)
        {
            if (!string.IsNullOrEmpty(storyTextPath))
            {
                if (overrightFlg)
                {
                    // WARNING! This will overwrite details in the save to file.
                    TextWriter tsw = new StreamWriter(storyTextPath, false);
                    tsw.WriteLine(_Output);
                    tsw.Close();
                }
                else
                {
                    TextWriter tsw = new StreamWriter(storyTextPath, true);
                    tsw.WriteLine(_Output);
                    tsw.Close();
                    _Output = string.Empty;
                }
            }
        }

        #region PROPERTY CHANGED EVENT
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}