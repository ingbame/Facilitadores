using Facilitadores.Util.ConsoleApps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facilitadores.Core.Actions
{
    public class PaqueteParaRelease
    {
        private static PaqueteParaRelease? _instance;
        private static readonly object _locker = new object();

        public static PaqueteParaRelease Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                            _instance = new PaqueteParaRelease();
                    }
                }
                return _instance;
            }
        }
        public string? RouteFileList { get; set; }
        public string? RouteProjectPublished { get; set; }
        public string? DestinationRoute { get; set; }
        public List<string>? ExtensionsToInclude { get; set; }
        public void Process()
        {
            ConsoleMessages.Instance.ShowInfoMessage("Es necesario ingresar la siquiente información");
            ConsoleMessages.Instance.ShowInfoMessage("**********************************************");
            this.RequestData();

            var RoutesListArr = GetRoutesFromList(this.RouteFileList, out List<string> PossibleDlls);

            if (RoutesListArr.Count > 0)
            {
                ConsoleMessages.Instance.ShowInfoMessage("La lista anterior representa los datos que se empaquetarán");
                var KeyPressed = ConsoleMessages.Instance.RequestKey("¿Es correcto? (S/N): ");
                var KeyType = KeyPressed.KeyChar;
                bool _continue = false;
                while (!_continue)
                {
                    if (Char.IsLetter(KeyType))
                    {
                        var Option = KeyType.ToString().ToLower();
                        switch (Option)
                        {
                            case "s":
                                CopyAndPasteFiles(RoutesListArr, PossibleDlls);
                                _continue = true;
                                break;
                            case "n":
                                throw new ArgumentException("Al ser los datos incorrectos no se puede realizar el proceso");
                            default:
                                ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"S\" para Si o \"N\" para no");
                                KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                                KeyType = KeyPressed.KeyChar;
                                Option = KeyType.ToString().ToLower();
                                break;
                        }
                    }
                    else
                    {
                        ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                        KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                        KeyType = KeyPressed.KeyChar;
                    }

                }
                ConsoleMessages.Instance.ShowInfoMessage("\n************************************************");
            }
            else
                throw new ArgumentException("Al no tener datos que procesar, no se puede realizar el proceso");
            //Copiar y pegar
            //Traer Bin si es necesario


        }
        private void RequestData()
        {
            ConsoleMessages.Instance.ShowSystemMessage(@"Ejemplo: C:\ListadoGit.txt");
            this.RouteFileList = ConsoleMessages.Instance.RequestAnswer("Ruta de Listado Git (.txt): ");

            ConsoleMessages.Instance.ShowSystemMessage(@"Ejemplo: C:\Liberaciones\OperacionesWEB");
            this.RouteProjectPublished = ConsoleMessages.Instance.RequestAnswer("Ruta del proyecto publicado: ");

            ConsoleMessages.Instance.ShowSystemMessage(@"Ejemplo: C:\Empaquetados\RXX\Empaquetado");
            this.DestinationRoute = ConsoleMessages.Instance.RequestAnswer("Ruta de destino: ");

            this.ExtensionsToInclude = new List<string> { ".css", ".js", ".cshtml", ".html", ".rdlc", ".rpt", ".json", ".xml" };
            this.AddOrDelete();

            if (string.IsNullOrEmpty(this.RouteFileList))
                throw new ArgumentException("No se pudo continuar con el proceso por falta de información");
            if (string.IsNullOrEmpty(this.RouteProjectPublished))
                throw new ArgumentException("No se pudo continuar con el proceso por falta de información");
            if (string.IsNullOrEmpty(this.DestinationRoute))
                throw new ArgumentException("No se pudo continuar con el proceso por falta de información");

            FileAttributes attr = File.GetAttributes(this.RouteFileList);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                throw new ArgumentException("Esta ruta (Lista) debe de coresponder a un archivo");

            attr = File.GetAttributes(this.RouteProjectPublished);
            if (!((attr & FileAttributes.Directory) == FileAttributes.Directory))
                throw new ArgumentException("Esta ruta (Proyecto) debe de coresponder a un folder");

            attr = File.GetAttributes(this.DestinationRoute);
            if (!((attr & FileAttributes.Directory) == FileAttributes.Directory))
                throw new ArgumentException("Esta ruta (Destino) debe de coresponder a un folder");

            if (this.ExtensionsToInclude.Count <= 0)
                throw new ArgumentException("Es necesario tener extensiones para incluir");
        }
        private void AddOrDelete()
        {
            ConsoleMessages.Instance.ShowInfoMessage("Las Extensiones a considerar son las siguientes:");
            foreach (var ext in ExtensionsToInclude ?? new List<string>())
            {
                ConsoleMessages.Instance.ShowInfoMessage(ext);
            }
            var KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
            var KeyType = KeyPressed.KeyChar;
            bool _continue = false;
            while (!_continue)
            {
                if (Char.IsLetter(KeyType))
                {
                    var Option = KeyType.ToString().ToLower();
                    switch (Option)
                    {
                        case "s":
                            var AddDelKeyP = ConsoleMessages.Instance.RequestKey("\n¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                            var AddDelKeyType = AddDelKeyP.KeyChar;
                            bool _continue2 = false;
                            if (Char.IsLetter(AddDelKeyType))
                            {
                                while (!_continue2)
                                {
                                    var Option2 = AddDelKeyType.ToString().ToLower();
                                    switch (Option2)
                                    {
                                        case "a":
                                            var _add = ConsoleMessages.Instance.RequestAnswer("\nIngrese extensión a agregar: ");
                                            if (_add.StartsWith("."))
                                            {
                                                this.ExtensionsToInclude?.Add(_add);
                                                ConsoleMessages.Instance.ShowSystemMessage("\nLa extensión se ha agregado correctamente");
                                                foreach (var ext in ExtensionsToInclude ?? new List<string>())
                                                {
                                                    ConsoleMessages.Instance.ShowInfoMessage(ext);
                                                }
                                            }
                                            else
                                                ConsoleMessages.Instance.ShowDangerMessage("\nEl dato ingresado no cumple con el formato de la extensión");
                                            break;
                                        case "e":
                                            var _del = ConsoleMessages.Instance.RequestAnswer("\nIngrese extensión a eliminar: ");
                                            if (_del.StartsWith("."))
                                            {
                                                this.ExtensionsToInclude?.Remove(_del);
                                                ConsoleMessages.Instance.ShowSystemMessage("\nLa extensión se ha eliminado correctamente");
                                                foreach (var ext in ExtensionsToInclude ?? new List<string>())
                                                {
                                                    ConsoleMessages.Instance.ShowInfoMessage(ext);
                                                }
                                            }
                                            else
                                                ConsoleMessages.Instance.ShowDangerMessage("\nEl dato ingresado no cumple con el formato de la extensión");
                                            break;
                                        case "c":
                                            _continue2 = true;
                                            break;
                                        default:
                                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"A\" para Agregar, \"E\" para Eliminar o \"C\" para Cancelar");
                                            break;
                                    }
                                    AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                    AddDelKeyType = AddDelKeyP.KeyChar;
                                    Option2 = AddDelKeyType.ToString().ToLower();
                                }
                            }
                            else
                            {
                                ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                                AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                AddDelKeyType = AddDelKeyP.KeyChar;
                            }
                            _continue = true;
                            break;
                        case "n":
                            _continue = true;
                            break;
                        default:
                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"S\" para Si o \"N\" para no");
                            KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                            KeyType = KeyPressed.KeyChar;
                            Option = KeyType.ToString().ToLower();
                            break;
                    }
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                    KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                    KeyType = KeyPressed.KeyChar;
                }

            }
            ConsoleMessages.Instance.ShowInfoMessage("\n************************************************");
        }
        private List<string> GetRoutesFromList(string? FileToRead, out List<string> PossibleDlls)
        {
            List<string> RoutesList = new List<string>();
            PossibleDlls = new List<string>();
            IEnumerable<string> _lines = File.ReadLines(FileToRead ?? string.Empty);
            foreach (var line in _lines)
            {
                var _line = line.Replace(@"\", "/");
                string? Route = _line;
                string? _Root = string.Concat(Route.Split('/').ToList().First(), ".dll");
                if (!PossibleDlls.Contains(_Root))
                    PossibleDlls.Add(_Root);
                var _lineExt = Route.Substring(Route.LastIndexOf('.'));
                if (this.ExtensionsToInclude?.Contains(_lineExt) ?? false)
                {
                    var FullRoute = string.Concat(this.RouteProjectPublished, @"\", Route);
                    bool Exists = File.Exists(FullRoute);
                    while (!Exists)
                    {
                        string? NewRoute = string.Empty;
                        var SplitLine = Route.Split('/').ToList();
                        SplitLine.Remove(SplitLine.First());
                        foreach (var item in SplitLine)
                        {
                            NewRoute += item;
                            if (item != SplitLine.Last())
                                NewRoute += "/";
                        }
                        Route = NewRoute;
                        FullRoute = string.Concat(this.RouteProjectPublished, @"\", Route);
                        Exists = File.Exists(FullRoute);
                        if (string.IsNullOrEmpty(Route))
                            throw new ArgumentException($"El archivo no existe en el proyecto liberado: {_line}");
                    }
                    RoutesList.Add(Route);
                    Console.WriteLine(Route);
                }
            }
            return RoutesList;
        }
        private void CopyAndPasteFiles(List<string> RoutesListArr, List<string> PossibleDlls)
        {
            bool HaveBin = AskIfHaveBinToCopy();
            bool DllsReady = false;
            bool HaveResourceDlls = false;
            bool ResourceDllsReady = false;
            List<string>? ResourceDlls = null;

            if (HaveBin)
            {
                if (PossibleDlls.Count > 0)
                {
                    ConsoleMessages.Instance.ShowSystemMessage($"\nSe detectaron {PossibleDlls.Count} posibles dlls para poner en carpeta \"bin\"");
                    foreach (var dll in PossibleDlls)
                    {
                        ConsoleMessages.Instance.ShowInfoMessage(dll);
                    }
                    DllsReady = AskIfPossibleDllsIsCorrect();
                    while (!DllsReady)
                    {
                        PossibleDlls = AddOrDeleteDlls(PossibleDlls);
                        DllsReady = AskIfPossibleDllsIsCorrect();
                    }

                }
                else
                {
                    PossibleDlls = AddOrDeleteDlls(PossibleDlls);
                    DllsReady = AskIfPossibleDllsIsCorrect();
                }
                HaveResourceDlls = AskIfHaveResourceDllsToCopy();
                if (HaveResourceDlls)
                {
                    ResourceDlls = new List<string>();
                    while (!ResourceDllsReady)
                    {
                        ResourceDlls = AddOrDeleteResourceDlls(ResourceDlls);
                        ResourceDllsReady = AskIfResourceDllsIsCorrect();
                    }
                }
            }

            ConsoleMessages.Instance.ShowSystemMessage("\nCopiando y pegando en Destino....");

            var FolderInicial = "ArchivosProyecto";
            string? FolderPath = string.Concat(this.DestinationRoute, "/", FolderInicial);
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            if (HaveBin)
            {
                string? BinFolderPath = string.Concat(this.DestinationRoute, "/", FolderInicial, "/bin");
                if (!Directory.Exists(BinFolderPath))
                    Directory.CreateDirectory(BinFolderPath);
                if (DllsReady)
                {
                    foreach (var dll in PossibleDlls)
                    {
                        var OrigingRoute = string.Concat(this.RouteProjectPublished, "/bin/", dll);
                        var FullDestinationRoute = string.Concat(BinFolderPath, "/", dll);
                        File.Copy(OrigingRoute, FullDestinationRoute);
                    }
                }
                if (HaveResourceDlls)
                {
                    if (ResourceDllsReady)
                    {
                        string? BinResourceMxFolderPath = string.Concat(this.DestinationRoute, "/", FolderInicial, "/bin/es-MX");
                        string? BinResourceUsFolderPath = string.Concat(this.DestinationRoute, "/", FolderInicial, "/bin/en-US");
                        if (!Directory.Exists(BinResourceMxFolderPath))
                            Directory.CreateDirectory(BinResourceMxFolderPath);
                        if (!Directory.Exists(BinResourceUsFolderPath))
                            Directory.CreateDirectory(BinResourceUsFolderPath);

                        foreach (var dll in ResourceDlls)
                        {
                            var OrigingMxRoute = string.Concat(this.RouteProjectPublished, "/bin/es-MX/", dll);
                            var OrigingUsRoute = string.Concat(this.RouteProjectPublished, "/bin/en-US/", dll);
                            var FullMxDestinationRoute = string.Concat(BinResourceMxFolderPath, "/", dll);
                            var FullUsDestinationRoute = string.Concat(BinResourceUsFolderPath, "/", dll);
                            File.Copy(OrigingMxRoute, FullMxDestinationRoute);
                            File.Copy(OrigingUsRoute, FullUsDestinationRoute);
                        }
                    }
                }
            }
            foreach (var item in RoutesListArr)
            {
                var OrigingRoute = string.Concat(this.RouteProjectPublished, "/", item);
                var FullDestinationRoute = string.Concat(FolderPath, "/", item);

                var directoryName = Path.GetDirectoryName(FullDestinationRoute);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName ?? string.Empty);

                File.Copy(OrigingRoute, FullDestinationRoute);
            }

            ConsoleMessages.Instance.ShowSuccessMessage("Proceso terminado correctamente....");
        }
        private bool AskIfHaveBinToCopy()
        {
            var KeyPressed = ConsoleMessages.Instance.RequestKey("\n¿Se incluirá carpeta \"bin\"? (S/N): ");
            var KeyType = KeyPressed.KeyChar;
            bool _continue = false;
            while (!_continue)
            {
                if (Char.IsLetter(KeyType))
                {
                    var Option = KeyType.ToString().ToLower();
                    switch (Option)
                    {
                        case "s":
                            return true;
                        case "n":
                            _continue = true;
                            break;
                        default:
                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"S\" para Si o \"N\" para no");
                            KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                            KeyType = KeyPressed.KeyChar;
                            Option = KeyType.ToString().ToLower();
                            break;
                    }
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                    KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                    KeyType = KeyPressed.KeyChar;
                }

            }
            return false;
        }
        private bool AskIfHaveResourceDllsToCopy()
        {
            var KeyPressed = ConsoleMessages.Instance.RequestKey("\n¿Se incluirá dlls de recursos? (S/N): ");
            var KeyType = KeyPressed.KeyChar;
            bool _continue = false;
            while (!_continue)
            {
                if (Char.IsLetter(KeyType))
                {
                    var Option = KeyType.ToString().ToLower();
                    switch (Option)
                    {
                        case "s":
                            return true;
                        case "n":
                            _continue = true;
                            break;
                        default:
                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"S\" para Si o \"N\" para no");
                            KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                            KeyType = KeyPressed.KeyChar;
                            Option = KeyType.ToString().ToLower();
                            break;
                    }
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                    KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                    KeyType = KeyPressed.KeyChar;
                }

            }
            return false;
        }
        private bool AskIfPossibleDllsIsCorrect()
        {
            var KeyPressed = ConsoleMessages.Instance.RequestKey("\n¿Las Dlls encontradas son correctas? (S/N): ");
            var KeyType = KeyPressed.KeyChar;
            bool _continue = false;
            while (!_continue)
            {
                if (Char.IsLetter(KeyType))
                {
                    var Option = KeyType.ToString().ToLower();
                    switch (Option)
                    {
                        case "s":
                            return true;
                        case "n":
                            _continue = true;
                            break;
                        default:
                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"S\" para Si o \"N\" para no");
                            KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                            KeyType = KeyPressed.KeyChar;
                            Option = KeyType.ToString().ToLower();
                            break;
                    }
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                    KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                    KeyType = KeyPressed.KeyChar;
                }

            }
            return false;
        }
        private bool AskIfResourceDllsIsCorrect()
        {
            var KeyPressed = ConsoleMessages.Instance.RequestKey("\n¿Lleva Dll de Recursos son correctas? (S/N): ");
            var KeyType = KeyPressed.KeyChar;
            bool _continue = false;
            while (!_continue)
            {
                if (Char.IsLetter(KeyType))
                {
                    var Option = KeyType.ToString().ToLower();
                    switch (Option)
                    {
                        case "s":
                            return true;
                        case "n":
                            _continue = true;
                            break;
                        default:
                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"S\" para Si o \"N\" para no");
                            KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                            KeyType = KeyPressed.KeyChar;
                            Option = KeyType.ToString().ToLower();
                            break;
                    }
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                    KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                    KeyType = KeyPressed.KeyChar;
                }

            }
            return false;
        }
        private List<string> AddOrDeleteDlls(List<string> PossibleDlls)
        {
            ConsoleMessages.Instance.ShowInfoMessage("\nLas dlls a editar son las siguientes:");
            foreach (var dll in PossibleDlls)
            {
                ConsoleMessages.Instance.ShowInfoMessage($"{PossibleDlls.IndexOf(dll)}.- {dll}");
            }
            var KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
            var KeyType = KeyPressed.KeyChar;
            bool _continue = false;
            while (!_continue)
            {
                if (Char.IsLetter(KeyType))
                {
                    var Option = KeyType.ToString().ToLower();
                    switch (Option)
                    {
                        case "s":
                            var AddDelKeyP = ConsoleMessages.Instance.RequestKey("\n¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                            var AddDelKeyType = AddDelKeyP.KeyChar;
                            bool _continue2 = false;
                            if (Char.IsLetter(AddDelKeyType))
                            {
                                while (!_continue2)
                                {
                                    var Option2 = AddDelKeyType.ToString().ToLower();
                                    switch (Option2)
                                    {
                                        case "a":
                                            PossibleDlls = AddRemoveFromList(true, PossibleDlls);

                                            ConsoleMessages.Instance.ShowSystemMessage("\nLa dll se ha agregado correctamente a la lista");
                                            foreach (var dll in PossibleDlls)
                                            {
                                                ConsoleMessages.Instance.ShowInfoMessage($"{PossibleDlls.IndexOf(dll)}.- {dll}");
                                            }
                                            AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                            AddDelKeyType = AddDelKeyP.KeyChar;
                                            Option2 = AddDelKeyType.ToString().ToLower();
                                            break;
                                        case "e":
                                            PossibleDlls = AddRemoveFromList(false, PossibleDlls);

                                            ConsoleMessages.Instance.ShowSystemMessage("\nLa dll se ha eliminado correctamente");
                                            foreach (var dll in PossibleDlls)
                                            {
                                                ConsoleMessages.Instance.ShowInfoMessage($"{PossibleDlls.IndexOf(dll)}.- {dll}");
                                            }
                                            AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                            AddDelKeyType = AddDelKeyP.KeyChar;
                                            Option2 = AddDelKeyType.ToString().ToLower();
                                            break;
                                        case "c":
                                            _continue2 = true;
                                            break;
                                        default:
                                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"A\" para Agregar, \"E\" para Eliminar o \"C\" para Cancelar");
                                            AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                            AddDelKeyType = AddDelKeyP.KeyChar;
                                            Option2 = AddDelKeyType.ToString().ToLower();
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                                AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                AddDelKeyType = AddDelKeyP.KeyChar;
                            }
                            _continue = true;
                            break;
                        case "n":
                            _continue = true;
                            break;
                        default:
                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"S\" para Si o \"N\" para no");
                            KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                            KeyType = KeyPressed.KeyChar;
                            Option = KeyType.ToString().ToLower();
                            break;
                    }
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                    KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                    KeyType = KeyPressed.KeyChar;
                }

            }
            ConsoleMessages.Instance.ShowInfoMessage("\n************************************************");
            return PossibleDlls;
        }
        private List<string> AddRemoveFromList(bool Add, List<string> PossibleDlls)
        {
            if (Add)
            {
                var _add = ConsoleMessages.Instance.RequestAnswer("\nIngrese nombre de la dll a agregar con su extensión: ");
                if (_add.Contains(".dll"))
                {
                    PossibleDlls.Add(_add);
                }
                else
                    ConsoleMessages.Instance.ShowDangerMessage("\nEl dato ingresado no cumple con el formato dll, ingrese la extensión");

            }
            else
            {
                var KeyPressed = ConsoleMessages.Instance.RequestKey("\nEscriba él número a eliminar: ");
                var KeyType = KeyPressed.KeyChar;
                if (Char.IsDigit(KeyType))
                {
                    int _Index = int.Parse(KeyType.ToString());
                    var StrArr = PossibleDlls.ToArray();
                    PossibleDlls.Remove(StrArr[_Index]);
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nEl dato ingresado no cumple con el formato de la solicitado, solo numeros porfavor");
                }
            }
            return PossibleDlls;
        }
        private List<string> AddOrDeleteResourceDlls(List<string> ResourceDlls)
        {
            ConsoleMessages.Instance.ShowInfoMessage($"\nLas dlls a editar son las siguientes: {ResourceDlls.Count()}");
            foreach (var dll in ResourceDlls)
            {
                ConsoleMessages.Instance.ShowInfoMessage($"{ResourceDlls.IndexOf(dll)}.- {dll}");
            }
            var KeyPressed = ConsoleMessages.Instance.RequestKey("¿Deseas agregar o eliminar? (S/N): ");
            var KeyType = KeyPressed.KeyChar;
            bool _continue = false;
            while (!_continue)
            {
                if (Char.IsLetter(KeyType))
                {
                    var Option = KeyType.ToString().ToLower();
                    switch (Option)
                    {
                        case "s":
                            var AddDelKeyP = ConsoleMessages.Instance.RequestKey("\n¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                            var AddDelKeyType = AddDelKeyP.KeyChar;
                            bool _continue2 = false;
                            if (Char.IsLetter(AddDelKeyType))
                            {
                                while (!_continue2)
                                {
                                    var Option2 = AddDelKeyType.ToString().ToLower();
                                    switch (Option2)
                                    {
                                        case "a":
                                            ResourceDlls = AddRemoveResourceFromList(true, ResourceDlls);

                                            ConsoleMessages.Instance.ShowSystemMessage("\nLa dll se ha agregado correctamente a la lista");
                                            foreach (var dll in ResourceDlls)
                                            {
                                                ConsoleMessages.Instance.ShowInfoMessage($"{ResourceDlls.IndexOf(dll)}.- {dll}");
                                            }
                                            AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                            AddDelKeyType = AddDelKeyP.KeyChar;
                                            Option2 = AddDelKeyType.ToString().ToLower();
                                            break;
                                        case "e":
                                            ResourceDlls = AddRemoveResourceFromList(false, ResourceDlls);

                                            ConsoleMessages.Instance.ShowSystemMessage("\nLa dll se ha eliminado correctamente");
                                            foreach (var dll in ResourceDlls)
                                            {
                                                ConsoleMessages.Instance.ShowInfoMessage($"{ResourceDlls.IndexOf(dll)}.- {dll}");
                                            }
                                            AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                            AddDelKeyType = AddDelKeyP.KeyChar;
                                            Option2 = AddDelKeyType.ToString().ToLower();
                                            break;
                                        case "c":
                                            _continue2 = true;
                                            break;
                                        default:
                                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"A\" para Agregar, \"E\" para Eliminar o \"C\" para Cancelar");
                                            AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                            AddDelKeyType = AddDelKeyP.KeyChar;
                                            Option2 = AddDelKeyType.ToString().ToLower();
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                                AddDelKeyP = ConsoleMessages.Instance.RequestKey("¿Agregar, Eliminar o Cancelar? (A/E/C): ");
                                AddDelKeyType = AddDelKeyP.KeyChar;
                            }
                            _continue = true;
                            break;
                        case "n":
                            _continue = true;
                            break;
                        default:
                            ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar \"S\" para Si o \"N\" para no");
                            KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                            KeyType = KeyPressed.KeyChar;
                            Option = KeyType.ToString().ToLower();
                            break;
                    }
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nOpción no válida: debe ingresar las letras mencionadas");
                    KeyPressed = ConsoleMessages.Instance.RequestKey("Deseas agregar o eliminar más (S/N): ");
                    KeyType = KeyPressed.KeyChar;
                }

            }
            ConsoleMessages.Instance.ShowInfoMessage("\n************************************************");
            return ResourceDlls;
        }
        private List<string> AddRemoveResourceFromList(bool Add, List<string> PossibleDlls)
        {
            if (Add)
            {
                var _add = ConsoleMessages.Instance.RequestAnswer("\nIngrese nombre de la dll a agregar con su extensión: ");
                if (_add.Contains(".dll"))
                {
                    PossibleDlls.Add(_add);
                }
                else
                    ConsoleMessages.Instance.ShowDangerMessage("\nEl dato ingresado no cumple con el formato dll, ingrese la extensión");

            }
            else
            {
                var KeyPressed = ConsoleMessages.Instance.RequestKey("Escriba él número a eliminar: ");
                var KeyType = KeyPressed.KeyChar;
                if (Char.IsDigit(KeyType))
                {
                    int _Index = int.Parse(KeyType.ToString());
                    var StrArr = PossibleDlls.ToArray();
                    PossibleDlls.Remove(StrArr[_Index]);
                }
                else
                {
                    ConsoleMessages.Instance.ShowDangerMessage("\nEl dato ingresado no cumple con el formato de la solicitado, solo numeros porfavor");
                }
            }
            return PossibleDlls;
        }
    }
}
