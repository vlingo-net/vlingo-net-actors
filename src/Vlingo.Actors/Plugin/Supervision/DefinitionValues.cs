﻿using System.Collections.Generic;

namespace Vlingo.Actors.Plugin.Supervision
{
    internal class DefinitionValues
    {
        private const string STAGE = "stage=";
        private const string NAME = "name=";
        private const string PROTOCOL = "protocol=";
        private const string SUPERVISOR = "supervisor=";
  
        internal string Name { get; private set; }
        internal string Protocol { get; private set; }
        internal string StageName { get; private set; }
        internal string Supervisor { get; private set; }

        internal static IList<DefinitionValues> AllDefinitionValues(PluginProperties properties)
        {
            var settings = new List<DefinitionValues>();

            var types = properties.GetString("types", "");

            var nextDefinition = 0;
            var hasNext = true;
            while (hasNext)
            {
                var open = types.IndexOf("[", nextDefinition);
                var close = types.IndexOf("]", open + 1);

                if (open >= 0 && close >= 0)
                {
                    var definition = types.Substring(open + 1, close);
                    settings.Add(new DefinitionValues(definition));
                    nextDefinition = close + 1;
                }
                else
                {
                    hasNext = false;
                }
            }

            return settings;
        }

        internal DefinitionValues(string definition)
        {
            StageName = StageFrom(definition);
            Name = NameFrom(definition);
            Protocol = ProtocolFrom(definition);
            Supervisor = SupervisorFrom(definition);
        }

        private string NameFrom(string definition) => PartFor(definition, NAME);

        private string ProtocolFrom(string definition) => PartFor(definition, PROTOCOL);

        private string StageFrom(string definition) => PartFor(definition, STAGE);

        private string SupervisorFrom(string definition) => PartFor(definition, SUPERVISOR);

        private string PartFor(string definition, string partName)
        {
            var start = definition.IndexOf(partName);

            if (start == -1)
            {
                return "";
            }

            var startName = start + partName.Length;
            var end = definition.IndexOf(" ", startName);
            var actualEnd = end >= 0 ? end : definition.Length;
            var part = definition.Substring(startName, actualEnd);
            return part;
        }
    }
}