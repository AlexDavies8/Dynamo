using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dynamo.Model
{
    public class MemberInfo
    {
        FieldInfo _fieldInfo;
        PropertyInfo _propertyInfo;

        public object GetValue(object obj)
        {
            if (_fieldInfo != null)
                return _fieldInfo.GetValue(obj);
            if (_propertyInfo != null)
                return _propertyInfo.GetValue(obj);
            return null;
        }

        public void SetValue(object obj, object value)
        {
            if (_fieldInfo != null)
                _fieldInfo.SetValue(obj, value);
            if (_propertyInfo != null)
                _propertyInfo.SetValue(obj, value);
        }
    }
}
