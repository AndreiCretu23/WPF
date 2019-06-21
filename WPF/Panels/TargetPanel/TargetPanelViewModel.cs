using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WPF.Panels
{
    [Guid("3AF60523-CED6-4E8B-918B-207647C018FD")]
    public class TargetPanelViewModel : ListViewModel, ITargetPanelViewModel
    {
        private IEnumerable<Person> Persons { get; set; }

        public TargetPanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            InitializePersons();
        }

        private void InitializePersons()
        {
            Persons = new List<Person>()
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

        protected override IEnumerable<IListViewModelItem> CreateContentItems()
        {
            foreach(var person in Persons)
            {
                yield return new TargetPanelVMI(person)
                {
                    HeaderGetter = o => person.Name,
                    IconGetter = o => null
                };
            }
        }
    }
    
 
    public class TargetPanelVMI : ListViewModelItem<Person>
    {

        public string Description { get { return Value.Description; } }

        public TargetPanelVMI(Person person)
            : base(person)
        {
        }
    }
    

    public class Person
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
