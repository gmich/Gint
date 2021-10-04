﻿using Gint.SyntaxHighlighting.Analysis;

namespace Gint.SyntaxHighlighting
{
    internal sealed class HighlighterRenderItem : RenderItem
    {
        public HighlighterRenderItem(HighlightToken token) : base(token.Span)
        {
            Token = token;
        }

        public HighlightToken Token { get; }

        public override RenderItemType RenderItemType => RenderItemType.HighlighterLexer;
    }


}
