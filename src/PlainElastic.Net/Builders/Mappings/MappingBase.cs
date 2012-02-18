using System;
using System.Collections.Generic;
using System.Linq;
using PlainElastic.Net.Builders;

namespace PlainElastic.Net.Mappings
{
    public abstract class MappingBase: IJsonConvertible
    {

        protected MappingBase()
        {
            Mappings = new List<string>();
        }


        public List<string> Mappings { get; private set; }



        /// <summary>
        /// Adds a custom JSON mapping to Object Map.
        /// You can use ' instead of " to simplify mapFormat creation.
        /// </summary>
        protected void RegisterCustomJsonMap(string mapFormat, params string[] args)
        {
            if (mapFormat.IsNullOrEmpty())
                return;

            var map = mapFormat.SmartQuoteF(args);
            Mappings.Add(map);
        }


        /// <summary>
        /// Registers the passed map function as JSON got as result of its execution.
        /// </summary>
        protected TResultMap RegisterMapAsJson<TMap, TResultMap>(Func<TMap, TResultMap> map)
            where TMap : new()
            where TResultMap : IJsonConvertible
        {
            var instance = new TMap();
            var resultMap = map.Invoke(instance);

            var jsonMap = resultMap.ToJson();

            if (!jsonMap.IsNullOrEmpty())
                Mappings.Add(jsonMap);

            return resultMap;
        }


        protected abstract string ApplyMappingTemplate(string mappingBody);


        string IJsonConvertible.ToJson()
        {
            // Return empty string if no mappings registered to eliminate empty mapping body in final JSON.
            if (!Mappings.Any())
                return "";

            var body = Mappings.JoinWithSeparator(", ");
            return ApplyMappingTemplate(body);
        }


    }
}