using Quantum.Services;
using Quantum.UIComponents;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Timers;

namespace WPF.Panels
{
    [Guid("3AF60523-CED6-4E8B-918B-207647C018FD")]
    public class ListPanelViewModel : ListViewModel, IListPanelViewModel
    {
        public PropertyInfo HeaderSortKey { get { return ReflectionUtils.GetPropertyInfo((TargetPanelVMI vmi) => vmi.Header); } }
        public PropertyInfo DescriptionSortKey { get { return ReflectionUtils.GetPropertyInfo((TargetPanelVMI vmi) => vmi.Description); } }

        private IEnumerable<Person> AllPersons { get; set; }
        private IList<Person> Persons { get; } = new List<Person>();
        private int CurrentIndex { get; set; }
        private Timer InvalidationTimer { get; set; }

        public ListPanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            AllPersons = GetPersons();
            InitializeInvalidationTimer();
        }

        private IEnumerable<Person> GetPersons()
        {
            return new List<Person>()
            {
                new Person()
                {
                    Name = "John Smith",
                    Description = "Engineer"
                },
                new Person()
                {
                    Name = "Alicia Parker",
                    Description = "Doctor"
                },
                new Person()
                {
                    Name = "Jon Doe",
                    Description = "Manager"
                }
            };
        }

        private void InitializeInvalidationTimer()
        {
            InvalidationTimer = new Timer()
            {
                 AutoReset = true,
                 Interval = 5000,
                 Enabled = false
            };

            InvalidationTimer.Elapsed += (sender, e) =>
            {
                if(CurrentIndex > 2) { return; }
                Persons.Add(AllPersons.ElementAt(CurrentIndex++));
                InvalidateChildren();
            };

            InvalidationTimer.Start();

        }

        protected override IEnumerable<IListViewModelItem> CreateContentItems()
        {
            foreach(var person in Persons)
            {
                yield return new TargetPanelVMI(person);
            }
        }
    }
    
 
    public class TargetPanelVMI : ListViewModelItem<Person>
    {

        public string Description { get { return Value.Description; } }

        public TargetPanelVMI(Person person)
            : base(person)
        {
            HeaderGetter = o => o.Name;
            IconGetter = o => null;
        }
    }
    

    public class Person
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
