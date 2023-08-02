using Util;

namespace ErrorHandler;


public class Thrower
{
    
    public string Text;

    public Thrower(string text)
    {
        Text = text;
    }

    public class BaseException
    {
        public string Message;
        public Span Span;

        public BaseException(string message, Span span)
        {
            Message = message;
            Span = span;
        }

        public override string ToString()
        {
            return $"{C.RED}{GetType().Name}{C.END}: {C.RED2}{Message}{C.END}\n"
                 + $"{C.ORANGE}at {Span}{C.END}";
        }

        public void Print()
        {
            Console.WriteLine(this);
        }
    }
}