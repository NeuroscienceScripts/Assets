using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;


//https://stackoverflow.com/a/22975649
public static class StreamReaderExtension
{
    readonly static FieldInfo charPosField = typeof(StreamReader).GetField("_charPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    readonly static FieldInfo byteLenField = typeof(StreamReader).GetField("_byteLen", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    readonly static FieldInfo charBufferField = typeof(StreamReader).GetField("_charBuffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    public static long GetPosition(this StreamReader reader)
    {
        int byteLen = (int)byteLenField.GetValue(reader);
        var position = reader.BaseStream.Position - byteLen;


        int charPos = (int)charPosField.GetValue(reader);
        if (charPos > 0)
        {
            var charBuffer = (char[])charBufferField.GetValue(reader);
            var encoding = reader.CurrentEncoding;
            var bytesConsumed = encoding.GetBytes(charBuffer, 0, charPos).Length;
            position += bytesConsumed;
        }

        return position;
    }

    public static void SetPosition(this StreamReader reader, long position)
    {
        reader.DiscardBufferedData();
        reader.BaseStream.Seek(position, SeekOrigin.Begin);
    }
}
