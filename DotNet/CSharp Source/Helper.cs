// Helper.cs
//
// Provide win32 struc/functions access and list all const used in the library
//
// Author : Thierry Parent
//
// HomePage :  http://www.codeproject.com/csharp/TraceTool.asp
// Download :  http://sourceforge.net/projects/tracetool/
// See License.txt for license information

using System;
using System.Text;

#if !NETSTANDARD1_6
using System.Runtime.InteropServices;// for DDL import
#endif

using System.Diagnostics;  // Process
using System.Collections.Generic;
using System.Collections.ObjectModel;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ConvertIfStatementToNullCoalescingExpression
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InlineOutVariableDeclaration
// ReSharper disable UseStringInterpolation
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable UseNullPropagation
// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable UsePatternMatching
// ReSharper disable ArrangeAccessorOwnerBody

namespace TraceTool
{
    /// <summary>
    /// Specify a font detail for traces columns items and members.
    /// </summary>
    internal class FontDetail
    {
        internal int ColId;
        internal bool Bold;
        internal bool Italic;
        internal Int32 Color;    // To reduce the number assembly reference, the Color structure is not used. Use YourColor.ToArgb() instead.
        internal int Size;
        internal string FontName;
    }

    //------------------------------------------------------------------------------

    /// <summary>
    /// List of parsed object.
    /// </summary>

    public class ParsedObjectList : KeyedCollection<String, Object>
    {
        // The parameter less constructor of the base class creates a KeyedCollection with an internal dictionary. 
        // public ParsedObjectList() : base() { }

        /// <summary>
        /// This is the only method that absolutely must be overridden,
        /// because without it the KeyedCollection cannot extract the
        /// keys from the items. The input parameter type is the 
        /// second generic type argument, in this case OrderItem, and 
        /// the return value type is the first generic type argument,
        /// in this case int.
        /// </summary>

        protected override string GetKeyForItem(object objToSend)
        {
            Type oType = objToSend.GetType();
            string hashCode = new StringBuilder().Append(oType.Name).Append("@").
               Append(objToSend.GetHashCode().ToString("X2")).ToString();
            return hashCode;
        }

        /// <summary>
        /// </summary>
        public bool ContainsKey(Object key)  // virtual
        {
            return Contains(key);
        }
    }

    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------

    /// 2 ways to send traces : windows messages or socket messages. Under ASP, you must use Socket
    public enum SendMode
    {
#if !NETSTANDARD1_6
        /// Windows message
        WinMsg = 1,
#endif
        /// Socket message
        Socket = 2,
        /// Socket message
        WebSocket = 3,
        /// No messages are send (use local log)
        None = 4
    }

    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------

    /// <summary>
    /// What information to display
    /// </summary>
    [Flags]
    public enum TraceDisplayFlags
    {
        /// <summary>
        /// show modifiers (public class,...)
        /// </summary>
        ShowModifiers = 1,
        /// <summary>
        /// show class info (assembly,guid,...) and bases classes names
        /// </summary>
        ShowClassInfo = 2,
        /// <summary>
        /// show fields values
        /// </summary>
        ShowFields = 4,
        /// <summary>
        /// show custom attributes
        /// </summary>
        ShowCustomAttributes = 8,
        /// <summary>
        /// show non public (private and protected) members
        /// </summary>
        ShowNonPublic = 16,
        /// <summary>
        /// show Inherited members
        /// </summary>
        ShowInheritedMembers = 32,
        /// <summary>
        /// show events (delegates)
        /// </summary>
        ShowEvents = 64,
        /// <summary>
        /// show methods and constructors
        /// </summary>
        ShowMethods = 128,
        /// <summary>
        /// show documentation for type, fields, methods,..
        /// </summary>
        ShowDoc = 256
    }

    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------

    /// <summary>
    /// Define all const and windows functions used by the TraceTool namespace
    /// </summary>
    public class TraceConst
    {
        // general const

        /// <summary> the windows constant to use to send bloc of data across process </summary>
        // ReSharper disable InconsistentNaming
        public const int WM_COPYDATA = 0x004A;
        /// <summary> identification code 'traceTool'. Other code are discarded by the server </summary>
        public const int WMD = 123;

        // Icones const

        /// <summary> Use the default Icon on the gutter for the trace </summary>
        public const int CST_ICO_DEFAULT = -1;
        /// <summary> Use the 'form' Icon on the gutter for the trace </summary>
        public const int CST_ICO_FORM = 0;
        /// <summary> Use the 'component' Icon on the gutter for the trace </summary>
        public const int CST_ICO_COMPONENT = 1;
        /// <summary> Use the 'control' Icon on the gutter for the trace </summary>
        public const int CST_ICO_CONTROL = 3;
        /// <summary> Use the 'property' Icon on the gutter for the trace </summary>
        public const int CST_ICO_PROP = 5;
        /// <summary> Use the 'menu' Icon on the gutter for the trace </summary>
        public const int CST_ICO_MENU = 15;
        /// <summary> Use the 'menu item' Icon on the gutter for the trace </summary>
        public const int CST_ICO_MENU_ITEM = 16;
        /// <summary> Use the 'Collection Item' Icon on the gutter for the trace </summary>
        public const int CST_ICO_COLLECT_ITEM = 21;
        /// <summary> Use the 'warning' Icon on the gutter for the trace </summary>
        public const int CST_ICO_WARNING = 22;
        /// <summary> Use the 'Error' Icon on the gutter for the trace </summary>
        public const int CST_ICO_ERROR = 23;
        /// <summary> Use the 'Info' Icon on the gutter for the trace </summary>
        public const int CST_ICO_INFO = 24;     // default

        // viewer kind
        /// <summary> viewer kind : default viewer, no icon</summary>
        public const int CST_VIEWER_NONE = 0;
        /// <summary> viewer kind : dump viewer </summary>
        public const int CST_VIEWER_DUMP = 1;
        /// <summary> viewer kind : xml viewer </summary>
        public const int CST_VIEWER_XML = 2;
        /// <summary> viewer kind : table viewer </summary>
        public const int CST_VIEWER_TABLE = 3;
        /// <summary> viewer kind : stack </summary>
        public const int CST_VIEWER_STACK = 4;
        /// <summary> viewer kind : bitmap viewer </summary>
        public const int CST_VIEWER_BITMAP = 5;
        /// <summary> viewer kind : object structure </summary>
        public const int CST_VIEWER_OBJECT = 6;
        /// <summary> viewer kind : object value </summary>
        public const int CST_VIEWER_VALUE = 7;
        /// <summary> viewer kind : enter method </summary>
        public const int CST_VIEWER_ENTER = 8;
        /// <summary> viewer kind : exit method </summary>
        public const int CST_VIEWER_EXIT = 9;
        /// <summary> viewer kind : text added to default viewer </summary>
        public const int CST_VIEWER_TXT = 10;

        // plugin const

        /// <summary> Ask to receive OnAction event </summary>
        public const int CST_PLUG_ONACTION = 1;
        /// <summary>  Ask to receive OnBeforeDelete event </summary>
        public const int CST_PLUG_ONBEFOREDELETE = 2;
        /// <summary>  Ask to receive OnTimer event </summary>
        public const int CST_PLUG_ONTIMER = 4;

        // plugin resource kind

        /// <summary> Button on right</summary>
        public const int CST_RES_BUT_RIGHT = 1;
        /// <summary> Button on left</summary>
        public const int CST_RES_BUT_LEFT = 2;
        /// <summary> Label on right</summary>
        public const int CST_RES_LABEL_RIGHT = 3;
        /// <summary> Label on right HyperLink</summary>
        public const int CST_RES_LABELH_RIGHT = 4;
        /// <summary> Label on left</summary>
        public const int CST_RES_LABEL_LEFT = 5;
        /// <summary> Label on left hyperlink</summary>
        public const int CST_RES_LABELH_LEFT = 6;
        /// <summary> Item menu in the Actions Menu</summary>
        public const int CST_RES_MENU_ACTION = 7;
        /// <summary> Item menu in the Windows Menu. Call CreateResource on the main win trace to create this menu item</summary>
        public const int CST_RES_MENU_WINDOW = 8;

        // plugin resource id

        /// <summary> Cut. Same as copy then delete </summary>
        public const int CST_ACTION_CUT = 1;
        /// <summary> Copy </summary>
        public const int CST_ACTION_COPY = 2;
        /// <summary> Delete selected </summary>
        public const int CST_ACTION_DELETE = 3;
        /// <summary> Select all </summary>
        public const int CST_ACTION_SELECT_ALL = 4;
        /// <summary> Resize columns </summary>
        public const int CST_ACTION_RESIZE_COLS = 5;
        /// <summary> View trace info </summary>
        public const int CST_ACTION_VIEW_INFO = 6;
        /// <summary> View properties </summary>
        public const int CST_ACTION_VIEW_PROP = 7;
        /// <summary> Pause </summary>
        public const int CST_ACTION_PAUSE = 8;
        /// <summary> SaveToFile </summary>
        public const int CST_ACTION_SAVE = 9;
        /// <summary> Clear all </summary>
        public const int CST_ACTION_CLEAR_ALL = 10;
        /// <summary> Close win </summary>
        public const int CST_ACTION_CLOSE_WIN = 11;
        /// <summary> Resume from Pause </summary>
        public const int CST_ACTION_RESUME = 12;
        /// <summary> TracesInfo label </summary>
        public const int CST_ACTION_LABEL_INFO = 20;
        /// <summary> LabelLogFile label </summary>
        public const int CST_ACTION_LABEL_LOGFILE = 21;
        /// <summary> View Main trace </summary>
        public const int CST_ACTION_VIEW_MAIN = 50;
        /// <summary> ODS </summary>
        public const int CST_ACTION_VIEW_ODS = 51;
        /// <summary> XML trace -> Tracetool XML traces </summary>
        public const int CST_ACTION_OPEN_XML = 52;
        /// <summary> Event log </summary>
        public const int CST_ACTION_EVENTLOG = 53;
        /// <summary> Tail </summary>
        public const int CST_ACTION_TAIL = 54;

        // command

        // INTERNAL
        //--------------------------------------------------------------------------
        /// <summary>VIEWER INTERNAL : enter debug mode  </summary>
        public const int CST_ENTER_DEBUG_MODE = 107;   // Param : none
        /// <summary>VIEWER INTERNAL : leave debug mode  </summary>
        public const int CST_LEAVE_DEBUG_MODE = 108;   // Param : none
        /// <summary>VIEWER INTERNAL : Open tail file  </summary>
        public const int CST_OPEN_TAIL = 109;   // Param : file name
        /// <summary>VIEWER INTERNAL : Open xml file on a new window (don't confuse with CST_LOADXML)  </summary>
        public const int CST_OPEN_XML = 113;   // Param : file name
        /// <summary>VIEWER INTERNAL : the user interface ask to retrieve an object  </summary>
        public const int CST_GET_OBJECT = 700;   // param : node
        /// <summary>Flush remaining traces to server</summary>
        public const int CST_FLUSH = 800;  // param : event id


        // Wintrace / WinWatch. New commands should be added before 80
        //--------------------------------------------------------------------------
        /// <summary>WinTrace.GotoFirstNode()  </summary>
        public const int CST_GOTO_FIRST_NODE = 80;    // param : node
        /// <summary>WinTrace.GotoLastNode()  </summary>
        public const int CST_GOTO_LAST_NODE = 81;    // param : node
        /// <summary>WinTrace.FindNext(forward)  </summary>
        public const int CST_FIND_NEXT = 82;    // param : node
        /// <summary>WinTrace.GotoBookmark(pos)  </summary>
        public const int CST_GOTO_BOOKMARK = 83;
        /// <summary>WinTrace.ClearBookmark()  </summary>
        public const int CST_CLEAR_BOOKMARK = 84;    // param : node
        /// <summary>WinTrace.ClearFilter()  </summary>
        public const int CST_CLEAR_FILTER = 85;    // param : node
        /// <summary>WinTrace.AddFilter(column,compare,text)  </summary>
        public const int CST_ADD_FILTER = 86;    //  
        /// <summary>WinTrace.ApplyFilter(ConditionAnd, ShowMatch,IncludeChildren)  </summary>
        public const int CST_APPLY_FILTER = 87;    // param : integer (3 bools)
        /// <summary>Columns widths</summary>
        public const int CST_TREE_COLUMNWIDTH = 93;
        /// <summary>change the tree to display multiple column</summary>
        public const int CST_TREE_MULTI_COLUMN = 95;  // Param : Main column index
        /// <summary>change the columns titles</summary>
        public const int CST_TREE_COLUMNTITLE = 96;
        /// <summary>display tree windows</summary>
        public const int CST_DISPLAY_TREE = 97;
        /// <summary>new name of the tree</summary>
        public const int CST_TREE_NAME = 98;  // param : the new name of the tree (use CST_USE_TREE just before to specify the tree)
        /// <summary>the tree to use for other command</summary>
        public const int CST_USE_TREE = 99;  // param : Id (CLSID for example) of the tree to use for other command.
        /// <summary>The command to clear all nodes on the viewer</summary>
        public const int CST_CLEAR_ALL = 104;  // no param
        /// <summary>Close the window (wintrace or winwatch)</summary>
        public const int CST_CLOSE_WIN = 105;   // no param. Close winwatch or wintrace
        /// <summary>Watch Window name</summary>
        public const int CST_WINWATCH_NAME = 110;  // param : window name
        /// <summary>Watch Window ID</summary>
        public const int CST_WINWATCH_ID = 111;  // param : Window id
        /// <summary>watch name</summary>
        public const int CST_WATCH_NAME = 112;  // param : watch name
        /// <summary>Save to text file</summary>
        public const int CST_SAVETOTEXT = 559;  // save to text file, parameter : filename
        /// <summary>Save to XML file</summary>
        public const int CST_SAVETOXML = 560;  // save to  XML file, parameter : filename
        /// <summary>Load XML file</summary>
        public const int CST_LOADXML = 561;  // load an XML file to the current wintrace
        /// <summary>set the log file</summary>
        public const int CST_LOGFILE = 562;  // set the log file for a wintrace

        // Wintrace plugins
        //--------------------------------------------------------------------------

        /// <summary>link a wintrace to a plugin</summary>
        public const int CST_LINKTOPLUGIN = 563;  // link a wintrace to a plugin
        /// <summary>create a resource on a wintrace</summary>
        public const int CST_CREATE_RESOURCE = 564;
        /// <summary>set the text resource</summary>
        public const int CST_SET_TEXT_RESOURCE = 565;
        /// <summary>disable a resource </summary>
        public const int CST_DISABLE_RESOURCE = 566;

        // TTrace
        //--------------------------------------------------------------------------

        /// <summary>TTrace.Find (text, bool Sensitive, bool WholeWord , bool highlight )</summary>
        public const int CST_FIND_TEXT = 100;  // param : int (Sensitive+WholeWord+highlight) , string
        /// <summary>The command to bring the trace tool to front</summary>
        public const int CST_SHOW = 102;  // param : 1/0
        /// <summary>Close the viewer (shutdown)</summary>
        public const int CST_CLOSE_VIEWER = 106;   // no param : quit tracetool

        // Node
        //--------------------------------------------------------------------------

        /// <summary>the unique ID (from the server point of view) of the node (preferably a GUID)</summary>
        public const int CST_TRACE_ID = 101;   // param : CLSID
        /// <summary>the index of the Icon to use (CST_ICO_INFO, CST_ICO_WARNING,...)</summary>
        public const int CST_ICO_INDEX = 103;   // param : image index
        /// <summary>ITraceNode.GotoNextSibling ()  </summary>
        public const int CST_GOTO_NEXTSIBLING = 114;   //  param : node
        /// <summary>ITraceNode.GotoPrevSibling ()  </summary>
        public const int CST_GOTO_PREVSIBLING = 115;   //  param : node
        /// <summary>ITraceNode.GotoFirstChild  ()  </summary>
        public const int CST_GOTO_FIRST_CHILD = 116;   //  param : node
        /// <summary>ITraceNode.GotoLastChild   ()  </summary>
        public const int CST_GOTO_LAST_CHILD = 117;   //  param : node
        /// <summary>ITraceNode.SetBookmark (bool enabled)  </summary>
        public const int CST_SET_BOOKMARK = 122;   //  param : int
        /// <summary>ITraceNode.SetVisible  (visible)  </summary>
        public const int CST_VISIBLE_NODE = 123;   //  param : int
        /// <summary>Delete the node on the viewer</summary>
        public const int CST_CLEAR_NODE = 300;  // param : the node to clear
        /// <summary>Delete children node on the viewer</summary>
        public const int CST_CLEAR_SUBNODES = 301;  // param : the parent node
        /// <summary>The Thread ID of the sender thread (optional).Used when tracing multiple thread (active by default)</summary>
        public const int CST_THREAD_ID = 302;  // param : thread ID
        /// <summary>The process name (optional).Used when tracing multiple process </summary>
        public const int CST_PROCESS_NAME = 303;  // param process name
        /// <summary>The time of trace</summary>
        public const int CST_MESSAGE_TIME = 304;  // param : the time of the message
        /// <summary>Thread name (Java or user defined)</summary>
        public const int CST_THREAD_NAME = 305;  // param : thread name
        /// <summary>Client Ip address</summary>
        public const int CST_IP = 306;   // param : client IP address   
        /// <summary>Command to create a new trace node</summary>
        public const int CST_NEW_NODE = 550;  // param : parent node ID
        /// <summary>The left message ("traces column")</summary>
        public const int CST_LEFT_MSG = 551;  // param : msg
        /// <summary>the right message ("Comment column")</summary>
        public const int CST_RIGHT_MSG = 552;  // param : msg
        /// <summary>set the node as 'Selected' by the user</summary>
        public const int CST_SELECT_NODE = 553;  // set the node as 'Selected' by the user.  param : Node id
        /// <summary>use an existing node</summary>
        public const int CST_USE_NODE = 555;  // param : Node id
        /// <summary>The left message to append to "traces column"</summary>
        public const int CST_APPEND_LEFT_MSG = 556;  // param : msg
        /// <summary>The right message to append to "Comment column"</summary>
        public const int CST_APPEND_RIGHT_MSG = 557;  // param : msg
        /// <summary>Focus to the node</summary>
        public const int CST_FOCUS_NODE = 558;  // Focus to the node.
        /// <summary>Font detail : ColId Bold Italic Color(BGR) size  Fontname</summary>
        public const int CST_FONT_DETAIL = 567;
        /// <summary>Background color</summary>
        public const int CST_BACKGROUND_COLOR = 568;   // param : background color

        // Members
        //--------------------------------------------------------------------------

        /// <summary>Command to create a member for the current trace node</summary>
        public const int CST_CREATE_MEMBER = 500;  // param : Member name
        /// <summary>Member Font detail : ColId Bold Italic Color(BGR) size  Fontname</summary>
        public const int CST_MEMBER_FONT_DETAIL = 501;
        /// <summary>The text of the second member column</summary>
        public const int CST_MEMBER_COL2 = 502;  // param : info col 2
        /// <summary>Viewer kind id</summary>
        public const int CST_MEMBER_VIEWER_KIND = 503;   // param : viewer id
        /// <summary>The text of the third member column</summary>
        public const int CST_MEMBER_COL3 = 504;  // param : info col 3
        /// <summary>Add the member. Close the previous CST_CREATE_MEMBER </summary>
        public const int CST_ADD_MEMBER = 505;  // add member to upper level. No param (for now)
                                                // ReSharper restore InconsistentNaming

    }   // TraceConst class


    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------

#if !NETSTANDARD1_6
    /// <summary>
    /// The windows structure to send data to another process
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)] // convert string to unicode string
                                                                     // ReSharper disable once InconsistentNaming
    public struct COPYDATASTRUCT
    {
        /// <summary>
        /// The identifier of the message (checked at destination)
        /// </summary>
        public IntPtr dwData;// int
        /// <summary>
        /// The number of byte to send
        /// </summary>
        public int cbData;
        /// <summary>
        /// The message to send
        /// </summary>
        public IntPtr lpData; //string
    }
#endif

    //----------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------

    /// <summary>
    /// Define all windows functions used by the TraceTool namespace
    /// </summary>

    public class Helper
    {
#if !NETSTANDARD1_6

        /// <summary>
        /// The windows function that send a message to a windows handle
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
           int hWnd,       // handle to destination window
           uint msg,       // message
           IntPtr wParam,  // first message parameter
           IntPtr lParam   //COPYDATASTRUCT second message parameter
           );

        //----------------------------------------------------------------------------------
        /// <summary>
        /// The windows function that send a message to a windows handle
        /// </summary>
        [DllImport("user32.Dll")]
        public static extern IntPtr PostMessage(
           IntPtr hWnd,
           int msg,
           int wParam,
           int lParam);

        //----------------------------------------------------------------------------------
        /// <summary>
        /// The windows function that search a windows
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int FindWindow(
           string lpszClass,
           string lpszWindow);

#endif

        //----------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------

        /// <summary>
        /// Return the current thread id (Win Ce / win 32)
        /// </summary>
        public static string GetCurrentThreadId()
        {
            return "0x" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("X2");
            //return "0x"+AppDomain.GetCurrentThreadId().ToString ("X2"); // obsolete, use CurrentThread.ManagedThreadId
        }

        //----------------------------------------------------------------------------------

        /// <summary>
        /// Return the current process name with path (Win Ce / win 32) without extension
        /// </summary>
        public static string GetCurrentProcessName()
        {
            return Process.GetCurrentProcess().ProcessName;
        }

        //----------------------------------------------------------------------------------

        /// <summary>
        /// Return a new System.Guid object.
        /// </summary>
        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        //----------------------------------------------------------------------------------
        /// <summary>
        /// convert a ARGB color (dotnet) to a BGR color (windows)
        /// </summary>
        public static int ARGB_to_BGR(int rgb)
        {
            // remove Alpha blending
            rgb &= 0xFFFFFF;
            // extract the 3 colors
            int b = rgb & 0xff;
            int g = (rgb >> 8) & 0xff;
            int r = (rgb >> 0x10) & 0xff;
            // recompose in BGR
            return (b << 0x10) + (g << 8) + r;
        }

        //----------------------------------------------------------------------------------
        //if NETFULL && !NETSTANDARD1_6 && !NETSTANDARD2_0 
        // convert a System.Windows.Media.Color color to ARGB  
        //public static uint ToArgb(System.Windows.Media.Color color)
        //{
        //   return (uint)((((color.A << 0x18) | (color.R << 0x10)) | (color.G << 8)) | color.B);
        //}
        //endif

        //----------------------------------------------------------------------

        /// <summary>
        /// Html encode. To reduce dependencies, the HttpUtility.HtmlEncode is reproduce here, with StringBuilder as target (faster)
        /// </summary>
        public static void HtmlEncode(string s, StringBuilder target)
        {
            if (s == "")
                return;

            int length = s.Length;
            int startIndex = 0;
            int currentIndex = 0;
            while (currentIndex < length)
            {

                char ch = s[currentIndex];
                switch (ch)
                {
                    case '<':
                        if (startIndex < currentIndex)
                            target.Append(s, startIndex, currentIndex - startIndex);
                        target.Append("&lt;");
                        startIndex = currentIndex + 1;
                        break;

                    case '>':
                        if (startIndex < currentIndex)
                            target.Append(s, startIndex, currentIndex - startIndex);
                        target.Append("&gt;");
                        startIndex = currentIndex + 1;
                        break;

                    case '"':
                        if (startIndex < currentIndex)
                            target.Append(s, startIndex, currentIndex - startIndex);
                        target.Append("&quot;");
                        startIndex = currentIndex + 1;
                        break;

                    case '&':
                        if (startIndex < currentIndex)
                            target.Append(s, startIndex, currentIndex - startIndex);
                        target.Append("&amp;");
                        startIndex = currentIndex + 1;
                        break;

                    default:
                        if ((ch < ' ') || ((ch >= '\x00a0') && (ch < 'Ā')))  // 0 to 31 and 160 to 256
                        {
                            if (startIndex < currentIndex)
                                target.Append(s, startIndex, currentIndex - startIndex);
                            target.Append("&#");
                            target.Append(((int)ch).ToString());  // System.Globalization.NumberFormatInfo.InvariantInfo
                            target.Append(';');
                            startIndex = currentIndex + 1;
                        } // else no encoding
                        break;
                } // switch
                currentIndex++;
            }
            target.Append(s, startIndex, length - startIndex);
        }

        //------------------------------------------------------------------------------

        /// code only
        internal static void AddCommand(List<string> commandList, int code)
        {
            var res = String.Format("{0,5}", code);
            commandList.Add(res);
        }

        //------------------------------------------------------------------------------
        /// code + int
        internal static void AddCommand(List<string> commandList, int code, int intValue)
        {
            commandList.Add(String.Format("{0,5}{1,11}", code, intValue));
        }

        //------------------------------------------------------------------------------
        /// code + bool
        internal static void AddCommand(List<string> commandList, int code, bool boolValue)
        {
            if (boolValue)
                commandList.Add(String.Format("{0,5}{1,11}", code, 1));
            else
                commandList.Add(String.Format("{0,5}{1,11}", code, 0));
        }

        //------------------------------------------------------------------------------
        /// code + string
        internal static void AddCommand(List<string> commandList, int code, string strValue)
        {
            commandList.Add(String.Format("{0,5}{1}", code, strValue));
        }

        //------------------------------------------------------------------------------
        /// code + int + string
        internal static void AddCommand(List<string> commandList, int code, int intValue, string strValue)
        {
            commandList.Add(String.Format("{0,5}{1,11}{2}", code, intValue, strValue));
        }
        //------------------------------------------------------------------------------
        /// code + int + int + string
        internal static void AddCommand(List<string> commandList, int code, int intValue1, int intValue2, string strValue)
        {
            commandList.Add(String.Format("{0,5}{1,11}{2,11}{3}", code, intValue1, intValue2, strValue));
        }
        //------------------------------------------------------------------------------
        /// code + int + int + int + string
        internal static void AddCommand(List<string> commandList, int code, int intValue1, int intValue2, int intValue3, string strValue)
        {
            commandList.Add(String.Format("{0,5}{1,11}{2,11}{3,11}{4}", code, intValue1, intValue2, intValue3, strValue));
        }

        //------------------------------------------------------------------------------

    } // Helper class
}    // namespace TraceTool

