using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Image2XPM
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            // Init variables.
            string inputFile = args[0];
            string outputFile = null;
            string variableName = null;
            string variableType = null;
            string validChars = null;
            int alphaLimit = 192;

            // Parse arguments.
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--output" || args[i] == "-o")
                {
                    if (++i < args.Length)
                        outputFile = args[i];
                }
                else if (args[i] == "--variable-name" || args[i] == "-vn")
                {
                    if (++i < args.Length)
                        variableName = args[i];
                }
                else if (args[i] == "--variable-type" || args[i] == "-vt")
                {
                    if (++i < args.Length)
                        variableType = args[i];
                }
                else if (args[i] == "--valid-characters" || args[i] == "-vc")
                {
                    if (++i < args.Length)
                        validChars = args[i];
                }
                else if (args[i] == "--alpha-limit" || args[i] == "-al")
                {
                    if (++i < args.Length)
                        alphaLimit = int.Parse(args[i]);
                }
            }

            // Set default values for variables that weren't supplied.
            if (outputFile == null)
                outputFile = Path.ChangeExtension(inputFile, ".xpm");

            if (variableName == null)
                variableName = Regex.Replace(Path.GetFileNameWithoutExtension(outputFile).Replace(' ', '_'), "[^0-9A-Za-z_]", "");

            if (variableType == null)
                variableType = "static const char*";

            if (validChars == null)
                validChars = " !#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}";

            using (var bmp = new Bitmap(inputFile))
            {
                // Get all pixel data.
                var pixels = new uint[bmp.Width * bmp.Height];
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        var color = bmp.GetPixel(x, y);
                        pixels[y * bmp.Width + x] = (uint)(color.A < alphaLimit ? 0 : color.ToArgb() | 0xFF000000);
                    }
                }

                // Get all unique colors.
                var palette = pixels
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var colorLookup = new Dictionary<uint, string>();
                var charsPerPixel = 1;

                // Calculate the number of chars we'll need per pixel.
                while (Math.Pow(validChars.Length, charsPerPixel) < palette.Count)
                    charsPerPixel++;

                // Loop through each color in the palette to generate the color lookup table.
                foreach (var color in palette)
                {
                    var chars = new char[charsPerPixel];
                    var index = colorLookup.Count;
                    for (int i = chars.Length - 1; i >= 0; i--)
                    {
                        chars[i] = validChars[index % validChars.Length];
                        index /= validChars.Length;
                    }
                    colorLookup[color] = new string(chars);
                }

                // Lets write the output file.
                using (var writer = new StreamWriter(outputFile))
                {
                    writer.Write("/* XPM */\n");
                    writer.Write("{0} {1}[]={{\n", variableType, variableName);
                    writer.Write("\"{0} {1} {2} {3}\",\n", bmp.Width, bmp.Height, colorLookup.Count, charsPerPixel);
                    foreach (var color in palette)
                    {
                        var chars = colorLookup[color];
                        if (color == 0)
                            writer.Write("\"{0} c None\",\n", chars);
                        else
                            writer.Write("\"{0} c #{1:X6}\",\n", chars, color & 0xFFFFFF);
                    }
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        writer.Write(y > 0 ? ",\n\"" : "\"");
                        for (int x = 0; x < bmp.Width; x++)
                            writer.Write(colorLookup[pixels[y * bmp.Width + x]]);
                        writer.Write("\"");
                    }
                    writer.Write("};");
                }
            }
        }
    }
}
