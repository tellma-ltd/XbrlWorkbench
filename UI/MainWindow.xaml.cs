using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Banan.Tools.Xbrl.Instances;
using Banan.Tools.Xbrl.Taxonomies;
using Banan.Tools.XbrlBench.UI.Commands;
using Banan.Tools.XbrlBench.UI.Parsing;
using Banan.Tools.XbrlBench.UI.Services;
using Unity;
using Unity.Lifetime;

namespace Banan.Tools.XbrlBench.UI
{
    public partial class MainWindow : Window, ILogger
    {
        private IUnityContainer _container;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var commandRegistry = new Mef2BasedCommandRegistry();
            commandRegistry.Discover();

            _container = new UnityContainer();
            _container.RegisterType<CommandParser>();
            _container.RegisterType<IState, DictionaryBackedState>(new SingletonLifetimeManager());
            _container.RegisterInstance<ILogger>(this);
            _container.RegisterInstance<ICommandRegistry>(commandRegistry);

            _container.RegisterInstance(new Instance());

            txtCommand.Text = "Load-Taxonomy -archive \"C:\\Data\\Banan\\TFS\\banantfs\\Scrum\\Tools\\XbrlWorkbench\\UI\\Samples\\IFRST_2017-03-09.zip\" -entry \"full_ifrs_entry_point_2017-03-09.xsd\"";
        }

        #region IInterface

        public void WriteLine(string line)
        {
            txtLog.Text += line + Environment.NewLine;
        }

        #endregion

        private void txtCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                return;
            }

            var line = txtCommand.Text;
            txtCommand.Text = string.Empty;

            ProcessInput(line);
        }

        private void ProcessInput(string line)
        {
            //try
            //{
                var commandLines = line.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var commandLine in commandLines)
                {
                    ProcessCommandLine(commandLine);
                }
            //}
            //catch (Exception e)
            //{
            //    WriteLine(e.Message);
            //}
        }

        private void ProcessCommandLine(string commandLine)
        {
            WriteLine("> " + commandLine);

            ParsedCommand parsedCommand;

            var parser = _container.Resolve<CommandParser>();
            parsedCommand = parser.Parse(commandLine);

            var commandRegistry = _container.Resolve<ICommandRegistry>();

            var commandType = commandRegistry.FindType(parsedCommand.CommandName);
            if (commandType == null)
            {
                throw new InvalidOperationException($"{commandLine} is not a known command. Use Get-Command to get the list of all commands.");
            }

            var commandChildContainer = _container.CreateChildContainer();
            commandChildContainer.RegisterInstance("NamedParameters", parsedCommand.NamedParameters);
            commandChildContainer.RegisterInstance("PositionalParameters", parsedCommand.PositionalParameters);

            var watch = Stopwatch.StartNew();

            var command = commandChildContainer.Resolve(commandType) as IShellCommand;
            command.Invoke();

            watch.Stop();
            WriteLine($"Executed in {watch.ElapsedMilliseconds} ms.");

            WriteLine(Environment.NewLine);
        }

        private void Log(string line)
        {
            Application.Current.Dispatcher.Invoke(() => { txtLog.Text = txtLog.Text + line + Environment.NewLine; });
        }

    }
}
