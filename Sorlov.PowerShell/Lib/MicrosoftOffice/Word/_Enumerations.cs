﻿namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Word
{
    public enum PageNumberFormat
    {
        normal,
        roman
    }

    public enum BorderSize
    {
        one,
        two,
        three,
        four,
        five,
        six,
        seven,
        eight,
        nine
    }

    public enum EditRestrictions
    {
        none,
        readOnly,
        forms,
        comments,
        trackedChanges
    }

    /// <summary>
    /// Table Cell Border styles
    /// Added by lckuiper @ 20101117
    /// source: http://msdn.microsoft.com/en-us/library/documentformat.openxml.wordprocessing.tablecellborders.aspx
    /// </summary>
    public enum BorderStyle
    {
        Tcbs_none = 0,
        Tcbs_single,
        Tcbs_thick,
        Tcbs_double,
        Tcbs_dotted,
        Tcbs_dashed,
        Tcbs_dotDash,
        Tcbs_dotDotDash,
        Tcbs_triple,
        Tcbs_thinThickSmallGap,
        Tcbs_thickThinSmallGap,
        Tcbs_thinThickThinSmallGap,
        Tcbs_thinThickMediumGap,
        Tcbs_thickThinMediumGap,
        Tcbs_thinThickThinMediumGap,
        Tcbs_thinThickLargeGap,
        Tcbs_thickThinLargeGap,
        Tcbs_thinThickThinLargeGap,
        Tcbs_wave,
        Tcbs_doubleWave,
        Tcbs_dashSmallGap,
        Tcbs_dashDotStroked,
        Tcbs_threeDEmboss,
        Tcbs_threeDEngrave,
        Tcbs_outset,
        Tcbs_inset
    }

    /// <summary>
    /// Table Cell Border Types
    /// Added by lckuiper @ 20101117
    /// source: http://msdn.microsoft.com/en-us/library/documentformat.openxml.wordprocessing.tablecellborders.aspx
    /// </summary>
    public enum TableCellBorderType
    {
        Top,
        Bottom,
        Left,
        Right,
        InsideH,
        InsideV,
        TopLeftToBottomRight,
        TopRightToBottomLeft
    }

    /// <summary>
    /// Table Border Types
    /// Added by lckuiper @ 20101117
    /// source: http://msdn.microsoft.com/en-us/library/documentformat.openxml.wordprocessing.tableborders.aspx
    /// </summary>
    public enum TableBorderType
    {
        Top,
        Bottom,
        Left,
        Right,
        InsideH,
        InsideV
    }

    // Patch 7398 added by lckuiper on Nov 16th 2010 @ 2:23 PM
    public enum VerticalAlignment { Top, Center, Bottom };

    public enum Orientation { Portrait, Landscape };
    public enum XmlDocument { Main, HeaderOdd, HeaderEven, HeaderFirst, FooterOdd, FooterEven, FooterFirst };
    public enum MatchFormattingOptions { ExactMatch, SubsetMatch};
    public enum Script { superscript, subscript, none }
    public enum Highlight { yellow, green, cyan, magenta, blue, red, darkBlue, darkCyan, darkGreen, darkMagenta, darkRed, darkYellow, darkGray, lightGray, black, none };
    public enum UnderlineStyle { none, singleLine, doubleLine, thick, dotted, dottedHeavy, dash, dashedHeavy, dashLong, dashLongHeavy, dotDash, dashDotHeavy, dotDotDash, dashDotDotHeavy, wave, wavyHeavy, wavyDouble, words };
    public enum StrikeThrough { none, strike, doubleStrike };
    public enum Misc { none, shadow, outline, outlineShadow, emboss, engrave };
    
    /// <summary>
    /// Change the caps style of text, for use with Append and AppendLine.
    /// </summary>
    public enum CapsStyle 
    { 
        /// <summary>
        /// No caps, make all characters are lowercase.
        /// </summary>
        none, 
        /// <summary>
        /// All caps, make every character uppercase.
        /// </summary>
        caps,
        /// <summary>
        /// Small caps, make all characters capital but with a small font size.
        /// </summary>
        smallCaps };

    /// <summary>
    /// Designs\Styles that can be applied to a table.
    /// </summary>
    public enum TableDesign { Custom, TableNormal, TableGrid, LightShading, LightShadingAccent1, LightShadingAccent2, LightShadingAccent3, LightShadingAccent4, LightShadingAccent5, LightShadingAccent6, LightList, LightListAccent1, LightListAccent2, LightListAccent3, LightListAccent4, LightListAccent5, LightListAccent6, LightGrid, LightGridAccent1, LightGridAccent2, LightGridAccent3, LightGridAccent4, LightGridAccent5, LightGridAccent6, MediumShading1, MediumShading1Accent1, MediumShading1Accent2, MediumShading1Accent3, MediumShading1Accent4, MediumShading1Accent5, MediumShading1Accent6, MediumShading2, MediumShading2Accent1, MediumShading2Accent2, MediumShading2Accent3, MediumShading2Accent4, MediumShading2Accent5, MediumShading2Accent6, MediumList1, MediumList1Accent1, MediumList1Accent2, MediumList1Accent3, MediumList1Accent4, MediumList1Accent5, MediumList1Accent6, MediumList2, MediumList2Accent1, MediumList2Accent2, MediumList2Accent3, MediumList2Accent4, MediumList2Accent5, MediumList2Accent6, MediumGrid1, MediumGrid1Accent1, MediumGrid1Accent2, MediumGrid1Accent3, MediumGrid1Accent4, MediumGrid1Accent5, MediumGrid1Accent6, MediumGrid2, MediumGrid2Accent1, MediumGrid2Accent2, MediumGrid2Accent3, MediumGrid2Accent4, MediumGrid2Accent5, MediumGrid2Accent6, MediumGrid3, MediumGrid3Accent1, MediumGrid3Accent2, MediumGrid3Accent3, MediumGrid3Accent4, MediumGrid3Accent5, MediumGrid3Accent6, DarkList, DarkListAccent1, DarkListAccent2, DarkListAccent3, DarkListAccent4, DarkListAccent5, DarkListAccent6, ColorfulShading, ColorfulShadingAccent1, ColorfulShadingAccent2, ColorfulShadingAccent3, ColorfulShadingAccent4, ColorfulShadingAccent5, ColorfulShadingAccent6, ColorfulList, ColorfulListAccent1, ColorfulListAccent2, ColorfulListAccent3, ColorfulListAccent4, ColorfulListAccent5, ColorfulListAccent6, ColorfulGrid, ColorfulGridAccent1, ColorfulGridAccent2, ColorfulGridAccent3, ColorfulGridAccent4, ColorfulGridAccent5, ColorfulGridAccent6, None };
    
    /// <summary>
    /// How a Table should auto resize.
    /// </summary>
    public enum AutoFit 
    { 
        /// <summary>
        /// Autofit to Table contents.
        /// </summary>
        Contents, 
        /// <summary>
        /// Autofit to Window.
        /// </summary>
        Window,
        /// <summary>
        /// Autofit to Column width.
        /// </summary>
        ColumnWidth 
    };

    public enum RectangleShapes
    {
        rect,
        roundRect,
        snip1Rect,
        snip2SameRect,
        snip2DiagRect,
        snipRoundRect,
        round1Rect,
        round2SameRect,
        round2DiagRect
    };

    public enum BasicShapes
    {
        ellipse,
        triangle,
        rtTriangle,
        parallelogram,
        trapezoid,
        diamond,
        pentagon,
        hexagon,
        heptagon,
        octagon,
        decagon,
        dodecagon,
        pie,
        chord,
        teardrop,
        frame,
        halfFrame,
        corner,
        diagStripe,
        plus,
        plaque,
        can,
        cube,
        bevel,
        donut,
        noSmoking,
        blockArc,
        foldedCorner,
        smileyFace,
        heart,
        lightningBolt,
        sun,
        moon,
        cloud,
        arc,
        backetPair,
        bracePair,
        leftBracket,
        rightBracket,
        leftBrace,
        rightBrace
    };

    public enum BlockArrowShapes
    {
        rightArrow,
        leftArrow,
        upArrow,
        downArrow,
        leftRightArrow,
        upDownArrow,
        quadArrow,
        leftRightUpArrow,
        bentArrow,
        uturnArrow,
        leftUpArrow,
        bentUpArrow,
        curvedRightArrow,
        curvedLeftArrow,
        curvedUpArrow,
        curvedDownArrow,
        stripedRightArrow,
        notchedRightArrow,
        homePlate,
        chevron,
        rightArrowCallout,
        downArrowCallout,
        leftArrowCallout,
        upArrowCallout,
        leftRightArrowCallout,
        quadArrowCallout,
        circularArrow
    };

    public enum EquationShapes
    {
        mathPlus,
        mathMinus,
        mathMultiply,
        mathDivide,
        mathEqual,
        mathNotEqual
    };

    public enum FlowchartShapes
    {
        flowChartProcess,
        flowChartAlternateProcess,
        flowChartDecision,
        flowChartInputOutput,
        flowChartPredefinedProcess,
        flowChartInternalStorage,
        flowChartDocument,
        flowChartMultidocument,
        flowChartTerminator,
        flowChartPreparation,
        flowChartManualInput,
        flowChartManualOperation,
        flowChartConnector,
        flowChartOffpageConnector,
        flowChartPunchedCard,
        flowChartPunchedTape,
        flowChartSummingJunction,
        flowChartOr,
        flowChartCollate,
        flowChartSort,
        flowChartExtract,
        flowChartMerge,
        flowChartOnlineStorage,
        flowChartDelay,
        flowChartMagneticTape,
        flowChartMagneticDisk,
        flowChartMagneticDrum,
        flowChartDisplay
    };

    public enum StarAndBannerShapes
    {
        irregularSeal1,
        irregularSeal2,
        star4,
        star5,
        star6,
        star7,
        star8,
        star10,
        star12,
        star16,
        star24,
        star32,
        ribbon,
        ribbon2,
        ellipseRibbon,
        ellipseRibbon2,
        verticalScroll,
        horizontalScroll,
        wave,
        doubleWave
    };

    public enum CalloutShapes
    {
        wedgeRectCallout,
        wedgeRoundRectCallout,
        wedgeEllipseCallout,
        cloudCallout,
        borderCallout1,
        borderCallout2,
        borderCallout3,
        accentCallout1,
        accentCallout2,
        accentCallout3,
        callout1,
        callout2,
        callout3,
        accentBorderCallout1,
        accentBorderCallout2,
        accentBorderCallout3
    };

    /// <summary>
    /// Text alignment of a Paragraph.
    /// </summary>
    public enum Alignment
    {
        /// <summary>
        /// Align Paragraph to the left.
        /// </summary>
        left,

        /// <summary>
        /// Align Paragraph as centered.
        /// </summary>
        center,

        /// <summary>
        /// Align Paragraph to the right.
        /// </summary>
        right,

        /// <summary>
        /// (Justified) Align Paragraph to both the left and right margins, adding extra space between content as necessary.
        /// </summary>
        both
    };
    
    public enum Direction
    {
        LeftToRight, 
        RightToLeft
    };

    /// <summary>
    /// Paragraph edit types
    /// </summary>
    internal enum EditType
    {
        /// <summary>
        /// A ins is a tracked insertion
        /// </summary>
        ins,
        /// <summary>
        /// A del is  tracked deletion
        /// </summary>
        del
    }

    /// <summary>
    /// Custom property types.
    /// </summary>
    internal enum CustomPropertyType
    {
        /// <summary>
        /// System.String
        /// </summary>
        Text,
        /// <summary>
        /// System.DateTime
        /// </summary>
        Date,
        /// <summary>
        /// System.Int32
        /// </summary>
        NumberInteger,
        /// <summary>
        /// System.Double
        /// </summary>
        NumberDecimal,
        /// <summary>
        /// System.Boolean
        /// </summary>
        YesOrNo
    }

    /// <summary>
    /// Text types in a Run
    /// </summary>
    public enum RunTextType
    {
        /// <summary>
        /// System.String
        /// </summary>
        Text,
        /// <summary>
        /// System.String
        /// </summary>
        DelText,
    }
}
