using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class FormattedTextBlock : TextBlock
    {
        #region Properties

        #region Dependency Properties
        public string FormattedText
        {
            get { return (string)GetValue(FormattedTextProperty); }
            set { SetValue(FormattedTextProperty, value); }
        }
        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.Register("FormattedText", typeof(string), typeof(FormattedTextBlock), new UIPropertyMetadata(null, (s, e) =>
            {
                if (!(s is FormattedTextBlock)) return;
                FormattedTextBlock block = (FormattedTextBlock)s;
                block.Inlines.Clear();
                block.Inlines.Add(Traverse((string)e.NewValue));
            }));
        #endregion

        #endregion

        #region Methods
        Span ProcessText(string text)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                return ProcessNode(doc.FirstChild);
            }
            catch (Exception ex)
            {
                // Send error message to my custom global debugger
                Core.Log.Error(ex);
            }
            return new Span();
        }

        Span ProcessNode(XmlNode node)
        {
            Span span = new Span();

            foreach (XmlNode child in node)
            {
                if (child is XmlText)
                {
                    span.Inlines.Add(new Run(child.InnerText));
                    continue;
                }
                if (!(child is XmlElement)) continue;

                Span processedNode = ProcessNode(child);
                XmlElement element = (XmlElement)child;
                switch (child.Name.ToLower())
                {
                    case "b":
                    case "bold":
                        span.Inlines.Add(new Bold(processedNode));
                        break;

                    case "i":
                    case "italic":
                        span.Inlines.Add(new Italic(processedNode));
                        break;

                    case "u":
                    case "underline":
                        span.Inlines.Add(new Underline(processedNode));
                        break;

                    case "a":
                    case "link":
                        string link = "";
                        if (element.HasAttribute("href"))
                            link = element.GetAttribute("href");
                        Uri uri = new Uri("about:blank");

                        try { uri = new Uri(link); }
                        catch (Exception ex) { Core.Log.Error(ex); }
                        span.Inlines.Add(new ActiveHyperlink(processedNode) { NavigateUri = new Uri(link) });
                        break;

                    case "br":
                    case "linebreak":
                        span.Inlines.Add(new LineBreak());
                        span.Inlines.Add(processedNode);
                        break;
                }
            }
            return span;
        }

        static Inline Traverse(string value)
        {
            // Get the sections/inlines
            string[] sections = SplitIntoSections(value);

            // Check for grouping
            if (sections.Length.Equals(1))
            {
                string section = sections[0];
                string token; // E.g <Bold>
                int tokenStart, tokenEnd; // Where the token/section starts and ends.

                // Check for token
                if (GetTokenInfo(section, out token, out tokenStart, out tokenEnd))
                {
                    // Get the content to further examination
                    string content = token.Length.Equals(tokenEnd - tokenStart) ?
                        null :
                        section.Substring(token.Length, section.Length - 1 - token.Length * 2);

                    switch (token.ToUpper())
                    {
                        case "<B>":
                        case "<BOLD>":
                            /* <b>Bold text</b> */
                            return new Bold(Traverse(content));
                        case "<I>":
                        case "<ITALIC>":
                            /* <i>Italic text</i> */
                            return new Italic(Traverse(content));
                        case "<U>":
                        case "<UNDERLINE>":
                            /* <u>Underlined text</u> */
                            return new Underline(Traverse(content));
                        case "<BR>":
                        case "<BR/>":
                        case "<LINEBREAK/>":
                            /* Line 1<br/>line 2 */
                            return new LineBreak();
                        case "<A>":
                        case "<LINK>":
                            /* <a>{http://www.google.de}Google</a> */
                            var start = content.IndexOf("{");
                            var end = content.IndexOf("}");
                            var url = content.Substring(start + 1, end - 1);
                            var text = content.Substring(end + 1);
                            var link = new Hyperlink();
                            link.NavigateUri = new System.Uri(url);
                            link.RequestNavigate += (s, e) => Process.Start(e.Uri.ToString());
                            link.Inlines.Add(text);
                            return link;
                        case "<IMG>":
                        case "<IMAGE>":
                            /* <image>pack://application:,,,/ProjectName;component/directory1/directory2/image.png</image> */
                            var image = new Image();
                            var bitmap = new BitmapImage(new Uri(content));
                            image.Source = bitmap;
                            image.Width = bitmap.Width;
                            image.Height = bitmap.Height;
                            var container = new InlineUIContainer();
                            container.Child = image;
                            return container;
                        default:
                            return new Run(section);
                    }
                }
                else return new Run(section);
            }
            else // Group together
            {
                Span span = new Span();

                foreach (string section in sections)
                    span.Inlines.Add(Traverse(section));

                return span;
            }
        }

        /// <summary>
        /// Examines the passed string and find the first token, where it begins and where it ends.
        /// </summary>
        /// <param name="value">The string to examine.</param>
        /// <param name="token">The found token.</param>
        /// <param name="startIndex">Where the token begins.</param>
        /// <param name="endIndex">Where the end-token ends.</param>
        /// <returns>True if a token was found.</returns>
        static bool GetTokenInfo(string value, out string token, out int startIndex, out int endIndex)
        {
            token = null;
            startIndex = 0;
            endIndex = -1;

            ParseStart:
            startIndex = value.IndexOf("<", startIndex);
            int startTokenEndIndex = value.IndexOf(">");

            // No token here
            if (startIndex < 0)
                return false;

            // No token here
            if (startTokenEndIndex < 0)
                return false;

            // If (<) is surrounded by quotes, ignore it.
            if (startIndex > 0 && value[startIndex - 1] == '\'' && value[startIndex + 1] == '\'')
            {
                startIndex++; // Search for the next <
                goto ParseStart;
            }


            token = value.Substring(startIndex, startTokenEndIndex - startIndex + 1);

            // Check for closed token. E.g. <LineBreak/>
            if (token.EndsWith("/>"))
            {
                endIndex = startIndex + token.Length;
                return true;
            }

            string endToken = token.Insert(1, "/");

            // Detect nesting;
            int nesting = 0;
            int temp_startTokenIndex = -1;
            int temp_endTokenIndex = -1;
            int pos = 0;
            do
            {
                temp_startTokenIndex = value.IndexOf(token, pos);
                temp_endTokenIndex = value.IndexOf(endToken, pos);

                if (temp_startTokenIndex >= 0 && temp_startTokenIndex < temp_endTokenIndex)
                {
                    nesting++;
                    pos = temp_startTokenIndex + token.Length;
                }
                else if (temp_endTokenIndex >= 0 && nesting > 0)
                {
                    nesting--;
                    pos = temp_endTokenIndex + endToken.Length;
                }
                else // Invalid tokenized string
                    return false;

            } while (nesting > 0);

            endIndex = pos;

            return true;
        }

        /// <summary>
        /// Splits the string into sections of tokens and regular text.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <returns>An array with the sections.</returns>
        static string[] SplitIntoSections(string value)
        {
            List<string> sections = new List<string>();

            while (!string.IsNullOrEmpty(value))
            {
                string token;
                int tokenStartIndex, tokenEndIndex;

                // Check if this is a token section
                if (GetTokenInfo(value, out token, out tokenStartIndex, out tokenEndIndex))
                {
                    // Add pretext if the token isn't from the start
                    if (tokenStartIndex > 0)
                        sections.Add(value.Substring(0, tokenStartIndex));

                    sections.Add(value.Substring(tokenStartIndex, tokenEndIndex - tokenStartIndex));
                    value = value.Substring(tokenEndIndex); // Trim away
                }
                else
                { // No tokens, just add the text
                    sections.Add(value);
                    value = null;
                }
            }

            return sections.ToArray();
        }
        #endregion
    }
}
