using Autodesk.Revit.DB;
using Utility;

namespace RevitGateway.Commands
{
    public interface IBaseCommand
    {
        Message Execute(Document doc, Message msg);
    }
}
