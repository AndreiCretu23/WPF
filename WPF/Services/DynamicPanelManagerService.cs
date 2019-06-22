using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Common;
using Quantum.Events;
using Quantum.Services;
using Quantum.Utils;
using WPF.Panels;

namespace WPF
{
    public interface IDynamicPanelSelectionManagerService
    {
        void InitializeDynamicPanelSelection();
    }

    public class DynamicPanelSelectionManagerService : ServiceBase, IDynamicPanelSelectionManagerService
    {
        [Selection]
        public DynamicPanelSelection SelectedPanels { get; set; }
        
        private string SerializationFile { get { return Path.Combine(AppInfo.ApplicationRepository, "SelectedDynamicPanels.bin"); } }

        public DynamicPanelSelectionManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public void InitializeDynamicPanelSelection()
        {
            if(File.Exists(SerializationFile))
            {
                var data = BinarySerializer.Deserialize<PanelSelectionData>(SerializationFile);
                foreach(var id in data.Ids)
                {
                    SelectedPanels.Add(new DynamicPanelViewModel(InitializationService)
                    {
                        DisplayText = $"DynamicPanel {id.ToString()}",
                        Guid = id.ToString()
                    });
                }
            }
            else
            {
                for (int i = 1; i <= 5; i++)
                {
                    SelectedPanels.Add(new DynamicPanelViewModel(InitializationService)
                    {
                        DisplayText = $"DynamicPanel {i.ToString()}", 
                        Guid = i.ToString()
                    });
                }
            }
        }


        [Handles(typeof(ShutdownEvent))]
        public void OnAppExit()
        {
            var info = new PanelSelectionData();
            foreach(var viewModel in SelectedPanels.Value)
            {
                info.Ids.Add(Int32.Parse(viewModel.SafeCast<IIdentifiable>().Guid));
            }

            BinarySerializer.Serialize(info, SerializationFile, true);
        }
    }

    [Serializable]
    public class PanelSelectionData
    {
        public List<int> Ids { get; set; } = new List<int>();
    }
}
