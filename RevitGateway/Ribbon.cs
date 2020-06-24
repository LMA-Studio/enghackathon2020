using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RevitGateway
{
    [Transaction(TransactionMode.Manual)]
    public class Ribbon : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            String tabName = "StreamVR";
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException)
            { }

            List<RibbonPanel> allRibbonPanels = application.GetRibbonPanels(tabName);
            foreach (RibbonPanel rp in allRibbonPanels)
            {

                if (rp.Name == "Stream Actions")
                {
                    AddPushButton(rp);
                    return Result.Succeeded;
                }

            }
            RibbonPanel intElevPanel = application.CreateRibbonPanel(tabName, "Stream Actions");
            AddPushButton(intElevPanel);
            return Result.Succeeded;

        }

        private void AddPushButton(RibbonPanel intElevPanel)
        {
            var assembly = Assembly.GetCallingAssembly();
            var assemblyDir = new FileInfo(assembly.Location).Directory.FullName;
            PushButtonData intElevButtonData = new PushButtonData("Begin Streaming", "Begin Streaming", assembly.Location, "RevitGateway.StreamingServer");
            PushButton placeIntElevButton = intElevPanel.AddItem(intElevButtonData) as PushButton;

            placeIntElevButton.ToolTip = "Automatically places interior elevations into all bound rooms";
        }

    }
}