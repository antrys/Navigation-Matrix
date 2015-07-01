using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;


namespace AerSpeech
{
    /// <summary>
    /// Controls all of AER's responses. 
    /// </summary>
    /// <remarks>This needs cleaning as well. Essentially it could  use a re-design to get rid 
    ///    of the flat interface design. Maybe event driven responses? -SingularTier
    /// </remarks>
    public class NMTalk
    {
        //Member Variables
        private SpeechSynthesizer _synth;
        Random rand;
        public String LastSpelledWord;
        private String CSP; 
        
        //Default Constructor
        public NMTalk()
        {
            rand = new Random();
            LastSpelledWord = "";
            _synth = new SpeechSynthesizer();
            _synth.SetOutputToDefaultAudioDevice();
            CSP = @"Sounds\";
        }
        //Overload Constructor - Antrys
        public NMTalk(string SoundPath)
        {
            rand = new Random();
            LastSpelledWord = "";
            _synth = new SpeechSynthesizer();
            _synth.SetOutputToDefaultAudioDevice();
            CSP = SoundPath;
        }
        
        
        public string SoundPath(String GetPath)
        {
            CSP = GetPath;
            return GetPath;
            
        }
        
        public void Say(string text)
        {
            string sayme = stripFormatting(text);
            NMDebug.LogSay(sayme);
            _synth.SpeakAsyncCancelAll();
            _synth.SpeakAsync(stripFormatting(sayme));
        }

        public void SayInitializing()
        {
            this.Say("Please wait while the Navigation Matrix loads its grammar file.");
        }

        public void SayReady()
        {
            string ThePathOfSounds = CSP;
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(CSP + @"NM-On.wav");
            player.Play(); 
            
            this.Say(". . . Navigation Matrix On, listening for commands.");
        }

        public void SayBlocking(string text)
        {
            _synth.SpeakAsyncCancelAll();
            _synth.Speak(stripFormatting(text));
        }
        public void SayCurrentTime()
        {
            DateTime now = DateTime.Now;
            string time = now.ToShortTimeString();
            this.Say(Regex.Replace(time, ":", " "));
        }
        public void SayAveragePrice(EliteCommodity ec)
        {
            this.Say("The EDDB Average price for " + ec.Name + " is " + ec.AveragePrice);
        }

        public void RandomGreetings(string commanderName)
        {
            StringBuilder greeting = new StringBuilder();
            int rsp = rand.Next(0, 2);
            switch (rsp)
            {
                case 0:
                    greeting.Append("Greetings Commander");
                    break;
                case 1:
                    greeting.Append("Hello Commander");
                    break;
                default:
                    break;
            }

            if (commanderName != null)
            {
                greeting.Append(" " + commanderName);
            }

            this.Say(greeting.ToString());

        }
        public void RandomQueryAck()
        {
            int rsp = rand.Next(0, 3);
            switch(rsp)
            {
                case 0:
                    this.Say("Yes?");
                    break;
                case 1:
                    this.Say("Yes?");
                    break;
                case 2:
                    this.Say("Yes Commander?");
                    break;
                default:
                    break;
            }
        }

        public void RandomQueryEndAck()
        {
            int rsp = rand.Next(0, 2);
            switch (rsp)
            {
                case 0:
                    this.Say("You're Welcome");
                    break;
                case 1:
                    this.Say("My Pleasure");
                    break;
                default:
                    break;
            }
        }
        public void RandomNack()
        {
            int rsp = rand.Next(0, 3);
            switch (rsp)
            {
                case 0:
                    this.Say("I can't do that");
                    break;
                case 1:
                    this.Say("Sorry, impossible");
                    break;
                case 2:
                    this.Say("I cannot do that");
                    break;
            }
        }
        public void RandomAck()
        {
         /*   int rsp = rand.Next(0, 3);    //Random Acknoledgement disabled for Matrix -Antrys
            switch (rsp)
            {
                case 0:
                    this.Say("Fine");
                    break;
                case 1:
                    this.Say("Ok");
                    break;
                case 2:
                    this.Say("Sure");
                    break;
            } */
        }
        public void RandomUnknownAck()
        {
            int rsp = rand.Next(0, 2);
            switch (rsp)
            {
                case 0:
                    this.Say("I don't know.");
                    break;
                case 1:
                    this.Say("I do not know.");
                    break;
            }
        }
        public void SayIdentity()
        {
            this.Say("Navigation Matrix 1.3");
        }
        public void SayCreaterInfo()
        {
            this.Say("Further developed by Commander Antrys, Based on A.E.R. developed by Commander Tei Lin in an effort to create a more robust Speech interface than what was available at the time. Some features were programmed by Commander Win-Epic.");
        }
        public void SayCapabilities()
        {
            this.Say("I can search for commodities, search wikipedia, and even tell jokes. For more information, ask for instructions, or visit the website at aer-interface.com");
        }
        public void SayInstructions()
        {
            string instructions = @"You can customize voice commands, by editing the default dot xml file, found in the grammars folder.
                                    Here are some examples:
                                    Information on Alpha Centaree. Intel on Wolf 3 5 9. Tell me about Sol. .
                                    To get information on a station, say 'Information on', followed by the station name and the system it is in, spoken or spelled.
                                    To set your local system, say 'set current system to', followed by the system name, also either spoken or spelled. 
                                    You can also say 'that system' or 'that station' to reuse the last system or station mentioned.
                                    Once you set your local system, I can search for commodities and get distances to other systems.
                                    To browse Galnet, say 'browse galnet', 'next article', and 'read article'.
                                    To Search Wikipedia, say 'Search wikipedia for', followed by the NATO alphabet spelling of what you would like to search.
                                    To disable all command processing, say 'Stop Listening'. To Resume processing, say 'Start Listening'.
                                    ";
                                    
            this.Say(instructions);
        }

        public void SayStationAllegiance(EliteStation est)
        {
            this.Say(est.Allegiance);
        }
        public void SayStationMaxLandingPadSize(EliteStation est)
        {
            this.Say(est.MaxPadSize);
        }

        public void SayStationServices(EliteStation est)
        {
            StringBuilder stationInfo = new StringBuilder();

            stationInfo.Append("Known Available Services");
            if (est.HasCommodities)
                stationInfo.Append(", Commodities");
            if (est.HasRefuel)
                stationInfo.Append(", Refueling");
            if (est.HasRepair)
                stationInfo.Append(", Repair");
            if (est.HasRearm)
                stationInfo.Append(", Rearming");
            if (est.HasOutfitting)
                stationInfo.Append(", Outfitting");
            if (est.HasShipyard)
                stationInfo.Append(", Shipyard");
            if (est.HasBlackmarket)
                stationInfo.Append(", Black Market");

            this.Say(stationInfo.ToString());
        }
        public void SayAndSpell(string say)
        {
            this.Say(say + ", Spelled " + Spell(say));
        }
        
        public void SaySystem(EliteSystem es)
        {
            
            string str = es.Name + ". Allegiance: ";
            //Let's make those system name readouts, that have numbers, a little more comprehensable.
            str = str.Replace("1", "1 "); //Yeah I know I should loop this, but I was lazy. Copy/paste was faster.
            str = str.Replace("2", "2 ");
            str = str.Replace("3", "3 ");
            str = str.Replace("4", "4 ");
            str = str.Replace("5", "5 ");
            str = str.Replace("6", "6 ");
            str = str.Replace("7", "7 ");
            str = str.Replace("8", "8 ");
            str = str.Replace("9", "9 ");
            if(es.Allegiance.Equals(""))
            {
                str+= "Unknown";
            }
            else
            {
                str += es.Allegiance;
            }
            str += ". Controlling Faction: ";
            if (es.Faction.Equals(""))
            {
                str += "Unknown";
            }
            else
            {
                str += es.Faction;
            }
            str += ". Population of ";
            if (es.Faction.Equals(""))
            {
                str += "Unknown";
            }
            else
            {
                string FormattedPopulation = Convert.ToDecimal(es.Population).ToString("#,##0");
                //es.Population = es.Population.ToString("{N0}");
                str += FormattedPopulation;
            }
            string dashname = str.Replace("-", " dash "); 
            Say(dashname);
        }
        public void SaySetSystem(EliteSystem es)
        {
            Say("Setting Current System to " + es.Name);
        }
        public void SayFoundCommodity(EliteCommodity ec, EliteStation est)
        {
            this.Say("You can find " + ec.Name + ", at " + est.Name + ", in " + est.System.Name + ", Spelled " + Spell(est.System.Name));
        }
        public void SayFoundStation(EliteStation est)
        {
            this.Say(est.Name + ", in " + est.System.Name + ", Spelled " + Spell(est.System.Name));
        }
        public string Spell(string spellMe)
        {
            string spellName = Regex.Replace(spellMe, @"(?<=[^0-9])(?!$)", ",");
            LastSpelledWord = spellMe;
            return spellName;
        }
        public void SayUnknownLocation()
        {
            this.Say(@"I don't know where we are. Please set location using 'set current system'");
        }
        public void SayDistance(double distance)
        {
            Say(distance.ToString("0.##") + " light years");
        }
        public void SayCannotFindCommodity(EliteCommodity ec)
        {
            this.Say("No " + ec.Name + " was found within 250 light years");
        }

        public void SayStartListening()
        {
            //this.Say("On");
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(CSP + @"NM-On.wav");
            player.Play();
        }

        public void SayStopListening()
        {
            //this.Say("Off");
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(CSP + @"NM-Off.wav");
            player.Play();       
        
        }
        public void SaySelectSystem(EliteSystem es)
        {
            this.Say("Selected " + es.Name);
        }

        public void SayStationDistance(EliteStation est)
        {
            this.Say(est.Name + " is " + est.DistanceFromStar + "light seconds from" + est.System.Name);
        }
        public void SayUnknownStation()
        {
            this.Say("Unknown Station");
        }
        public void SayUnknownSystem()
        {
            this.Say("Unknown System");
        }

        private string _BlanksToUnknown(string input)
        {
            if (input == null || input.Equals(""))
            {
                return "Unknown";
            }
            else
                return input;
        }

        public double DaysSince(long timeSinceEpoch)
        {
            DateTime currentTime = DateTime.UtcNow;
            DateTime updatedAt = FromEpoch(timeSinceEpoch);
            TimeSpan elapsed = currentTime.Subtract(updatedAt);
            return elapsed.TotalDays;
        }
        public DateTime FromEpoch(long epochTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(epochTime);
        }

        public void SayStation(EliteStation est)
        {
            //This could be faster, it is also more elegant
            StringBuilder stationInfo = new StringBuilder();

            /*double daysSinceUpdate = DaysSince(est.UpdatedAt);

            if(daysSinceUpdate > 7) //EDDB stations are RARELY updated.  This info just isn't that useful - Antrys
            {
                stationInfo.Append("updated, " + daysSinceUpdate.ToString("0") + " days ago, ,");
            } */
            
            
            stationInfo.Append(@"" + _BlanksToUnknown(est.Name) +
                ", The station is ");  
            
                if (est.StarportType == "Coriolis")
            {
              stationInfo.Append("a ");
            }
            else
            {
                stationInfo.Append("an ");
            }

                stationInfo.Append(_BlanksToUnknown(est.StarportType) + " situated " + est.DistanceFromStar + " light seconds from the nav point" +
                " and is in a current state of " + _BlanksToUnknown(est.State) +
                ". They are aligned with the " + _BlanksToUnknown(est.Faction) + " Faction" +
                ". Their Allegiance is to the " + _BlanksToUnknown(est.Allegiance) +
                " and the government's structure is a " +  _BlanksToUnknown(est.Government));

            //stationInfo.Append(". Its distance from the star is " + est.DistanceFromStar + " light seconds. ");

            stationInfo.Append(". They offer ");
            switch(est.MaxPadSize)
            {
                case ("S"):
                    stationInfo.Append("Small sized landing pads");
                    break;
                case ("M"):
                    stationInfo.Append("Medium sized landing pads");
                    break;
                case ("L"):
                    stationInfo.Append("Large sized landing pads");
                    break;
                default:
                    stationInfo.Append("Unknown Sized landing pads");
                    break;
            }
            //need to make this build an independent string, extract the last word and then substitute "and lastword" for the last word. -antrys
            stationInfo.Append(" with");
            if (est.HasCommodities)
                stationInfo.Append(" Commodities,");
            
            if (est.HasRefuel)
                stationInfo.Append(" Refueling,");
            
            if (est.HasRepair)
                stationInfo.Append(" Repair,");
            
            if (est.HasRearm)
                stationInfo.Append(" Rearming,");
            
            if (est.HasOutfitting)
                stationInfo.Append(" Outfitting,");
            if (est.HasShipyard)
                stationInfo.Append(" Shipyard,");
            if (est.HasBlackmarket)
                stationInfo.Append(" and a Black Market");

          

            this.Say(stationInfo.ToString());
        }

        private string stripFormatting(string input)
        {
            string output;

            output = Regex.Replace(input, "<[^<]+?>", "");
            output = Regex.Replace(output, "</[^<]+?>", "");
            output = Regex.Replace(output, "<br />", "");
            output = Regex.Replace(output, "<br>", "");
            output = Regex.Replace(output, "<b>", "");
            output = Regex.Replace(output, "&#[0-9]+;", "");

            return output;
        }

    }

}
