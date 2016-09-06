using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Content.Internal
{
    public class CachedResult
    {
        private object _value;
        private bool _isNew;

        public CachedResult(object value, bool isNew)
        {
            _value = value;
            _isNew = isNew;
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public bool IsNew
        {
            get
            {
                return _isNew;
            }
        }
    }
}
