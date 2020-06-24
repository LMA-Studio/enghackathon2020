using Autodesk.Revit.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevitGateway.Conversions;
using System;
using Utility;

namespace RevitGateway.Commands
{
    public class Set: IBaseCommand
    {
        private readonly Action<string> _log;
        private readonly IGenericConverter _converter;

        public Set(Action<string> log, IGenericConverter converter)
        {
            _log = log;
            _converter = converter;
        }

        public Message Execute(Document doc, Message msg)
        {
            _log("EXECUTE SET");

            JObject dto = (JObject)msg.Data;

            _log("GOT DTO");
            _log(JsonConvert.SerializeObject(dto));

            _log("GETTING ELEMENT");

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Update Element");

                Element dbValue = doc.GetElement(new ElementId(Int32.Parse(dto["Id"].ToString())));

                _log($"GOT MATERIAL {dbValue?.Id.ToString()}");

                _converter.MapFromDTO(dto, dbValue);

                _log($"MAPPED MATERIAL");

                dto = _converter.ConvertToDTO(dbValue);

                tx.Commit();
            }

            _log("NEW VALUE");
            _log(JsonConvert.SerializeObject(dto));

            return new Message
            {
                Type = "CURRENT_VALUE",
                Data = JObject.FromObject(dto)
            };
        }
    }
}
