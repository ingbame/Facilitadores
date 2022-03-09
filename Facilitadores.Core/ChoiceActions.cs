using Facilitadores.Core.Actions;
using Facilitadores.Util.ConsoleApps;
using System.ComponentModel.DataAnnotations;
using static Facilitadores.Entity.Enums.Enums;

namespace Facilitadores.Core
{
    public class ChoiceActions
    {
        private static ChoiceActions? _instance;
        private static readonly object _locker = new object();

        public static ChoiceActions Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                            _instance = new ChoiceActions();
                    }
                }
                return _instance;
            }
        }
        public void SelectActionsToDo()
        {
            try
            {
                ConsoleMessages.Instance.ShowInfoMessage("Seleccione la tarea que desea hacer");
                ConsoleMessages.Instance.ShowInfoMessage("***********************************");

                Tareas[] EnumArray = (Tareas[])Enum.GetValues(typeof(Tareas));
                foreach (Tareas _tareas in EnumArray)
                {
                    ConsoleMessages.Instance.ShowInfoMessage($"{(int)_tareas}.- {_tareas.GetAttribute<DisplayAttribute>()?.Name}");
                }
                ConsoleMessages.Instance.ShowInfoMessage("S.- Salir");
                ConsoleMessages.Instance.ShowInfoMessage("***********************************");

                var KeyPressed = ConsoleMessages.Instance.RequestKey("Escriba su opción: ");

                ConsoleMessages.Instance.ShowSystemMessage("\nSeleccionando.....");

                var KeyType = KeyPressed.KeyChar;
                if (Char.IsDigit(KeyType))
                {
                    var Exists = EnumArray.Contains((Tareas)int.Parse(KeyType.ToString()));
                    if (!Exists)
                        throw new FormatException("La opción no existe en la lista.");
                    Tareas _Task = (Tareas)int.Parse(KeyType.ToString());
                    switch (_Task)
                    {
                        case Tareas.ArmaPaqueteRelease:
                            PaqueteParaRelease.Instance.Process();
                            break;
                        default:
                            throw new FormatException("La opción no está configurada aún.");
                    }
                }
                else
                {
                    var TeclaE = KeyType.ToString().ToLower();
                    if (TeclaE == "s")
                        return;
                    else
                        throw new FormatException("Ingresa solo número como se muestra en la lista.");
                }


                ConsoleMessages.Instance.ShowSystemMessage("\nPresione cualquier letra para salir...");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.Clear();
                ConsoleMessages.Instance.ShowDangerMessage($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    ConsoleMessages.Instance.ShowDangerMessage($"Error: {ex.InnerException.Message}");
                SelectActionsToDo();
            }
        }
    }
}