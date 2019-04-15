using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace ConsoleView
{
    class TextBoxWriter : TextWriter
    {
        ICSharpCode.AvalonEdit.TextEditor textBox;
        delegate void WriteFunc(string value);
        WriteFunc write;
        WriteFunc writeLine;

        public TextBoxWriter(ICSharpCode.AvalonEdit.TextEditor textBox)
        {
            this.textBox = textBox;
            write = Write;
            writeLine = WriteLine;
        }


        // 使用UTF-16避免不必要的编码转换
        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

        private delegate void outputDelegate(string msg);
        private void outputAction(string msg)
        {
            textBox.AppendText(msg);
            textBox.AppendText("\n");
        }

        public override void Write(string value)
        {
            if (textBox.Dispatcher.CheckAccess())
                textBox.AppendText(value);
            else
                textBox.Dispatcher.Invoke(new outputDelegate(outputAction), value);
        }

        public override void WriteLine(string value)
        {
            if (textBox.Dispatcher.CheckAccess())
                textBox.AppendText(value);
            else
                textBox.Dispatcher.Invoke(new outputDelegate(outputAction), value);
        }
    }
}
