using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Debug = GameEngine.Debug;

namespace DREngine
{
    /// <summary>
    /// lmao 482 p4 coming in hot
    /// </summary>
    public class NetworkHelper
    {

        public const string DEBUG_COMMAND = "DEBUG";
        public const string LOG_COMMAND = "LOG";
        public const string WARNING_COMMAND = "WARNING";
        public const string ERROR_COMMAND = "ERROR";

        internal const string INIT_PING_STRING = "PING";
        internal const string CLOSE_CONNECTION_STRING = "CLOSE";

        private const int MAX_HEADER_SIZE = 5;

        private const char SPECIAL_COMMAND_INDICATOR = '@';
        private const char DELIMETER = '\uFFFF';

        private const double READ_TIMEOUT_SECONDS = 5;

        internal static string ReadNextMessage(StreamReader stream, out bool special)
        {
            string remainder;
            string header = ReadHeader(stream, out remainder, out special);

            int messageSize;
            if (!int.TryParse(header, out messageSize))
            {
                throw new InvalidDataException($"Sent header is invalid: {header} (remainder={remainder})");
            }

            string secondPart = ReadString(stream, messageSize - remainder.Length);
            return remainder + secondPart;
        }

        internal static Task WriteMessage(StreamWriter stream, String toSend, bool block = true, bool special = false)
        {
            stream.AutoFlush = true;

            if (ContainsDelimeter(toSend))
            {
                throw new ArgumentException($"Send data contains delimeter {DELIMETER}. This cannot be properly parsed.");
            }

            int size = toSend.Length;
            string header = size.ToString();
            if (special)
            {
                header = SPECIAL_COMMAND_INDICATOR + header;
            }
            if (header.Length > MAX_HEADER_SIZE)
            {
                throw new ArgumentException($"Header too big: {header}. Header must be less than {MAX_HEADER_SIZE} characters.");
            }

            string finalMessage = header + DELIMETER + toSend;
            if (block)
            {
                stream.Write(finalMessage);
                return null;
            }

            return stream.WriteAsync(finalMessage);
        }

        private static string ReadHeader(StreamReader stream, out string remainder, out bool special)
        {
            // There's no timeouts here. We may not receive a header.

            char[] header = new char[MAX_HEADER_SIZE];
            int read = 0;
            int headerLength = MAX_HEADER_SIZE;
            while (true)
            {
                int numRead = stream.Read(header, read, MAX_HEADER_SIZE - read);

                // check for delimeter. If we found one, that means our message is good.
                for (int i = read; i < read + numRead; ++i)
                {
                    char c = header[i];
                    if (c == DELIMETER)
                    {
                        headerLength = i;
                        break;
                    }
                }

                if (numRead > 0)
                {
                    read += numRead;
                }
                
                // We've read up to the max header.
                if (read >= MAX_HEADER_SIZE) break;
            }
            // our header is a string in the range header[0 : headerLength]
            // our remainder (not counting DELIMETER) is in the range header[headerLength : read]

            // If our header begins with a "special" code, that means our command is special (to be handled behind the scenes)
            special = headerLength > 0 && header[0] == SPECIAL_COMMAND_INDICATOR;

            string result;
            if (special)
            {
                // Cut off the special char. headerLength includes special.
                result = new string(header, 1, headerLength - 1);
            }
            else
            {
                result = new string(header, 0, headerLength);
            }
            remainder = new string(header, headerLength + 1, read - headerLength - 1);

            
            return result;
        }

        private static string ReadString(StreamReader stream, int size)
        {
            Stopwatch timeout = new Stopwatch();
            timeout.Start();

            char[] result = new char[size];
            int read = 0;
            while (read < size)
            {
                int numRead = stream.Read(result, read, size - read);
                if (numRead > 0)
                {
                    read += numRead;
                    timeout.Restart();
                }
                else
                {
                    if (timeout.Elapsed.TotalSeconds > READ_TIMEOUT_SECONDS)
                    {
                        throw new TimeoutException("Reading message took too long.");
                    }
                }
            }
            return new string(result);
        }

        private static bool ContainsDelimeter(string text)
        {
            return ContainsDelimeter(text.ToCharArray(), 0, text.Length);
        }
        private static bool ContainsDelimeter(char[] text, int startIndex, int length)
        {
            return false;
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                if (text[i] == DELIMETER) return true;
            }

            return false;
        }
    }

    public abstract class DRConnection : IDisposable
    {
        protected PipeStream _inputPipe;
        protected PipeStream _outputPipe;
        private StreamReader _input;
        private StreamWriter _output;

        private Task _receiverTask;
        private bool _receiving;

        private Action _onPinged = null;

        private int _pingCounter;
        
        private object _lock = new object();

        public Action<string> OnMessage;
        public Action OnClose;

        public DRConnection(PipeStream inputPipe, PipeStream outputPipe)
        {
            _inputPipe = inputPipe;
            _outputPipe = outputPipe;
            if (_inputPipe != null)
            {
                _input = new StreamReader(_inputPipe);
            }

            if (_outputPipe != null)
            {
                _output = new StreamWriter(_outputPipe);
            }

            _receiving = false;
        }

        public void SendMessageBlocked(string message)
        {
            if (_output == null) return;
            NetworkHelper.WriteMessage(_output, message, true);
        }

        public Task SendMessageAsync(string message)
        {
            if (_output == null) return null;
            return NetworkHelper.WriteMessage(_output, message, false);
        }

        private void SendSpecialMessage(string message)
        {
            // This would lead to harder to detect bugs, if this is null crash here.
            //if (_output == null) return;
            NetworkHelper.WriteMessage(_output, message, true, true);
        }

        public void SendPing()
        {
            SendSpecialMessage(NetworkHelper.INIT_PING_STRING);
        }
        public void SendStop()
        {
            SendSpecialMessage(NetworkHelper.CLOSE_CONNECTION_STRING);
        }


        public void WaitForPing(Action onComplete)
        {
            lock (_lock)
            {
                // We pinged already and nobody took charge, so we will!
                if (_pingCounter > 0)
                {
                    --_pingCounter;
                    onComplete.Invoke();
                }
                else
                {
                    _onPinged = onComplete;
                }
            }
        }

        public void BeginReceiving()
        {
            if (_receiving) return;
            _receiving = true;
            _receiverTask = Task.Run(() =>
            {
                while (_receiving)
                {
                    bool special;
                    string message;
                    try
                    {
                        message = NetworkHelper.ReadNextMessage(_input, out special);
                    }
                    catch (Exception e)
                    {
                        // Something failed (invalid args, timeout, etc.)
                        Debug.LogError($"{e.Message}: {e.StackTrace}");
                        continue;
                    }

                    // Only receive one message at a time. Other messages must wait their turn to be received.
                    lock (_lock)
                    {
                        if (special)
                        {
                            // Do special shit
                            try
                            {
                                ParseSpecialCommand(message);
                            }
                            catch (ArgumentException e)
                            {
                                // Message was invalid. Oof.
                                Debug.LogError($"{e.Message}: {e.StackTrace}");
                            }
                        }
                        else
                        {
                            // Regular message
                            OnMessage?.Invoke(message);
                        }
                    }
                }
            });
        }

        public void StopReceiving(bool toLock = true)
        {
            if (!_receiving) return;

            if (toLock)
            {
                lock (_lock)
                {
                    _receiving = false;
                }
            }
            else
            {
                _receiving = false;
            }

            _receiverTask.Wait();
            _receiverTask.Dispose();
            _receiverTask = null;
        }
        public void Dispose()
        {
            StopReceiving();
            _inputPipe?.Dispose();
            _outputPipe?.Dispose();
            _input?.Dispose();
            _output?.Dispose();
        }

        private void ParseSpecialCommand(string message)
        {
            switch (message)
            {
                case NetworkHelper.INIT_PING_STRING:
                    if (_onPinged != null)
                    {
                        // We're receiving ping NOW
                        _onPinged?.Invoke();
                        _onPinged = null;
                    }
                    else
                    {
                        // We'll receive ping LATER
                        _pingCounter++;
                    }
                    break;
                case NetworkHelper.CLOSE_CONNECTION_STRING:
                    // Close connection
                    //SendPing();
                    StopReceiving(false);
                    OnClose?.Invoke();
                    break;
                default:
                    throw new ArgumentException($"Special message not valid: {message}");
            }
        }
    }
}
