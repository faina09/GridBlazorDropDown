using GridBlazor;
using GridBlazorDropDown.Data;
using GridShared;
using GridShared.Columns;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazorDropDown.Pages.Components
{
    public partial class RemoteDropDownComponent<T, V> : ICustomGridComponent<T> where V : class//, IGenericModel
    {
        [Parameter]
        public T Item { get; set; }

        [Parameter]
        public CGrid<T> Grid { get; set; }

        [Parameter]
        public object Object { get; set; }

        [Inject]
        //private IGenericService<V> RlstService { get; set; }
        private CustomerService RlstService { get; set; }

        public IEnumerable<SelectItem> SelectedItems;
        public string selectedValue;
        private string searchTerm;
        public bool allowChange;
        private string _RlstField;
        private string _message;
        private Func<T, string?> _expr;
        private ModelExtension Model { get; set; }

        public string SearchTerm
        {
            get { return searchTerm; }
            set { searchTerm = value; OnSearchChange(); }
        }

        protected override void OnParametersSet()
        {
            if (Object.GetType() == typeof((string, Func<T, string?>)))
            {
                (_RlstField, _expr) = ((string, Func<T, string?>))Object;
                try
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    Model = new ModelExtension(typeof(T), Item);
#pragma warning restore CS8604 // Possible null reference argument.
                    selectedValue = Model.GetValue($"{_RlstField}")?.ToString() ?? "";
                    SearchTerm = _expr(Item) ?? "";
                }
                catch (Exception e)
                {
                    throw new Exception("ERROR RemoteDropDownComponent must have (string[], Func<T, string?>) parameters and GetSelectedItems");
                }
            }

            string gridState = Grid.GetState();
            allowChange = Grid.Mode == GridMode.Update || Grid.Mode == GridMode.Create;
            if (!allowChange)
            {
                selectedValue = String.Concat(Model.GetValue($"{_RlstField}")?.ToString(), " - ", _expr(Item));
            }
        }

        public void OnSearchChange()
        {
            SelectedItems = RlstService.GetSelectedItems(searchTerm);
            _message = $"please select... [{SelectedItems.Count()}]";
        }

        private void ChangeValue(ChangeEventArgs e)
        {
            var value = e?.Value?.ToString();
            Model.SetValue($"{_RlstField}", value);
        }
    }
}
