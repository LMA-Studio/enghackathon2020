/*
    This file is part of LMAStudio.StreamVR
    Copyright(C) 2020  Andreas Brake, Lisa-Marie Mueller

    LMAStudio.StreamVR is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMAStudio.StreamVR.Revit.Conversions
{
    public interface IConverter<T1>
    {
        JObject ConvertToDTO(T1 source);
        void MapFromDTO(JObject source, T1 dest);
        T1 CreateFromDTO(Document doc, JObject source);
    }

    public interface IGenericConverter
    {
        JObject ConvertToDTO(object source);
        void MapFromDTO(JObject source, object dest);
        JObject CreateFromDTO<T1>(Document doc, JObject source, out T1 newElement);
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

                response = (JObject)converterType.GetMethod("ConvertToDTO").Invoke(converter, new[] { source });
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

        public JObject CreateFromDTO<T1>(Document doc, JObject source, out T1 newElement)
        {
            JObject response;
            try
            {
                Type converterType = GetConverter(typeof(T1));

                object converter = Activator.CreateInstance(converterType);

                newElement = (T1)converterType.GetMethod("CreateFromDTO").Invoke(converter, new object[] { doc, source });
                response = new JObject();
                response["OK"] = 1;
            }
            catch (Exception e)
            {
                newElement = default(T1);

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

        private Type GetConverter(object o)
        {
            Type matchType = o.GetType();
            return GetConverter(matchType);
        }

        private Type GetConverter(Type matchType)
        {
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
