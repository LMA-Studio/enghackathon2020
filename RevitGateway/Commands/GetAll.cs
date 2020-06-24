using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using RevitGateway.Conversions;
using System;
using System.Linq;
using Utility;

namespace RevitGateway.Commands
{
    public class GetAll: IBaseCommand
    {
        private readonly Action<string> _log;
        private readonly IGenericConverter _converter;

        public GetAll(Action<string> log, IGenericConverter converter)
        {
            _log = log;
            _converter = converter;
        }

        public Message Execute(Document doc, Message msg)
        {
            string dataType = msg.Data["Type"].ToString();

            _log($"Getting data type {dataType}");

            Type t = doc.GetType().Assembly.GetType(dataType);

            if(t == null)
            {
                return new Message
                {
                    Type = "ERROR",
                    Data = JObject.FromObject(new
                    {
                        Msg = "Type does not exist"
                    })
                };
            }

            _log($"Got data type {t?.FullName ?? "NULL"}");

            var materials = new FilteredElementCollector(doc).
                OfClass(t).
                Where(e => e != null).
                Select(e =>
                {
                    _log($"Id {e.Id}");
                    return e;
                }).
                Select(_converter.ConvertToDTO).
                ToList();

            _log($"Found {materials.Count} materials");

            var converted = JArray.FromObject(materials);

            _log($"Converted {converted.Count} materials");

            return new Message
            {
                Type = "CURRENT_STATE",
                Data = converted
            };
        }
    }
}
