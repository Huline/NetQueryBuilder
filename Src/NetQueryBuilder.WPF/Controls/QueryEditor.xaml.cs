using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.WPF.Controls
{
    /// <summary>
    ///     Interaction logic for QueryEditor.xaml
    /// </summary>
    public partial class QueryEditor : UserControl
    {
        public QueryEditor()
        {
            InitializeComponent();
            // A FAIRE : Injectez ici un IQuery réel
            // Par exemple, avec un paramètre, ou via un property pour le DataContext
            // Ici, c'est juste un exemple :
            // var vm = new QueryEditorViewModel(queryInstance);
            // this.DataContext = vm;
        }

        // Une méthode pour permettre au parent d’injecter IQuery si besoin :
        public void Initialize(IQuery query)
        {
            DataContext = new QueryEditorViewModel(query);
        }
    }


// Un ICommand simple pour la démo
    public class RelayCommand : ICommand
    {
        private readonly Func<object?, bool>? _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class QueryEditorViewModel : INotifyPropertyChanged
    {
        private string? _querySummary;

        public QueryEditorViewModel(IQuery query)
        {
            Query = query;
            ExecuteQueryCommand = new RelayCommand(_ => ExecuteQuery());
            UpdateQuerySummary();
            Query.OnChanged += (_, __) => UpdateQuerySummary();
        }

        public string? QuerySummary
        {
            get => _querySummary;
            set
            {
                _querySummary = value;
                OnPropertyChanged();
            }
        }

        public IQuery Query { get; }
        public ICommand ExecuteQueryCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async void ExecuteQuery()
        {
            // Exécution exemple — A adapter selon vos besoins
            var result = await Query.Execute(10); // Page de 10 éléments
            QuerySummary = $"Exécuté : {result.Items.Count} items récupérés (Page 1)";
        }

        private void UpdateQuerySummary()
        {
            // Précisez le résumé souhaité
            var selects = string.Join(", ", Query.SelectPropertyPaths.Where(p => p.IsSelected).Select(p => p.Property.PropertyFullName));
            QuerySummary = $"Champs sélectionnés : {selects}";
        }
    }
}