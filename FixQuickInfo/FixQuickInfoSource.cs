/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FixQuickInfoSource.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace FixQuickInfo
{
    class FixQuickInfoSource : IQuickInfoSource
    {
        FixQuickInfoSourceProvider m_provider;
        ITextBuffer m_subjectBuffer;
        Dictionary<string, Type> m_dictionary;

        public FixQuickInfoSource(FixQuickInfoSourceProvider provider, ITextBuffer subjectBuffer)
        {
            m_provider = provider;
            m_subjectBuffer = subjectBuffer;
            m_dictionary = new Dictionary<string, Type>();
            m_dictionary["Fix.Dictionary.Field"] = typeof(Fix.Dictionary.Field);
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
        {
            // Map the trigger point down to our buffer.
            SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(m_subjectBuffer.CurrentSnapshot);
            if (!subjectTriggerPoint.HasValue)
            {
                applicableToSpan = null;
                return;
            }

            ITextSnapshot currentSnapshot = subjectTriggerPoint.Value.Snapshot;
            var querySpan = new SnapshotSpan(subjectTriggerPoint.Value, 0);

            //look for occurrences of our QuickInfo words in the span
            ITextStructureNavigator navigator = m_provider.NavigatorService.GetTextStructureNavigator(m_subjectBuffer);

            SnapshotSpan enclosingSpan = navigator.GetSpanOfEnclosing(querySpan);
            TextExtent enclosingExtent = navigator.GetExtentOfWord(enclosingSpan.Start);
            string enclosingText = enclosingExtent.Span.GetText();

            SnapshotSpan previousSpan = navigator.GetSpanOfPreviousSibling(querySpan);
            TextExtent previousExtent = navigator.GetExtentOfWord(previousSpan.Start);
            string previousText = previousExtent.Span.GetText();

            SnapshotSpan firstSpan = navigator.GetSpanOfFirstChild(querySpan);
            TextExtent firstExtent = navigator.GetExtentOfWord(firstSpan.Start);
            string firstText = firstExtent.Span.GetText();


            TextExtent extent = navigator.GetExtentOfWord(subjectTriggerPoint.Value);
            string searchText = extent.Span.GetText();

            System.Diagnostics.Debug.WriteLine($"ENCLOSING '{enclosingText}' PREVIOUS '{previousText}' TRIGGER '{searchText}' FIRST '{firstText}'");


            foreach (string key in m_dictionary.Keys)
            {
                int foundIndex = searchText.IndexOf(key, StringComparison.CurrentCultureIgnoreCase);
                if (foundIndex > -1)
                {
                    applicableToSpan = currentSnapshot.CreateTrackingSpan
                        (
                                                //querySpan.Start.Add(foundIndex).Position, 9, SpanTrackingMode.EdgeInclusive
                                                extent.Span.Start + foundIndex, key.Length, SpanTrackingMode.EdgeInclusive
                        );

                    Type value;
                    m_dictionary.TryGetValue(key, out value);
                    if (value != null)
                    {
                        qiContent.Add("YOU TYPED A FIX DICTIONARY FIELD");
                    }
                    else
                    {
                        qiContent.Add($"NO MATCH '{key}'");
                    }

                    return;
                }
            }

            applicableToSpan = null;
        }

        bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }

    }
}
