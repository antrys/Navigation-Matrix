using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AerSpeech;

namespace AerVAPlugin
{
    public class AerVoiceAttackPlugin
    {
        static NMInput _AerInput;
        static NMHandler _AerHandler;
        static bool _RunWorker;


        public static string VA_DisplayName()
        {
            return "Navigation Matrix";
        }

        public static string VA_DisplayInfo()
        {
            return "By CMDR Antrys. Based on A.E.R. by CMDR Tei Lin (SingularTier)";
        }

        public static Guid VA_Id()
        {
            return new Guid("{ec7e5005-591c-486f-9b0c-07be23285ba6}");;
        }

        public static void VA_Init1(ref Dictionary<string, object> state, ref Dictionary<string, Int16?> conditions, ref Dictionary<string, string> textValues, ref Dictionary<string, object> extendedValues)
        {
            _RunWorker = true;
            Thread exeThread = new Thread(ExecuteThread);
            exeThread.Start();
        }

        public static void VA_Exit1(ref Dictionary<string, object> state)
        {
            _RunWorker = false;
        }

        public static void VA_Invoke1(String context, ref Dictionary<string, object> state, ref Dictionary<string, Int16?> conditions, ref Dictionary<string, string> textValues, ref Dictionary<string, object> extendedValues)
        {
           
        }

        public static void ExecuteThread()
        {
            NMTalk talk = new NMTalk(AppDomain.CurrentDomain.BaseDirectory + @"\Apps\Aer\Sounds\");
            NMDB data = new NMDB(AppDomain.CurrentDomain.BaseDirectory + @"\Apps\AER\json\");
            Personality person = new Personality(talk, data);
            _AerHandler = new NMHandler(data, person);
            _AerInput = new NMInput(AppDomain.CurrentDomain.BaseDirectory + @"\Apps\AER\Grammars\", person.GrammarLoaded_Handler);
            //Settings pluginp = new Settings(AppDomain.CurrentDomain.BaseDirectory + @"\Apps\AER\");
            while(_RunWorker)
            {
                if (_AerInput.NewInput)
                {
                    _AerInput.NewInput = false;
                    _AerHandler.InputHandler(_AerInput.LastResult);
                }
            }
        }
    }
}

