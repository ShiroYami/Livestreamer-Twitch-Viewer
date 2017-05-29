using System;
using System.Diagnostics;
using System.Windows.Input;

namespace LivestreamerTwitchViewer
{
    class KeyBinding
    {
        private Scroll m_scroll;
        private ICommand m_refreshCommand;

        public Scroll Scroll { get { return m_scroll; } }
        public ICommand RefreshCommand { get { return m_refreshCommand; } }

        public KeyBinding(Scroll scroll)
        {
            m_scroll = scroll;
            m_refreshCommand = new RelayCommand(Refresh);
        }

        private void Refresh()
        {
            Scroll.SelectTabToReload();
        }
    }

    public class RelayCommand : ICommand
    {

        readonly Action m_execute;
        readonly Func<bool> m_canExecute;

        public Action CurrentAction { get { return m_execute; } }

        public RelayCommand(Action p_execute, Func<bool> p_canExecute = null)
        {
            if (p_execute == null)
                throw new ArgumentNullException("execute");

            m_execute = p_execute;
            m_canExecute = p_canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return m_canExecute == null || m_canExecute();
        }

        public void Execute(object parameter)
        {
            m_execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (m_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (m_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }
    }
}
