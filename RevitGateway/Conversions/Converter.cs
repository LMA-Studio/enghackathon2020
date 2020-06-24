using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGateway.Conversions
{
    public interface IConverter<T1>
    {
        JObject ConvertToDTO(T1 source);
        void MapFromDTO(JObject source, T1 dest);
    }

    public interface IGenericConverter
    {
        JObject ConvertToDTO(object source);
        void MapFromDTO(JObject source, object dest);
    }

    public class GenericConverter: IGenericConverter
    {
        private readonly Action<string> _log;

        public GenericConverter(Action<string> log)
        {
            _log = log;
        }

        public JObject ConvertToDTO(object source)
        {
            JObject response;
            try
            {
                Type converterType = GetConverter(source);

                object converter = Activator.CreateInstance(converterType);

                response =(JObject)converterType.GetMethod("ConvertToDTO").Invoke(converter, new[] { source });
            }
            catch(Exception e)
            {
                Exception ex = e.InnerException?.InnerException?.InnerException ?? e.InnerException?.InnerException ?? e.InnerException ?? e;
                response = new JObject();
                response["ERROR"] = 1;
                response["Msg"] = e.Message.ToString();
                response["Stack"] = e.StackTrace.ToString();
                response["Inner"] = e.InnerException == null ? null : JObject.FromObject(new
                {
                    Msg = e.InnerException.Message.ToString(),
                    Stack = e.InnerException.StackTrace.ToString(),
                    Inner = e.InnerException.InnerException == null ? null : JObject.FromObject(new
                    {
                        Msg = e.InnerException.InnerException.Message.ToString(),
                        Stack = e.InnerException.InnerException.StackTrace.ToString()
                    })
                });
            }

            response["Type"] = source.GetType().FullName;
            return response;
        }
        
        public void MapFromDTO(JObject source, object dest)
        {
            Type converterType = GetConverter(dest);

            object converter = Activator.CreateInstance(converterType);

            converterType.GetMethod("MapFromDTO").Invoke(converter, new[] { source, dest });
        }

        private Type GetConverter(object o)
        {
            Type matchType = o.GetType();

            return typeof(GenericConverter).Assembly.GetTypes().FirstOrDefault(
                t => !t.IsAbstract
                    && !t.IsInterface
                    && t.GetInterfaces().Any(
                        i => i.IsGenericType
                            && typeof(IConverter<>).IsAssignableFrom(i.GetGenericTypeDefinition())
                            && i.GetGenericArguments()[0].Equals(matchType)
                    )
            );
        }
    }
}
