﻿//Licensed to the Apache Software Foundation(ASF) under one
//or more contributor license agreements.See the NOTICE file
//distributed with this work for additional information
//regarding copyright ownership.The ASF licenses this file
//to you under the Apache License, Version 2.0 (the
//"License"); you may not use this file except in compliance
//with the License.  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.  See the License for the
//specific language governing permissions and limitations
//under the License.

using System.Reflection;
using Xilium.CefGlue;

namespace Paragon.Runtime.Plugins.TypeConversion
{
    public class NativeTypeFieldConverter : INativeTypeMemberConverter
    {
        private readonly FieldInfo _fieldInfo;

        public NativeTypeFieldConverter(string scriptName, FieldInfo fieldInfo)
        {
            MemberName = scriptName;
            _fieldInfo = fieldInfo;
        }

        public string MemberName { get; private set; }

        public void SetNativeValue(object nativeObject, CefV8Value cefObject)
        {
            var fieldValue = CefNativeValueConverter.ToNative(
                cefObject.GetValue(MemberName),
                _fieldInfo.FieldType);
            _fieldInfo.SetValue(nativeObject, fieldValue);
        }

        public void SetCefValue(CefV8Value cefObject, object nativeObject)
        {
            var fieldValue = _fieldInfo.GetValue(nativeObject);
            var cefValue = fieldValue != null
                ? CefNativeValueConverter.ToCef(fieldValue, _fieldInfo.FieldType)
                : CefV8Value.CreateNull();
            cefObject.SetValue(MemberName, cefValue, CefV8PropertyAttribute.None);
        }
    }
}