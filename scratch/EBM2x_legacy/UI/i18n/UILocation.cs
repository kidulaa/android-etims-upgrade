using EBM2x.Datafile.env;
using EBM2x.Models.config;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace EBM2x.UI.i18n
{
    public class UILocation
    {
        private static UILocation _UILocation = null;

        public string Location { get; set; }

        private JObject LocationJson { get; set; }

        private UILocation()
        {
            EnvPosSetup envPosSetup = EnvPosSetupService.LoadEnvPosSetup();
            Location = envPosSetup.LocationType;
            GetLocationJson(Location + ".json"); ;
        }

        public void SetLocation(string location)
        {
            if (!_UILocation.Location.Equals(location))
            {
                _UILocation.Location = location;
                _UILocation.GetLocationJson(_UILocation.Location + ".json");
            }
        }

        public string GetLocationText(string text)
        {
            if(LocationJson != null)
            {
                //JToken jtoken = LocationJson.GetValue(text);
                string jtoken = (string)LocationJson[text];//.GetValue(text);
                if (string.IsNullOrEmpty(jtoken)) return text;
                else return jtoken;
            }
            return text;
        }

        private void GetLocationJson(string locationName)
        {
            try
            {
                var type = typeof(UILocation).GetTypeInfo();
                var assembly = type.Assembly;
                Stream stream = assembly.GetManifestResourceStream($"EBM2x.UI.i18n.{locationName}");
                if (stream == null)
                {
                    LocationJson = null;
                }
                else
                {
                    string json = new StreamReader(stream).ReadToEnd();
                    LocationJson = JObject.Parse(json);
                }
            }
            catch (Exception e)
            {
                LocationJson = null;
            }
        }

        public static UILocation Instance()
        {
            if (_UILocation == null) _UILocation = new UILocation();

            return _UILocation;
        }
    }
}
