using System;
using System.Collections.Generic;
using System.Linq;
using RDM.Core;

namespace RDM.Models.ImageViewerAPI
{
    public class GeneralDocument : IEquatable<GeneralDocument>
    {
        public GeneralDocument(Maybe<string> documentName, IList<GeneralDocumentPage> pages)
        {
            Contract.Requires<ArgumentNullException>(documentName != null, nameof(documentName));
            Contract.Requires<ArgumentNullException>(pages != null, nameof(pages));
            Contract.Requires<ArgumentException>(pages.Any(), "General Documents must contain at least one page.");

            DocumentName = documentName;
            Pages = pages;
        }

        public Maybe<string> DocumentName { get; }

        public IList<GeneralDocumentPage> Pages { get; }

        public static bool operator ==(GeneralDocument lhs, GeneralDocument rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
            {
                return true;
            }

            if (ReferenceEquals(lhs, null))
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(GeneralDocument lhs, GeneralDocument rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GeneralDocument);
        }

        public bool Equals(GeneralDocument other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return
                DocumentName.HasValue == other.DocumentName.HasValue &&
                (!DocumentName.HasValue || string.CompareOrdinal(DocumentName.Value, other.DocumentName.Value) == 0) &&
                Pages.SequenceEqual(other.Pages);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 13;

                if (DocumentName.HasValue)
                {
                    result = (result * 31) + DocumentName.Value.GetHashCode();
                }

                foreach (var page in Pages)
                {
                    result = (result * 31) + page.GetHashCode();
                }

                return result;
            }
        }
    }
}
