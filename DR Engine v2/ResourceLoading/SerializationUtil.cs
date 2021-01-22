using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;

namespace DREngine.ResourceLoading
{
    public static class SerializationUtil
    {

        public static UniFieldInfo GetUniField(this Type t, string fieldName)
        {
            return new UniFieldInfo(t.GetMember(fieldName).FirstOrDefault());
        }

        public static IEnumerable<UniFieldInfo> GetUniFields(this Type t)
        {
            foreach (var info in t.GetFields())
            {
                yield return new UniFieldInfo(info);
            }
            foreach (var info in t.GetProperties())
            {
                yield return new UniFieldInfo(info);
            }
        }

    }

    public class UniFieldInfo
    {

        private MemberInfo _info;

        public UniFieldInfo(MemberInfo info)
        {
            _info = info;
        }

        public void SetValue(object obj, object value)
        {
            if (_info is FieldInfo finfo)
            {
                finfo.SetValue(obj, value);
            }
            else if (_info is PropertyInfo pinfo)
            {
                pinfo.SetValue(obj, value);
            }
        }

        public object GetValue(object obj)
        {
            if (_info is FieldInfo finfo)
            {
                return finfo.GetValue(obj);
            }

            if (_info is PropertyInfo pinfo)
            {
                return pinfo.GetValue(obj);
            }

            return null;
        }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            if (_info is FieldInfo finfo)
            {
                return finfo.GetCustomAttribute<T>();
            }

            if (_info is PropertyInfo pinfo)
            {
                return pinfo.GetCustomAttribute<T>();
            }

            return null;
        }

        public Type FieldType {
            get {
                if (_info is FieldInfo finfo)
                {
                    return finfo.FieldType;
                }

                if (_info is PropertyInfo pinfo)
                {
                    return pinfo.PropertyType;
                }

                return null;
            }
        }

        public Type DeclaringType
        {
            get {
                if (_info is FieldInfo finfo)
                {
                    return finfo.DeclaringType;
                }

                if (_info is PropertyInfo pinfo)
                {
                    return pinfo.DeclaringType;
                }

                return null;
            }
        }

        public bool IsStatic
        {
            get
            {
                if (_info is FieldInfo finfo)
                {
                    return finfo.IsStatic;
                }

                if (_info is PropertyInfo)
                {
                    return false;
                }

                return false;
            }
        }

        public bool HasSetter
        {
            get
            {
                if (_info is FieldInfo)
                {
                    return true;
                }

                if (_info is PropertyInfo pinfo)
                {
                    return pinfo.SetMethod != null;
                }

                return false;
            }
        }
        public bool HasGetter
        {
            get
            {
                if (_info is FieldInfo)
                {
                    return true;
                }

                if (_info is PropertyInfo pinfo)
                {
                    return pinfo.GetMethod != null;
                }

                return false;
            }
        }

        public string Name => _info.Name;
    }
}
