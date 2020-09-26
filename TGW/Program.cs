using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TGW
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test code
            Settings controller = new Settings(); //Create class instance
            controller.ReadFile(Directory.GetCurrentDirectory() + @"\Base_Config.txt"); //Read file
            DateTime asdTime = controller.GetParameterValue<DateTime>("resultStartTime"); //Creating a variable and setting it to a value of a chosen parameter
            Console.WriteLine(asdTime); //print the variables set value in the console
            controller.SetParameterValue("ordersPerHour", asdTime); //Set parameters value to a different one
            Console.WriteLine(controller.GetParameterValue<DateTime>("resultStartTime").ToString("HH:mm:ss")); //Getting a parameters value

            //Feature in textual form:
            //You could create an Event that triggers each time a new parameter is added to the dictionary of parameters.
            //That event would be able to change the values of other parameters that are dependant to it.
            //However that would require the user to set those dependacies while loading a config file.
        }
    }

    class Settings
    {
        Dictionary<string, string> settings = new Dictionary<string, string>();

        public void ReadFile(string filePath)
        {
            if (File.ReadAllLines(filePath) == null)
                throw new ArgumentException($"{filePath} is an invalid file path");

            string[] textLines = File.ReadAllLines(filePath);

            foreach (string item in textLines)
            {
                Match match = Regex.Match(item, @"([a-zA-Z]+):(\t+)(.+)\t");
                if (match.Success)
                {
                    Match keyMatch = Regex.Match(match.ToString(), @"([a-zA-Z]+)");
                    Match valueMatch = Regex.Match(match.ToString(), @"(\t+)(.+)\t");

                    settings[keyMatch.ToString()] = valueMatch.ToString().Trim();
                }
            }
        }
        public T GetParameterValue<T>(string parameterName)
        {
            return (T)Convert.ChangeType(GetParamaterValueAsString(parameterName), typeof(T));
        }

        public void ClearAllSettings()
        {
            settings.Clear();
        }

        private string GetParamaterValueAsString(string parameterName)
        {
            if (settings.ContainsKey(parameterName))
                return settings[parameterName];
            else
                throw new ArgumentException($"{parameterName} name does not exist");
        }

        public void SetParameterValue<T>(string parameterName, T parameterValue)
        {
            if (settings.ContainsKey(parameterName))
            {
                //If given parameter value can't be converted from string back to it's original type let's not parse it

                //This solution will only work with standart .NET types (int, string...)

                try
                {
                    Convert.ChangeType(parameterValue.ToString(), typeof(T));
                }
                catch
                {
                    throw new InvalidDataException($"Cannot convert {parameterName} to string");
                }
                settings[parameterName] = parameterValue.ToString();
            }
            else
                throw new ArgumentException($"{parameterName} name does not exist");
        }

        public List<string> GetAllParameterValues()
        {
            List<string> allValues = new List<string>();

            foreach (var item in settings)
            {
                allValues.Add(item.Value);
            }

            return allValues;
        }
    }
}
