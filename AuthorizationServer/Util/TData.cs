//
// Copyright (C) 2018 Authlete, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific
// language governing permissions and limitations under the
// License.
//


using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Authlete.Util;


namespace AuthorizationServer.Util
{
    /// <summary>
    /// Wrapper over <c>ITempDataDictionary</c>.
    /// </summary>
    public class TData
    {
        readonly ITempDataDictionary _tempdata;


        public TData(ITempDataDictionary tempdata)
        {
            _tempdata = tempdata;
        }


        public object Get(string key)
        {
            return _tempdata.Peek(key);
        }


        public object Get(object key)
        {
            return Get(key.ToString());
        }


        public T GetObject<T>(string key)
        {
            return TextUtility.FromJson<T>((string)Get(key));
        }


        public T GetObject<T>(object key)
        {
            return GetObject<T>(key.ToString());
        }


        public long GetLong(string key)
        {
            object value = Get(key);

            if (value == null)
            {
                return default(long);
            }

            return Convert.ToInt64(value);
        }


        public long GetLong(object key)
        {
            return GetLong(key.ToString());
        }


        public void Set(string key, object data)
        {
            _tempdata[key] = data;
        }


        public void Set(object key, object data)
        {
            Set(key.ToString(), data);
        }


        public void SetObject(string key, object data)
        {
            Set(key, TextUtility.ToJson(data));
        }


        public void SetObject(object key, object data)
        {
            SetObject(key.ToString(), data);
        }


        public void SetLong(string key, long data)
        {
            Set(key, data);
        }


        public void SetLong(object key, long data)
        {
            SetLong(key.ToString(), data);
        }


        public void Remove(string key)
        {
            _tempdata.Remove(key);
        }


        public void Remove(object key)
        {
            Remove(key.ToString());
        }


        public void Clear()
        {
            _tempdata.Clear();
        }


        public bool ContainsKey(string key)
        {
            return _tempdata.ContainsKey(key);
        }


        public bool ContainsKey(object key)
        {
            return ContainsKey(key.ToString());
        }
    }
}
