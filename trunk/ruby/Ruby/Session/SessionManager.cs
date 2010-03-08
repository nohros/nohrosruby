using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using log4net;
using Nohros;

namespace Nohros.Ruby
{
    public sealed class SessionManager
    {
        delegate bool RbCommandHandler(Session session, Stack<RbNode> stack);

        #region command structures
        /// <summary>
        /// A structure that represents a node in the command stack
        /// </summary>
        struct RbNode
        {
            public string data; /* the node data */
            public char pi; /* the parameter identification */

            /// <summary>
            /// Initializes a new instance of the RbNode structure by using the specified node data and
            /// identification.
            /// </summary>
            /// <param name="data">The node data</param>
            /// <param name="pi">The parameter identification</param>
            public RbNode(string data, char pi)
            {
                this.data = data;
                this.pi = pi;
            }
        }

        struct RbCommand
        {
            public string cmd;
            public RbCommandHandler proc;

            /// <summary>
            /// Initializes a new instance of the RbCommand class by using the specified command name and handler
            /// </summary>
            /// <param name="cmd">The name of the command</param>
            /// <param name="proc">A delegate used to process the command</param>
            public RbCommand(string cmd, RbCommandHandler proc)
            {
                this.cmd = cmd;
                this.proc = proc;
            }
        }

        struct RbVocabulary
        {
            public RbCommand[] cmds;
            public RbVocabulary(RbCommand[] subvoc)
            {
                cmds = subvoc;
            }
        }
        #endregion

        #region know commands
        static RbCommand[] rb_cmd = {
            new RbCommand("NOP", nop)
        };

        static RbCommand[] rb_cmd_u ={
            new RbCommand("USR", USR)
        };

        static RbVocabulary[] commands = {
            new RbVocabulary(rb_cmd), /* A */
            new RbVocabulary(rb_cmd), /* B */
            new RbVocabulary(rb_cmd), /* C */
            new RbVocabulary(rb_cmd), /* D */
            new RbVocabulary(rb_cmd), /* E */
            new RbVocabulary(rb_cmd), /* F */
            new RbVocabulary(rb_cmd), /* G */
            new RbVocabulary(rb_cmd), /* H */
            new RbVocabulary(rb_cmd), /* I */
            new RbVocabulary(rb_cmd), /* J */
            new RbVocabulary(rb_cmd), /* K */
            new RbVocabulary(rb_cmd), /* L */
            new RbVocabulary(rb_cmd), /* M */
            new RbVocabulary(rb_cmd), /* N */
            new RbVocabulary(rb_cmd), /* O */
            new RbVocabulary(rb_cmd), /* P */
            new RbVocabulary(rb_cmd), /* Q */
            new RbVocabulary(rb_cmd), /* R */
            new RbVocabulary(rb_cmd), /* S */
            new RbVocabulary(rb_cmd), /* T */
            new RbVocabulary(rb_cmd_u), /* U */
            new RbVocabulary(rb_cmd), /* V */
            new RbVocabulary(rb_cmd), /* W */
            new RbVocabulary(rb_cmd), /* X */
            new RbVocabulary(rb_cmd), /* Y */
            new RbVocabulary(rb_cmd)  /* Z */
        };
        #endregion

        static ILog logger = LogManager.GetLogger(Const.RUBY_LOGGER_NAME);

        /// <summary>
        /// Creates a new <see cref="Session"/> object and starts the data receiving operation.
        /// </summary>
        /// <returns></returns>
        public static Session NewSession(Socket fd, SocketServer ss)
        {
            if (fd == null)
                throw new ArgumentNullException("socket");

            Session session = new Session();
            session.Socket = fd;
            session.Server = ss;

            // starts the data receiving operation
            fd.BeginReceive(session.inbuf, 0, Const.RUBY_BUFFER_SIZE, SocketFlags.None, new AsyncCallback(SessionManager.rcvd), session);

            return session;
        }

        /// <summary>
        /// Handle the data receive operation.
        /// </summary>
        /// <param name="state">An object containing the session related with the receive operation</param>
        private static void rcvd(IAsyncResult state)
        {
            int rcvd;
            char eol;
            byte[] inbuf;
            Session session;
            
            // retrieve the session object from the asynchronous state object.
            session = (Session)state.AsyncState;
            inbuf = session.inbuf;

            try
            {
                if ((rcvd = session.Socket.EndReceive(state)) == 0)
                {
                    Close(session);
                }
            }
            catch (Exception ex)
            {
                Close(session);
                logger.Error(ex.Message);
                return;
            }

            // loop until EOL is reached
            eol = (char)inbuf[session.len];
            for (int i = session.len; rcvd > 0; rcvd--, eol = (char)inbuf[++i])
            {
                if (eol == '\r')
                {
                    // CRLF?
                    if (rcvd > 1 && inbuf[i + 1] == '\n')
                    {
                        i++;
                        rcvd--;
                    }

                    // process the command
                    Process(session.inbuf, 0, session.len, session);

                    // have more data
                    session.extra = (--rcvd > 0) ? i+1 : 0;
                    session.len = 0;
                    continue;
                }
                else if (eol == '\n')
                {
                    // process the command
                    Process(session.inbuf, 0, session.len, session);

                    // have more data
                    session.extra = (--rcvd > 0) ? i+1 : 0;
                    session.len = 0;
                    continue;
                }
                session.len++;
            }

            /* shift the extra to the front */
            if (session.extra > 0)
            {
                inbuf = new byte[session.len];
                Buffer.BlockCopy(session.inbuf, session.extra, inbuf, 0, session.len);
                Buffer.BlockCopy(inbuf, 0, session.inbuf, 0, inbuf.Length);
                session.extra = 0;
            }

            /* closes the session when the received message is too long. */
            if (session.len == Const.RUBY_BUFFER_SIZE)
            {
                logger.Info(Resources.Session_MessageTooLong);
                Close(session);
                return;
            }

            /* the data located between the indexes 0 and "session.len" of the
             * "session.inbuf" array has been processed. So, we need to processes
             * only the new coming data.
             */
            //session.extra += session.len;

            // continue the data receiving operation
            if (session.ended)
            {
                Close(session);
                return;
            }

            try {
                session.Socket.BeginReceive(session.inbuf, session.len, Const.RUBY_BUFFER_SIZE - session.len, SocketFlags.None, new AsyncCallback(SessionManager.rcvd), session);
            }
            catch {
            }
        }

        internal static bool Send(Socket fd, string msg)
        {
            byte[] buffer;
            int sent = 0, size;

            size = msg.Length;
            if (size > Const.RUBY_BUFFER_SIZE)
                throw new ArgumentOutOfRangeException(Resources.Session_MessageTooLong);

            buffer = Encoding.Default.GetBytes(msg);

            try { sent = fd.Send(buffer, 0, size, SocketFlags.None); }
            catch (ObjectDisposedException)
            {
                logger.Info(Resources.Socket_Closed);
                return false;
            }
            catch (Exception e)
            {
                logger.Error(e.Message, e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Shuts the session down and marks it to be closed.
        /// </summary>
        /// <param name="session">The session to shuts down</param>
        public static void Shutdown(Session session)
        {
            session.ended = false;
        }

        /// <summary>
        /// Closes and free any resources that are associated with the session.
        /// </summary>
        public static void Close(Session session)
        {
            session.ended = true;
            session.Socket.Close();
            session.Server.RemoveSession(session.ID);
        }

        /// <summary>
        /// Process the message
        /// </summary>
        /// <param name="cmdbuf">An array containing the command to process.</param>
        /// <param name="offset">The index of the first byte of the message.</param>
        /// <param name="len">The number of characters within the message.</param>
        /// <param name="session">The session related with the command sender</param>
        internal static bool Process(byte[] cmdbuf, int index, int count, Session session)
        {
            int num, num2, len;
            char c;
            string command;
            Stack<RbNode> stack;
            RbNode node;

            len = index+count;
            stack = new Stack<RbNode>(10);

            if (len > cmdbuf.Length)
                throw new ArgumentOutOfRangeException("index");

            if (index < 0 || index > cmdbuf.Length)
                throw new ArgumentOutOfRangeException(StringResources.GetString(StringResources.Arg_IndexOutOfRange));

            // to upper
            for (int i = index; i < count; i++)
            {
                c = (char)cmdbuf[index + i];
                if (c >= 'a' && c <= 'z')
                    cmdbuf[index + i] = (byte)(c - 32);
            }

            c = ((char)cmdbuf[index]);
            
            // the command must starts with a letter
            if (c < 'A' || c > 'Z')
                return false;

            /* parse command
             *  The command is the first string before the first occurence of the
             *  space character in the string inside the "cmdbuf" array, or, when the
             *  command has no parameter, the entire string inside the "cmdbuf" array.
             */
            num = strchr(' ', cmdbuf, index, count);
            if (num == -1)
                num = count;

            // command node
            node = new RbNode();
            node.data = command = Encoding.Default.GetString(cmdbuf, index, num);
            node.pi = 'C';
            stack.Push(node);

            /* parse parameters
             *  A parameter is composed by a parameter identification and a parameter value.
             *  Parameter identification is composed of a string space, a slash(/), a single
             *  character and a ending space. Parameter value is a string whitout any parameter
             *  identification in it.
             */
            while(num < len)
            {
                num2 = num;
                num = strchr('/', cmdbuf, index + num, count - num);
                if (num == -1)
                    num = len;

                /* string space
                 * followed by a slash
                 * and another single space
                 */
                if ((num + 2 < len) && (num - 1 >= 0) && (cmdbuf[num - 1] == cmdbuf[num + 2]) && (cmdbuf[num - 1] == ' '))
                {
                    num2 = num + 3; /* next character after the the parameter identification */
                    num = strchr('/', cmdbuf, num2, count - num2);

                    // the last parameter?
                    if (num == -1)
                        num = len;

                    node = new RbNode();
                    node.data = Encoding.Default.GetString(cmdbuf, num2, num - num2);
                    node.pi = (char)cmdbuf[num2 - 2];

                    stack.Push(node);
                }
            }

            // command handle
            RbCommand[] vocabulary = commands[c-'A'].cmds;
            RbCommand subvoc = vocabulary[0];
            for (int i = 0; string.Compare(subvoc.cmd, "NOP", false) != 0; subvoc = vocabulary[i++])
                if (string.Compare(subvoc.cmd, command, false) == 0)
                    return subvoc.proc(session, stack);

            return false;
        }

        #region Helpers
        private static int strchr(char c, byte[] array, int index, int count)
        {
            char chr;
            for (int i = index, j=index+count; i < j; i++)
            {
                chr = (char)array[i];
                if (chr == c)
                    return i;
            }
            return -1;
        }
        #endregion

        #region Commands
        static bool nop(Session session, Stack<RbNode> stack)
        {
            return true;
        }

        static bool USR(Session session, Stack<RbNode> stack)
        {
            int id = 0;
            string sp = null, sequence = null, data = null;
            RbNode node;

            // validate the parameters
            while (stack.Count > 0)
            {
                node = stack.Pop();
                switch(node.pi)
                {
                    case 'Y':
                        int.TryParse(node.data, out id);
                        break;
                    case 'P':
                        sp = node.data;
                        break;
                    case 'S':
                        sequence = node.data;
                        break;
                    case 'D':
                        data = node.data;
                        if(data == null)
                            Send(session.Socket, Resources.Session_Err_InvalidParameter);
                        break;
                    case 'C':
                        break;
                    default:
                        Send(session.Socket, Resources.Session_Err_InvalidParameter);
                        return false;
                }
            }

            return session.USR(id, sp, sequence[0], data);
        }
        #endregion
    }
}