using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace BEQuestionBank.Shared.Helpers;
public static class HtmlHelper
{
    // =========================
    // 1. Lấy nội dung trong <body> và làm sạch
    // =========================
    public static string FilterHtml(string htmlSource)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlSource);

        var bodyNode = doc.DocumentNode.SelectSingleNode("//body");
        if (bodyNode == null) return string.Empty;

        CleanWordHtml(bodyNode);
        FixEntities(bodyNode);

        return bodyNode.InnerHtml.Trim();
    }

    // =========================
    // 2. Loại bỏ thẻ rác từ Word
    // =========================
    private static void CleanWordHtml(HtmlNode root)
    {
        var removeTags = new[]
        {
                "o:p", "meta", "link", "style", "script", "xml",
                "v:shape", "v:imagedata", "v:shapetype", "v:path",
                "v:stroke", "v:formulas", "v:f", "o:lock", "o:oleobject", "p"
            };

        foreach (var node in root.Descendants().ToList())
        {
            if (removeTags.Contains(node.Name, StringComparer.OrdinalIgnoreCase))
                node.Remove();
        }

        // Xóa style & class
        foreach (var node in root.Descendants())
        {
            node.Attributes.Remove("style");
            node.Attributes.Remove("class");
        }

        // Xóa comment
        foreach (var comment in root.SelectNodes("//comment()") ?? new HtmlNodeCollection(null))
            comment.Remove();
    }

    // =========================
    // 3. Fix ký tự đặc biệt
    // =========================
    private static void FixEntities(HtmlNode root)
    {
        string html = root.InnerHtml;
        html = html.Replace("“", "&ldquo;")
                   .Replace("”", "&rdquo;")
                   .Replace("–", "&mdash;");
        root.InnerHtml = html;
    }

    // =========================
    // 4. Lấy danh sách ảnh
    // =========================
    public static List<string> GetImgSrc(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc.DocumentNode.SelectNodes("//img")
            ?.Select(x => x.GetAttributeValue("src", "").Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList() ?? new List<string>();
    }

    public static List<string> GetImgSrc2(string html) => GetImgSrc(html);

    // =========================
    // 5. Lấy danh sách audio
    // =========================
    public static List<string> GetAudioPath(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc.DocumentNode.SelectNodes("//audio/source|//audio")
            ?.Select(x => x.GetAttributeValue("src", "").Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList() ?? new List<string>();
    }

    // =========================
    // 6. Chuyển audio thành HTML5
    // =========================
    public static string NiceHtml(string html) => ReplaceAudioWithHtml5(html);
    public static string MediableHtml(string html, string playerSource) => ReplaceAudioWithHtml5(html);
    public static string MediableHtml(string html, string playerSource, bool isOnceTimePlay)
    {
        string note = isOnceTimePlay
            ? "<span><i style=\"font-size:90%; color:#090\"><em>Lưu ý: các đoạn âm thanh chỉ được nghe một lần.</em></i></span>"
            : "";
        return ReplaceAudioWithHtml5(html, note);
    }
    public static string MediableHtml(string html, string playerSource, int maxListeningTimes)
    {
        string note = maxListeningTimes > 0
            ? $"<span><i style=\"font-size:90%; color:#090\"><em>Lưu ý: các đoạn âm thanh chỉ được nghe {maxListeningTimes} lần.</em></i></span>"
            : "";
        return ReplaceAudioWithHtml5(html, note);
    }

    private static string ReplaceAudioWithHtml5(string html, string noteHtml = "")
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var audios = doc.DocumentNode.SelectNodes("//audio") ?? new HtmlNodeCollection(null);
        foreach (var audio in audios)
        {
            string src = audio.GetAttributeValue("src", "").Trim();
            if (string.IsNullOrWhiteSpace(src)) src = audio.InnerText.Trim();
            if (string.IsNullOrWhiteSpace(src)) continue;

            var newAudio = $"{noteHtml}<audio controls preload=\"none\" style=\"width:300px;\">" +
                           $"<source src=\"{HttpUtility.HtmlEncode(src)}\" type=\"audio/mpeg\">" +
                           "Your browser does not support the audio element.</audio>";
            audio.ParentNode.ReplaceChild(HtmlNode.CreateNode(newAudio), audio);
        }
        return doc.DocumentNode.OuterHtml;
    }

    // =========================
    // 7. HTML cho Windows App
    // =========================
    public static string HtmlWithMediaPlayer(string html, string appPath)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var audios = doc.DocumentNode.SelectNodes("//audio") ?? new HtmlNodeCollection(null);

        foreach (var audio in audios)
        {
            string src = audio.GetAttributeValue("src", "").Trim();
            if (string.IsNullOrWhiteSpace(src)) src = audio.InnerText.Trim();
            src = "file:///" + (appPath + "/" + src).Replace("\\", "/").Replace("//", "/");

            var newAudio = $"<audio controls preload=\"none\" style=\"width:300px;\">" +
                           $"<source src=\"{HttpUtility.HtmlEncode(src)}\" type=\"audio/mpeg\">" +
                           "Your browser does not support the audio element.</audio>";
            audio.ParentNode.ReplaceChild(HtmlNode.CreateNode(newAudio), audio);
        }
        return doc.DocumentNode.OuterHtml;
    }
    public static string HtmlWithMediaPlayer(string html) => ReplaceAudioWithHtml5(html);

    // =========================
    // 8. Preview HTML
    // =========================
    public static string PreviewHtml(string html, string appPath)
    {
        html = HtmlWithMediaPlayer(html, appPath);
        foreach (string imgSrc in GetImgSrc2(html))
        {
            if (!string.IsNullOrWhiteSpace(imgSrc))
            {
                string replaceSrc = "file:///" + (appPath + "/" + imgSrc).Replace("\\", "/").Replace("//", "/");
                html = html.Replace(imgSrc, replaceSrc);
            }
        }
        return html;
    }

    // =========================
    // 9. Định dạng đáp án từ Word Interop
    // =========================
    public static string GetAnswerHtmlFormated(Run run)
    {
        string text = run.InnerText ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var runProps = run.RunProperties;
        if (runProps != null)
        {
            if (runProps.Italic != null) // in nghiêng
                text = $"<i>{text}</i>";

            if (runProps.Bold != null) // in đậm
                text = $"<b>{text}</b>";

            if (runProps.Underline != null && runProps.Underline.Val != UnderlineValues.None) // gạch chân
                text = $"<u>{text}</u>";
        }
        return text;
    }

    // =========================
    // 10. Chuyển HTML sang Text
    // =========================
    public static string ConvertHtmlToText(string html) => ConvertHtmlToText(html, false);
    public static string ConvertHtmlToText(string html, bool replaceMediaTag)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        if (replaceMediaTag)
        {
            foreach (var img in doc.DocumentNode.SelectNodes("//img") ?? Enumerable.Empty<HtmlNode>())
                img.ParentNode.ReplaceChild(HtmlNode.CreateNode("[IMAGE]"), img);
            foreach (var audio in doc.DocumentNode.SelectNodes("//audio") ?? Enumerable.Empty<HtmlNode>())
                audio.ParentNode.ReplaceChild(HtmlNode.CreateNode("[AUDIO]"), audio);
        }

        return HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
    }
}

