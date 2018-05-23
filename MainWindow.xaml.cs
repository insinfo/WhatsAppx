using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.OffScreen;
using CefSharp.Wpf;
using System.Diagnostics;
using System.Threading.Tasks;
using CefSharp;
using System.Threading;

namespace WhatsAppx
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        CefSharp.IBrowser browser;
        CefSharp.IBrowserHost host;
        CefSharp.Wpf.ChromiumWebBrowser Browser;  
        TempFileDialogHandler tempFileDialogHandler;
        DBHandlerMySQL dbHandlerMySQL;
        Random randon = null;
        IntPoint btnOpenMenuAnexoXY = null;
        IntPoint searchInputXY = null;
        

        public MySettings Settings { get; private set; }

        public void LoadSettings()
        {
            TxtHost.Text = Settings.Server;
            TxtDataBase.Text = Settings.Database;
            TxtPassword.Password = Settings.Password;
            TxtUser.Text = Settings.User;
            TxtUserAgent.Text = Settings.UserAgent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();           
        }

        public MainWindow()
        {
            randon = new Random();
            var mySett = new MySettings();
            this.Settings = mySett.Read();
            
            dbHandlerMySQL = new DBHandlerMySQL();
            dbHandlerMySQL.Server = Settings.Server;
            dbHandlerMySQL.Port = Settings.Port;
            dbHandlerMySQL.Password = Settings.Password;
            dbHandlerMySQL.User = Settings.User;
            dbHandlerMySQL.Database = Settings.Database;
            dbHandlerMySQL.Connect();           

            InitializeComponent();
            CefSharp.CefSettings settings = new CefSharp.CefSettings();
            settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";
            settings.PersistSessionCookies = true;
            settings.PersistUserPreferences = true;
            settings.UserAgent = Settings.UserAgent;        
            CefSharp.Cef.Initialize(settings);

            Browser = new CefSharp.Wpf.ChromiumWebBrowser();
            Browser.Address = "https://web.whatsapp.com/";
            Browser.FrameLoadEnd += Browser_FrameLoadEnd;
            Browser.ConsoleMessage += Browser_ConsoleMessage;
            tempFileDialogHandler = new TempFileDialogHandler();
            Browser.DialogHandler = tempFileDialogHandler;

            GridBrowser.Children.Add(Browser);
        }

        private void Browser_ConsoleMessage(object sender, CefSharp.ConsoleMessageEventArgs e)
        {
            if (!CheckAccess())
            {
                // On a different thread
                Dispatcher.Invoke(() => TxtConsoleLog.Text += e.Message + Environment.NewLine);
                return;
            }
        }

        public void DisplayOutput(string output)
        {
            if (!CheckAccess())
            {
                // On a different thread
                Dispatcher.Invoke(() => TxtConsoleLog.Text += output);
                return;
            }
        }

        private void Browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            Debug.WriteLine("fim do carregamento ");

            //coloca o jquery na pagina
            browser = Browser.GetBrowser();
            browser.MainFrame.ExecuteJavaScriptAsync(@"var script = document.createElement('script');
            script.src = 'https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js';
            document.getElementsByTagName('head')[0].appendChild(script); 
            function getPosition(el) {
                var xPos = 0;
                var yPos = 0;
 
                while (el) {
                if (el.tagName == 'BODY') {
                    // deal with browser quirks with body/window/document and page scroll
                    var xScroll = el.scrollLeft || document.documentElement.scrollLeft;
                        var yScroll = el.scrollTop || document.documentElement.scrollTop;

                        xPos += (el.offsetLeft - xScroll + el.clientLeft);
                        yPos += (el.offsetTop - yScroll + el.clientTop);
                    } else {
                    // for all other non-BODY elements
                    xPos += (el.offsetLeft - el.scrollLeft + el.clientLeft);
                    yPos += (el.offsetTop - el.scrollTop + el.clientTop);
                }

                el = el.offsetParent;
                }
                return {
                x: xPos,
                y: yPos
                };
            }

            ");
            host = browser.GetHost();

        }
                
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            TriggerClickEvent(searchInputXY);
            //var contatos = dbHandlerMySQL.GetContatos();
            //Console.WriteLine(contatos[0].Nome);
            // Console.WriteLine(dbHandlerMySQL.GetLastMensagem().Media);
            //var script = @"(() => { console.log('sad'); var element = document.activeElement; return element.tagName; })(); ";
            /*var script = @" 
                    setTimeout(function(){ 

                        console.log('inicio'); 
                        $('._3CPl4').trigger('focus');
                        $('._3CPl4').trigger({ type : 'keydown', which : 'A'.charCodeAt(0) });
                        $('.jN-F5 ').val('A');
                        $('._3CPl4').trigger({ type : 'keypress', which : 'A'.charCodeAt(0) });
                        $('.jN-F5 ').val('A');
                        $('._3CPl4').trigger('focus');
                        console.log('fin'); 

                     }, 9000);
            ";
            var task = browser.MainFrame.EvaluateScriptAsync(script);
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    var result = response.Success ? (response.Result ?? "null") : response.Message;
                    Debug.WriteLine(result);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

           */
            //browser.MainFrame.Paste();

            /*KeyEvent k = new KeyEvent();
            k.WindowsKeyCode = 0x41;
            k.FocusOnEditableField = true;
            k.IsSystemKey = false;
            k.Type = KeyEventType.KeyDown;

            browser.GetHost().SendKeyEvent(k);*/
        }
        public int GetTime(bool max = false,bool med = false)
        {
            if (max)
            {
                return randon.Next(10000, 14000);
            }
            if (med)
            {
                return randon.Next(10000, 14000);
            }
            return randon.Next(1550, 3850);
        }

        private async void BtnColar_Click(object sender, RoutedEventArgs e)
        {
            //browser.MainFrame.Paste();
            //LeftMouseClick(170, 231);

            var contatos = dbHandlerMySQL.GetContatos();
            var mensagem = dbHandlerMySQL.GetLastMensagem();
            var mainFrame = browser.MainFrame;

            foreach (Contato contato in contatos)
            {
                var mensagemTexto = mensagem.Texto;               
                var nomeContato = contato.Nome;

                //1º obtem a posição x y da caixa de pesquisa e digita o nome do contato
                var task1 = mainFrame.EvaluateScriptAsync(GetSearchInputXY());
                await task1.ContinueWith(tas1 =>
                {
                    if (!tas1.IsFaulted)
                    {
                        var p1 = tas1.Result.Success ? (tas1.Result.Result ?? "null") : tas1.Result.Message;
                        Debug.WriteLine("GetSearchInputXY " + p1);
                        //clica na caixa de pesquisa
                        searchInputXY = GetPointFromObj(p1);
                        TriggerClickEvent(searchInputXY);                      
                        //digita o nome do contato
                        DigitaNomeContato(nomeContato);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
                //aguarda 3 segundos
                await Task.Delay(GetTime());
                //2º inicia o chat
                var task2 = mainFrame.EvaluateScriptAsync(StartChatWith(nomeContato));
                await task2.ContinueWith(tas2 =>
                {
                    if (!tas2.IsFaulted)
                    {
                        var isOk = tas2.Result.Success ? (tas2.Result.Result ?? "null") : tas2.Result.Message;
                        Debug.WriteLine("StartChatWith " + isOk);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
               
                //5º anexa arquivo se tiver
                if (mensagem.Media != null)
                {
                    //tempFileDialogHandler.FilePath = mensagem.Media;
                    //aguarda 3 segundos
                    await Task.Delay(GetTime());
                    //5º opem menu anexa arquivo
                    var task5 = mainFrame.EvaluateScriptAsync(GetBtnOpenMenuAnexoXY());
                    await task5.ContinueWith(tas5 =>
                    {
                        if (!tas5.IsFaulted)
                        {
                            var p5 = tas5.Result.Success ? (tas5.Result.Result ?? "null") : tas5.Result.Message;
                            Debug.WriteLine("GetBtnOpenMenuAnexoXY " + p5);
                            //clica 
                            btnOpenMenuAnexoXY = GetPointFromObj(p5);
                            TriggerClickEvent(btnOpenMenuAnexoXY);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    //aguarda 3 segundos
                    await Task.Delay(GetTime());
                    //6º clica no botão anexa arquivo
                    var task6 = mainFrame.EvaluateScriptAsync(GetBtnAnexaFotoXY());
                    await task6.ContinueWith(tas6 =>
                    {
                        if (!tas6.IsFaulted)
                        {
                            var p6 = tas6.Result.Success ? (tas6.Result.Result ?? "null") : tas6.Result.Message;
                            Debug.WriteLine("GetBtnAnexaFotoXY " + p6);
                            //clica 
                            TriggerClickEvent(GetPointFromObj(p6));
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    //aguarda 10 segundos
                    await Task.Delay(GetTime(true));
                }
                //aguarda 3 segundos
                await Task.Delay(GetTime());
                //3º obtem a posição x y da caixa mensagem 
                var task3 = mainFrame.EvaluateScriptAsync(GetMessageInputXY());
                await task3.ContinueWith(tas3 =>
                {
                    if (!tas3.IsFaulted)
                    {
                        var p2 = tas3.Result.Success ? (tas3.Result.Result ?? "null") : tas3.Result.Message;
                        Debug.WriteLine("GetMessageInputXY " + p2);
                        //clica na caixa de mensagem
                        TriggerClickEvent(GetPointFromObj(p2));
                        //digita o testo da mensagem
                        DigitaNomeContato(mensagemTexto);
                        //LeftMouseClick(GetPointFromObj(p2).X, GetPointFromObj(p2).Y);
                 }
                }, TaskScheduler.FromCurrentSynchronizationContext());
                await Task.Delay(GetTime()); 
                //4º envia a mensagem
                var task4 = mainFrame.EvaluateScriptAsync(GetBtnSendMessageXY());
                await task4.ContinueWith(tas4 =>
                {
                    if (!tas4.IsFaulted)
                    {
                        var p3 = tas4.Result.Success ? (tas4.Result.Result ?? "null") : tas4.Result.Message;
                        Debug.WriteLine("GetBtnSendMessageXY " + p3);
                        //clica no botão de envio de mensagem
                        TriggerClickEvent(GetPointFromObj(p3)); 
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
                await Task.Delay(3000);
                Debug.WriteLine("fexa o menu " + btnOpenMenuAnexoXY.X + ";" + btnOpenMenuAnexoXY.Y);
                TriggerClickEvent(btnOpenMenuAnexoXY);
                //aguarda 3 segundos
                await Task.Delay(GetTime(false,true));
                TriggerClickEvent(searchInputXY);
                Debug.WriteLine("fim do envio da memsagem");                
            }
        }                      

        public string StartChatWith(string contatoNome)
        {

            var scriptInitChat = @"
            //procura na lista o nome do contato informado
            //e simula a execução de click do mouse
            (() => { 
            var contatos = document.querySelectorAll('._1wjpf[title]');
            for (var i = 0; i < contatos.length; i++)
                        {
                            var contatoNome = contatos[i].textContent;
                            var element = contatos[i];
                            var contatoInput = '" + contatoNome + @"';
                            if (contatoNome.trim().toLowerCase() === contatoInput.trim().toLowerCase())
                            {
                                var event = document.createEvent('MouseEvents');
                    event.initEvent('mousedown', true, true);
                    element.dispatchEvent(event);
                    //console.log('executou');
                    //console.log(contatoNome);
                    console.log('StartChatWith '+contatoInput);
                    return true;
                }
            }
            return false;
            })();
            ";

            return scriptInitChat;
        }

        public string GetSearchInputXY()
        {
            //script para pegar a posição exata do botão de pesquisa
            var scriptGetXYsearchInput = @"
                (() => {        //retorna a posição do input de pesquisa
                    var elem = document.querySelector('input.jN-F5');                
                    var xy = getPosition(elem).x + ';' + getPosition(elem).y;
                    console.log('GetSearchInputXY ' + xy);
                    return xy;  
                })();
            ";
            return scriptGetXYsearchInput;
        }

        public string GetMessageInputXY()
        {
            //script para pegar a posição exata do botão de pesquisa
            var scriptGetXYsearchInput = @"
               (() => {        //retorna a posição do input de mensagem
                var elem = document.querySelector('._2S1VP');
                var xy = getPosition(elem).x + ';' + getPosition(elem).y;
                console.log('GetMessageInputXY ' + xy);
                return xy;               
                })();
            ";           
            return scriptGetXYsearchInput;
        }

        public string GetBtnOpenMenuAnexoXY()
        {            
            var script = @"
               (() => {       
                var elem = document.querySelector('div[title=Anexar]');    
                var xy = getPosition(elem).x + ';' + getPosition(elem).y;
                console.log('GetBtnOpenMenuAnexoXY ' + xy);
                return xy;             
                })();
            ";
            return script;
        }

        public string GetBtnAnexaFotoXY()
        {
            var script2 = @"
               (() => {       
                var elem = document.querySelector('.GK4Lv span');  
                var xy = getPosition(elem).x + ';' + getPosition(elem).y;
                console.log('GetBtnAnexaFotoXY ' + xy);
                return xy;              
                })();
            ";
            return script2;
        }

        public string AnexaArquivoScript()
        {
            //script anexar arquivo
            var scriptAnexaArquivo = @"
               (() => {        
                //menu anexar div[title=Anexar]
                var event = document.createEvent('MouseEvents');
                event.initEvent('mousedown', true, true);
                document.querySelector('div[title=Anexar]').dispatchEvent(event);

                //botão anexar foto/video .GK4Lv
                var event = document.createEvent('MouseEvents');
                event.initEvent('click', true, false);
                document.querySelector('._10anr input[type=file]').dispatchEvent(event);
                return true;
                })();
            ";
            return scriptAnexaArquivo;
        }

        public string GetBtnSendMessageXY()
        {
            //script para pegar a posição exata do botão de eviar mensagem
            var scriptGetXYsearchInput = @"
               (() => {        //retorna a posição exata do botão de eviar mensagem
                // btn enviar mensagem quando não tem anexo
                var elem = document.querySelector('button._2lkdt span');
                // btn enviar mensagem quando tem anexo
                elem = elem ? elem : document.querySelector('div._3hV1n span');               
                var xy = getPosition(elem).x + ';' + getPosition(elem).y;
                console.log('GetBtnSendMessageXY ' + xy);
                return xy;              
                })();
            ";
            return scriptGetXYsearchInput;
        }
               
        public string CheckIsLoading()
        {
            var scriptCheckIsLoading = @"
            (() => {
                //._2xarx class do loading animation
                var btn = document.querySelector('body ._2xarx');
                if (btn != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            })();
            ";
            return scriptCheckIsLoading;
        }

        //simula digitação
        public void DigitaNomeContato(string nome)
        {
            KeyEvent k = new KeyEvent();
            k.FocusOnEditableField = true;
            k.IsSystemKey = false;
            k.Type = KeyEventType.Char;

            foreach (char c in nome)
            {
                k.WindowsKeyCode = ConvertCharToVirtualKey(c);
                host.SendKeyEvent(k);
            }

        }
        //simula click do mouse
        public void TriggerClickEvent(IntPoint p)
        {
            host.SetFocus(true);
            host.SendMouseClickEvent(p.X, p.Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
            host.SendMouseClickEvent(p.X, p.Y, MouseButtonType.Left, true, 1, CefEventFlags.None);
        }
        
        /***************** UTILS ********************/

        public IntPoint GetPointFromObj(object str)
        {
            int x = 0;
            int y = 0;
            x = ParseInt(str.ToString().Split(';')[0]);
            y = ParseInt(str.ToString().Split(';')[1]);
            var p = new IntPoint();
            p.X = x;
            p.Y = y;
            return p;
        }

        //This is a replacement for Cursor.Position in WinForms
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        public int ParseInt(string s, bool alwaysRoundDown = false)
        {
            try
            {
                //converts null/empty strings to zero
                if (string.IsNullOrEmpty(s)) return 0;

                if (!s.Contains('.')) return int.Parse(s);

                string[] parts = s.Split('.');
                int i = int.Parse(parts[0]);
                if (alwaysRoundDown || parts.Length == 1) return i;

                int digitAfterPoint = int.Parse(parts[1]);
                return (digitAfterPoint < 5) ? i : i + 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
            
        }

        public static int ConvertCharToVirtualKey(char ch)
        {
            short vkey = VkKeyScan(ch);
            int retval = (vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) retval |= (int)KeyCodes.Keys.Shift;
            if ((modifiers & 2) != 0) retval |= (int)KeyCodes.Keys.Control;
            if ((modifiers & 4) != 0) retval |= (int)KeyCodes.Keys.Alt;
            return retval;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings.Server = TxtHost.Text;
            Settings.Database = TxtDataBase.Text;
            Settings.Password = TxtPassword.Password;
            Settings.User = TxtUser.Text;
            Settings.UserAgent = TxtUserAgent.Text;

            Settings.Save();
        }
    }
    //subistitui o file dialog do navegar
    public class TempFileDialogHandler : IDialogHandler
    {
        public string FilePath { get; set; } = @"c:\teste.jpeg";

        public bool OnFileDialog(IWebBrowser browserControl, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
        {
            defaultFilePath = @"c:\teste.jpeg";
            Debug.WriteLine("OnFileDialog ");
            //callback = new FileDialogCallback();
            //MessageBox.Show("dfg");
            callback.Continue(selectedAcceptFilter, new List<string> { FilePath });            
            return true;
        }
    }
    public class FileDialogCallback : IFileDialogCallback
    {
        public bool IsDisposed  { get; set; }

        public void Cancel()
        {
           
        }

        public void Continue(int selectedAcceptFilter, List<string> filePaths)
        {
            filePaths = new List<string> { @"c:\teste.jpeg"};
        }

        public void Dispose()
        {
            
        }
    }
}
